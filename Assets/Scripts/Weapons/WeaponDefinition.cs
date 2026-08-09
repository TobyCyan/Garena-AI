using System;
using UnityEngine;

namespace Player.Weapons
{
    public enum WeaponCategory
    {
        Sword,
        Mace,
        Spear,
        Dagger,
    }

    [Serializable]
    public sealed class WeaponStats
    {
        [Min(1)] public int Damage = 1;
        public Vector2 Size = Vector2.one;
        [Min(0.1f)] public float Speed = 1.6666667f;

        public WeaponStats()
        {
        }

        public WeaponStats(int damage, Vector2 size, float speed)
        {
            Damage = Mathf.Max(1, damage);
            Size = new Vector2(Mathf.Max(0.05f, size.x), Mathf.Max(0.05f, size.y));
            Speed = Mathf.Max(0.1f, speed);
        }

        public WeaponStats CopySanitized()
        {
            // TODO: Apply generated size to the sprite and hitbox separately once animation-safe sizing is implemented.
            return new WeaponStats(Damage, Vector2.one, Speed);
        }
    }

    [Serializable]
    public sealed class WeaponDefinition
    {
        public string Name = "AI Weapon";
        public WeaponCategory Category = WeaponCategory.Sword;
        public Sprite Image;
        public WeaponStats Stats = new();

        public WeaponDefinition()
        {
        }

        public WeaponDefinition(
            string name,
            WeaponCategory category,
            Sprite image,
            WeaponStats stats)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "AI Weapon" : name;
            Category = category;
            Image = image;
            Stats = stats?.CopySanitized() ?? new WeaponStats();
        }
    }
}
