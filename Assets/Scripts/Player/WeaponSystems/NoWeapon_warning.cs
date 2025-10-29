using UnityEngine;

public class NoWeapon_warning : MonoBehaviour
{
    [SerializeField] private GameObject NoWeapon_Barrier;
    [SerializeField] private GameObject NoWeapon_warn_panel;

    [Header("Player UI")]
    public GameObject Pause;
    public GameObject Weapon_system;
    public GameObject ControlButtons;
    public GameObject BackBtn;

    [SerializeField] 
    private Inventory_manager playerInventory; // assign in Inspector if you can

    void Start()
    {
        if (NoWeapon_Barrier)    NoWeapon_Barrier.SetActive(true);
        if (NoWeapon_warn_panel) NoWeapon_warn_panel.SetActive(false);
        gameObject.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Auto-find the player's inventory if not wired
        if (playerInventory == null)
        {
            playerInventory = other.GetComponentInChildren<Inventory_manager>();
            if (playerInventory == null) playerInventory = FindObjectOfType<Inventory_manager>();
        }

        bool inventoryEmpty = (playerInventory == null) || playerInventory.IsEmpty;

        if (inventoryEmpty)
        {
            if (NoWeapon_Barrier)    NoWeapon_Barrier.SetActive(true);
            if (NoWeapon_warn_panel) NoWeapon_warn_panel.SetActive(true);
            if (BackBtn) BackBtn.SetActive(true);

            Time.timeScale = 0f;
            if (Pause)         Pause.SetActive(false);
            if (Weapon_system) Weapon_system.SetActive(false);
            if (ControlButtons) ControlButtons.SetActive(false);

            Debug.Log("[NoWeapon_warning] Player has NO weapon -> showing warning.");
        }
        else
        {
            // Allow progress
            if (NoWeapon_Barrier)    NoWeapon_Barrier.SetActive(false);
            if (NoWeapon_warn_panel) NoWeapon_warn_panel.SetActive(false);

            // This trigger is done; disable it
            gameObject.SetActive(false);

            Debug.Log("[NoWeapon_warning] Player has a weapon -> barrier opened.");
        }
    }

    // Button callback on the warning panel
    public void NoWeaponWarn_continue()
    {
        Time.timeScale = 1f;

        if (NoWeapon_warn_panel) NoWeapon_warn_panel.SetActive(false);
        if (Pause)               Pause.SetActive(true);
        if (Weapon_system)       Weapon_system.SetActive(true);
        if (ControlButtons) ControlButtons.SetActive(true);
        
    }
}
