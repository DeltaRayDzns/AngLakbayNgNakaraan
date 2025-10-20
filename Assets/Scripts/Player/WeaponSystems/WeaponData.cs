using UnityEngine;

public enum AttackPattern
{
    Swing,
    Ranged
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite icon;
    public int damage = 1;
    public float attackSpeed = 1f;
    public AttackPattern pattern = AttackPattern.Swing;
    
    public float reach = 1.5f;
}