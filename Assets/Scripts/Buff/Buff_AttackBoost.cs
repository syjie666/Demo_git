// Buff_AttackBoost.cs
public class Buff_AttackBoost : IBuffEffect
{
    public void Apply(PlayerStats player)
    {
        player.attackPower *= 1.2f; // 攻击力+20%
        UnityEngine.Debug.Log("获得 Buff：攻击力提升 20%");
    }
}
