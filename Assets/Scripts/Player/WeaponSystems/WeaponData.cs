using UnityEngine;

public enum AttackPattern { Swing, Ranged }

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Details")]
    public string weaponName;
    public Sprite icon;
    public int damage = 1;
    public float attackSpeed = 1f;      // lower = faster
    public AttackPattern pattern = AttackPattern.Swing;
    public float reach = 1.5f;

    [Header("Animation")]
    public AnimationClip attackClip;     // drag the clip to play
    public float attackFade = 0.05f;
}