using UnityEngine;

namespace Player.Weapons
{
    public static class WeaponFactory
    {
        public static Weapon CreateWeapon(WeaponDataSO data, Transform parent)
        {
            if (data == null)
            {
                return null;
            }

            GameObject root = new(data.WeaponName);
            root.transform.SetParent(parent, false);

            GameObject baseObject = new("Base");
            baseObject.transform.SetParent(root.transform, false);
            baseObject.AddComponent<Animator>();
            baseObject.AddComponent<AnimationEventHandler>();

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
            weapon.SetData(data);

            if (data.Image != null)
            {
                Bounds spriteBounds = data.Image.bounds;
                spriteObject.transform.localPosition = new Vector3(
                    -spriteBounds.min.x,
                    -spriteBounds.center.y,
                    0f);
            }

            if (data.AnimatorController == null)
            {
                WeaponSwingAnimator swingAnimator = root.AddComponent<WeaponSwingAnimator>();
                swingAnimator.SetPivot(baseObject.transform);
            }

            return weapon;
        }

        public static Weapon CreateWeapon(WeaponBuildRequest request, Transform parent)
        {
            return CreateWeapon(WeaponDataSO.CreateRuntime(request), parent);
        }

        public static Weapon CreateMeleeWeapon(string displayName, int damage, Rect hitBox, Transform parent)
        {
            WeaponDataSO data = WeaponDataSO.CreateRuntimeMelee(displayName, damage, hitBox);
            return CreateWeapon(data, parent);
        }

        public static Weapon CreateMeleeWeapon(Sprite image, int damage, Vector2 size, float speed, Transform parent)
        {
            return CreateWeapon(new WeaponBuildRequest(image, damage, size, speed), parent);
        }
    }
}
