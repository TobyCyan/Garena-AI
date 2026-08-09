using Player.Weapons;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player;

[DisallowMultipleComponent]
public sealed class GeneratedWeaponContextManager : MonoBehaviour
{
    public static GeneratedWeaponContextManager Instance { get; private set; }

    private WeaponDefinition pendingWeapon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            Instance = null;
        }
    }

    public void SaveGeneratedWeapon(Weapon weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("[GeneratedWeaponContext] Cannot save a null weapon.", this);
            return;
        }

        pendingWeapon = new WeaponDefinition(
            weapon.WeaponName,
            weapon.Category,
            weapon.Image,
            weapon.Stats);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EquipPendingWeapon();
    }

    private void EquipPendingWeapon()
    {
        if (pendingWeapon == null)
        {
            return;
        }

        WeaponInventory inventory = FindWeaponInventory();
        if (inventory == null)
        {
            Debug.LogWarning("[GeneratedWeaponContext] No WeaponInventory found in loaded scene.", this);
            return;
        }

        Weapon equippedWeapon = inventory.BuildAndEquip(pendingWeapon);
        if (equippedWeapon == null)
        {
            Debug.LogWarning("[GeneratedWeaponContext] Failed to equip persisted generated weapon.", this);
            return;
        }

        pendingWeapon = null;
    }

    private static WeaponInventory FindWeaponInventory()
    {
        PlayerController player =
            FindAnyObjectByType<PlayerController>(
                FindObjectsInactive.Include);
        if (player != null && player.WeaponInventory != null)
        {
            return player.WeaponInventory;
        }

        return FindAnyObjectByType<WeaponInventory>(
            FindObjectsInactive.Include);
    }
}
