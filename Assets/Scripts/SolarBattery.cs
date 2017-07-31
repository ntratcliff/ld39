using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SolarBattery : MonoBehaviour, ILightAffected
{
    public float MaxCharge;
    public float ChargeSpeed;
    public bool StartCharged;

    /// <summary>
    /// Whether or not light hit the battery this frame
    /// </summary>
    private bool _hitThisFrame = false;

    public float _charge;
    /// <summary>
    /// The current charge of the battery
    /// </summary>
    public float Charge
    {
        get { return _charge; }
    }
    /// <summary>
    /// The charge of the battery as a scalar value
    /// </summary>
    public float ScalarCharge
    {
        get { return _charge / MaxCharge; }
    }

    private void Start()
    {
        if (StartCharged)
            _charge = MaxCharge;
    }

    private void Update()
    {
        if (GameManager.Instance.Paused)
            return;

        _hitThisFrame = false; // reset hit flag for this frame
        //TODO: it might be best to reset the flag with a message sent from GameManager
    }

    /// <summary>
    /// Drains the battery by an amount
    /// </summary>
    public void Drain(float amount)
    {
        _charge = Mathf.Clamp(_charge - amount, 0, MaxCharge);
    }

    /// <summary>
    /// Recharges the battery by an amount
    /// </summary>
    /// <param name="amount"></param>
    public void Recharge(float amount)
    {
        _charge = Mathf.Clamp(_charge + amount, 0, MaxCharge);
    }

    public void OnLightHit()
    {
        // don't do anything if we've already been hit this frame
        if (_hitThisFrame) return;

        // set hit flag
        _hitThisFrame = true;

        // recharge
        Recharge(ChargeSpeed * Time.deltaTime);
    }
}
