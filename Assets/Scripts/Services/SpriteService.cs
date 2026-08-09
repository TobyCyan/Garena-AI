using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;

public class SpriteService
{
    public static async Task<Sprite> GenerateSpriteAsync(SpriteGenerationRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            throw new ArgumentException("Prompt cannot be empty.");
        }

        string prompt = request.Prompt + ", Weapon Category: " + request.WeaponCategory;

        byte[] spriteBytes = await ImageApiService.GetImage(prompt);

        if (spriteBytes == null || spriteBytes.Length == 0)
        {
            throw new InvalidOperationException(
                "Image generation returned no image data.");
        }

        Texture2D texture = new(2, 2);

        if (!texture.LoadImage(spriteBytes))
        {
            Destroy(texture);
            throw new InvalidOperationException(
                "Failed to load generated image into texture.");
        }

        texture.name = "GeneratedSpriteTexture";
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        SaveGeneratedSpriteTexture(texture);

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            32f
        );

        sprite.name = "GeneratedSprite";

        return sprite;
    }

    private static void SaveGeneratedSpriteTexture(Texture2D texture)
    {
        if (texture == null)
        {
            return;
        }

        try
        {
            string directory = Path.Combine(
                Application.persistentDataPath,
                "GeneratedSprites");
            Directory.CreateDirectory(directory);

            string fileName =
                $"weapon-sprite-{DateTime.Now:yyyyMMdd-HHmmss}.png";
            string path = Path.Combine(directory, fileName);

            File.WriteAllBytes(path, texture.EncodeToPNG());
            Debug.Log($"Saved generated weapon sprite to: {path}");
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"Generated sprite was created but could not be saved: " +
                exception.Message);
        }
    }
}
