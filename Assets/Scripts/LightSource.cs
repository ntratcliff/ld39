using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour 
{
	public float RaysPerDegree = 1; // resolution of raycasting
	public float Range = 45f; // range (in degrees) of raycast
	private int _numRays; // number of rays to be cast
    private float _degreeStep;

	// Use this for initialization
	void Start () 
	{
		_numRays = Mathf.RoundToInt(Range * RaysPerDegree);
        _degreeStep = Range / _numRays;
	}
	
	// Update is called once per frame
	void Update () 
	{
        _castRays();			
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        _drawGizmos();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,1,1,0.3f);
        _drawGizmos();
    }

    private void _drawGizmos()
    {
        if (RaysPerDegree <= 0) return;

		_numRays = Mathf.RoundToInt(Range * RaysPerDegree);
        _degreeStep = Range / _numRays;

        Vector2[] points = _castRays(false);
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawLine(transform.position, points[i]);
            if(i > 0)
            {
                Gizmos.DrawLine(points[i - 1], points[i]);
            }            
        }
    }

    private Vector2[] _castRays(bool invokeAffected = true)
	{
        Vector2[] points = new Vector2[_numRays];

        for (int i = 0; i < _numRays; i++)
        {
            Vector2 direction = _getDirection(i);
            points[i] = (Vector2)transform.position + direction * 100;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);

            if(hit.transform != null)
            {
                points[i] = hit.point;
            }

            if (invokeAffected 
                && hit.transform != null
                && hit.transform.GetComponentInChildren<ILightAffected>() != null)
            {
                hit.transform.GetComponentInChildren<ILightAffected>().OnLightHit(); // TODO: check more generically
            }

        }

        return points;
	}

    private Vector2 _getDirection(int ray)
    {
        float angle = (-Range / 2f) + _degreeStep * ray;
        Vector2 direction = Quaternion.Euler(0, 0, angle) * transform.right;
        return direction;
    }
}
