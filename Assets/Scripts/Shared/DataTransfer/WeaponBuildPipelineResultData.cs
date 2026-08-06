using Player.Weapons;

public class WeaponBuildPipelineResultData : BasePipelineData
{
    public Weapon Weapon { get; set; }

    public WeaponBuildPipelineResultData(Weapon weapon)
    {
        Weapon = weapon;
    }
}
