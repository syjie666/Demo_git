using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter : MonoBehaviour
{
    //����ģʽ ȫ��Ψһ
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
    public event Action<GameObject, int> OnRangedAttackHit;//����Զ�̹����¼� ���շ�
    public event Action<GameObject, int> OnAttackHit;//�����ս�����¼�
    public event Action OnCameraShake;//��������������¼�
    public event Action OnOpenSelectUI;//����ѡ��Buff�¼�
    public event Action<GameObject> OnEnemyKilled;//������������¼�
    public event Action <int>OnSetCharactger;//�������ɽ�ɫ�¼�
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
    public void RangedAttackeHit(GameObject hitTarget, int value)//����Զ�̹����¼�  ������
    {
        OnRangedAttackHit?.Invoke(hitTarget, value);
    }
    public void AttackHit(GameObject hitTarget, int value)//���ý�ս�����¼�
    {
        OnAttackHit?.Invoke(hitTarget, value);
    }
    public void StartShake()//��������������¼�
    {
        OnCameraShake?.Invoke();
    }
    public void LevelUp()//��������ѡ��Buff�¼�
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
