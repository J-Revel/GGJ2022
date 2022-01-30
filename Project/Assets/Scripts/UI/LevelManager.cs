using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    [SerializeField]
    private InputAction reloadAction;

    [SerializeField]
    private int levelID = 0;

    private CanvasGroup canvasGroup;

    private float transitionTime;
    private float transitionDuration = 1.5f;
    private bool isTransitionning;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            DontDestroyOnLoad(this);

            transitionTime = transitionDuration;
            reloadAction.Enable();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (reloadAction.ReadValue<float>() == 1f && ! isTransitionning)
        {
            StartTransition(levelID);
        }

        if(!isTransitionning && transitionTime > 0f)
        {
            transitionTime = Mathf.Max(0f, this.transitionTime - Time.deltaTime);
            AdaptVisual();
        }
        else if (isTransitionning)
        {
            if (transitionTime < transitionDuration)
            {
                this.transitionTime += Time.deltaTime;
                AdaptVisual();
            }
            else
            {
                this.isTransitionning = false;
                this.transitionTime = transitionDuration;
                AdaptVisual();
                LoadSceneByID();
            }
        }
    }

    private void AdaptVisual()
    {
        canvasGroup.alpha = transitionTime / transitionDuration;
    }

    public static void StartTransitionToScene(int levelID)
    {
        instance.StartTransition(levelID);
    }

    public static void StartTransitionToNextScene()
    {
        StartTransitionToScene(instance.levelID + 1);
    }

    private void StartTransition(int levelID)
    {
        this.levelID = levelID;
        this.isTransitionning = true;
    }

    private void LoadSceneByID()
    {
        SceneManager.LoadScene($"Level{levelID}", LoadSceneMode.Single);
    }
}