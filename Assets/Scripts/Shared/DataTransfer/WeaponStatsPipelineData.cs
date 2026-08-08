using Player.Weapons;

public class WeaponStatsPipelineData : BasePipelineData
{
    public WeaponStats WeaponStats { get; }

    public WeaponStatsPipelineData(WeaponStats weaponStats)
    {
        WeaponStats = weaponStats;
    }
}