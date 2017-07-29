using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    public float MovementForce;
    public float MaxSpeed;

    public HingeJoint2D RearWheel, FrontWheel;

    public SolarBattery Battery;
    public float ConstantDrain = 1f;
    public float MovementDrain = 2f;
    private float _frameDrain;

    // attached components
    private Rigidbody2D _body;

    // movement variables

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    // apply forces in fixedupdate
    private void FixedUpdate()
    {
        _frameDrain = ConstantDrain;

        _updateMovement();

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

    private void _clampSpeed()
    {
        Vector2 localVelo = transform.InverseTransformDirection(_body.velocity);
        localVelo.x = Mathf.Clamp(localVelo.x, -MaxSpeed, MaxSpeed);
        _body.velocity = localVelo;
    }
}
