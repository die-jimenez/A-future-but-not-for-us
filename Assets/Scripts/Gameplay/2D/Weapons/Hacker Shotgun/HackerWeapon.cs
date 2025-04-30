using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Weapon))]
public class HackerWeapon : MonoBehaviour
{
    private Weapon weapon;
    public HackSequence hackSequence;
    public List<HackableObject> objetosHackeados;
    [Range(0.1f, 2f)] public float stunDuration;


    void Start()
    {
        weapon = GetComponent<Weapon>();

        weapon.SetBulletDirection.AddListener(SetBulletDirection);
        hackSequence?.HackingCompleted.AddListener(() => InvokeSuccesHackOnHackableObjects());
    }


    void SetBulletDirection(Bullet bullet, int index)
    {
        Quaternion armRotation = weapon.GetArm().rotation;
        bullet.transform.position = transform.position;
        //bullet.transform.rotation = armRotation;

        int maxAngle = 45;
        float extraRandomAngle = Random.Range(-maxAngle / 2, maxAngle / 2);

        //Se pasa la direccion del brazo al componente de movimiento de la bala
        //Se multipla por la escala porque cuando se invierte, el angulo es 0 pero gira con un cambio de signo en la escala
        Vector2 bulletDirection = Matematicas.AnguloToDireccion2D(armRotation.eulerAngles.z + extraRandomAngle);
        bulletDirection *= weapon.GetArm().localScale.x;
        bullet.move2D.SetMoveDirection(bulletDirection);

        //Cambio ligero de la velocidad
        bullet.move2D.speedMultiplier = bullet.move2D.speedMultiplier * Random.Range(0.85f, 1.15f);

        //Da la rotacion del brazo a la bala qpara que las particulas sigan la misma trayectoria
        bullet.transform.rotation = armRotation;
    }



    void SetBulletDirectonsLikeVampire(Bullet bullet, int index)
    {
        //Lo cree cuando aun no sabia como iba a ser el disparo, funciona como vampire survivor en donde 
        //segun el numero de balas, se van posicionando alrededor del personaje, pero sumando la direccion del arma/brazo
        //como Til 20 minutes down

        Quaternion bulletRotation = Quaternion.identity;
        Quaternion armRotation = weapon.GetArm().rotation;
        bullet.transform.localPosition = weapon.transform.localPosition;

        //Mantiene el angulo del aunque este invertido en "scale.x"
        if (Mathf.Sign(weapon.GetArm().localScale.x) == -1)
            armRotation *= Quaternion.Euler(new Vector3(0, 0, 180));

        switch (weapon.ammoPerShot)
        {
            case 1:
                bulletRotation = armRotation;
                bullet.transform.rotation = bulletRotation;
                break;

            case 2:
                if (index == 0) bulletRotation = armRotation;
                if (index == 1) bulletRotation = armRotation * Quaternion.Euler(new Vector3(0, 0, 180));
                bullet.transform.rotation = bulletRotation;
                break;
        }
    }

    void SetInitialParameters()
    {
        if (weapon.ammoPerShot == 0)
        {
            weapon.ammoPerShot = 1;
            Debug.LogWarning("HackerShotgun debe disparar minimo 1 bala");
        }
    }

    void InvokeSuccesHackOnHackableObjects()
    {
        foreach (HackableObject hackeados in objetosHackeados)
        {
            hackeados.SuccesHack.Invoke();
        }
        objetosHackeados.Clear();
    }

    //Metodo para colocar manual mente en el evento de cada boton de la secuencia
    public void StunEnemiesHacked()
    {
        for (int i = 0; i < objetosHackeados.Count; i++)
        {
            if (objetosHackeados[i].type == HackableObject.Type.enemy && objetosHackeados[i].enemy != null)
            {
                StartCoroutine(objetosHackeados[i].enemy.Stunned(stunDuration));
            }
        }
    }

}
