/*
 * File: CameraController.cs
 * Description: Follows the center point between both players using bounds. Use this to keep both sumo wrestlers in view.
 * Author: Seifer Albacete, UnPAUSE
 */

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] Transform _player1;
    [SerializeField] Transform _player2;

    [Header("Camera Settings")]
    [SerializeField] Vector3 _offset = new Vector3( 0f, 15f, -15f );
    [SerializeField] float _smoothSpeed = 5f;
    [SerializeField] float _boundsPadding = 3f;
    [SerializeField] float _minZoom = 1f;
    [SerializeField] float _maxZoom = 2f;

    void Start()
    {
    }

    void LateUpdate()
    {
        if ( _player1 == null || _player2 == null )
        {
            return;
        }

        Vector3 centerPoint = GetCenterPoint();
        Vector3 targetPosition = CalculateCameraPosition( centerPoint );

        transform.position = Vector3.Lerp( transform.position, targetPosition, Time.deltaTime * _smoothSpeed );
    }

    Vector3 GetCenterPoint()
    {
        Bounds player1Bounds = new Bounds( _player1.position, Vector3.one * 2f );
        Bounds player2Bounds = new Bounds( _player2.position, Vector3.one * 2f );

        Bounds combinedBounds = player1Bounds;
        combinedBounds.Encapsulate( player2Bounds );

        return combinedBounds.center;
    }

    Vector3 CalculateCameraPosition( Vector3 centerPoint )
    {
        Bounds playerBounds = new Bounds( _player1.position, Vector3.zero );
        playerBounds.Encapsulate( _player2.position );

        float maxExtent = Mathf.Max( playerBounds.extents.x, playerBounds.extents.z );
        float zoomFactor = Mathf.Clamp( ( maxExtent + _boundsPadding ) / 10f, _minZoom, _maxZoom );

        Vector3 scaledOffset = _offset * zoomFactor;
        return centerPoint + scaledOffset;
    }
}
