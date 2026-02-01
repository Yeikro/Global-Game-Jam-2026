using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public PlayerRBController playerController;
    public float pushForceMultiplier = 5f;

    void Start()
    {
        
    }

    public void GetDamage(int damage, Vector3 direction, float stunDuration = 1.5f)
    {
        Debug.Log("Player received " + damage + " damage.");
        direction = direction == Vector3.zero ? playerController.desiredVel : direction;
        playerController.blockNormalMovement = true;
        playerController.rb.velocity = Vector3.zero;
        playerController.rb.AddForce(direction * pushForceMultiplier, ForceMode.Force);
        playerController.anim.CambiarACansado();
        Invoke(nameof(ActivateNormalMovement), stunDuration);
    }

    private void ActivateNormalMovement()
    {
        playerController.blockNormalMovement = false;
    }

    void Update()
    {
        
    }
}
