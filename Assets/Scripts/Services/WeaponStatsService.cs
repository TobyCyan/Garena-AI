using Player.Weapons;
using System;
using UnityEngine;

public class WeaponStatsService
{
    public static WeaponStatsData GenerateWeaponStats(string description)
    {
        string weaponResult = StatsApiService
            .GetStats(description)
            .GetAwaiter()
            .GetResult();

        return ParseToStats(weaponResult);
    }

    private static WeaponStatsData ParseToStats(string weaponResult)
    {
        if (string.IsNullOrWhiteSpace(weaponResult))
        {
            throw new InvalidOperationException(
                "Weapon stat generation returned an empty response.");
        }

        try
        {
            WeaponStatsResponse response = JsonUtility.FromJson<WeaponStatsResponse>(weaponResult);
            ValidateResponse(response);

            return new WeaponStatsData(
                GetStats(response),
                ParseWeaponCategory(response.type)
            );
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(
                $"Failed to parse weapon stats: {weaponResult}",
                e);
        }
    }

    private static WeaponStats GetStats(WeaponStatsResponse response)
    {
        return new WeaponStats(
            response.damage,
            new Vector2(response.size, response.size),
            response.swing_speed);
    }

    private static void ValidateResponse(WeaponStatsResponse response)
    {
        if (response == null)
        {
            throw new NullReferenceException("Weapon stats response is null.");
        }

        if (response.damage is < 1 or > 10 ||
            response.size is < 1 or > 10 ||
            response.swing_speed is < 1 or > 10)
        {
            throw new InvalidOperationException(
                "Weapon stats must be between 1 and 10.");
        }

        if (response.damage + response.size + response.swing_speed > 20)
        {
            throw new InvalidOperationException(
                "Weapon stat total cannot exceed 20.");
        }
    }

    private static WeaponCategory ParseWeaponCategory(string type)
    {
        if (Enum.TryParse(
                type,
                ignoreCase: true,
                out WeaponCategory weaponType))
        {
            return weaponType;
        }

        throw new InvalidOperationException(
            $"Unknown weapon type: {type}");
    }
}

[Serializable]
public class WeaponStatsResponse
{
    public int damage;
    public int size;
    public int swing_speed;
    public string type;
}
