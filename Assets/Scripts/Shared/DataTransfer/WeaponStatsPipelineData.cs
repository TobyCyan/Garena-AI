using Player.Weapons;

public class WeaponStatsPipelineData : BasePipelineData
{
    public WeaponStatsData WeaponStatsData { get; }

    public WeaponStatsPipelineData(WeaponStatsData weaponStatsData)
    {
        WeaponStatsData = weaponStatsData;
    }
}