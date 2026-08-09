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
        [SerializeField] private Transform weaponSocket;
        [SerializeField] private Transform weaponRaised;
        [SerializeField] private Transform weaponEnd;
        [SerializeField] private bool createFallbackWeapons = true;
        [SerializeField] private int startingWeaponIndex;

        public event Action<Weapon> OnWeaponChanged;

        public IReadOnlyList<Weapon> Weapons => weapons;
        public Weapon CurrentWeapon { get; private set; }
        public Weapon PendingWeapon { get; private set; }
        public int CurrentIndex { get; private set; } = -1;
        public Transform WeaponSocket => weaponSocket != null ? weaponSocket : transform;

        private PlayerCore core;

        public void Initialize(PlayerCore playerCore)
        {
            core = playerCore;
            RefreshWeaponList();

            if (weapons.Count == 0 && createFallbackWeapons)
            {
                weapons.Add(WeaponFactory.CreateMeleeWeapon(
                    "Practice Sword", null, new WeaponStats(1, new Vector2(1f, 0.5f), 1.6666667f), WeaponSocket));
                weapons.Add(WeaponFactory.CreateMeleeWeapon(
                    "Practice Hammer", null, new WeaponStats(2, new Vector2(1.25f, 0.7f), 1.25f), WeaponSocket));
            }

            foreach (Weapon weapon in weapons)
            {
                ConfigureWeapon(weapon);
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
            ConfigureWeapon(CurrentWeapon);
            OnWeaponChanged?.Invoke(CurrentWeapon);
        }

        public Weapon Build(WeaponDefinition definition)
        {
            if (definition == null)
            {
                return null;
            }

            DiscardPendingWeapon();
            Weapon weapon = WeaponFactory.CreateWeapon(definition, WeaponSocket);
            if (weapon == null)
            {
                return null;
            }

            ConfigureWeapon(weapon);
            weapon.gameObject.SetActive(false);
            PendingWeapon = weapon;
            return weapon;
        }

        public Weapon EquipBuiltWeapon(Weapon weapon = null, int slot = -1)
        {
            weapon ??= PendingWeapon;
            if (weapon == null)
            {
                return null;
            }

            Weapon equippedWeapon = AddAndEquip(weapon, slot);
            if (equippedWeapon != null && weapon == PendingWeapon)
            {
                PendingWeapon = null;
            }

            return equippedWeapon;
        }

        public void DiscardPendingWeapon()
        {
            if (PendingWeapon == null)
            {
                return;
            }

            Destroy(PendingWeapon.gameObject);
            PendingWeapon = null;
        }

        public Weapon BuildAndEquip(WeaponDefinition definition, int slot = -1)
        {
            Weapon weapon = Build(definition);
            return EquipBuiltWeapon(weapon, slot);
        }

        private Weapon AddAndEquip(Weapon weapon, int slot)
        {
            if (weapon == null)
            {
                return null;
            }

            weapon.transform.SetParent(WeaponSocket, false);
            WeaponFactory.ApplyRendererSorting(weapon, WeaponSocket);
            ConfigureWeapon(weapon);
            ApplyEquippedSpriteBoundsOffset(weapon);
            WeaponSwingAnimator swingAnimator =
                weapon.GetComponent<WeaponSwingAnimator>();
            swingAnimator?.SetKeepVisible(true);
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

        private static void ApplyEquippedSpriteBoundsOffset(Weapon weapon)
        {
            if (weapon == null || weapon.Image == null)
            {
                return;
            }

            SpriteRenderer spriteRenderer =
                weapon.GetComponentInChildren<SpriteRenderer>(true);
            if (spriteRenderer == null)
            {
                return;
            }

            spriteRenderer.enabled = true;
            spriteRenderer.gameObject.SetActive(true);
            Bounds spriteBounds = weapon.Image.bounds;
            spriteRenderer.transform.localPosition = new Vector3(
                -spriteBounds.min.x,
                -spriteBounds.center.y,
                0f);
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

        private void ConfigureWeapon(Weapon weapon)
        {
            if (weapon == null)
            {
                return;
            }

            weapon.SetCore(core);
            WeaponSwingAnimator swingAnimator = weapon.GetComponent<WeaponSwingAnimator>();
            if (swingAnimator != null)
            {
                swingAnimator.SetGripPath(WeaponSocket, weaponRaised, weaponEnd);
            }
        }
    }
}
