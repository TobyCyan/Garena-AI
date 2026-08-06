using Player.Weapons;

public class WeaponBuildPipelineResultData : BasePipelineData
{
    Weapon Weapon { get; set; }

    public WeaponBuildPipelineResultData(Weapon weapon)
    {
        Weapon = weapon;
    }
}
