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
            EventCenter.instance.RangedAttackeHit(collision.gameObject,attackValue);//����Զ�̹����¼�����Ŀ����˭
            if(collision.gameObject.CompareTag("Defensive role"))
            {
                Destroy(gameObject);
            }
            
        }
        else
        {
            Debug.Log("δ����������");
        }
    }
}
