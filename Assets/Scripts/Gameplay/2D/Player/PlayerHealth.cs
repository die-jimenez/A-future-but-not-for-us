using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int currentHp
    {
        get
        {
            return _currentHp;
        }
        set
        {
            _currentHp = Mathf.Clamp(value, 0, maxHp);
            if (healthBar != null)
            {
                healthBar.UpdateSprite(_currentHp);
            }
        }
    }

    //Inspector
    [SerializeField] private int maxHp;
    [SerializeField] private int _currentHp;
    [SerializeField] private PlayerHealthBar healthBar;
    [SerializeField, Range(0.1f, 1f)] private float inmortalityDuration;
    public bool isInmortal;

    [Space(5)]
    public UnityEvent Die = new UnityEvent();

    [Header("VFX")]
    public UnityEvent TakeDamageEffects = new UnityEvent();


    void Start()
    {
        if (healthBar != null)
        {
            healthBar.UpdateSprite(currentHp);
        }
        else Debug.LogWarning(transform.name + " no tiene referencia de su HealthBar");

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (!isInmortal && currentHp > 0)
            {
                StartCoroutine(TakeDamage(collision));
                StartCoroutine(DoInmortal());
                //Blink(3, inmortalityDuration);
                TakeDamageEffects.Invoke();
            }
        }
    }



    #region Take Damage y asociados
    IEnumerator TakeDamage(Collider2D collision)
    {
        if (collision.TryGetComponent(out Enemy enemy))
        {

            currentHp -= enemy.damage;
            if (currentHp < 0)
            {
                isInmortal = true;
                yield return new WaitForSeconds(inmortalityDuration);
                isInmortal = false;
            }
            else if (currentHp <= 0)
            {
                Die.Invoke();
            }
            
        }
    }

    IEnumerator DoInmortal()
    {
        isInmortal = true;
        yield return new WaitForSeconds(inmortalityDuration);
        isInmortal = false;
    }

    public IEnumerator RestoreLife(Color _color)
    {
        isInmortal = true;
        healthBar.ChangeColorHealthPoints(_color);
        for (int i = 0; i < maxHp; i++)
        {
            healthBar.RestorePoint(i);
            yield return new WaitForSeconds(0.45f);
        }
        _currentHp = maxHp;
        healthBar.ChangeColorHealthPoints(Color.white);
        yield return new WaitForSeconds(0.2f);
        isInmortal = false;
    }

    //void Blink(int blinks, float time)
    //{
    //    if (blinkScript == null) return;
    //    StartCoroutine(blinkScript.BlinkAsEvent(blinks, time, spriteRenderer));
    //}
    #endregion
}
