using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Info")]
    public string weaponName; 
    public WeaponData weaponData;    
    public GameObject weaponPanel;          
    public GameObject interactUI;           
    public GameObject xButton;              

    private bool hasPickedUp = false;       

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
            Debug.Log($"[WeaponPickup] Picked up weapon: {weaponName}");
            hasPickedUp = true;

            if (weaponPanel != null) weaponPanel.SetActive(true);
            if (xButton != null) xButton.SetActive(true);
            if (interactUI != null) interactUI.SetActive(false);

            Time.timeScale = 0f;

            var inv = other.GetComponentInChildren<Inventory_manager>();
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

            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && interactUI != null)
        {
            interactUI.SetActive(false);
            Debug.Log($"[WeaponPickup] Exited pickup range for {weaponName}");
        }
    }

    public void ClosePanel()
    {
        if (weaponPanel != null)
            weaponPanel.SetActive(false);

        if (xButton != null)
            xButton.SetActive(false);

        Time.timeScale = 1f;
    }
}
