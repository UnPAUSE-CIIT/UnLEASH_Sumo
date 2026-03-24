/*
 * File: SuperBouncy.cs
 * Description: Makes any collider super bouncy. You can add this to your arena walls or objects to make players bounce off them.
 * Author: Seifer Albacete, UnPAUSE
 */

using UnityEngine;

[RequireComponent( typeof( Collider ) )]
public class SuperBouncy : MonoBehaviour
{
    [Header( "Bounce Settings" )]
    [SerializeField][Range( 0f, 3f )] float _bounceMultiplier = 2f;

    void Awake()
    {
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = false;
    }

    void OnCollisionEnter( Collision collision )
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if ( player != null )
        {
            player.ApplyBounce( collision.contacts[ 0 ].normal, _bounceMultiplier * 2 );
        }
    }
}
