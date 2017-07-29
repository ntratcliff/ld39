using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryStatusMonitor : MonoBehaviour 
{
    public SolarBattery Battery;

    public SpriteRenderer StatusRenderer;

    public Sprite StatusGood;
    public Sprite StatusMeh;
    public Sprite StatusBad;
    public Sprite StatusDead;

    public float GoodMin    = 0.6f;
    public float MehMin     = 0.3f;
    public float BadMin     = 0f;

    public MonitorStatus _currentStatus;

    private void Start()
    {
        _currentStatus = _getBatteryStatus();
        _updateSprite();
    }

    private void Update()
    {
        _updateStatus();
    }

    private void _updateStatus()
    {
        MonitorStatus frameStatus = _getBatteryStatus();

        if(frameStatus != _currentStatus)
        {
            _currentStatus = frameStatus;
            _updateSprite();
        }
    }

    private MonitorStatus _getBatteryStatus()
    {
        if (Battery.ScalarCharge > GoodMin)
            return MonitorStatus.Good;

        if (Battery.ScalarCharge > MehMin)
            return MonitorStatus.Meh;

        if (Battery.ScalarCharge > BadMin)
            return MonitorStatus.Bad;

        return MonitorStatus.Dead;
    }

    private void _updateSprite()
    {
        switch (_currentStatus)
        {
            case MonitorStatus.Good: StatusRenderer.sprite = StatusGood;
                break;
            case MonitorStatus.Meh: StatusRenderer.sprite = StatusMeh;
                break;
            case MonitorStatus.Bad: StatusRenderer.sprite = StatusBad;
                break;
            case MonitorStatus.Dead: StatusRenderer.sprite = StatusDead;
                break;
        }
    }
}

public enum MonitorStatus
{
    Good,
    Meh,
    Bad,
    Dead
}
