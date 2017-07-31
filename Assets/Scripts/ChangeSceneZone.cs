using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ChangeSceneZone : MonoBehaviour 
{
    public string NextScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            GameManager.Instance.ChangeScene(NextScene);
        }
    }
}
