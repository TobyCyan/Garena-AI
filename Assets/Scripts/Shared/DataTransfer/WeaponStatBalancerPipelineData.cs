using Player.Weapons;

public class WeaponStatBalancerPipelineData : BasePipelineData
{
    public WeaponStats WeaponStats { get; }

    public WeaponStatBalancerPipelineData(WeaponStats weaponStats)
    {
        WeaponStats = weaponStats;
    }
}