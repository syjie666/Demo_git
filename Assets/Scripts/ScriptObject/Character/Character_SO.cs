using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName ="NewCharacterData",menuName ="Character/CharacterData")]
public class Character_SO : ScriptableObject 
{
    [Header("基础属性")]
    public string characterName;
    public AttackType attackType;
    public int maxHelth;
    public int attackDamage;
    public float moveSpeed ;
    public float attackRange ;
    public string _description;
    public Sprite sprite;



}
public enum AttackType
{
    Melee,//近战
    Ranged//远程
}