using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter : MonoBehaviour
{
    //单例模式 全局唯一
    private static EventCenter _instance;
    public static EventCenter instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EventCenter>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("EventCenter");
                    _instance = obj.AddComponent<EventCenter>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }
    public event Action<GameObject, int> OnRangedAttackHit;//定义远程攻击事件 接收方
    public event Action<GameObject, int> OnAttackHit;//定义近战攻击事件
    public event Action OnCameraShake;//定义摄像机抖动事件
    public event Action OnOpenSelectUI;//定义选择Buff事件
    public event Action<GameObject> OnEnemyKilled;//定义敌人死亡事件
    public event Action <int>OnSetCharactger;//定义生成角色事件
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void RangedAttackeHit(GameObject hitTarget, int value)//调用远程攻击事件  发出方
    {
        OnRangedAttackHit?.Invoke(hitTarget, value);
    }
    public void AttackHit(GameObject hitTarget, int value)//调用近战攻击事件
    {
        OnAttackHit?.Invoke(hitTarget, value);
    }
    public void StartShake()//调用摄像机抖动事件
    {
        OnCameraShake?.Invoke();
    }
    public void LevelUp()//升级调用选择Buff事件
    {
        OnOpenSelectUI?.Invoke();
    }
    public void TriggerEnemyKilled(GameObject enemy)
    {
        OnEnemyKilled?.Invoke(enemy);
    }
    public void AfterChoseCharacter(int ID)
    {
        OnSetCharactger?.Invoke(ID);
    }

}
