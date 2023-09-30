using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 20;

    private ulong ownerClient;
    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClient = ownerClientId;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody != null)
        {
            if(collision.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
            {
                if(ownerClient == netObj.OwnerClientId)
                {
                    return;
                }
            }

            if(collision.attachedRigidbody.TryGetComponent<Health>(out Health healthComp))
            {
                healthComp.TakeDamage(damage);
            }
        }
    }
}
