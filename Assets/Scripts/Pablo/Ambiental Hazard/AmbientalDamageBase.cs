using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientalDamageBase : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerLife player))
        {
            player.GetDamage(damage, Vector3.zero);
        }   
    }
}
