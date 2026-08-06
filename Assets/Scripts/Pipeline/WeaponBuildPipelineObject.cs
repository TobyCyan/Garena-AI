using Player.Weapons;

public class WeaponBuildPipelineObject : IPipelineObject
{
    public BasePipelineData DoPipe(BasePipelineData input)
    {
        if (input is not WeaponBuildRequestPipelineData data)
        {
            throw new PipelineException("Input data must be of type WeaponBuildRequestPipelineData.");
        }

        WeaponBuildRequest request = new(data.Image, data.Damage, data.Size, data.Speed);
        var weapon = WeaponGenerator.Instance.BuildWeapon(request);
        return new WeaponBuildPipelineResultData(weapon);
    }
}