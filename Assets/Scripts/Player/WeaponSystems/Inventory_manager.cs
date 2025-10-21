using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; 

public class Inventory_manager : MonoBehaviour
{
    public TMP_Dropdown weaponDropdown; 
    public Image weaponIcon; 
    public WeaponData equippedWeapon;   

    private List<WeaponData> inventory = new List<WeaponData>();

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
        inventory.Add(w);
        RefreshDropdown();

        // auto-equip collected weapon
        EquipWeapon(inventory.Count - 1);
    }

    private void OnDropdownChanged(int index)
    {
        EquipWeapon(index);
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= inventory.Count) return;
        equippedWeapon = inventory[index];

        if (weaponIcon != null && equippedWeapon.icon != null)
            weaponIcon.sprite = equippedWeapon.icon;
    }

    private void RefreshDropdown()
    {
        if (weaponDropdown == null) return;

        weaponDropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (var weapon in inventory)
        {
            options.Add(weapon.weaponName);
        }

        weaponDropdown.AddOptions(options);
    }
}