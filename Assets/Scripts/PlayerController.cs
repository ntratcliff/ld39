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

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _updateElevator();
    }

    // apply forces in fixedupdate
    private void FixedUpdate()
    {
        _frameDrain = ConstantDrain;

        if(StatusMonitor.Status != MonitorStatus.Dead)
            _updateMovement();
        else
        {
            RearWheel.useMotor = false;
            FrontWheel.useMotor = false;
        }

        // apply drain to battery
        Battery.Drain(_frameDrain * Time.deltaTime);
    }

    private void _updateMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if(horizontal != 0)
        {
            _body.AddForce(transform.right * Mathf.Sign(horizontal) * MovementForce);
            _clampSpeed();
            _frameDrain += MovementDrain;
        }

        // apply brakes when not moving
        RearWheel.useMotor = horizontal == 0;
        FrontWheel.useMotor = horizontal == 0;
    }

    private void _updateElevator()
    {
        // preserve last frame's elevator flag
        bool onElevatorLastFrame = _onElevator;

        // cast rays downwards on front and back
        int layerMask = LayerMask.GetMask(new string[] { "Default" });
        RaycastHit2D frontHit = Physics2D.Raycast(transform.position + transform.right, -transform.up, 0.5f, layerMask);
        RaycastHit2D rearHit = Physics2D.Raycast(transform.position - transform.right, -transform.up, 0.5f, layerMask);

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
