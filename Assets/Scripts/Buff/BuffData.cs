using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "Roguelike/Buff")]
public class BuffData : ScriptableObject
{
    public string buffName;
    public string description;
    public string effectClassName;  // 这里以后可以用来实现具体效果
}
