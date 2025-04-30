using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level2DManager : MonoBehaviour
{
    //Iniciar Tutorial y Terminar Tutorial son eventos que se ejecutan con triggers en la escena o con "TutorialManager"
    //TerminarTutorial da inicio al "juego", actiavando el spawn de enemigos entre otras cosas

    #region Variables

    public static Level2DManager instance;

    public enum State { Cinematica, Tutorial, Game, Upgrades };//"Cinematica" es la pantalla de hackeo incial
    public State state
    {
        get
        {
            return _state;
        }
        set
        {
            switch (value)
            {
                case State.Cinematica:
                    tutorialEvents.IniciarCinematica.Invoke();
                    break;
                case State.Tutorial:
                    tutorialEvents.IniciarTutorial.Invoke();
                    break;
                case State.Game:
                    tutorialEvents.TerminarTutorial.Invoke();
                    break;
                case State.Upgrades:
                    break;
            }

            _state = value;
        }
    }

    public PlayerController2D player2D
    {
        get
        {
            if (_player2D == null)
            {
                _player2D = FindAnyObjectByType<PlayerController2D>();
                if (_player2D != null)
                {
                    return _player2D;
                }
                else
                {
#if UNITY_EDITOR
                    //Este "if" evita que salte el Debug cuando es llamador por cualquier script heredero de "Editor"
                    if (new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Assembly.GetName().Name == "Assembly-CSharp-Editor")
                        return null;

                    //"new System.Diagnostics... te indica quien ejecuta la funcion
                    Debug.LogWarning(new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType +
                    ": solicito al Player2D, pero aún no existe");
#endif
                    return null;
                }
            }
            return _player2D;
        }
        set
        {
            _player2D = value;
        }
    }

    public Weapon.type playerWeaponType { get { return _playerWeaponType; } set { _playerWeaponType = value; } }



    //Publicas
    [Header("Estados y cambios de estados")]
    [SerializeField] State _state;
    [SerializeField] TutorialEvents tutorialEvents;


    [Header("Referencias de player")]
    [SerializeField] PlayerController2D _player2D;
    public DialogueWindow dialogoPlayer;
    public DialogueWindow dialogoRobotIA;


    [Space(10)]
    [Header("Eventos de cambios de estados")]
    [Space(5)] public UnityEvent UpgradesMenuOpen;
    public UnityEvent UpgradesMenuClose;

    public GameObject glitch;


    //Privadas
    private Weapon.type _playerWeaponType;
    private AudioSource audioSource;
    #endregion


    private void Awake()
    {
        if (instance != null) Destroy(instance);
        else instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (tutorialEvents != null)
        {
            tutorialEvents.IniciarCinematica.AddListener(() => _state = State.Cinematica);
            tutorialEvents.IniciarTutorial.AddListener(() => _state = State.Tutorial);
            tutorialEvents.TerminarTutorial.AddListener(() => _state = State.Game);
        }

        if (_player2D == null)
        {
            _player2D = FindAnyObjectByType<PlayerController2D>();
        }

        if (tutorialEvents != null)
        {
            switch (state)
            {
                case State.Cinematica:
                    tutorialEvents.IniciarCinematica.Invoke();
                    break;
                case State.Tutorial:
                    tutorialEvents.IniciarTutorial.Invoke();
                    break;
                case State.Game:
                    tutorialEvents.TerminarTutorial.Invoke();
                    break;
                case State.Upgrades:
                    break;
            }
        }
        else Debug.LogWarning(transform.name + " no tiene referenciado tutorialEvents");

        GameManager.instance.LoadFinalScene.AddListener(AutomatizarPersonaje);
        GameManager.instance.LoadFinalScene.AddListener(GameplayMusicController.instance.CrossFadeToFifth);

    }

    private void Update()
    {

    }


    public void PauseGame()
    {
        Time.timeScale = 0;
        if (player2D != null) player2D.enabled = false;
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        if (player2D != null) player2D.enabled = true;
    }

    //Funcion para forzar la transicion final
    void AutomatizarPersonaje()
    {
        player2D.automaticHack = true;
        player2D.automaticShoot = true;
        player2D.automaticMove = true;
    }

    IEnumerator ActivarGlitch()
    {
        glitch.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        glitch.SetActive(false);
    }


}
