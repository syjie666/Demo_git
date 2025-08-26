#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class Editor_CreateExampleScene
{
    [MenuItem("Tools/RoguelikeBuff/Create Example Scene and Assets")]
    public static void CreateSceneAndAssets()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "RoguelikeBuffDemoScene";

        // 创建摄像机
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 0, -10);

        // 创建玩家（如果你想自己管理玩家数据可以手动加PlayerStats，这里不强制）
        var playerGO = new GameObject("Player");
        // var playerStats = playerGO.AddComponent<PlayerStats>(); // 删除这行，因LevelUpManager无player字段

        // 创建Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // 创建LevelUpPanel
        var panelGO = new GameObject("LevelUpPanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        var panelRect = panelGO.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(800, 300);
        var panelImg = panelGO.AddComponent<UnityEngine.UI.Image>();
        panelImg.color = new Color(0, 0, 0, 0.6f);

        // 创建CardParent
        var cardParentGO = new GameObject("CardParent");
        cardParentGO.transform.SetParent(panelGO.transform, false);
        var cardParentRect = cardParentGO.AddComponent<RectTransform>();
        cardParentRect.anchorMin = new Vector2(0.1f, 0.1f);
        cardParentRect.anchorMax = new Vector2(0.9f, 0.9f);
        cardParentRect.anchoredPosition = Vector2.zero;
        cardParentRect.sizeDelta = Vector2.zero;

        // 创建BuffCardPrefab（带UI组件）
        var buffCardGO = new GameObject("BuffCardPrefab");
        var buffCardRect = buffCardGO.AddComponent<RectTransform>();
        buffCardRect.sizeDelta = new Vector2(220, 260);
        var img = buffCardGO.AddComponent<UnityEngine.UI.Image>();
        img.color = Color.white;
        var btn = buffCardGO.AddComponent<UnityEngine.UI.Button>();
        var buffCardUI = buffCardGO.AddComponent<BuffCardUI>();

        // 创建Title文本
        var titleGO = new GameObject("Title");
        titleGO.transform.SetParent(buffCardGO.transform, false);
        var titleText = titleGO.AddComponent<UnityEngine.UI.Text>();
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.alignment = TextAnchor.UpperCenter;
        titleText.color = Color.white;  // 文字颜色白色，方便看到
        var titleRect = titleText.rectTransform;
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.anchoredPosition = new Vector2(0, -10);
        titleRect.sizeDelta = new Vector2(0, 40); // 横向拉满，40高

        // 创建Desc文本
        var descGO = new GameObject("Desc");
        descGO.transform.SetParent(buffCardGO.transform, false);
        var descText = descGO.AddComponent<UnityEngine.UI.Text>();
        descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        descText.alignment = TextAnchor.MiddleCenter;
        descText.color = Color.white;  // 白色文字
        var descRect = descText.rectTransform;
        descRect.anchorMin = new Vector2(0, 0);
        descRect.anchorMax = new Vector2(1, 0.8f);
        descRect.pivot = new Vector2(0.5f, 0);
        descRect.anchoredPosition = new Vector2(0, 10);
        descRect.sizeDelta = new Vector2(0, 0);


        // 赋值BuffCardUI引用
        buffCardUI.title = titleText;
        buffCardUI.desc = descText;
        // 不用赋icon，因BuffCardUI无此字段

        // 保存prefab到Assets/Resources
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        string prefabPath = "Assets/Resources/BuffCardPrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(buffCardGO, prefabPath);
        GameObject.DestroyImmediate(buffCardGO);

        // 创建LevelUpManager，赋值引用
        var managerGO = new GameObject("LevelUpManager");
        var manager = managerGO.AddComponent<LevelUpManager>();
        manager.panelRoot = panelGO;
        manager.cardParent = cardParentGO.transform;

        // 通过Resources加载Prefab引用
        manager.buffCardPrefab = Resources.Load<GameObject>("BuffCardPrefab");

        // 保存场景
        EditorSceneManager.SaveScene(scene, "Assets/RoguelikeBuffDemoScene.unity");

        // 创建示例BuffData资源
        if (!AssetDatabase.IsValidFolder("Assets/ExampleBuffs"))
            AssetDatabase.CreateFolder("Assets", "ExampleBuffs");

        CreateBuffAsset("AttackBoost", "攻击力提升 20%", "Assets/ExampleBuffs/AttackBoost.asset", "Buff_AttackBoost");
        CreateBuffAsset("SpeedBoost", "移动速度 +2", "Assets/ExampleBuffs/SpeedBoost.asset", "Buff_SpeedBoost");
        CreateBuffAsset("DoubleBoost", "攻击力+10% & 速度+1", "Assets/ExampleBuffs/DoubleBoost.asset", "Buff_DoubleBoost");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("RoguelikeBuff", "示例场景、Prefab与 Buff 资源创建完成。\n\n" +
            "打开场景后，确认 LevelUpManager.buffCardPrefab 已自动加载。\n" +
            "请在 LevelUpManager.allBuffs 列表中添加 ExampleBuffs 文件夹下的 BuffData。\n" +
            "运行场景，调用 LevelUpManager.ShowLevelUpOptions() 测试。", "知道了");
    }

    static void CreateBuffAsset(string name, string desc, string path, string className)
    {
        var buff = ScriptableObject.CreateInstance<BuffData>();
        buff.buffName = name;
        buff.description = desc;
        buff.effectClassName = className;
        AssetDatabase.CreateAsset(buff, path);
    }
}
#endif
