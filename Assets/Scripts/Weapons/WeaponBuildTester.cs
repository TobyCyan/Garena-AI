using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Player.Weapons
{
    [DefaultExecutionOrder(110)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(global::Player.PlayerController))]
    public sealed class WeaponBuildTester : MonoBehaviour
    {
        [SerializeField] private bool buildGeneratedSpritesOnStart;
        [SerializeField] private WeaponCategory generatedSpriteCategory = WeaponCategory.Sword;
        [SerializeField] private int generatedSpriteDamage = 3;
        [SerializeField] private float generatedSpriteSpeed = 1.5f;
        [SerializeField, Min(0)] private int maxGeneratedSprites;

        private global::Player.PlayerController player;

        private void Awake()
        {
            player = GetComponent<global::Player.PlayerController>();
        }

        private void Start()
        {
            if (buildGeneratedSpritesOnStart)
            {
                BuildGeneratedSpriteWeapons();
            }
        }

        [ContextMenu("Build Generated Sprite Weapons During Play Mode")]
        private void BuildGeneratedSpriteWeapons()
        {
            if (!CanBuild())
            {
                return;
            }

            List<WeaponDefinition> generatedWeapons = LoadGeneratedSpriteWeapons();
            int builtCount = 0;
            foreach (WeaponDefinition definition in generatedWeapons)
            {
                if (player.WeaponInventory.BuildAndEquip(definition) != null)
                {
                    builtCount++;
                }
            }

            Debug.Log($"[WeaponBuildTester] Built {builtCount} generated sprite weapons.", this);
        }

        [ContextMenu("Equip Latest Generated Sprite During Play Mode")]
        private void EquipLatestGeneratedSprite()
        {
            if (!CanBuild())
            {
                return;
            }

            WeaponDefinition latestGeneratedWeapon = LoadGeneratedSpriteWeapons()
                .LastOrDefault();
            if (latestGeneratedWeapon == null)
            {
                Debug.LogWarning("[WeaponBuildTester] No generated sprites were found.", this);
                return;
            }

            player.WeaponInventory.BuildAndEquip(latestGeneratedWeapon);
            Debug.Log("[WeaponBuildTester] Equipped latest generated sprite.", this);
        }

        private bool CanBuild()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[WeaponBuildTester] Enter Play Mode before building generated sprite weapons.", this);
                return false;
            }

            if (player == null || player.WeaponInventory == null)
            {
                Debug.LogError("[WeaponBuildTester] Player or WeaponInventory is missing.", this);
                return false;
            }

            return true;
        }

        private List<WeaponDefinition> LoadGeneratedSpriteWeapons()
        {
            string directory = GetGeneratedSpritesDirectory();
            if (!Directory.Exists(directory))
            {
                Debug.LogWarning($"[WeaponBuildTester] Generated sprites folder does not exist: {directory}", this);
                return new List<WeaponDefinition>();
            }

            IEnumerable<string> spritePaths = Directory
                .EnumerateFiles(directory, "*.png", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path);

            if (maxGeneratedSprites > 0)
            {
                spritePaths = spritePaths.TakeLast(maxGeneratedSprites);
            }

            List<WeaponDefinition> generatedWeapons = new();
            foreach (string path in spritePaths)
            {
                Sprite sprite = LoadSprite(path);
                if (sprite == null)
                {
                    Debug.LogWarning($"[WeaponBuildTester] Failed to load generated sprite: {path}", this);
                    continue;
                }

                generatedWeapons.Add(new WeaponDefinition(
                    Path.GetFileNameWithoutExtension(path),
                    generatedSpriteCategory,
                    sprite,
                    new WeaponStats(
                        generatedSpriteDamage,
                        Vector2.one,
                        generatedSpriteSpeed)));
            }

            Debug.Log($"[WeaponBuildTester] Loaded {generatedWeapons.Count} generated sprites from {directory}.", this);
            return generatedWeapons;
        }

        private static Sprite LoadSprite(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(bytes))
            {
                Destroy(texture);
                return null;
            }

            texture.name = Path.GetFileNameWithoutExtension(path);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                SpriteService.GENERATED_SPRITE_PIXELS_PER_UNIT);
            sprite.name = texture.name;
            return sprite;
        }

        private static string GetGeneratedSpritesDirectory()
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            if (string.IsNullOrWhiteSpace(projectRoot))
            {
                projectRoot = Application.dataPath;
            }

            return Path.Combine(projectRoot, "GeneratedSprites");
        }
    }
}
