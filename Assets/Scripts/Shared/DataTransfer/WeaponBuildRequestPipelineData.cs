using UnityEngine;

public class WeaponBuildRequestPipelineData : BasePipelineData
{
    public int Damage { get; set; }
    public Vector2 Size { get; set; }
    public float Speed { get; set; }
    public Sprite Image { get; set; }
}
