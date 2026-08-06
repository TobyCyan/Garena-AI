using System;
using System.Collections.Generic;
using Player.Core;
using UnityEngine;
using PlayerCore = Player.Core.Core;

namespace Player.Weapons
{
    [DisallowMultipleComponent]
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private List<Weapon> weapons = new();
        [SerializeField] private bool createFallbackWeapons = true;
        [SerializeField] private int startingWeaponIndex;

        public event Action<Weapon> OnWeaponChanged;

        public IReadOnlyList<Weapon> Weapons => weapons;
        public Weapon CurrentWeapon { get; private set; }
        public int CurrentIndex { get; private set; } = -1;

        private PlayerCore core;

        public void Initialize(PlayerCore playerCore)
        {
            core = playerCore;
            RefreshWeaponList();

            if (weapons.Count == 0 && createFallbackWeapons)
            {
                weapons.Add(WeaponFactory.CreateMeleeWeapon(
                    "Practice Sword", 1, new Rect(0.3f, -0.25f, 1f, 0.5f), transform));
                weapons.Add(WeaponFactory.CreateMeleeWeapon(
                    "Practice Hammer", 2, new Rect(0.2f, -0.35f, 1.25f, 0.7f), transform));
            }

            foreach (Weapon weapon in weapons)
            {
                weapon?.SetCore(core);
            }

            if (weapons.Count > 0)
            {
                Equip(Mathf.Clamp(startingWeaponIndex, 0, weapons.Count - 1));
            }
        }

        public void EquipNext()
        {
            if (weapons.Count > 0)
            {
                Equip((CurrentIndex + 1 + weapons.Count) % weapons.Count);
            }
        }

        public void EquipPrevious()
        {
            if (weapons.Count > 0)
            {
                Equip((CurrentIndex - 1 + weapons.Count) % weapons.Count);
            }
        }

        public void Equip(int index)
        {
            if (index < 0 || index >= weapons.Count || weapons[index] == null)
            {
                return;
            }

            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i] != null)
                {
                    weapons[i].gameObject.SetActive(i == index);
                }
            }

            CurrentIndex = index;
            CurrentWeapon = weapons[index];
            CurrentWeapon.SetCore(core);
            OnWeaponChanged?.Invoke(CurrentWeapon);
        }

        public Weapon BuildAndEquip(WeaponDataSO data, int slot = -1)
        {
            return AddAndEquip(WeaponFactory.CreateWeapon(data, transform), slot);
        }

        public Weapon BuildAndEquip(WeaponBuildRequest request, int slot = -1)
        {
            Weapon weapon = WeaponFactory.CreateWeapon(request, transform);
            return AddAndEquip(weapon, slot);
        }

        private Weapon AddAndEquip(Weapon weapon, int slot)
        {
            if (weapon == null)
            {
                return null;
            }

            weapon.SetCore(core);
            if (slot >= 0 && slot < weapons.Count)
            {
                Weapon oldWeapon = weapons[slot];
                weapons[slot] = weapon;
                if (oldWeapon != null)
                {
                    Destroy(oldWeapon.gameObject);
                }

                Equip(slot);
            }
            else
            {
                weapons.Add(weapon);
                Equip(weapons.Count - 1);
            }

            return weapon;
        }

        private void RefreshWeaponList()
        {
            weapons.RemoveAll(weapon => weapon == null);
            foreach (Weapon childWeapon in GetComponentsInChildren<Weapon>(true))
            {
                if (!weapons.Contains(childWeapon))
                {
                    weapons.Add(childWeapon);
                }
            }
        }
    }
}
