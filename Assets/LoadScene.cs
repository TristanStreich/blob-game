using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadScene : MonoBehaviour
{
    public Slider loadingBar;
    public TMP_Text loadingText;

    public float speedOfLoad;

    [Header("SceneNameToBeLoaded")]
    public string sceneNameToLoad;

    private AsyncOperation asyncOperation;

    void Start()
    {
     
        

        // Start loading the new scene in the background
        asyncOperation = SceneManager.LoadSceneAsync(sceneNameToLoad) ;

        // Don't allow the new scene to activate until we're ready
        asyncOperation.allowSceneActivation = false;
    }

    void Update()
    {
        /*
        // Update the loading bar based on the progress of the async operation
        loadingBar.value = asyncOperation.progress;

        // Update the text to show the loading percentage
        loadingText.text = (asyncOperation.progress * 100f).ToString("F0") + "%";
        // Check if the load has finished
        if (asyncOperation.progress >= 0.9f)
        {
            //Change the Text to show the Scene is ready
            loadingText.text = "Press the space bar to continue";
            //Wait to you press the space key to activate the Scene
            if (Input.GetKeyDown(KeyCode.Space))
                //Activate the Scene
                asyncOperation.allowSceneActivation = true;
        }
        */

        loadingBar.value += speedOfLoad;
        // Update the text to show the loading percentage
        loadingText.text = (loadingBar.value).ToString("F0") + "%";

        // Check if the load has finished
        if (asyncOperation.progress >= 0.9f && loadingBar.value >= loadingBar.maxValue)
        {
            //Change the Text to show the Scene is ready
            loadingText.text = "Press the space bar to continue";
            //Wait to you press the space key to activate the Scene
            if (Input.GetKeyDown(KeyCode.Space))
                //Activate the Scene
                asyncOperation.allowSceneActivation = true;
        }
    }
}
