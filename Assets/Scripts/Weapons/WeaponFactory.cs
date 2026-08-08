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
            SpriteRenderer weaponRenderer = spriteObject.AddComponent<SpriteRenderer>();
            SpriteRenderer playerRenderer = parent != null
                ? parent.GetComponentInParent<SpriteRenderer>()
                : null;
            if (playerRenderer != null)
            {
                weaponRenderer.sortingLayerID = playerRenderer.sortingLayerID;
                weaponRenderer.sortingOrder = playerRenderer.sortingOrder + 1;
            }

            Weapon weapon = root.AddComponent<Weapon>();
            weapon.Configure(definition);

            if (definition.Image != null)
            {
                Bounds spriteBounds = definition.Image.bounds;
                spriteObject.transform.localPosition = new Vector3(
                    -spriteBounds.min.x,
                    -spriteBounds.center.y,
                    0f);
            }

            WeaponSwingAnimator swingAnimator = root.AddComponent<WeaponSwingAnimator>();
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
                new WeaponDefinition(name, WeaponCategory.Melee, image, stats),
                parent);
        }
    }
}
