using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Camera MainCamera;
    public ChangeSceneZone RestartZone;

    public PlayerController Player;

    public Image FadeOverlay;
    public float FadeSpeed;

    private bool _paused;
    public bool Paused
    {
        get { return _paused; }
    }

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("There's already a game manager in this scene!");
            DestroyImmediate(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // setup restart zone
        RestartZone.NextScene = SceneManager.GetActiveScene().name;

        // set fade
        Color c = FadeOverlay.color;
        c.a = 1f;
        FadeOverlay.color = c;

        // pause game
        PauseGameplay();

        // fade out and unpause
        StartCoroutine(_fadeTo(0f, 1f, UnpauseGameplay));
    }

    private void Update()
    {
        if (Input.GetAxis("Restart") != 0)
        {
            ChangeScene(SceneManager.GetActiveScene().name);
        }
    }

    public void PauseGameplay()
    {
        _paused = true;
    }

    public void UnpauseGameplay()
    {
        _paused = false;
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