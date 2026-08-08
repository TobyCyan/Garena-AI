using Player.Weapons;
using System;
using UnityEngine;

public class WeaponStatsService
{
    public static WeaponStats GenerateWeaponStats(string prompt)
    {
        string weaponResult = StatsApiService
                    .GetStats(prompt)
                    .GetAwaiter()
                    .GetResult();
        return ParseToStats(weaponResult);
    }

    private static WeaponStats ParseToStats(string weaponResult)
    {
        if (string.IsNullOrWhiteSpace(weaponResult))
        {
            throw new InvalidOperationException(
                "Weapon stat generation returned an empty response.");
        }

        try
        {
            return JsonUtility.FromJson<WeaponStats>(weaponResult);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(
                $"Failed to parse weapon stats: {weaponResult}", e);
        }
    }

    public static WeaponStats BalanceWeaponStats(WeaponStats weaponStats)
    {
        // Implement weapon balancing logic here
        return weaponStats;
    }
}
