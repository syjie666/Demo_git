using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardCreate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("卡牌配置")]
    public Card_SO card_SO;
    public int createCount = 5;
    public float createRadius = 1.5f;

    [Header("对象池")]
    public Queue<GameObject> pool = new Queue<GameObject>();

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 dragOffset;
    private Vector3 createOffset;
    private Transform originalParent;
    private GameObject prefab;
    private bool canCrate;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        prefab = card_SO._prefab;

        // 初始化对象池
        for (int i = 0; i < 30; i++)
        {
            GameObject character = Instantiate(prefab);
            character.SetActive(false);
            pool.Enqueue(character);
        }
    }

    public void CardCreateCharacter(Vector3 pos)
    {
        for (int i = 0; i < createCount; i++)
        {
            if (pool.Count > 0)
            {
                GameObject character = pool.Dequeue();
                ResetCharacter(character);
                Vector2 offset = Random.insideUnitCircle * createRadius;
                pos.z = 0;
                character.transform.position = pos + (Vector3)offset;
                character.SetActive(true);
            }
        }
        Destroy(gameObject);
    }
    public void ResetCharacter(GameObject character )
    {
        if (character.GetComponent<DefensiveObj>() != null)
        {
            character.GetComponent<DefensiveObj>().ResetValue();
        }
        else return;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 计算基于卡牌Pivot的精确偏移量
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPointerPos
        );

        dragOffset = rectTransform.rect.center - localPointerPos;
        originalParent = transform.parent;

        // 保持世界坐标切换父物体
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f; // 拖拽时半透明
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 使用RectTransform的精确拖拽
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPos))
        {
            rectTransform.localPosition = localPos + dragOffset;
        }
        Debug.Log($"释放时下方的物体: {eventData.pointerEnter?.name ?? "null"}");
        Debug.Log($"该物体标签: {eventData.pointerEnter?.tag ?? "无标签"}");
        // ▼ 新的Tilemap检测逻辑 ▼
        canCrate = false;

        // 将鼠标位置转换为世界坐标（2D游戏Z=0）
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0;

        // 检测Tilemap（确保Tilemap有Collider2D）
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("DropZone"))
        {
            canCrate = true;
            createOffset = hit.point; // 使用实际碰撞点坐标
            Debug.Log($"检测到Tilemap位置: {hit.point}"); // 调试用
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canCrate)
        {
            // ▼ 关键修改：实时获取鼠标的世界坐标 ▼
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0; // 2D游戏需要Z=0
            CardCreateCharacter(worldPos);
            canCrate = false;
        }
        else
        {
            // 恢复原始状态
            transform.SetParent(originalParent, true);
            rectTransform.localPosition = Vector3.zero;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }
       
        
        
        
        
        // 在OnEndDrag中添加调试语句：
       
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // 在编辑器中显示生成半径
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, createRadius);
    }
#endif
}