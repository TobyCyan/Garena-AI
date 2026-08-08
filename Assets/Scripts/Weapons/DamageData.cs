using UnityEngine;

namespace Player.Weapons
{
    public readonly struct DamageData
    {
        public int Amount { get; }
        public GameObject Source { get; }
        public Weapon Weapon { get; }

        public DamageData(int amount, GameObject source, Weapon weapon)
        {
            Amount = amount;
            Source = source;
            Weapon = weapon;
        }
    }
}
