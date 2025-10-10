using UnityEngine;
using UnityEngine.UI;

public class JumpChargeBar : MonoBehaviour
{
    [SerializeField] private Slider jumpSlider;
    [SerializeField] private PlayerMovement player;

    private void Start()
    {
        jumpSlider.minValue = player.MinJumpPower;
        jumpSlider.maxValue = player.MaxJumpPower;
    }

    private void Update()
    {
        jumpSlider.value = player.CurrentJumpPower;
    }
}