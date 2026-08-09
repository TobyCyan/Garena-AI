using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Player/Player Data")]
    public class PlayerData : ScriptableObject
    {
        [SerializeField, Min(0f)] private float movementSpeed = 7f;
        [SerializeField, Range(0f, 1f)] private float attackMovementMultiplier;

        public float MovementSpeed => movementSpeed;
        public float AttackMovementMultiplier => attackMovementMultiplier;

        public static PlayerData CreateRuntimeDefault()
        {
            PlayerData data = CreateInstance<PlayerData>();
            data.name = "Runtime Player Data";
            return data;
        }
    }
}
