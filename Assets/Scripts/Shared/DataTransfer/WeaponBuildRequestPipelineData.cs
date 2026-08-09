using Player.Weapons;

public class WeaponBuildRequestPipelineData : BasePipelineData
{
    public WeaponDefinition Weapon { get; set; } = new();

    public WeaponBuildRequestPipelineData()
    {
    }

    public WeaponBuildRequestPipelineData(WeaponDefinition weapon)
    {
        Weapon = weapon;
    }
}
