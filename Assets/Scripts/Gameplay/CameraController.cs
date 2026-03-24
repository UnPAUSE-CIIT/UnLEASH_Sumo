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
    [SerializeField] float _defaultDistance = 15f;
    [SerializeField] float _minDistance = 8f;
    [SerializeField] float _maxDistance = 25f;
    [SerializeField] float _smoothSpeed = 5f;
    [SerializeField] float _heightOffset = 10f;
    [SerializeField] float _boundsPadding = 3f;

    Camera _cam;
    float _currentDistance;

    void Start()
    {
        _cam = GetComponent<Camera>();
        _currentDistance = _defaultDistance;
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
        transform.LookAt( centerPoint );
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
        float targetDistance = ( maxExtent + _boundsPadding ) * 2f;
        targetDistance = Mathf.Clamp( targetDistance, _minDistance, _maxDistance );

        _currentDistance = Mathf.Lerp( _currentDistance, targetDistance, Time.deltaTime * _smoothSpeed );

        Vector3 direction = ( transform.position - centerPoint ).normalized;
        if ( direction == Vector3.zero )
        {
            direction = Vector3.back;
        }

        direction.y = 0f;
        direction.Normalize();

        Vector3 cameraPos = centerPoint + direction * _currentDistance;
        cameraPos.y = centerPoint.y + _heightOffset;

        return cameraPos;
    }
}
