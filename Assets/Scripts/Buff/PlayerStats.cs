// PlayerStats.cs
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float attackPower = 10f;
    public float moveSpeed = 5f;

    public void ShowStats()
    {
        Debug.Log($"[PlayerStats] 当前攻击力: {attackPower}, 移动速度: {moveSpeed}");
    }
}
