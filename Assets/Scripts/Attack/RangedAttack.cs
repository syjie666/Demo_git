using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    public int attackValue;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision != null)
        {
            EventCenter.instance.RangedAttackeHit(collision.gameObject,attackValue);//告诉远程攻击事件攻击目标是谁
            if(collision.gameObject.CompareTag("Defensive role"))
            {
                Destroy(gameObject);
            }
            
        }
        else
        {
            Debug.Log("未攻击到敌人");
        }
    }
}
