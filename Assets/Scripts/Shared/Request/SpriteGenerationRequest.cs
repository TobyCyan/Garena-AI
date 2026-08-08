using Player.Weapons;

public record SpriteGenerationRequest
{
    public string Prompt { get; }
    public WeaponCategory WeaponCategory { get; }

    public SpriteGenerationRequest(string prompt, WeaponCategory category)
    {
        Prompt = prompt;
        WeaponCategory = category;
    }
}