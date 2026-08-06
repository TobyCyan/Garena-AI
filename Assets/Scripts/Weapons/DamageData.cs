using UnityEngine;

namespace Player.Weapons
{
    public readonly struct DamageData
    {
        public int Amount { get; }
        public GameObject Source { get; }
        public WeaponDataSO WeaponData { get; }

        public DamageData(int amount, GameObject source, WeaponDataSO weaponData)
        {
            Amount = amount;
            Source = source;
            WeaponData = weaponData;
        }
    }
}
