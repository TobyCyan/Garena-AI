using Player.Weapons;
using System.Threading.Tasks;
using UnityEngine;

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
        global::Player.PlayerController player =
            Object.FindAnyObjectByType<global::Player.PlayerController>();
        if (player == null || player.WeaponInventory == null)
        {
            throw new PipelineException("A player with a WeaponInventory is required to build a weapon.");
        }

        Weapon weapon = player.WeaponInventory.Build(definition);
        return Task.FromResult<BasePipelineData>(
            new WeaponBuildPipelineResultData(weapon));
    }
}
