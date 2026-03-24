/*
 * File: PlayerController.cs
 * Description: Controls player movement with physics-based acceleration. You can use this to move your sumo wrestler around the arena.
 * Author: Seifer Albacete, UnPAUSE
 */

using UnityEngine;

public enum PlayerNumber
{
    Player1,
    Player2
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Setup")]
    [SerializeField] PlayerNumber _playerNumber;

    [Header("Movement Settings")]
    [SerializeField] float _acceleration = 50f;
    [SerializeField] float _maxSpeed = 10f;
    [SerializeField] float _turnSpeed = 10f;

    [Header("Bump Settings")]
    [SerializeField] float _stunDuration = 3f;
    [SerializeField] float _knockbackForce = 20f;
    [SerializeField] float _minSpeedForBump = 2f;

    bool _isStunned;
    float _stunTimer;
    float _bounceCooldown;

    Rigidbody _rb;
	Animator _anim;
    Vector3 _inputDirection;

    void Awake()
    {
		_anim = GetComponent<Animator>();

        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _rb.linearDamping = 3f;
        _rb.angularDamping = 0.05f;
    }

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        if ( _bounceCooldown > 0f )
        {
            _bounceCooldown -= Time.deltaTime;
        }

        HandleStun();
        HandleInput();
		HandleAnimations();
    }

    void HandleInput()
    {
        if ( _isStunned )
        {
            _inputDirection = Vector3.zero;
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        switch ( _playerNumber )
        {
            case PlayerNumber.Player1:
                if ( Input.GetKey( KeyCode.A ) )
                {
                    horizontal -= 1f;
                }
                if ( Input.GetKey( KeyCode.D ) )
                {
                    horizontal += 1f;
                }
                if ( Input.GetKey( KeyCode.W ) )
                {
                    vertical += 1f;
                }
                if ( Input.GetKey( KeyCode.S ) )
                {
                    vertical -= 1f;
                }
                break;

            case PlayerNumber.Player2:
                if ( Input.GetKey( KeyCode.LeftArrow ) )
                {
                    horizontal -= 1f;
                }
                if ( Input.GetKey( KeyCode.RightArrow ) )
                {
                    horizontal += 1f;
                }
                if ( Input.GetKey( KeyCode.UpArrow ) )
                {
                    vertical += 1f;
                }
                if ( Input.GetKey( KeyCode.DownArrow ) )
                {
                    vertical -= 1f;
                }
                break;
        }

        _inputDirection = new Vector3( horizontal, 0f, vertical ).normalized;
    }

	void HandleAnimations()
	{
		_anim.SetFloat( "Speed", _rb.linearVelocity.magnitude / _maxSpeed );
		_anim.SetBool( "Stunned", _isStunned );
	}

    void HandleStun()
    {
        if ( _isStunned )
        {
            _stunTimer -= Time.deltaTime;
            if ( _stunTimer <= 0f )
            {
                _isStunned = false;
            }
        }
    }

    void OnCollisionEnter( Collision collision )
    {
        if ( _isStunned )
        {
            return;
        }

        if ( !collision.gameObject.TryGetComponent<PlayerController>( out var otherPlayer ) )
            return;

        if ( otherPlayer._isStunned )
            return;

        float mySpeed = GetHorizontalSpeed();
        float otherSpeed = otherPlayer.GetHorizontalSpeed();

        if ( mySpeed < _minSpeedForBump || otherSpeed < _minSpeedForBump )
        {
            return;
        }

        if ( mySpeed > otherSpeed )
        {
            otherPlayer.GetStunned();
            Vector3 knockbackDir = ( otherPlayer.transform.position - transform.position ).normalized;
            _rb.AddForce( knockbackDir * _knockbackForce, ForceMode.Impulse );
        }
        else if ( otherSpeed > mySpeed )
        {
            GetStunned();
            Vector3 knockbackDir = ( transform.position - otherPlayer.transform.position ).normalized;
            otherPlayer.ApplyKnockback( knockbackDir * _knockbackForce );
        }
        else
        {
            Vector3 pushDir = ( otherPlayer.transform.position - transform.position ).normalized;
            _rb.AddForce( pushDir * _knockbackForce * 0.5f, ForceMode.Impulse );
            otherPlayer.ApplyKnockback( pushDir * _knockbackForce * 0.5f );
        }
    }

    public void GetStunned()
    {
        _isStunned = true;
        _stunTimer = _stunDuration;
    }

    public void ApplyKnockback( Vector3 force )
    {
        _rb.AddForce( force, ForceMode.Impulse );
    }

    public void ApplyBounce( Vector3 normal, float multiplier )
    {
        Vector3 horizontalVel = new Vector3( _rb.linearVelocity.x, 0f, _rb.linearVelocity.z );
        float speed = horizontalVel.magnitude;

        if ( speed > 0.1f )
        {
            Vector3 reflected = Vector3.Reflect( horizontalVel, normal );
            reflected = reflected.normalized * speed * multiplier;
            reflected.y = _rb.linearVelocity.y;
            _rb.linearVelocity = reflected;
            _bounceCooldown = 1f;
        }
    }

    public float GetHorizontalSpeed()
    {
        return new Vector3( _rb.linearVelocity.x, 0f, _rb.linearVelocity.z ).magnitude;
    }

    void Move()
    {
        if ( _bounceCooldown > 0f )
        {
            return;
        }

        if ( _inputDirection.magnitude > 0.1f )
        {
            Vector3 force = _inputDirection * _acceleration;
            force.y = 0f;
            _rb.AddForce( force, ForceMode.Acceleration );

            Vector3 horizontalVel = new( _rb.linearVelocity.x, 0f, _rb.linearVelocity.z );
            if ( horizontalVel.magnitude > _maxSpeed )
            {
                horizontalVel = horizontalVel.normalized * _maxSpeed;
                _rb.linearVelocity = new Vector3( horizontalVel.x, _rb.linearVelocity.y, horizontalVel.z );
            }

            Quaternion targetRotation = Quaternion.LookRotation( _inputDirection );
            transform.rotation = Quaternion.Slerp( transform.rotation, targetRotation, Time.fixedDeltaTime * _turnSpeed );
        }
    }
}
