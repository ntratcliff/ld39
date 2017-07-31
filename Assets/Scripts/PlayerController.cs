using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    public float MovementForce;
    public float MaxSpeed;

    public HingeJoint2D RearWheel, FrontWheel;

    public SolarBattery Battery;
    public BatteryStatusMonitor StatusMonitor;
    public float ConstantDrain = 1f;
    public float MovementDrain = 2f;

    private float _frameDrain;

    // attached components
    private Rigidbody2D _body;

    // elevator movement
    private bool _onElevator;
    private Vector2 _lastElevatorPos;
    private Transform _currentElevator;

    // movement
    private bool _onGround;
    private bool _movedThisFrame;
    private bool _velocityHalved;
    

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _updateGroundRaycasts();
    }

    // apply forces in fixedupdate
    private void FixedUpdate()
    {
        _frameDrain = ConstantDrain;
        _movedThisFrame = false;

        if(StatusMonitor.Status != MonitorStatus.Dead && _onGround)
            _updateMovement();
        else
        {
            RearWheel.useMotor = false;
            FrontWheel.useMotor = false;
        }

        if(!_movedThisFrame && !_velocityHalved)
        {
            // decrease horizontal velocity
            Vector2 velocity = _body.velocity;
            velocity.x /= 16;
            _body.velocity = velocity;
            _velocityHalved = true;
        }
        else if (_movedThisFrame)
        {
            _velocityHalved = false;
        }


        // apply drain to battery
        Battery.Drain(_frameDrain * Time.deltaTime);
    }

    private void _updateMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if(horizontal != 0)
        {
            Vector2 direction = transform.right * Mathf.Sign(horizontal) * MovementForce;
            _body.AddForce(direction);
            _body.AddForce(-transform.up * MovementForce * 2f);
            _clampSpeed();
            _frameDrain += MovementDrain;
            _movedThisFrame = true;
        }

        // apply brakes when not moving
        RearWheel.useMotor = horizontal == 0;
        FrontWheel.useMotor = horizontal == 0;
    }

    private void _updateGroundRaycasts()
    {
        // preserve last frame's elevator flag
        bool onElevatorLastFrame = _onElevator;

        // cast rays downwards on front and back
        int layerMask = LayerMask.GetMask(new string[] { "Default", "Transparent", "LightOnly" });
        RaycastHit2D frontHit = Physics2D.Raycast(transform.position + transform.right, -transform.up, 0.4f, layerMask);
        RaycastHit2D rearHit = Physics2D.Raycast(transform.position - transform.right, -transform.up, 0.4f, layerMask);
        Debug.DrawLine(transform.position, transform.position - (transform.up * 0.4f));

        // check if we're on the ground
        _onGround = frontHit.transform != null || rearHit.transform != null;

        // stop here if we're not on the ground
        if (!_onGround)
        {
            return;
        }

        // check if we're on an elevator
        _onElevator = (frontHit.transform != null && frontHit.transform.tag == "Elevator")
                    || (rearHit.transform != null && rearHit.transform.tag == "Elevator");

        // stop here if we're not on an elevator
        if (!_onElevator)
        {
            if (onElevatorLastFrame)
            {
                Debug.Log("Left elevator!");
                _currentElevator = null;
            }
            return;
        }

        // get the transform we hit
        Transform elevator = frontHit.transform;
        if (!elevator)
            elevator = rearHit.transform;

        // weird edge case handling don't worry
        if(elevator == null || (_currentElevator != null && elevator != _currentElevator))
        {
            Debug.Log("Hold up, I'm on a different elevator!");
            _currentElevator = null;
            _onElevator = false;
            return;
        }

        // stop here if it's the first frame we are on an/this elevator
        if(!onElevatorLastFrame || elevator != _currentElevator)
        {
            Debug.Log("First time on elevator!");
            _currentElevator = elevator;
            _lastElevatorPos = elevator.position;
            return;
        }

        // calculate how much the elevator moved last frame
        Vector2 delta = (Vector2)elevator.position - _lastElevatorPos;

        // move ourself by the delta
        transform.position = (Vector2)transform.position + delta;

        _lastElevatorPos = elevator.position;
    }

    private void _clampSpeed()
    {
        Vector2 localVelo = transform.InverseTransformDirection(_body.velocity);
        localVelo.x = Mathf.Clamp(localVelo.x, -MaxSpeed, MaxSpeed);
        _body.velocity = localVelo;
    }
}
