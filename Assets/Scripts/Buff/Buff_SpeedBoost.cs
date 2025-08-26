// Buff_SpeedBoost.cs
public class Buff_SpeedBoost : IBuffEffect
{
    public void Apply(PlayerStats player)
    {
        player.moveSpeed += 2f; // 速度+2
        UnityEngine.Debug.Log("获得 Buff：移动速度 +2");
    }
}
