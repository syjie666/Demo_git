using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardCreate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("��������")]
    public Card_SO card_SO;
    public int createCount = 5;
    public float createRadius = 1.5f;

    [Header("�����")]
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

        // ��ʼ�������
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
        // ������ڿ���Pivot�ľ�ȷƫ����
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPointerPos
        );

        dragOffset = rectTransform.rect.center - localPointerPos;
        originalParent = transform.parent;

        // �������������л�������
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f; // ��קʱ��͸��
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ʹ��RectTransform�ľ�ȷ��ק
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPos))
        {
            rectTransform.localPosition = localPos + dragOffset;
        }
        Debug.Log($"�ͷ�ʱ�·�������: {eventData.pointerEnter?.name ?? "null"}");
        Debug.Log($"�������ǩ: {eventData.pointerEnter?.tag ?? "�ޱ�ǩ"}");
        // �� �µ�Tilemap����߼� ��
        canCrate = false;

        // �����λ��ת��Ϊ�������꣨2D��ϷZ=0��
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0;

        // ���Tilemap��ȷ��Tilemap��Collider2D��
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("DropZone"))
        {
            canCrate = true;
            createOffset = hit.point; // ʹ��ʵ����ײ������
            Debug.Log($"��⵽Tilemapλ��: {hit.point}"); // ������
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canCrate)
        {
            // �� �ؼ��޸ģ�ʵʱ��ȡ������������ ��
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0; // 2D��Ϸ��ҪZ=0
            CardCreateCharacter(worldPos);
            canCrate = false;
        }
        else
        {
            // �ָ�ԭʼ״̬
            transform.SetParent(originalParent, true);
            rectTransform.localPosition = Vector3.zero;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }
       
        
        
        
        
        // ��OnEndDrag����ӵ�����䣺
       
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // �ڱ༭������ʾ���ɰ뾶
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, createRadius);
    }
#endif
}