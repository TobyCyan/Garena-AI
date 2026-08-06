using Player.Weapons;
using System;
using System.Collections;

public class WeaponBuildPipelineObject : IPipelineObject
{
    public IEnumerator DoPipe(BasePipelineData input, Action<BasePipelineData> onComplete)
    {
        if (input is not WeaponBuildRequestPipelineData data)
        {
            throw new PipelineException("Input data must be of type WeaponBuildRequestPipelineData.");
        }

        WeaponBuildRequest request = new(data.Image, data.Damage, data.Size, data.Speed);
        var weapon = WeaponGenerator.Instance.BuildWeapon(request);
        onComplete?.Invoke(new WeaponBuildPipelineResultData(weapon));
        yield break;
    }
}
