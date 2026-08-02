using System;
using UnityEngine;

namespace Player.Weapons
{
    [Serializable]
    public sealed class WeaponBuildRequest
    {
        public Sprite Image;
        [Min(1)] public int Damage = 1;
        public Vector2 Size = Vector2.one;
        [Min(0.1f)] public float Speed = 3f;

        public WeaponBuildRequest()
        {
        }

        public WeaponBuildRequest(Sprite image, int damage, Vector2 size, float speed)
        {
            Image = image;
            Damage = damage;
            Size = size;
            Speed = speed;
        }
    }
}
