using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BlockController : MonoBehaviour 
{
    Rigidbody2D _body;

    // elevator movement
    private bool _onGround;
    private bool _onElevator;
    private Vector2 _lastElevatorPos;
    private Transform _currentElevator;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // respect pause
        if (GameManager.Instance.Paused)
        {
            if (_body.bodyType != RigidbodyType2D.Static)
                _body.bodyType = RigidbodyType2D.Static;
            return;
        }
        else if (_body.bodyType == RigidbodyType2D.Static)
            _body.bodyType = RigidbodyType2D.Dynamic;

        _updateGroundRaycasts();
    }

    private void _updateGroundRaycasts()
    {
        // preserve last frame's elevator flag
        bool onElevatorLastFrame = _onElevator;

        // cast rays downwards on front and back
        int layerMask = LayerMask.GetMask(new string[] { "Default", "Transparent"});
        RaycastHit2D frontHit = Physics2D.Raycast(transform.position + transform.right / 2f - transform.up * 0.51f, -transform.up, 0.1f, layerMask);
        RaycastHit2D rearHit = Physics2D.Raycast(transform.position - transform.right / 2f - transform.up * 0.51f, -transform.up, 0.1f, layerMask);
        Debug.DrawLine(transform.position, transform.position - (transform.up * 0.6f));

        // check if we're on the ground
        _onGround = frontHit.transform != null || rearHit.transform != null;

        // stop here if we're not on the ground
        if (!_onGround)
        {
            return;
        }

        if (frontHit.transform)
            Debug.LogFormat("Front: " + frontHit.transform.name);
        if (rearHit.transform)
            Debug.LogFormat("Rear: " + rearHit.transform.name);

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
}
