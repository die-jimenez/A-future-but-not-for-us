using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LanguageSubMenu : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] Transform languageButton;
    [SerializeField] HackSequence mainLanguageSequence;
    [SerializeField] Transform subMenu;


    [Header("Line")]
    [SerializeField] RectTransform subMenuLine;
    [SerializeField] float maxlineHeight;


    [Header("Options")]
    [SerializeField] RectTransform english;
    [SerializeField] HackSequence englishButton;
    [SerializeField] RectTransform spanish;
    [SerializeField] HackSequence spanishButton;

    [Header("Efectos")]
    [SerializeField] GameObject glitchVolume;





    private void OnEnable()
    {
        OpenLanguageMenu();
    }


    public void OpenLanguageMenu()
    {
        //Reset values to start animation
        subMenu.gameObject.SetActive(true);
        spanish.sizeDelta = new Vector2(0, spanish.sizeDelta.y);
        english.sizeDelta = new Vector2(0, english.sizeDelta.y);
        subMenuLine.sizeDelta = new Vector2(subMenuLine.sizeDelta.x, 0);
        englishButton.gameObject.SetActive(false);
        spanishButton.gameObject.SetActive(false);
        englishButton.SetActiveAllCodes();
        spanishButton.SetActiveAllCodes();


        //Start move
        languageButton.DOMoveY(languageButton.position.y + 1.5f, 1);
        subMenuLine.DOSizeDelta(subMenuLine.sizeDelta + (Vector2.up * 150), 1f).OnComplete(() =>
        {
            english.DOSizeDelta(english.sizeDelta + (Vector2.right * 250), 0.55f).OnComplete(() => englishButton.gameObject.SetActive(true));
            spanish.DOSizeDelta(spanish.sizeDelta + (Vector2.right * 250), 0.55f).OnComplete(() => spanishButton.gameObject.SetActive(true));
        });
    }

    public void CloseLanguageMenu(string _language)
    {
        //Reset values to start animation
        englishButton.gameObject.SetActive(false);
        spanishButton.gameObject.SetActive(false);
        GameManager.instance?.ChangeLanguage(_language, false);

        //Start move
        english.DOSizeDelta(new Vector2(0, english.sizeDelta.y), 0.5f);
        spanish.DOSizeDelta(new Vector2(0, spanish.sizeDelta.y), 0.5f).OnComplete(() =>
        {
            languageButton.DOMoveY(languageButton.position.y - 1.5f, 1);
            subMenuLine.DOSizeDelta(new Vector2(subMenuLine.sizeDelta.x, 0), 1f).OnComplete(() => 
            {
                mainLanguageSequence?.gameObject.SetActive(true);
                DOVirtual.DelayedCall(0.3f, () => mainLanguageSequence.SetActiveAllCodes());
                DOVirtual.DelayedCall(0.3f, () => glitchVolume.SetActive(true));
                DOVirtual.DelayedCall(0.8f, () => {
                    glitchVolume.SetActive(false);
                    GameManager.instance?.ChangeLanguage(_language, true);
                    MainMenuManager.instance?.GoToTittleScrenn();
                    gameObject.SetActive(false);
                });
                //DOVirtual.DelayedCall(0.7f, () => GameManager.instance?.ChangeLanguage(_language, true));
                //DOVirtual.DelayedCall(0.6f, () => GameManager.instance);

            });
        });
    }
}
