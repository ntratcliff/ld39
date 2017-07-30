using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour 
{
	public float RaysPerDegree = 1; // resolution of raycasting
	public float Range = 45f; // range (in degrees) of raycast
    public GameObject LightObject;
    public LayerMask RaycastLayers;
	private int _numRays; // number of rays to be cast
	private float _degreeStep;
    private Mesh _mesh;
    private bool _initialized;

	// Use this for initialization
	void Start () 
	{
        _init();
	}

    private void _init()
    {
		_numRays = Mathf.RoundToInt(Range * RaysPerDegree);
		_degreeStep = Range / _numRays;

        Vector2[] hitPoints = _castRays();
        _updateMesh(hitPoints);

        LightObject.GetComponent<MeshRenderer>().enabled = true;

        _initialized = true;
    }
	
	// Update is called once per frame
	void Update () 
	{
		Vector2[] hitPoints = _castRays();
        _updateMesh(hitPoints);
	}

    private void _updateMesh(Vector2[] hits)
    {
        // add transform position to points
        Vector2[] points = new Vector2[hits.Length + 1];
        points[0] = transform.position;
        hits.CopyTo(points, 1);

        // get indicies from triangulator
        Triangulator tr = new Triangulator(points);
        int[] indicies = tr.Triangulate();

        // create verticies
        Vector3[] verticies = new Vector3[points.Length];
        for (int i = 0; i < verticies.Length; i++)
        {
            verticies[i] = new Vector3(points[i].x, points[i].y, 0);
        }

        if(_mesh == null)
        {
            // create the mesh
            _mesh = new Mesh();

            // set up game object
            LightObject.GetComponent<MeshFilter>().mesh = _mesh;
        }

        _mesh.vertices = verticies;
        _mesh.triangles = indicies;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
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

			RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, float.PositiveInfinity, RaycastLayers.value);

			if(hit.transform != null)
			{
				points[i] = hit.point;
			}

			if (invokeAffected 
				&& hit.transform != null
				&& hit.collider.transform.GetComponent<ILightAffected>() != null)
			{
				hit.collider.transform.GetComponent<ILightAffected>().OnLightHit(); 
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
