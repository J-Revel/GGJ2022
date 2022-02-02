using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    [SerializeField]
    private InputAction reloadAction;

    private CanvasGroup canvasGroup;

    private float transitionDuration = 1.5f;
    private bool isTransitionning;

    [Scene]
    public string[] levelScenes;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            DontDestroyOnLoad(this);

            reloadAction.Enable();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (reloadAction.ReadValue<float>() == 1f && ! isTransitionning)
        {
            Reset();
        }

        // if(!isTransitionning && transitionTime > 0f)
        // {
        //     transitionTime = Mathf.Max(0f, this.transitionTime - Time.deltaTime);
        //     AdaptVisual();
        // }
        // else if (isTransitionning)
        // {
        //     if (transitionTime < transitionDuration)
        //     {
        //         this.transitionTime += Time.deltaTime;
        //         AdaptVisual();
        //     }
        //     else
        //     {
        //         this.isTransitionning = false;
        //         this.transitionTime = transitionDuration;
        //         AdaptVisual();
        //         SceneManager.LoadScene(levelScenes[levelIndex], LoadSceneMode.Single);
        //     }
        // }
    }


    public static void Reset()
    {
        int selectedLevel = -1;
        for(int i=0; i<instance.levelScenes.Length; i++)
        {
            if(instance.levelScenes[i] == SceneManager.GetActiveScene().name)
            {
                selectedLevel = i;
            }
        }
        
        instance.LoadLevel(selectedLevel);
    }

    public void LoadLevel(int levelIndex)
    {
        instance.StartCoroutine(LoadLevelCoroutine(levelIndex));
    }

    public static void LoadNextLevel()
    {
        int selectedLevel = -1;
        for(int i=0; i<instance.levelScenes.Length; i++)
        {
            if(instance.levelScenes[i] == SceneManager.GetActiveScene().name)
            {
                selectedLevel = i;
            }
        }
        
        instance.LoadLevel(selectedLevel + 1);
    }

    private IEnumerator LoadLevelCoroutine(int levelIndex)
    {
        float time = 0;
        this.isTransitionning = true;
        while(time < transitionDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = time / transitionDuration;
            yield return null;
        }
        AsyncOperation op = SceneManager.LoadSceneAsync(levelScenes[levelIndex], LoadSceneMode.Single);
        while(!op.isDone)
            yield return null;
        while(time > 0)
        {
            time -= Time.deltaTime;
            canvasGroup.alpha = time / transitionDuration;
            yield return null;
        }
    }
}
