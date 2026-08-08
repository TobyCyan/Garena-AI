using Player.Weapons;

public class WeaponStatBalancerPipelineData : BasePipelineData
{
    public Weapon Weapon { get; }

    public WeaponStatBalancerPipelineData(Weapon weapon)
    {
        Weapon = weapon;
    }
}