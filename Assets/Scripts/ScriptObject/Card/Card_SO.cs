using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewCardData",menuName ="Card/CardData")]
public class Card_SO : ScriptableObject
{
    public GameObject _prefab;
    public string _name;
    public string _description;
    public int maxHelth;
    public int attackDamage;
    public float moveSpeed;
    public float attackRange;
    public Sprite sprite;
    


}
