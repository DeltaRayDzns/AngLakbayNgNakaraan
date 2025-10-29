using UnityEngine;
using UnityEngine.InputSystem;

public class Buttons_Manager : MonoBehaviour
{
    [Header("Player Action Scripts")]
    public PlayerMovement playerMovement;
    public PlayerAttack playerAttack; 
    public NPC_Talk npcTalk;
    public Interact_artifact interactArtifact;

    private bool movingleft;
    private bool movingright;
    private bool Jumping; 

    void Update()
    {
        if (playerMovement == null)
        {
            return; 
        }

        float horizontal = 0f;

        if (movingleft) horizontal = -1f; 
        if (movingright) horizontal = 1f;
        
        playerMovement.SetHorizontalInput(horizontal);

        if (Jumping)
        {
            playerMovement.HoldJumpButton(); 
        }
    }
    

    public void lefthold() => movingleft = true;
    public void leftstop() => movingleft = false;
    
    public void righthold() => movingright = true;
    public void rightstop() => movingright = false;


    public void jumphold()
    {
        Jumping = true;
        playerMovement.StartJumpCharge();
        Debug.Log("Holding Jump");
    }

    public void jumpstop()
    {
        Jumping = false;
        playerMovement.ReleaseJump();
        
        Debug.Log("Released Jump");
    }

    public void Attacking()
    {
        if (playerAttack != null && playerAttack.enabled)
        {
            playerAttack.StartAttack(); 
        }
    }
}
