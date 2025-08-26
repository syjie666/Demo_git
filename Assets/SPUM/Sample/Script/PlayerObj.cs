using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


public class PlayerObj : MonoBehaviour
{
    public SPUM_Prefabs _prefabs;
    public GameObject fireBallPrefab;
    public Transform lunchPos;
    public Slider healthSlider;
    public Slider expSlider;
    public AttackerCharacter_SO template_SO;
    public AttackerCharacter_SO aC_SO;
    public float _charMS;
    private PlayerState _currentState;
    public Vector3 _goalPos;
    public bool isAction = false;
    public Dictionary<PlayerState, int> IndexPair = new();
    private bool isActtacking;
    private int level;
    private float maxHealth=100;
    private int damagedValue;
    void Start()
    {

        
        if (_prefabs == null)
        {
            _prefabs = transform.GetChild(0).GetComponent<SPUM_Prefabs>();
            if (!_prefabs.allListsHaveItemsExist()) {
                _prefabs.PopulateAnimationLists();
            }
        }
        _prefabs.OverrideControllerInit();
        if(template_SO == null)
        {
            Debug.Log("没有赋值攻方角色Data");
        }
        else
        {
            aC_SO =Instantiate( template_SO);
        }
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            IndexPair[state] = 0;
        }
        
        healthSlider.maxValue = aC_SO.maxHelth;//血条初始化
        healthSlider.value = healthSlider.maxValue;
        expSlider.maxValue=aC_SO.maxExp;//经验条初始化
        expSlider.value = 0;
        level = aC_SO.Level;//等级初始化
    }
    private void OnEnable()
    {
        EventCenter.instance.OnRangedAttackHit += HandleRangedAttack;
        EventCenter.instance.OnAttackHit += HandleAttack;
    }

    

    private void OnDisable()
    {
        EventCenter.instance.OnRangedAttackHit -= HandleRangedAttack;
        EventCenter.instance.OnAttackHit -= HandleAttack;
    }



    public void SetStateAnimationIndex(PlayerState state, int index = 0) {
        IndexPair[state] = index;
    }//设置角色状态并放入字典
    public void PlayStateAnimation(PlayerState state) {
        _prefabs.PlayAnimation(state, IndexPair[state]);
    }//传入状态参数，通过状态参数播放对应状态动画
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("按下鼠标键");
            _currentState = PlayerState.ATTACK;
        }
        // 键盘输入检测
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");


        if (h != 0 || v != 0)
        {
            _currentState = PlayerState.MOVE;
            _goalPos = transform.position + new Vector3(h, v, 0) * 5f;
        }
        else if (_currentState == PlayerState.MOVE) // 新增：按键松开时立即停止
        {
            _currentState = PlayerState.IDLE;
            _goalPos = transform.position; // 将目标位置设为当前位置
        }

        // 移动和动画逻辑
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
            default:
                PlayStateAnimation(_currentState);
                break;
        }
    }

    private void DoDamaged()
    {
        PlayStateAnimation(PlayerState.DAMAGED);
        healthSlider.value -= damagedValue;
        damagedValue = 0;
        Debug.Log("血量"+healthSlider.value);
        if(healthSlider.value > 0)
        {
            _currentState = PlayerState.IDLE;
            EventCenter.instance.StartShake();
        }
        
        else
        {
            SceneTransform sceneTransform = new SceneTransform();
            sceneTransform.SceneTransform_("Preparation");
        }
    }

    void DoMove() //完成移动逻辑
    {
        Vector3 _dirVec = _goalPos - transform.position;
        if (_dirVec.sqrMagnitude < 0.1f)
        {
            _currentState = PlayerState.IDLE;
            return;
        }

        Vector3 _dirMVec = _dirVec.normalized;
        transform.position += _dirMVec * _charMS * Time.deltaTime;

        // 方向判断优先级：左右 > 上下
        if (Mathf.Abs(_dirMVec.x) > Mathf.Abs(_dirMVec.y))
        {
            // 水平移动为主
            _prefabs.transform.localScale = new Vector3(
                _dirMVec.x > 0 ? -1 : 1,
                1,
                1
            );
        }
        

        PlayStateAnimation(_currentState);
    }

    public void SetMovePos(Vector2 pos)
    {
        isAction = false;
        _goalPos = pos;
        _currentState = PlayerState.MOVE;
    }

    void DoAttack()
    {
        if (isActtacking) return;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(mousePosition.x >transform.position.x)//判断鼠标是在角色的右侧还是左侧
        {
            _prefabs.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            _prefabs.transform.localScale = new Vector3(1, 1, 1);
        }
        if (lunchPos == null)
        {
            Debug.Log("没有获取到发射位置");
            return;
        }
        if (fireBallPrefab == null)
        {
            Debug.Log("没有获取到火球预制体");
            return;
        }

        _prefabs.PlayAnimation(PlayerState.ATTACK, 0);
        StartCoroutine(FireToMouse());
        _currentState = PlayerState.IDLE;
    }

    IEnumerator FireToMouse()
    {
        //Debug.Log("发射");
        isActtacking = true;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 fireDirection = (mousePosition - lunchPos.position).normalized;
        // 计算旋转角度（使FireBallprefab朝向目标方向）
        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg + 90f;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        yield return new WaitForSeconds(0.8f);
        GameObject Fire = Instantiate(fireBallPrefab, lunchPos.position, rotation);
        Fire.GetComponent<RangedAttack>().attackValue =aC_SO.attackDamage;
        Fire.GetComponent<Rigidbody2D>().AddForceAtPosition(fireDirection * 1000, mousePosition);
        isActtacking = false;
       
    }
    public void PickUpEXP()
    {
        expSlider.value += 5;
        if(expSlider.value >=aC_SO.maxExp)
        {
            expSlider.value -= aC_SO.maxExp;
           
            aC_SO.Level += 1;
            EventCenter.instance.LevelUp();//通知事件中心升级了
            
            aC_SO.maxExp = aC_SO.Level * 10;
            expSlider.maxValue = aC_SO.maxExp;


        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("EXP"))
        {
            //Debug.Log("pick");
            PickUpEXP();
            collision.gameObject.SetActive(false);
        }
    }

    public void HandleRangedAttack(GameObject target, int value)
    {
        if (target == gameObject)
        {
           // Debug.Log("打到玩家");
            _currentState = PlayerState.DAMAGED;
            damagedValue = value;
        }
            
    }
    public void HandleAttack(GameObject target, int value)
    {
        if (target == gameObject)
        {
            //Debug.Log("打到玩家");
            _currentState = PlayerState.DAMAGED;
            damagedValue = value;
        }
       
    }

    

}
