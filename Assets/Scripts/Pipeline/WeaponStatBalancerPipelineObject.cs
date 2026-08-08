public class WeaponStatBalancerPipelineObject : IPipelineObject
{
    public BasePipelineData DoPipe(BasePipelineData input)
    {
        if (input is not WeaponStatBalancerPipelineData data)
        {
            throw new PipelineException("Input data must be of type WeaponStatBalancerPipelineInputData.");
        }
        var balancedStats = WeaponBalancerService.BalanceWeapon(data.Weapon);
        return new WeaponStatBalancerPipelineData(balancedStats);
    }
}