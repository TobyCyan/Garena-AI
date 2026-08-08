using Player.Weapons;

public class WeaponStatsPipelineData : BasePipelineData
{
    public WeaponStatsData WeaponStatsData { get; }
    public string Prompt { get; }

    public WeaponStatsPipelineData(WeaponStatsData weaponStatsData, string prompt)
    {
        WeaponStatsData = weaponStatsData;
        Prompt = prompt;
    }
}