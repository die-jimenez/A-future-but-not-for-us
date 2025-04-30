using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;


[RequireComponent(typeof(IAManager), typeof(Rigidbody2D))]
public class IAMove : MonoBehaviour
{
    //BASE DEL CÓDIGO SACADO DE: https://www.youtube.com/watch?v=6BrZryMz-ac&list=LL 
    //Que se base en este artículo que es realmente con el que construí el código http://www.gameaipro.com/GameAIPro2/GameAIPro2_Chapter18_Context_Steering_Behavior-Driven_Steering_at_the_Macro_Scale.pdf

    #region Clases y Enum
    [Serializable]
    public class TargetMap
    {
        public Transform target;
        public float[] dangerDir = new float[posiblesDirecciones];
        public float[] interestDir = new float[posiblesDirecciones];
    }
    public enum MoveAction { RunAway, Chase, Surround, KeepDirection }
    public enum MapTypes { Interest, Danger, Context }
    #endregion

    [Header("Parametros")]
    public const int posiblesDirecciones = 16;
    [SerializeField] IAManager controller;
    [SerializeField] float visionRadio;
    [SerializeField] Vector3 offset;

    [Header("Comportamientos")]
    [SerializeField] MoveAction moveAction;
    [SerializeField, Range(0.5f, 5f)] float timerRunAwayToChase;
    [SerializeField, Range(0.1f, 1f)] float timerChaseToAim;

    [Header("Gizmos y LinesRenderes")]
    [SerializeField] MapTypes mapaMostrado;
    [SerializeField, Tooltip("Debe ser un material SPRITE para que lineRender color le afecte")] Material lineRenderMaterial;
    [SerializeField, Range(0f, 1f)] float lineLengthPercentage;
    [SerializeField] Color goodColor;
    [SerializeField] Color midColor;
    [SerializeField] Color badColor;

    [SerializeField, Tooltip("Plano de dibujo de los linesRenderer")] float zPos;
    [SerializeField] List<LineRenderer> linesOfMaps;


    [Header("Mapas (targets)")]
    [SerializeField] List<TargetMap> targetMaps;


    //Mapas de movimiento 
    float[] interestMap;
    float[] dangerMap;
    public float[] contextMap;
    float lastContextMax;

    //Valores para generar mapas
    public float nearbyPosRatio = 0.8f;
    Vector2[] nearbyPositions;
    Vector3 spriteCenter;

    //Timers para cambiar comportamientos
    Utilidades.Timer timer_RunAway_Chase;
    Utilidades.Timer timer_Chase_Aim;



    private void Awake()
    {
        nearbyPositions = new Vector2[posiblesDirecciones];
        interestMap = new float[posiblesDirecciones];
        contextMap = new float[posiblesDirecciones];
        dangerMap = new float[posiblesDirecciones];
        targetMaps = new List<TargetMap>();

        timer_RunAway_Chase = new Utilidades.Timer(timerRunAwayToChase);
        timer_Chase_Aim = new Utilidades.Timer(timerChaseToAim);
    }

