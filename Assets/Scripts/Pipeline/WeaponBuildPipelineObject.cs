using Player.Weapons;
using System.Threading.Tasks;

public class WeaponBuildPipelineObject : IPipelineObject
{
    public Task<BasePipelineData> DoPipeAsync(BasePipelineData input)
    {
        if (input is not WeaponBuildRequestPipelineData data)
        {
            throw new PipelineException("Input data must be of type WeaponBuildRequestPipelineData.");
        }

        WeaponDefinition definition = data.Weapon;
        if (definition == null)
        {
            throw new PipelineException("Weapon definition is required.");
        }

        Weapon weapon = WeaponFactory.CreateWeapon(definition, null);
        return Task.FromResult<BasePipelineData>(
            new WeaponBuildPipelineResultData(weapon));
    }
}
