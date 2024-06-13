using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private GameObject spawObjectPrefab;

    public void OnDamage(float damage)
    {
        DOTween.Sequence().Append(transform.DOScale(Vector3.one * 0.9f, 0.1f)).Append(transform.DOScale(Vector3.one, 0.1f)).SetLink(gameObject);
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
            Instantiate(spawObjectPrefab, transform.position + Vector3.up * 1.2f, Quaternion.identity);
        }
    }
}
