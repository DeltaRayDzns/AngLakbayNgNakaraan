using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Inventory_manager : MonoBehaviour
{
    public TMP_Dropdown weaponDropdown;
    public Image weaponIcon;
    public WeaponData equippedWeapon;

    private readonly List<WeaponData> inventory = new List<WeaponData>();

    public bool IsEmpty => inventory.Count == 0;
    public bool HasEquipped => equippedWeapon != null;

    void Start()
    {
        if (weaponDropdown != null)
        {
            weaponDropdown.onValueChanged.AddListener(OnDropdownChanged);
            RefreshDropdown();
        }
    }

    public void AddWeapon(WeaponData w)
    {
        if (w == null) return;
        if (!inventory.Contains(w)) inventory.Add(w);

        RefreshDropdown();

        if (equippedWeapon == null) EquipWeapon(inventory.Count - 1);
    }

    void OnDropdownChanged(int index) => EquipWeapon(index);

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= inventory.Count) { equippedWeapon = null; return; }

        equippedWeapon = inventory[index];

        if (weaponIcon != null)
            weaponIcon.sprite = equippedWeapon != null ? equippedWeapon.icon : null;
    }

    void RefreshDropdown()
    {
        if (weaponDropdown == null) return;

        weaponDropdown.ClearOptions();

        var options = new List<TMP_Dropdown.OptionData>();
        if (inventory.Count == 0)
        {
            options.Add(new TMP_Dropdown.OptionData("Empty"));
            weaponDropdown.AddOptions(options);
            weaponDropdown.SetValueWithoutNotify(0);
            return;
        }

        foreach (var weapon in inventory)
            options.Add(new TMP_Dropdown.OptionData(weapon.weaponName));

        weaponDropdown.AddOptions(options);
        weaponDropdown.SetValueWithoutNotify(Mathf.Max(0, inventory.Count - 1));
        weaponDropdown.RefreshShownValue();
    }
}
