// Buff_DoubleBoost.cs
public class Buff_DoubleBoost : IBuffEffect
{
    public void Apply(PlayerStats player)
    {
        player.attackPower *= 1.1f;
        player.moveSpeed += 1f;
        UnityEngine.Debug.Log("获得 Buff：攻击力+10%，速度+1");
    }
}
