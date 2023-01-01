using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    public string mainMenuScene;
    public float splashScreenDuration;
    public float fadeOutDuration;
    public Image splashArt;

    void Start()
    {
        // Fade the splash art image in
        StartCoroutine(FadeInSplashArt());
    }

    IEnumerator FadeInSplashArt()
    {
        // Set the initial alpha to 0
        splashArt.color = new Color(1, 1, 1, 0);

        // Fade the image in over a period of time
        float elapsedTime = 0;
        while (elapsedTime < splashScreenDuration)
        {
            elapsedTime += Time.deltaTime;
            splashArt.color = new Color(1, 1, 1, elapsedTime / (splashScreenDuration - 4));

            // Check if the player has pressed any button to skip the splash screen
            if (Input.anyKeyDown)
            {
                StartCoroutine(FadeOutSplashArt(1f));
                break;
            }

            yield return null;
        }

        // Fade the splash art image out before transitioning to the main menu scene
        StartCoroutine(FadeOutSplashArt(fadeOutDuration));
    }

    IEnumerator FadeOutSplashArt(float fadeOutDuration)
    {
        // Fade the image out over a period of time
        float elapsedTime = 0;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            splashArt.color = new Color(1, 1, 1, 1 - elapsedTime / (fadeOutDuration - (fadeOutDuration * 0.5f)));
            yield return null;
        }

        // Load the main menu scene after the splash screen image has been faded out
        SceneManager.LoadScene(mainMenuScene);
    }
}
