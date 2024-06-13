using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int maxStack;
    [SerializeField] private Sprite icon;

    public GameObject Prefab => prefab;
    public int MaxStack => maxStack;
    public Sprite Icon => icon;
}
