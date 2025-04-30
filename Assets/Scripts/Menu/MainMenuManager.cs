using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    [Header("Canvas")]
    [SerializeField] GameObject titleCanvas;
    [SerializeField] GameObject updateCanvas;
    [SerializeField] GameObject mainMenuCanvas;

    [Header("Secuencias")]
    [SerializeField] HackSequence sequenceTitleScreen;
    [SerializeField] HackSequence sequencePopUp;
    [SerializeField] HackSequence sequenceComenzar;
    [SerializeField] HackSequence sequenceIdioma;

    [Header("Music")]
    [SerializeField] MenuMusicController menuMusicController;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        sequenceComenzar?.HackingCompleted.RemoveAllListeners();
        sequenceIdioma?.HackingCompleted.RemoveAllListeners();

        //GoToGameScene lo agrego aca porque cuado se rejuegue, como el GameManager es el mismo de la patida anteriro, no serve agregarlo por el inpsector
        sequenceComenzar?.HackingCompleted.AddListener(() => GameManager.instance?.GoToGameScene());
        //botonIdioma?.HackingCompleted.AddListener(() => GameManager.instance?.GoToCredits());


    }

    public void GoToTittleScrenn()
    {
        if (titleCanvas == null || updateCanvas == null || mainMenuCanvas == null)
        {
            Debug.LogWarning(transform.name + " le falta la referencia de uno de los canvas de las pantallas");
        }
        ResetAllSequence();
        titleCanvas.SetActive(true);
        updateCanvas.SetActive(false);
        mainMenuCanvas.SetActive(false);

        if(menuMusicController  != null) menuMusicController.ReverseCrossFadeClips();
        else Debug.LogWarning(transform.name + " le falta la referencia del musicController");
    }

    public void GoToUpdatePopUp()
    {
        if (titleCanvas == null || updateCanvas == null || mainMenuCanvas == null)
        {
            Debug.LogWarning(transform.name + " le falta la referencia de uno de los canvas de las pantallas");
        }
        ResetAllSequence();
        titleCanvas.SetActive(true);
        updateCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }

    public void GoToMainMenu()
    {
        if (titleCanvas == null || updateCanvas == null || mainMenuCanvas == null)
        {
            Debug.LogWarning(transform.name + " le falta la referencia de uno de los canvas de las pantallas");
        }
        ResetAllSequence();
        titleCanvas.SetActive(false);
        updateCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void ResetAllSequence()
    {
        if (sequenceTitleScreen == null || sequencePopUp == null || sequenceComenzar == null || sequenceIdioma == null)
        {
            Debug.LogWarning(transform.name + " le falta las referencia de una de las secuencias");
        }
        sequenceTitleScreen.SetActiveAllCodes();
        sequencePopUp.SetActiveAllCodes();
        sequenceComenzar.SetActiveAllCodes();
        sequenceIdioma.SetActiveAllCodes();
    }

}
