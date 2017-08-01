using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour 
{
    public SolarBattery Battery;
    public BatteryStatusMonitor StatusMonitor;
    public float ConstantDrain;
    public SpriteRenderer Hydraulics;

    public Vector2 PoweredPos;
    public Vector2 DeadPos;
    public float Speed;

    private Vector2 _targetPos;
    private Vector2 _startPos;
    private float _step;
    private float _lerpTime;

    private void Start()
    {
        _targetPos = transform.position;
        _updateHydraulics();
    }

    private void Update()
    {
        if (GameManager.Instance.Paused)
            return;

        if(StatusMonitor.Status == MonitorStatus.Dead
            && _targetPos != DeadPos)
        {
            _setTarget(DeadPos);
        }
        else if(StatusMonitor.Status == MonitorStatus.Good
            && _targetPos != PoweredPos)
        {
            _setTarget(PoweredPos);
        }

        _lerpToTarget();
        _updateHydraulics();

        Battery.Drain(ConstantDrain * Time.deltaTime);
    }

    private void _setTarget(Vector2 target)
    {
        _targetPos = target;
        _startPos = transform.position;
        float dist = (_startPos - _targetPos).magnitude;
        _step = Speed / dist;
        _lerpTime = 0;
    }

    private void _lerpToTarget()
    {
        if ((Vector2)transform.position == _targetPos) return;

        _lerpTime += _step * Time.deltaTime;
        transform.position = Vector2.Lerp(_startPos, _targetPos, _lerpTime);

    }
    
    private void _updateHydraulics()
    {
        if (Hydraulics == null) return;

        Vector2 size = Hydraulics.size;

        size.y = ((Vector2)transform.position - DeadPos).magnitude;
        size.y = Mathf.Clamp(size.y, 0, float.PositiveInfinity);

        Hydraulics.size = size;

    }

    private void OnDrawGizmos()
    {
        Color c = Color.cyan;
        c.a = 0.3f;
        Gizmos.color = c;
        _drawGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        _drawGizmos();
    }

    private void _drawGizmos()
    {
        Gizmos.DrawLine(PoweredPos, DeadPos);
    }
}
