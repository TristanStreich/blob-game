using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadScene : MonoBehaviour
{


    [TextArea]
    [SerializeField] private string[] QuipsForLoadingScreen;

    [Header("Elements to Drag Into Code")]
    public Slider loadingBar;
    public TMP_Text loadingText;
    public TMP_Text QuipText;
    public GameObject JellyLetterPrefab;

    [Header("Changeable Variables")]
    public float speedOfLoad;
    public int quipnumb;

    [Header("SceneNameToBeLoaded")]
    public string sceneNameToLoad;

    private AsyncOperation asyncOperation;
    

    void Start()
    {

        //pick a random quip to use in the loading screen
        quipnumb = Random.Range(0, QuipsForLoadingScreen.Length);
        QuipText.text = QuipsForLoadingScreen[quipnumb];

        // Start loading the new scene in the background
        asyncOperation = SceneManager.LoadSceneAsync(sceneNameToLoad) ;

        // Don't allow the new scene to activate until we're ready
        asyncOperation.allowSceneActivation = false;
    }

    void Update()
    {

        cycleQuips();

        //uses a fake loading bar that is easy to look at for the players
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



    //cycles between the list of texts on this script when you click left or right
    private void cycleQuips()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //get the previous quip so jellylettering works on the quip being turned away from
            int prevnum = quipnumb;

            //cycle through the quips on click
            if ((quipnumb + 1) >= QuipsForLoadingScreen.Length)
            {
                quipnumb = 0;
            }
            else quipnumb += 1;

            //present the text and jelly the last seen quip
            QuipText.text = QuipsForLoadingScreen[quipnumb];
            WordsToJelly(QuipsForLoadingScreen[prevnum]);
        }

        if (Input.GetMouseButtonUp(1))
        {
            //get the previous quip so jellylettering works on the quip being turned away from
            int prevnum = quipnumb;

            //cycle through the quips on click
            if ((quipnumb -1) <= -1)
            {
                quipnumb = (QuipsForLoadingScreen.Length -1);
            }
            else quipnumb -= 1;

            //present the text and jelly the last seen quip
            QuipText.text = QuipsForLoadingScreen[quipnumb];
            WordsToJelly(QuipsForLoadingScreen[prevnum]);
        }
    }


    //this code spawns a bunch of bouncy gameobjects that are each letter in a passed in sentence
    private void WordsToJelly(string text)
    {
        foreach (char c in text)
        {
            //makes sure there isnt too many letters to slow the game down
            if (gameObject.transform.childCount < 100)
            {
                //create obj at rand height and rotation
                GameObject NewLetter = Instantiate(JellyLetterPrefab, new Vector3(Random.Range(-10, 10), Random.Range(10, 15), 0), Quaternion.Euler(0, 0, Random.Range(0, 360)));

                //set it to child of UImanager
                NewLetter.transform.SetParent(gameObject.transform);

                //change the letter to mach the letters in the sentence used
                TMP_Text CurrentLettertext = NewLetter.GetComponent<TMP_Text>();
                CurrentLettertext.text = c.ToString();
            }

        }
    }
}


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