using System;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Move2D))]
public class Bullet : MonoBehaviour
{
    //La balas tienen varios eventos en varios scripts
    //1. Ejecuta una vez por disparo si este acierta
    //2. Ejecuta una vez por cada enemigo impactado

    public enum DisableIn { Script, Animation }

    [SerializeField] DisableIn disableIn;
    [SerializeField] Weapon weapon;
    [HideInInspector] public Move2D move2D;
    public float duration;
    bool didItHit;

    [Header("Eventos")]
    public UnityEvent FirstHitEvent;//Se ejecuta solo con el primer impacto a un enemigo
    public UnityEvent<GameObject> HitEvent;//Se ejecuta cada vez que la bala impacta a un enemigo



    void Awake()
    {
        if (duration == 0) duration = 0.5f;
        move2D = GetComponent<Move2D>();
    }

    private void OnEnable()
    {
        didItHit = false;
        if (disableIn == DisableIn.Script)
        {
            StartCoroutine(DisnableBullets());
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Hackable"))
        {
            if (!didItHit)
            {
                didItHit = true;
                FirstHitEvent.Invoke();
            }
            HitEvent.Invoke(collision.gameObject);
        }
    }

    public Weapon GetWeapon()
    {
        return weapon;
    }

    public void SetWeapon(Weapon _weapon)
    {
        weapon = _weapon;
    }


    private void OnDisable()
    {
        didItHit = false;
    }

    IEnumerator DisnableBullets()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
        yield return null;
    }

    void EndAnimation()
    {
        gameObject.SetActive(false);
    }
}
