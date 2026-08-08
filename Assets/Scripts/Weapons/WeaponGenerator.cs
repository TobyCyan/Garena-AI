using UnityEngine;

namespace Player.Weapons
{
    public class WeaponGenerator : MonoBehaviour
    {
        [SerializeField] private Transform weaponParent;
        public static WeaponGenerator Instance;

        private void Awake()
        {
            Instance = this;
        }

        public Weapon BuildWeapon(WeaponDataSO data)
        {
            return WeaponFactory.CreateWeapon(data, weaponParent != null ? weaponParent : transform);
        }

        public Weapon BuildWeapon(WeaponBuildRequest request)
        {
            return WeaponFactory.CreateWeapon(request, weaponParent != null ? weaponParent : transform);
        }

        public Weapon BuildMeleeWeapon(string displayName, int damage, Rect hitBox)
        {
            return WeaponFactory.CreateMeleeWeapon(
                displayName, damage, hitBox, weaponParent != null ? weaponParent : transform);
        }

        public Weapon BuildMeleeWeapon(Sprite image, int damage, Vector2 size, float speed)
        {
            return WeaponFactory.CreateMeleeWeapon(
                image, damage, size, speed, weaponParent != null ? weaponParent : transform);
        }
    }
}
