using Player.Weapons;

public class WeaponBuildRequestPipelineData : BasePipelineData
{
    public WeaponDefinition Weapon { get; set; } = new();
}
