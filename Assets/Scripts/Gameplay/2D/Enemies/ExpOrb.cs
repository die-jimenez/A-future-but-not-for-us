using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExpOrb : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    public Transform target;
    public int expValue;
    [SerializeField] float followSpeed;


    void Start()
    {
        if (enemy == null)
        {
            enemy = transform.parent.TryGetComponent(out Enemy _enemy) ? _enemy : null;
        }
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
            transform.position = newPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ExpCollector"))
        {
            target = collision.transform;
        }

        if (collision.CompareTag("Player"))
        {
            enemy.GetBackExpOrb();
        }
    }
}
