using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    //Eventos y enums
    public enum State { empty, loaded, reloding };
    public enum type { Unknown, Hacker }

    [Header("General")]
    [SerializeField] private State state;
    [SerializeField] private int maxAmmo;
    public int remainingAmmo;
    [SerializeField] private GameObject bulletContainer;
    [SerializeField] private GameObject bulletPrefab;


    [Header("Especificas del arma")]
    public int damage;
    public int ammoPerShot;
    [Range(0.1f, 0.75f)] public float reloadTime;
    public List<Bullet> bullets = new List<Bullet>();
    private GameObject arm;

    [Header("Eventos")]
    [HideInInspector] public UnityEvent Shoot;
    [HideInInspector] public UnityEvent<Bullet, int> SetBulletDirection;//GameObject: Bala disparada || int: Index del "for" del disparo


    private void Awake()
    {
        state = State.loaded;
        if (ammoPerShot <= 0) ammoPerShot = 1;
        if (maxAmmo <= 0) maxAmmo = 1;

    }

    private void Start()
    {
        Shoot.AddListener(WeaponActions);
        CreateBullets(maxAmmo);
        InmediateReload();
        SetArm();
    }


    void WeaponActions()
    {
        if (state == State.reloding)
        {
            return;
        }

        if (state == State.loaded)
        {
            for (int i = 0; i < ammoPerShot; i++)
            {
                Bullet bullet = GetCurrentBullet();
                SetBulletDirection.Invoke(bullet, i);
                bullet.gameObject.SetActive(true);
                remainingAmmo--;

                if (remainingAmmo <= 0)
                {
                    state = State.empty;
                    StartCoroutine(Reload());
                    break;
                }
            }
        }
    }


    public void CreateBullets(int amount)
    {
        //Debugs
        if (bulletContainer == null)
        {
            Debug.LogWarning(transform.name + " no se le asigno un 'bullet container'. Creo uno para evitar errores");
            bulletContainer = new GameObject(transform.parent.name + "...BULLET_DEBUG");
            bulletContainer.transform.parent = transform.parent.parent;
            Debug.LogWarning(bulletContainer);
        }
        if (bulletPrefab == null)
        {
            Debug.LogError(gameObject + " no tiene un bulletPrefab");
            return;
        }
        if (!bulletPrefab.TryGetComponent(out Bullet x))
        {
            Debug.LogError(gameObject + " tiene un bulletPrefab que no tiene el script: Bullet");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, new Vector3(100, 100, 0), Quaternion.identity);
            bullet.transform.SetParent(bulletContainer.transform);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetWeapon(this);
            bullets.Add(bulletScript);
        }
    }

    Bullet GetCurrentBullet()
    {
        if (maxAmmo - remainingAmmo >= bullets.Count)
        {
            Debug.LogWarning(transform.name + " intento una bala cuyo index no existe en la lista de 'bullets'.' " +
                "Para evitar errores se forzo a que usara ''bullet[0]");
            return bullets[0];
        }
        int currentBulletIndex = maxAmmo - remainingAmmo;
        return bullets[currentBulletIndex];
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        remainingAmmo = bullets.Count;
        state = State.loaded;
    }

    void InmediateReload()
    {
        remainingAmmo = bullets.Count;
        state = State.loaded;
    }

    public bool isLoaded()
    {
        if (state == State.loaded) return true;
        return false;
    }

    void SetArm()
    {
        arm = transform.parent.gameObject;
    }

    public Transform GetArm()
    {
        return arm.transform;
    }
}
