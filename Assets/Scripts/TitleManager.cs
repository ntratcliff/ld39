using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour 
{
    public string FirstLevel;
    public Image FadeOverlay;
    public float FadeSpeed;

	// Update is called once per frame
	void Update () 
	{
        if (Input.anyKeyDown)
            ChangeScene(FirstLevel);
	}

    public void ChangeScene(string scene)
    {
        // fade out, then load new scene
        StartCoroutine(_fadeTo(1f, 0, delegate
        {
            SceneManager.LoadScene(scene);
        }));
    }

    private IEnumerator _fadeTo(float alpha, float delay = 0, System.Action onFinished = null)
    {
        if(delay > 0)
            yield return new WaitForSeconds(delay);

        Color c = FadeOverlay.color;
        float start = c.a;
        float t = 0;
        do
        {
            yield return new WaitForEndOfFrame();
            t += FadeSpeed * Time.deltaTime;
            c.a = Mathf.Lerp(start, alpha, t);
            FadeOverlay.color = c;
        } while (c.a != alpha);


        if(onFinished != null)
            onFinished();
    }
        
}
