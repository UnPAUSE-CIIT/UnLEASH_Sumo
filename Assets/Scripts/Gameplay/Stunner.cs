/*
 * File: Stunner.cs
 * Description: Stuns the player on contact. You can add this to hazards, spikes, or special zones in your arena.
 * Author: Seifer Albacete, UnPAUSE
 */

using UnityEngine;

[RequireComponent( typeof( Collider ) )]
public class Stunner : MonoBehaviour
{
    [Header( "Stun Settings" )]
    [SerializeField] float _stunDuration = 3f;

    void Awake()
    {
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    void OnTriggerEnter( Collider other )
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if ( player != null )
        {
            player.GetStunned();
        }
    }
}
