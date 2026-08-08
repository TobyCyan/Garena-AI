using Player.Weapons;

public record WeaponStatsData
{
    public WeaponStats Stats { get; }
    public WeaponCategory Category { get; }

    public WeaponStatsData(WeaponStats stats, WeaponCategory category)
    {
        Stats = stats;
        Category = category;
    }
}