    private void Start()
    {
        controller = controller == null ? GetComponent<IAManager>() : controller;
        controller.Attack.AddListener(() =>
        {
            moveAction = MoveAction.RunAway;
            controller.target = null;
        });

        CreateLineRenderers();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TargetMap temp = new TargetMap();
            targetMaps.Add(temp);
            temp.target = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (targetMaps.Count == 0) return;
            foreach (TargetMap mapa in targetMaps)
            {
                if (mapa.target == collision.transform)
                {
                    StartCoroutine(RemoveTargetMap(mapa));
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawMapaContextual();
        MarkEnemyToChase();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    //----------------------------------------------------------------------------------------------------------------------------------------------------
    #region Metodos publicos: Comportamientos y cambiadores de estados
    public void RunAway()
    {
        if (!AreEnemiesNearby())
        {
            controller.SetMoveDirection(Vector2.zero);
            HideLinesOfMap();
            return;
        }
        CalculateConextMap();
        //Si no hay buenas opciones, que matenga la direccion
        if (isSurrounded()) return;
        Vector2 newDirection = BestDirection();
        controller.SetMoveDirection(newDirection);

    }


    public void Chase()
    {
        if (controller.target == null || !AreEnemiesNearby())
        {
            moveAction = MoveAction.RunAway;
            return;
        }
        CalculateConextMap();
        float currentMax = contextMap.Max();
        //Evita que cambie constantemente de direccion por una pequeña variacion
        if (currentMax >= lastContextMax - 0.02f && currentMax <= lastContextMax + 0.02f) return;
        lastContextMax = contextMap.Max();
        Vector2 newDirection = BestDirection();
        controller.SetMoveDirection(newDirection);
    }

    public void Surround()
    {
        if (controller.target == null || !AreEnemiesNearby())
        {
            moveAction = MoveAction.RunAway;
            return;
        }
        CalculateConextMap();
        float currentMax = contextMap.Max();
        //Evita que cambie constantemente de direccion por una pequeña variacion
        if (currentMax >= lastContextMax - 0.02f && currentMax <= lastContextMax + 0.02f) return;
        lastContextMax = contextMap.Max();
        Vector2 newDirection = BestDirection();
        controller.SetMoveDirection(newDirection);
    }

    public IEnumerator AimAndAttack()
    {
        if (controller.target == null) yield break;
        //Se usa Vector2.zero para normalizar el vector
        float angleWeaponToTarget = Matematicas.RadianesEntre(controller.weapon.transform.position, controller.target.transform.position);
        float angleBodyToTarget = Matematicas.RadianesEntre(controller.transform.position, controller.target.transform.position);
        Vector2 aimDirection = Matematicas.PolaresToRectangulares(1, angleWeaponToTarget, Vector2.zero);
        yield return new WaitForEndOfFrame();
        controller.SetMoveDirection(aimDirection);
        moveAction = MoveAction.KeepDirection;
        yield return new WaitForSeconds(0.05f);
        controller.Attack.Invoke();
        yield break;
    }

    public void SelectTarget()
    {
        //1. Se indentifica la direccion donde en promedio hay mas enemigos
        //2. Busca al primer enemigo que encuentre más cercano a esa direccion
        if (!AreEnemiesNearby()) return;
        if (contextMap[BestDrectionIndex()] < -0.1f) return;
        float[] averageDangerMap = AverageMapOf(MapTypes.Danger);
        int indexMaxAverage = Array.IndexOf(averageDangerMap, averageDangerMap.Max());

        foreach (TargetMap mapa in targetMaps)
        {
            int indexMaxOfTarget = Array.IndexOf(mapa.dangerDir, mapa.dangerDir.Max());
            if (indexMaxOfTarget == indexMaxAverage)
            {
                controller.target = mapa.target;
                moveAction = MoveAction.Chase;
                return;
            }
        }
    }

    public bool isReadyToChase()
    {
        timer_RunAway_Chase.Play();
        if (timer_RunAway_Chase.finished())
        {
            timer_RunAway_Chase.Reset();
            return true;
        }
        else return false;
    }

    public bool isReadyToAim()
    {
        timer_Chase_Aim.Play();
        if (timer_Chase_Aim.finished())
        {
            timer_Chase_Aim.Reset();
            return true;
        }
        else return false;
    }

    public MoveAction GetMoveAction() { return moveAction; }
    public void SetMoveAction(MoveAction _moveAction) { moveAction = _moveAction; }
    #endregion
    //----------------------------------------------------------------------------------------------------------------------------------------------------


    //----------------------------------------------------------------------------------------------------------------------------------------------------
    #region Generador de mapas
    void CalculateConextMap()
    {
        if (!AreEnemiesNearby()) return;
        CalculateNearbyPositions();
        CalculateInterestMap();
        CalculateDangerMap();

        for (int i = 0; i < posiblesDirecciones; i++)
        {
            contextMap[i] = interestMap[i] - dangerMap[i];
        }
        DrawLinesOfMap();
    }


    void CalculateDangerMap()
    {
        ResetMap(dangerMap);
        foreach (TargetMap mapa in targetMaps)
        {
            for (int i = 0; i < posiblesDirecciones; i++)
            {
                Vector2 start = nearbyPositions[i];
                if (moveAction == MoveAction.RunAway)
                {
                    //Entre mas de frente este, mayor es el peso
                    float angleWeight = AngleWeight(start, mapa.target.position);
                    //Invierto. Entre mas cerca, mayor es el peso
                    float distanceWeight = 1 - DistanceWeight(start, mapa.target.position);
                    mapa.dangerDir[i] = (angleWeight * 0.5f) + (distanceWeight * 0.5f);
                }

                if (moveAction == MoveAction.Chase)
                {
                    if (mapa.target == controller.target) continue;
                    //Entre mas de frente este, mayor es el peso
                    float angleWeight = AngleWeight(start, mapa.target.position);
                    //Invierto. Entre mas cerca, mayor es el peso
                    float distanceWeight = 1 - DistanceWeight(start, mapa.target.position);
                    mapa.dangerDir[i] = (angleWeight * 0.3f) + (distanceWeight * 0.7f);
                }

                if (moveAction == MoveAction.Surround)
                {
                    if (mapa.target == controller.target) continue;
                    //Entre mas de frente este, mayor es el peso
                    float angleWeight = AngleWeight(start, mapa.target.position);
                    //Invierto. Entre mas cerca, mayor es el peso
                    float distanceWeight = 1 - DistanceWeight(start, mapa.target.position);
                    mapa.dangerDir[i] = (angleWeight * 0.3f) + (distanceWeight * 0.7f);
                }
            }
        }
        //Se queda con los maximos
        GetTheMostDangerous();
    }


    void CalculateInterestMap()
    {
        ResetMap(interestMap);
        foreach (TargetMap mapa in targetMaps)
        {
            for (int i = 0; i < posiblesDirecciones; i++)
            {
                Vector2 start = nearbyPositions[i];
                if (moveAction == MoveAction.RunAway)
                {
                    //Invierto. Entre mas de espalda, mayor es el peso
                    float angleWeight = 1 - AngleWeight(start, mapa.target.position);
                    float currentDirectionWeight = CurrentDirectionWeight(start);
                    mapa.interestDir[i] = (angleWeight * 0.7f) + (currentDirectionWeight * 0.3f);
                }

                if (moveAction == MoveAction.Chase)
                {
                    if (mapa.target != controller.target) continue;
                    //Entre mas de frente, mayor es el peso
                    float angleWeight = AngleWeight(start, mapa.target.position);
                    //Favorece a los costados
                    angleWeight = 1 - Mathf.Abs(angleWeight - 0.8f);
                    //Favorece a mantener la misma direccion
                    float currentDirectionWeight = CurrentDirectionWeight(start);

                    mapa.interestDir[i] = (angleWeight * 0.7f) + (currentDirectionWeight * 0.3f);
                    interestMap[i] = mapa.interestDir[i];
                }

                if (moveAction == MoveAction.Surround)
                {
                    if (mapa.target != controller.target) continue;
                    float minDistToSurround = 0.7f;
                    //Entre mas lejos, mayor es el peso
                    float distanceWeight = DistanceWeight(start, mapa.target.position);
                    //Si "distanceWeight" supera al valor minimo, se reduce el interest
                    distanceWeight = distanceWeight >= minDistToSurround ? distanceWeight - 1 : distanceWeight;
                    //Favorece a mantener la misma direccion
                    float currentDirectionWeight = CurrentDirectionWeight(start);

                    mapa.interestDir[i] = (distanceWeight * 0.6f) + (currentDirectionWeight * 0.4f);
                    interestMap[i] = mapa.interestDir[i];
                }
            }
        }

        if (moveAction == MoveAction.RunAway)
        {
            interestMap = AverageMapOf(MapTypes.Interest);
        }
    }
    #endregion
    //----------------------------------------------------------------------------------------------------------------------------------------------------


    //----------------------------------------------------------------------------------------------------------------------------------------------------
    #region Metodos auxiliares a la generación de mapas
    float[] AverageMapOf(MapTypes mapToAverage)
    {
        if (mapToAverage == MapTypes.Context)
        {
            UnityEngine.Debug.LogWarning("No se puede promediar el mapa contextual");
            return null;
        }

        float[] tempMap = new float[posiblesDirecciones];
        foreach (TargetMap mapa in targetMaps)
        {
            for (int i = 0; i < posiblesDirecciones; i++)
            {
                if (mapToAverage == MapTypes.Interest)
                    tempMap[i] += mapa.interestDir[i];

                if (mapToAverage == MapTypes.Danger)
                    tempMap[i] += mapa.dangerDir[i];
            }
        }
        for (int i = 0; i < posiblesDirecciones; i++)
            tempMap[i] = tempMap[i] / targetMaps.Count;

        return tempMap;
    }

    void GetTheMostDangerous()
    {
        //Se queda las 3 direcciones mas peligrosas con cada "mapTarget"
        //Si las direcciones se repiten, se queda con el valor más alto
        //--------------------------------------------------------------------
        foreach (TargetMap mapa in targetMaps)
        {
            int dangerestDirIndex = Array.FindIndex(mapa.dangerDir, (x) => x == mapa.dangerDir.Max());
            int indexBefore = dangerestDirIndex != 0 ? dangerestDirIndex - 1 : posiblesDirecciones - 1;
            int indexAfter = dangerestDirIndex != posiblesDirecciones - 1 ? dangerestDirIndex + 1 : 0;

            if (mapa.dangerDir[indexBefore] > dangerMap[indexBefore])
                dangerMap[indexBefore] = mapa.dangerDir[indexBefore];

            if (mapa.dangerDir[dangerestDirIndex] > dangerMap[dangerestDirIndex])
                dangerMap[dangerestDirIndex] = mapa.dangerDir[dangerestDirIndex];

            if (mapa.dangerDir[indexAfter] > dangerMap[indexAfter])
                dangerMap[indexAfter] = mapa.dangerDir[indexAfter];
        }
    }

    void ResetMap(float[] map)
    {
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = 0;
        }
    }

    bool AreEnemiesNearby()
    {
        if (targetMaps.Count == 0) return false;
        else return true;
    }

    IEnumerator RemoveTargetMap(TargetMap _mapa)
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForEndOfFrame();
        if (_mapa.target == controller.target) controller.target = null;
        targetMaps.Remove(_mapa);
    }

    Vector2 BestDirection()
    {
        float bestDirectionIndex = Array.FindIndex(contextMap, (x) => x == contextMap.Max());
        float directionAngle = ((360 / posiblesDirecciones) * bestDirectionIndex) * Mathf.Deg2Rad;
        return Matematicas.PolaresToRectangulares(1, directionAngle, Vector2.zero);
    }

    int BestDrectionIndex()
    {
        return Array.FindIndex(contextMap, (x) => x == contextMap.Max());
    }

    void CalculateNearbyPositions()
    {
        spriteCenter = controller.body.position + offset;
        for (int i = 0; i < posiblesDirecciones; i++)
        {
            float angle = (360f / posiblesDirecciones) * i;
            nearbyPositions[i] = Matematicas.PolaresToRectangulares(nearbyPosRatio, angle * Mathf.Deg2Rad, spriteCenter);
        }
    }

    public bool isSurrounded()
    {
        if (contextMap[BestDrectionIndex()] < -0.15f) return true;
        else return false;
    }

    #endregion
    //----------------------------------------------------------------------------------------------------------------------------------------------------


    //----------------------------------------------------------------------------------------------------------------------------------------------------
    #region Calculo de pesos para mapas 
    float AngleWeight(Vector2 _startPos, Vector2 _target)
    {
        //Vector.Dot te permite saber el angulo entre dos vectores si estan normalizados.
        //Código sacado de la documentación: https://docs.unity3d.com/ScriptReference/Vector3.Dot.html
        //---------------------------------------------------------------------------------------------

        //Vector.Dot funciona de la forma que quiero solo con vectores normalizados
        Vector3 localPosNormalized = Vector3.Normalize((spriteCenter * Vector2.one) - _startPos);
        Vector2 targetDirectionNormalized = Vector3.Normalize(_startPos - _target);
        //Angulo dados de (-1,1): "-1" detras, "1" de frente
        float angleToTarget = Vector2.Dot(localPosNormalized, targetDirectionNormalized);
        //Entre mas de frente, mayor es el peso
        angleToTarget = Matematicas.Map(angleToTarget, -1, 1, 0, 1);
        return angleToTarget;
    }

    float DistanceWeight(Vector2 _startPos, Vector2 _target)
    {
        //Vector.Distance es... "(v1-v2).magnitude". La magnitud se calcula con una raiz cuadrada, operación que consume muchos recursos
        //Por ello se usa "sqrMagnitude" (cancela la raiz con otra raiz). https://docs.unity3d.com/ScriptReference/Vector3-sqrMagnitude.html
        //---------------------------------------------------------------------------------------------

        //Distancia al cuadrado
        float distance = (_startPos - _target).sqrMagnitude;
        //"maxEscala1" al cuadrado para compensar la distancia al cuadrado
        float distanceWeight = Matematicas.Map(distance, 0, Mathf.Pow(visionRadio, 2), 0, 1);
        //Mientras mas lejos, mayor es el peso
        distanceWeight = Mathf.Clamp(distanceWeight, 0, 1);
        return distanceWeight;
    }

    float CurrentDirectionWeight(Vector2 _startPos)
    {
        //la idea es ayudar a mantener la dirección actual o similares para evitar trabajas e indesición
        //---------------------------------------------------------------------------------------------

        //Vector.Dot funciona de la forma que quiero solo con vectores normalizados
        Vector3 localPosNormalized = Vector3.Normalize((spriteCenter * Vector2.one) - _startPos);
        //Devuelve valores invertidos. "-1" si mantiene la dirección y "1" si va a la opuesta
        float moveDirectionWeight = Vector2.Dot(localPosNormalized, controller.GetMoveDirection());
        moveDirectionWeight = Matematicas.Map(moveDirectionWeight, -1, 1, 0, 1);
        //Normalizo e invierto para tener lso valores correctos
        moveDirectionWeight = 1 - moveDirectionWeight;
        return moveDirectionWeight;
    }
    #endregion
    //----------------------------------------------------------------------------------------------------------------------------------------------------


    //----------------------------------------------------------------------------------------------------------------------------------------------------
    #region Dibujar mapas con Gizmos
    void DrawMapaContextual()
    {
        if (targetMaps.Count == 0) return;

        //Rango de vision
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(spriteCenter, visionRadio);

        //Representacion de mapa
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(spriteCenter, nearbyPosRatio);

        //Mapas mostrado
        float[] displayedMap = contextMap;
        if (mapaMostrado == MapTypes.Danger) displayedMap = dangerMap;
        if (mapaMostrado == MapTypes.Interest) displayedMap = interestMap;

        //Colores: Default
        Color good = Color.green;
        Color mid = Color.yellow;
        Color bad = Color.red;
        //Danger map
        if (mapaMostrado == MapTypes.Danger)
        {
            good = Color.red;
            mid = Color.yellow;
            bad = Color.green;
        }

        for (int i = 0; i < posiblesDirecciones; i++)
        {
            //Los valores pueden ser hasta -1, pero visualmente necesito que lleguen a 0
            displayedMap[i] = displayedMap[i] <= 0 ? 0.1f + UnityEngine.Random.Range(0f, 0.03f) : displayedMap[i];

            Vector2 start = nearbyPositions[i];
            float angle = (360f / posiblesDirecciones) * i;
            float ratio = ((displayedMap[i] * visionRadio) - nearbyPosRatio);
            Vector2 end = Matematicas.PolaresToRectangulares(ratio * lineLengthPercentage, angle * Mathf.Deg2Rad, start);

            if (displayedMap[i] >= 0 && displayedMap[i] < 0.3f)
                Gizmos.color = bad;
            else if (displayedMap[i] >= 0.3 && displayedMap[i] < 0.7f)
                Gizmos.color = mid;
            else if (displayedMap[i] >= 0.7)
                Gizmos.color = good;

            Gizmos.DrawLine(start, end);

            //Si es la idreccion que esta tomando, dibuja un circulo en la punta
            if (i == BestDrectionIndex())
            {
                Gizmos.DrawWireSphere(end, 0.3f);
            }
        }
    }

    void MarkEnemyToChase()
    {
        if (controller.target != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(controller.target.position, 0.5f);
        }
    }
    #endregion
    //----------------------------------------------------------------------------------------------------------------------------------------------------



    void CreateLineRenderers()
    {
        for (int i = 0; i < posiblesDirecciones; i++)
        {
            GameObject lineObject = new GameObject("Linea " + i);
            linesOfMaps.Add(lineObject.AddComponent<LineRenderer>());
            lineObject.transform.parent = transform;

            //if (lineRenderMaterial != null) linesOfMaps[i].material = lineRenderMaterial;
            linesOfMaps[i].material = new Material(Shader.Find("Sprites/Default"));
            //Ancho de linea
            linesOfMaps[i].startWidth = 0.15f;
            linesOfMaps[i].endWidth = 0.15f;
            //Redondear bordes, entre mayor el numero, mas redondeado
            linesOfMaps[i].numCapVertices = 20;
            linesOfMaps[i].numCornerVertices = 20;
            //Cambiando orden de dibujado 
            linesOfMaps[i].sortingOrder = -10;
        }
    }

    void HideLinesOfMap()
    {
        foreach (LineRenderer line in linesOfMaps)
        {
            line.gameObject.SetActive(false);
        }
    }

    void DrawLinesOfMap()
    {
        if (targetMaps.Count == 0) return;
        Color lineColor = Color.white;

        //Mapas mostrado
        float[] displayedMap = contextMap;
        if (mapaMostrado == MapTypes.Danger) displayedMap = dangerMap;
        if (mapaMostrado == MapTypes.Interest) displayedMap = interestMap;

        //Colores: Default
        Color good = goodColor;
        Color mid = midColor;
        Color bad = badColor;
        //Danger map
        if (mapaMostrado == MapTypes.Danger)
        {
            good = Color.red;
            mid = Color.yellow;
            bad = Color.green;
        }

        for (int i = 0; i < posiblesDirecciones; i++)
        {
            
            if (displayedMap[i] <= 0.15f)
            {
                linesOfMaps[i].gameObject.SetActive(false);
            }
            else linesOfMaps[i].gameObject.SetActive(true);

            Vector2 start = nearbyPositions[i];
            float angle = (360f / posiblesDirecciones) * i;
            float ratio = ((displayedMap[i] * visionRadio) - nearbyPosRatio);
            float largo = lineLengthPercentage;
            Vector2 end = Matematicas.PolaresToRectangulares(ratio * largo, angle * Mathf.Deg2Rad, start);

            if (displayedMap[i] >= 0 && displayedMap[i] < 0.3f)
                lineColor = bad;
            else if (displayedMap[i] >= 0.3 && displayedMap[i] < 0.7f)
                lineColor = mid;
            else if (displayedMap[i] >= 0.7)
                lineColor = good;

            //LineRenderer esta pensado para hacer gradientes, por eso se define el start y end color
            linesOfMaps[i].startColor = lineColor;
            linesOfMaps[i].endColor = lineColor;

            //Los puntos de un LineRenderer se cambian en arreglos
            Vector3[] vertexs = new Vector3[] {
                new Vector3 (start.x, start.y, zPos),
                new Vector3(end.x, end.y, zPos)
            };
            SetLineRendererPoints(linesOfMaps[i], vertexs);
        }
    }

    public void SetLineRendererPoints(LineRenderer lineRenderer, Vector3[] points)
    {
        lineRenderer.positionCount = points.Length; // Definir el número de puntos
        lineRenderer.SetPositions(points);
    }
}
