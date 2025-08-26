using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveObj : MonoBehaviour, IPoolable
{
    public GameObject fireBallPrefab;
    public Transform lunchPos;
    public GameObject EXPPrefab;

    private SPUM_Prefabs _prefabs;
    private PlayerState _currentState = PlayerState.MOVE;
    private Rigidbody2D rb;
    private Transform attackTarget;
    public Dictionary<PlayerState, int> IndexPair = new();

    [SerializeField]
    private DefensiveCharacter_SO dC_SO;
    private float moveSpeed = 1f;
    private float maxHealth = 100;
    private float currentHealth = 100;
    private int attackDamage;
    private float attackRange;

    private float attackCooldown = 1f; // �������
    private float lastAttackTime;
    private int damagedValue;

    private Coroutine deathCoroutine;

    private void OnEnable()
    {
        EventCenter.instance.OnRangedAttackHit += RangedDamaged;
    }

    private void OnDisable()
    {
        EventCenter.instance.OnRangedAttackHit -= RangedDamaged;
    }

    void Start()
    {
        if (_prefabs == null)
        {
            _prefabs = transform.GetChild(0).GetComponent<SPUM_Prefabs>();
            if (!_prefabs.allListsHaveItemsExist())
            {
                _prefabs.PopulateAnimationLists();
            }
        }
        _prefabs.OverrideControllerInit();

        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            IndexPair[state] = 0;
        }

        rb = GetComponent<Rigidbody2D>();
        attackTarget = GameObject.FindGameObjectWithTag("Player").transform;

        moveSpeed = dC_SO.moveSpeed;
        maxHealth = dC_SO.maxHelth;
        attackDamage = dC_SO.attackDamage;
        currentHealth = maxHealth;
    }

    public void SetStateAnimationIndex(PlayerState state, int index = 0)
    {
        IndexPair[state] = index;
    }

    public void PlayStateAnimation(PlayerState state)
    {
        _prefabs.PlayAnimation(state, IndexPair[state]);
    }

    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case PlayerState.MOVE:
                DoMove();
                break;
            case PlayerState.ATTACK:
                DoAttack();
                break;
            case PlayerState.DAMAGED:
                DoDamaged();
                break;
            case PlayerState.DEATH:
                // ����״̬������
                break;
            default:
                PlayStateAnimation(_currentState);
                break;
        }
    }

    // �ӿ�ʵ�� - ����ʱ����
    public void OnSpawn()
    {
        ResetValue();
        _currentState = PlayerState.MOVE;
        gameObject.SetActive(true);

        if (deathCoroutine != null)
        {
            StopCoroutine(deathCoroutine);
            deathCoroutine = null;
        }
    }

    // �ӿ�ʵ�� - ����ʱ����
    public void OnDespawn()
    {
        gameObject.SetActive(false);
        rb.velocity = Vector2.zero;
        damagedValue = 0;
        lastAttackTime = 0;
    }

    public void DoDeth()
    {
        if (deathCoroutine == null)
            deathCoroutine = StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        _currentState = PlayerState.DEATH;
        PlayStateAnimation(PlayerState.DEATH);
        yield return new WaitForSeconds(1f); // �ȴ�����1�룬���㶯�����ȸ�

        // ���ɾ�����ʹ�ö��������
        PoolManager.Instance.Spawn("EXP", transform.position, Quaternion.identity);
        // �������������¼���֪ͨˢ������
        EventCenter.instance.TriggerEnemyKilled(gameObject);

        // �����Լ�
        PoolManager.Instance.Despawn(gameObject);
        deathCoroutine = null;
    }

    private void DoDamaged()
    {
        PlayStateAnimation(PlayerState.DAMAGED);
        currentHealth -= damagedValue;
        damagedValue = 0;

        if (currentHealth > 0)
        {
            _currentState = PlayerState.MOVE;
        }
        else
        {
            DoDeth();
        }
    }

    public void DoAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        var distance = Physics2D.Distance(attackTarget.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        if (distance.distance > dC_SO.attackRange) // ��ս
        {
            _currentState = PlayerState.MOVE;
            return;
        }

        _prefabs.PlayAnimation(PlayerState.ATTACK, 0);
        EventCenter.instance.AttackHit(attackTarget.gameObject, attackDamage);
        _currentState = PlayerState.MOVE;
    }

    private void DoMove()
    {
        Vector2 direction = (attackTarget.position - transform.position).normalized;

        if (attackTarget.position.x > transform.position.x)
        {
            _prefabs.transform.localScale = new Vector3(-5, 5, 5);
        }
        else
        {
            _prefabs.transform.localScale = new Vector3(5, 5, 5);
        }

        var distance = Physics2D.Distance(attackTarget.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        if (distance.distance < dC_SO.attackRange) // ���빥����ΧתΪ����״̬
        {
            _currentState = PlayerState.ATTACK;
        }

        rb.velocity = direction * moveSpeed;
    }

    private void RangedDamaged(GameObject target, int value)
    {
        if (target.GetComponent<Collider2D>() == GetComponent<Collider2D>())
        {
            _currentState = PlayerState.DAMAGED;
            damagedValue = value;
        }
    }

    public void ResetValue()
    {
        moveSpeed = dC_SO.moveSpeed;
        maxHealth = dC_SO.maxHelth;
        currentHealth = maxHealth;
        attackDamage = dC_SO.attackDamage;
        damagedValue = 0;
        lastAttackTime = 0;
    }
}
