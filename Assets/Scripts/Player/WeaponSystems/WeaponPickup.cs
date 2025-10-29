using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Info")]
    public string weaponName;
    public WeaponData weaponData;

    [Header("UI References")]
    public GameObject weaponPanel;
    public GameObject interactUI;
    public GameObject xButton;
    public GameObject controlButtons; 

    private bool playerInRange = false;
    private bool hasPickedUp = false;
    private GameObject playerRef;

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
        if (weaponPanel != null) weaponPanel.SetActive(false);
        if (xButton != null) xButton.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasPickedUp) return;

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerRef = other.gameObject;

            if (interactUI != null)
                interactUI.SetActive(true);

            Debug.Log($"[WeaponPickup] Player near weapon: {weaponName}");
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (hasPickedUp) return;

        if (other.CompareTag("Player") && Keyboard.current.fKey.wasPressedThisFrame)
        {
            InteractButtonPressed();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerRef = null;

            if (interactUI != null)
                interactUI.SetActive(false);

            Debug.Log($"[WeaponPickup] Exited pickup range for {weaponName}");
        }
    }

    public void InteractButtonPressed()
    {
        if (!playerInRange || hasPickedUp)
        {
            Debug.Log("[WeaponPickup] No weapon in range or already picked up.");
            return;
        }

        hasPickedUp = true;
        Debug.Log($"[WeaponPickup] Picked up weapon: {weaponName}");

        if (interactUI != null) interactUI.SetActive(false);
        if (weaponPanel != null) weaponPanel.SetActive(true);
        if (xButton != null) xButton.SetActive(true);
        if (controlButtons != null) controlButtons.SetActive(false);

        Time.timeScale = 0f;

        if (playerRef != null)
        {
            var inv = playerRef.GetComponentInChildren<Inventory_manager>();
            if (inv != null && weaponData != null)
            {
                inv.AddWeapon(weaponData);
                Debug.Log($"[WeaponPickup] Added {weaponData.weaponName} to inventory.");
            }
            else if (inv == null)
            {
                Debug.LogWarning("[WeaponPickup] No Inventory_manager found on player!");
            }
            else if (weaponData == null)
            {
                Debug.LogWarning("[WeaponPickup] No WeaponData assigned!");
            }
        }

        gameObject.SetActive(false);
    }

    public void ClosePanel()
    {
        if (weaponPanel != null)
            weaponPanel.SetActive(false);

        if (xButton != null)
            xButton.SetActive(false);

        if (controlButtons != null)
            controlButtons.SetActive(true);

        Time.timeScale = 1f;
    }
}
