using Player.Weapons;

public class WeaponStatBalancerPipelineObject : IPipelineObject
{
    public BasePipelineData DoPipe(BasePipelineData input)
    {
        if (input is not WeaponStatBalancerPipelineData data)
        {
            throw new PipelineException("Input data must be of type WeaponStatBalancerPipelineInputData.");
        }
        WeaponStats balancedStats = WeaponBalancerService.BalanceWeapon(data.WeaponStats);
        return new WeaponStatBalancerPipelineData(balancedStats);
    }
}