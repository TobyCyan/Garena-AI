using Player.Weapons;
using UnityEngine;

public class WeaponBuildPipelineObject : IPipelineObject
{
    public BasePipelineData DoPipe(BasePipelineData input)
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
            throw new PipelineException("A player with a WeaponInventory is required to build and equip a weapon.");
        }

        Weapon weapon = player.WeaponInventory.BuildAndEquip(definition);
        return new WeaponBuildPipelineResultData(weapon);
    }
}
