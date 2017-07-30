using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OutlineCameraBorder : MonoBehaviour 
{
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.3f, 1, 0.3f);

        Camera camera = GetComponent<Camera>();
        float verticalHeightSeen = camera.orthographicSize * 2.0f;

        Gizmos.DrawWireCube(transform.position, new Vector3((verticalHeightSeen * camera.aspect), verticalHeightSeen, 0));
    }
}
