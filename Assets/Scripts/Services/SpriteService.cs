using System;
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

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            32f
        );

        sprite.name = "GeneratedSprite";

        return sprite;
    }
}
