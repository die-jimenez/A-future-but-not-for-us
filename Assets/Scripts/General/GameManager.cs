using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //GameManager -------------------------------------------------------------------------------------------------
    public static GameManager instance { get; set; }
    public enum State { Menu, Game2D, Game3D, Credits };
    public enum Controls { original, alternative};

    //Idioma ==========================================================
    public enum Language { Es, En };
    public Language language;
    public Controls controls;
    [HideInInspector] public UnityEvent ApplyChangeLanguage = new UnityEvent();


    //Otros ===========================================================
    [HideInInspector] public Camera CameraGame2D;
    [HideInInspector] public UnityEvent LoadFinalScene;



    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;
    }

    void Start()
    {
#if UNITY_STANDALONE_WIN
        Cursor.visible = false;
#endif
        //Tareas.Nueva(2f, () => GoToFinalScene());
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void GoToGameScene()
    {
        DOTween.PauseAll();
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void GoToMainMenuScene()
    {
        DOTween.PauseAll();
        SetControls(Controls.original);
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public void GoToFinalScene()
    {
        SceneManager.LoadScene("Final3D", LoadSceneMode.Additive);
        //MainCameraGame2D se suam a LoadFinalScene cambiando su salida por un render texture
        LoadFinalScene.Invoke();
        
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene("Creditos", LoadSceneMode.Single);
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    public void ChangeLanguage(string _language, bool apply)
    {
        if (_language == "Es") language = Language.Es;
        else if (_language == "En") language = Language.En;
        else
        {
            Debug.LogWarning($"-{_language}- no es un idioma habilitado, solo se acepta -Es- o -En-");
            return;
        }

        if (apply) ApplyChangeLanguage.Invoke();
    }

    public void SetControls(Controls _control)
    {
        controls = _control;
    }

    
}
