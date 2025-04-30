using DG.Tweening;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Bullet))]
public class HackerBullet : MonoBehaviour
{
    [SerializeField] Bullet bullet;
    [SerializeField] HackerWeapon hackerWeapon;
    float tiempoTranscurrido;
    float initialDelay;
    Vector3 initScale;


    private void Awake()
    {
        initScale = transform.localScale;
    }

    private void Start()
    {
        StartCoroutine(GetReferences());
    }

    private void OnEnable()
    {
        //transform.localScale = initScale;
        tiempoTranscurrido = 0f;
        initialDelay = Random.Range(0.05f, 0.1f);
        //transform.DOScale(0, 1f);
    }


    private void FixedUpdate()
    {
        if (tiempoTranscurrido >= initialDelay)
        {
            bullet.move2D.Move(false);
        }
        tiempoTranscurrido += Time.fixedDeltaTime;
    }


    IEnumerator GetReferences()
    {
        bullet = GetComponent<Bullet>();
        yield return new WaitUntil(() => bullet.GetWeapon() != null);
        hackerWeapon = bullet.GetWeapon().GetComponent<HackerWeapon>();

        //Eventos de impacto de bala
        bullet.HitEvent.AddListener(HitHackableObject);
        bullet.FirstHitEvent.AddListener(() => hackerWeapon.hackSequence.DisplayHackingSequence());
        gameObject.SetActive(false);
    }


    void HitHackableObject(GameObject _object)
    {
        if (_object.TryGetComponent(out HackableObject hackableObject))
        {
            if (!hackableObject.isBeingHacked)
            {
                hackerWeapon.objetosHackeados.Add(hackableObject);
                hackableObject.isBeingHacked = true;
                hackableObject.InitHack.Invoke();
            }
        }
    }
}
