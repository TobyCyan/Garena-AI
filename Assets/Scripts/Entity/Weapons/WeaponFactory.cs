using UnityEngine;

namespace Player.Weapons
{
    public static class WeaponFactory
    {
        public static Weapon CreateWeapon(WeaponDefinition definition, Transform parent)
        {
            if (definition == null)
            {
                return null;
            }

            GameObject root = new(definition.Name);
            root.transform.SetParent(parent, false);

            GameObject baseObject = new("Base");
            baseObject.transform.SetParent(root.transform, false);

            GameObject spriteObject = new("WeaponSprite");
            spriteObject.transform.SetParent(baseObject.transform, false);
            spriteObject.transform.localPosition = Vector3.zero;
            spriteObject.SetActive(true);
            
            SpriteRenderer weaponRenderer = spriteObject.AddComponent<SpriteRenderer>();
            weaponRenderer.enabled = true;

            Weapon weapon = root.AddComponent<Weapon>();
            weapon.Configure(definition);

            MeleeWeaponSwingAnimator swingAnimator = root.AddComponent<MeleeWeaponSwingAnimator>();
            swingAnimator.SetPivot(baseObject.transform);
            return weapon;
        }

        public static Weapon CreateMeleeWeapon(
            string name,
            Sprite image,
            WeaponStats stats,
            Transform parent)
        {
            return CreateWeapon(
                new WeaponDefinition(name, WeaponCategory.Sword, image, stats),
                parent);
        }
    }
}
