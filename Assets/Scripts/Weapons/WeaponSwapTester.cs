using System.Collections.Generic;
using UnityEngine;

namespace Player.Weapons
{
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(global::Player.PlayerController))]
    public sealed class WeaponSwapTester : MonoBehaviour
    {
        [SerializeField] private List<WeaponBuildRequest> testWeapons = new()
        {
            new WeaponBuildRequest(),
            new WeaponBuildRequest()
        };
        [SerializeField] private int startingSlot;

        private global::Player.PlayerController player;

        private void Awake()
        {
            player = GetComponent<global::Player.PlayerController>();
        }

        private void Start()
        {
            BuildTestWeapons();
        }

        [ContextMenu("Build Test Weapons During Play Mode")]
        private void BuildTestWeapons()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[WeaponSwapTester] Enter Play Mode before building test weapons.", this);
                return;
            }

            if (player == null || player.WeaponInventory == null)
            {
                Debug.LogError("[WeaponSwapTester] Player or WeaponInventory is missing.", this);
                return;
            }

            int builtCount = 0;
            for (int i = 0; i < testWeapons.Count; i++)
            {
                WeaponBuildRequest request = testWeapons[i];
                if (request == null || request.Image == null)
                {
                    Debug.LogWarning($"[WeaponSwapTester] Slot {i} has no weapon image and was skipped.", this);
                    continue;
                }

                if (player.WeaponInventory.BuildAndEquip(request, i) != null)
                {
                    builtCount++;
                }
            }

            if (builtCount > 0)
            {
                player.WeaponInventory.Equip(
                    Mathf.Clamp(startingSlot, 0, player.WeaponInventory.Weapons.Count - 1));
            }

            Debug.Log($"[WeaponSwapTester] Built {builtCount} test weapons. Press Enter to cycle weapons.", this);
        }
    }
}
