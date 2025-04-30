using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ExplosionWave : MonoBehaviour
{
    [SerializeField] Transform origin;
    [SerializeField] float duracion;
    [SerializeField] Vector3 minSize;
    [SerializeField] Vector3 maxSize;

    private void OnEnable()
    {
        transform.localScale = minSize;
        if (origin != null) transform.position = origin.position;
        transform.DOScale(maxSize, duracion).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.gameObject.SetActive(false);
        });

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Enemy enemy))
        {
            enemy.Die();
        }
    }

    void Start()
    {

    }



}
