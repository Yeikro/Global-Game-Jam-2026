using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public PlayerRBController playerController;
    public float pushForceMultiplier = 5f;

    private int stunCounter = 0;

    public void GetDamage(int damage, Vector3 direction, float stunDuration = 1.5f)
    {
        Debug.Log("Player received " + damage + " damage.");
        direction = direction == Vector3.zero ? playerController.desiredVel : direction;
        playerController.blockNormalMovement = true;
        playerController.rb.velocity = Vector3.zero;
        playerController.rb.AddForce(direction * pushForceMultiplier, ForceMode.Force);
        playerController.anim.CambiarACaido();
        StartCoroutine(ActivateNormalMovement(stunDuration));
    }

    private IEnumerator ActivateNormalMovement(float stunDuration)
    {
        stunCounter++;
        yield return new WaitForSeconds(stunDuration);
        if (--stunCounter == 0)
        {
            playerController.anim.CambiarACaminar();
            playerController.GetComponent<SpiritsCollector>().ReleaseRandomSpirit(3);
            yield return new WaitForSeconds(2);
            playerController.blockNormalMovement = false;
        }
    }
}
