using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public GameObject buffCardPrefab; // 你生成的BuffCard预制体
    public Transform cardParent;      // 卡牌的父物体（UI）
    public GameObject panelRoot;      // 弹窗根节点Panel
    public Text countText; //倒计时text
    public float seconds=3f;//倒计时秒速
    public List<BuffData> allBuffs;

    private List<GameObject> activeCards = new List<GameObject>();

    void Start()
    {
        panelRoot.SetActive(false);
    }
    private void OnEnable()
    {
        EventCenter.instance.OnOpenSelectUI += ShowLevelUpOptions;
    }
    private void OnDisable()
    {
        EventCenter.instance.OnOpenSelectUI -= ShowLevelUpOptions;
    }

   

    public void ShowLevelUpOptions()
    {
        panelRoot.SetActive(true);

        // 清理旧卡牌
        foreach (var c in activeCards)
            Destroy(c);
        activeCards.Clear();

        // 随机选三张卡牌
        var selectedBuffs = new List<BuffData>();
        for (int i = 0; i < 3; i++)
        {
            if (allBuffs.Count == 0) break;
            var buff = allBuffs[Random.Range(0, allBuffs.Count)];
            selectedBuffs.Add(buff);
        }

        // 创建卡牌UI
        foreach (var buff in selectedBuffs)
        {
            var cardGO = Instantiate(buffCardPrefab, cardParent);
            var ui = cardGO.GetComponent<BuffCardUI>();
            ui.SetBuffData(buff);
            activeCards.Add(cardGO);

            var btn = cardGO.GetComponent<Button>();
            btn.onClick.AddListener(() => OnCardSelected(buff));
        }
        StartCoroutine(StopGame());
    }
    IEnumerator StopGame()
    {
        Time.timeScale = 0;
        float remainingTime = seconds;
        while (remainingTime > 0)
        {
            countText.text = Mathf.Ceil(remainingTime).ToString();
            yield return null;
            remainingTime -= Time.unscaledDeltaTime;
        }
        countText.text = "结束";
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        panelRoot.SetActive(false);
    }

    private void OnCardSelected(BuffData buff)
    {
        Debug.Log("选择了卡牌：" + buff.buffName);
        StopCoroutine(StopGame());
        Time.timeScale = 1;
        panelRoot.SetActive(false);

        // TODO: 应用buff的效果
    }
}
