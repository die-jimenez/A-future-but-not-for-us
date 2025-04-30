using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using static EnemyManager;

[System.Serializable]
public class EnemySpawn
{
    public GameObject prefab;
    public GameObject container;
    public int amount;
    public float spawnTimer;
    public float time;
    public List<Enemy> enemiesPool;
}



public class EnemyManager : MonoBehaviour
{
    static public EnemyManager instance;

    [Header("Lista de todos los enemigos activo")]
    public List<Enemy> enemiesSpawned;

    [Space(15)]
    [Header("Spawns")]
    public EnemySpawn basicRobotSpawn;


    Transform character;



    private void Awake()
    {
        if (instance != null) Destroy(instance);
        else instance = this;
    }

    void Start()
    {
        if (GameManager.instance != null)
        {
            character = Level2DManager.instance.player2D.transform;
        }
        //--- Robots Basicos ---
        CreateEnemies(basicRobotSpawn, basicRobotSpawn.prefab, basicRobotSpawn.amount);
        SetParentToEnemyList(basicRobotSpawn.enemiesPool, ref basicRobotSpawn.container);
        SetPositionToEnemyList(basicRobotSpawn.enemiesPool, new Vector3(20, 9.5f, 0));
        //----------------------
    }

    void Update()
    {
        SpawnEnemy(basicRobotSpawn);
        basicRobotSpawn.time += Time.deltaTime;
    }


    //----------------------------------------------------------------------------------------------------------
    #region Crear Enemigos
    void CreateEnemies(EnemySpawn _spawner, GameObject prefab, float amount)
    {
        if (prefab == null)
        {
            Debug.LogError("EnemyManager no tiene asignado el prefab de un enemigo");
            return;
        }
        for (int i = 0; i < amount; i++)
        {
            GameObject currentEnemy = Instantiate(prefab);
            Enemy script = currentEnemy.GetComponent<Enemy>();
            _spawner.enemiesPool.Add(script);
            script.spawner = _spawner;
            script.gameObject.SetActive(false);
        }
    }

    void SetPositionToEnemyList(List<Enemy> enemyList, Vector3 position)
    {
        foreach (Enemy enemy in enemyList)
        {
            enemy.transform.position = position;
        }
    }

    void SetParentToEnemyList(List<Enemy> _enemyList, ref GameObject container)
    {
        if (container == null)
        {
            Debug.LogWarning("EnemyManager no tiene asignado un container para " + _enemyList[0].transform.name + ", por ello se creo uno propio");
            container = new GameObject("..." + _enemyList[0].transform.name + "_CONTAINER_DEBUG");
        }

        foreach (Enemy enemy in _enemyList)
        {
            enemy.transform.parent = container.transform;
        }
    }
    #endregion
    //----------------------------------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------------------------

    #region Spawn de enemigos
    void SpawnEnemy(EnemySpawn enemySpawn)
    {
        if (character == null) return;
        if (enemySpawn.time >= enemySpawn.spawnTimer)
        {
            if (enemySpawn.enemiesPool.Count == 0)
            {
                enemySpawn.time = 0;
                return;
            }

            Enemy currentEnemy = enemySpawn.enemiesPool[0];
            currentEnemy.SpawnAt(currentEnemy.RandomPositionAround(character.position));
            currentEnemy.SetTarget(character);

            //Lo cambia de la lista de enemigos a spawnear a los spawneados
            enemySpawn.enemiesPool.Remove(currentEnemy);
            enemiesSpawned.Add(currentEnemy);
            enemySpawn.time = 0;
        }
    }

    
    #endregion
    //----------------------------------------------------------------------------------------------------------

}
