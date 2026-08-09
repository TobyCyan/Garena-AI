using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;

public class SpriteService
{
    public const float GENERATED_SPRITE_PIXELS_PER_UNIT = 512f;
    public static readonly Vector2 GENERATED_SPRITE_PIVOT = new(0.5f, 0.5f);

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
        Texture2D texCopy = PrepareGeneratedWeaponTexture(texture);
        SaveGeneratedSpriteTexture(texCopy);

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            GENERATED_SPRITE_PIVOT,
            GENERATED_SPRITE_PIXELS_PER_UNIT
        );

        sprite.name = "GeneratedSprite";

        return sprite;
    }

    public static Texture2D PrepareGeneratedWeaponTexture(Texture2D texture)
    {
        if (texture == null)
        {
            return null;
        }

        string textureName = texture.name;
        Texture2D preparedTexture = CopyToRgbaTexture(texture);
        preparedTexture.name = textureName;
        preparedTexture.filterMode = FilterMode.Point;
        preparedTexture.wrapMode = TextureWrapMode.Clamp;
        ClearWhiteBackground(preparedTexture);
        return CropTransparentPadding(preparedTexture);
    }

    private static Texture2D CropTransparentPadding(Texture2D source)
    {
        const byte alphaThreshold = 8;

        Color32[] pixels = source.GetPixels32();
        int width = source.width;
        int height = source.height;
        int minX = width;
        int minY = height;
        int maxX = -1;
        int maxY = -1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (pixels[y * width + x].a <= alphaThreshold)
                {
                    continue;
                }

                minX = Mathf.Min(minX, x);
                minY = Mathf.Min(minY, y);
                maxX = Mathf.Max(maxX, x);
                maxY = Mathf.Max(maxY, y);
            }
        }

        if (maxX < minX || maxY < minY)
        {
            return source;
        }

        int croppedWidth = maxX - minX + 1;
        int croppedHeight = maxY - minY + 1;
        if (croppedWidth == width && croppedHeight == height)
        {
            return source;
        }

        Texture2D croppedTexture = new(
            croppedWidth,
            croppedHeight,
            TextureFormat.RGBA32,
            false);
        Color32[] croppedPixels = new Color32[croppedWidth * croppedHeight];
        for (int y = 0; y < croppedHeight; y++)
        {
            for (int x = 0; x < croppedWidth; x++)
            {
                croppedPixels[y * croppedWidth + x] =
                    pixels[(minY + y) * width + minX + x];
            }
        }

        croppedTexture.SetPixels32(croppedPixels);
        croppedTexture.Apply();
        croppedTexture.name = source.name;
        croppedTexture.filterMode = source.filterMode;
        croppedTexture.wrapMode = source.wrapMode;
        Destroy(source);
        return croppedTexture;
    }

    private static void ClearWhiteBackground(Texture2D texture)
    {
        const byte backgroundThreshold = 200;

        Color32[] pixels = texture.GetPixels32();
        bool[] visited = new bool[pixels.Length];
        Queue<int> pixelsToVisit = new();
        int width = texture.width;
        int height = texture.height;

        for (int x = 0; x < width; x++)
        {
            EnqueueBackgroundPixel(x, 0);
            EnqueueBackgroundPixel(x, height - 1);
        }

        for (int y = 1; y < height - 1; y++)
        {
            EnqueueBackgroundPixel(0, y);
            EnqueueBackgroundPixel(width - 1, y);
        }

        while (pixelsToVisit.Count > 0)
        {
            int index = pixelsToVisit.Dequeue();
            pixels[index] = new Color32(
                pixels[index].r,
                pixels[index].g,
                pixels[index].b,
                0);

            int x = index % width;
            int y = index / width;
            EnqueueBackgroundPixel(x - 1, y);
            EnqueueBackgroundPixel(x + 1, y);
            EnqueueBackgroundPixel(x, y - 1);
            EnqueueBackgroundPixel(x, y + 1);
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        void EnqueueBackgroundPixel(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                return;
            }

            int index = y * width + x;
            if (visited[index] || !IsBackgroundPixel(pixels[index]))
            {
                return;
            }

            visited[index] = true;
            pixelsToVisit.Enqueue(index);
        }

        bool IsBackgroundPixel(Color32 pixel)
        {
            return pixel.a > 0 &&
                pixel.r >= backgroundThreshold &&
                pixel.g >= backgroundThreshold &&
                pixel.b >= backgroundThreshold;
        }
    }

    private static Texture2D CopyToRgbaTexture(Texture2D source)
    {
        Texture2D rgbaTexture = new(
            source.width,
            source.height,
            TextureFormat.RGBA32,
            false);
        rgbaTexture.SetPixels32(source.GetPixels32());
        rgbaTexture.Apply();
        Destroy(source);
        return rgbaTexture;
    }

    private static void SaveGeneratedSpriteTexture(Texture2D texture)
    {
        if (texture == null)
        {
            return;
        }

        try
        {
            string directory = GetGeneratedSpritesDirectory();
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
