using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using Game.Languages;

[RequireComponent(typeof(TutorialEvents))]
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("Fondo")]
    [SerializeField] Canvas canvas;
    public RawImage fondo;
    [SerializeField] Color originalColor;
    [SerializeField] Color succesColor;
    [SerializeField] Color errorColor;

    [Header("Secuencia")]
    [SerializeField] TextMeshProUGUI instrucciones;
    [SerializeField] HackSequence secuenciaTutorial;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip alarma;

    [Header("Slider")]
    [SerializeField] TextMeshProUGUI encabezado;
    [SerializeField] Color colorEliminado;
    [SerializeField] Color colorDestruyendo;
    [SerializeField] Slider slider;

    [Header("Pasillos moviles")]
    [SerializeField] GameObject edificio;
    [SerializeField] Transform muroSuperior;
    [SerializeField] Transform muroInferior;

    int contadorHackeos;
    float duracionHackeo = 3f;
    TutorialEvents tutorialEvents;




    private void Awake()
    {
        if (instance != null) Destroy(instance);
        else instance = this;

        tutorialEvents = GetComponent<TutorialEvents>();
    }

    void Start()
    {
        if (Level2DManager.instance != null)
        {
            tutorialEvents.IniciarTutorial.AddListener(() =>
            {
                StartCoroutine(DialogosInicioTutorial());
            });
        }
    }


    void Update()
    {
        if (!isOnTutorial()) return;
    }




    bool isOnTutorial()
    {
        if (Level2DManager.instance.state != Level2DManager.State.Tutorial) return true;
        else return false;
    }

    public void SucessEffect()
    {
        if (contadorHackeos >= 2) return;
        StartCoroutine(CambiarColorFondoPorSucess());
        StartCoroutine(DisplayNewSequence());
        LlenarSliderYCambiarTextos();
    }

    public void ErrorEffects()
    {
        StartCoroutine(CambiarColorFondoPorError());
    }

    public void AlarmEffect()
    {
        LlenarSlider75();
        CambiarColorFondoPorSucess();
        instrucciones.enabled = false;
        secuenciaTutorial.gameObject.SetActive(false);

    }

    IEnumerator CambiarColorFondoPorError()
    {
        fondo.color = errorColor;
        yield return new WaitForSeconds(0.4f);
        fondo.color = originalColor;
    }

    IEnumerator CambiarColorFondoPorSucess()
    {
        fondo.color = succesColor;
        yield return new WaitForSeconds(duracionHackeo);
        fondo.color = originalColor;
    }

    IEnumerator DisplayNewSequence()
    {
        instrucciones.enabled = false;
        yield return new WaitForSeconds(duracionHackeo);
        secuenciaTutorial.ChangeHackSequence();
        secuenciaTutorial.SetActiveAllCodes();
        secuenciaTutorial.ResetHackSequence();
        secuenciaTutorial.gameObject.SetActive(true);
        instrucciones.enabled = true;
    }

    void LlenarSliderYCambiarTextos()
    {
        slider.gameObject.SetActive(true);
        encabezado.gameObject.SetActive(true);
        DOVirtual.Float(slider.value, 1, duracionHackeo, (x) =>
        {
            secuenciaTutorial.gameObject.SetActive(false);
            slider.value = x;
        }).OnComplete(() =>
        {
            if (contadorHackeos == 0)
            {
                slider.gameObject.SetActive(false);
                encabezado.gameObject.SetActive(false);

                //Esto lo hice para mantener la refertencia evitar modificar refernecias puestas manualmente
                if (instrucciones.gameObject.TryGetComponent(out TextSearcher x)) 
                    x.UpdateText("CINE_CTA_02");
                if (encabezado.gameObject.TryGetComponent(out TextSearcher a))
                    a.UpdateText("CINE_BAR_02");

                instrucciones.color = colorEliminado;
                instrucciones.fontSize = 40;
                encabezado.fontSize = 42;
                encabezado.color = colorEliminado;

            }
            else if (contadorHackeos == 1)
            {
                slider.gameObject.SetActive(false);
                encabezado.gameObject.SetActive(false);

                //Esto lo hice para mantener la refertencia evitar modificar refernecias puestas manualmente
                if (instrucciones.gameObject.TryGetComponent(out TextSearcher x))
                    x.UpdateText("CINE_CTA_03");
                if (encabezado.gameObject.TryGetComponent(out TextSearcher a))
                    a.UpdateText("CINE_BAR_03");

                instrucciones.color = colorDestruyendo;
                instrucciones.fontSize = 44;
                encabezado.fontSize = 48;
                encabezado.color = colorDestruyendo;

                duracionHackeo = 6;
                secuenciaTutorial.HackingCompleted.AddListener(AlarmEffect);

            }
            slider.value = 0;
            contadorHackeos++;
        });
    }

    void LlenarSlider75()
    {
        slider.gameObject.SetActive(true);
        encabezado.gameObject.SetActive(true);
        DOVirtual.Float(slider.value, 0.75f, 6f, (x) =>
        {
            slider.value = x;
        }).OnComplete(() =>
        {
            StartCoroutine(ParpadearAlarma());
            if (instrucciones.gameObject.TryGetComponent(out TextSearcher x))
                x.UpdateText("CINE_ALARM");
            instrucciones.fontSize = 150;
            instrucciones.color = colorDestruyendo;
            instrucciones.enabled = true;
            slider.gameObject.SetActive(false);
            encabezado.gameObject.SetActive(false);
        });
    }

    IEnumerator ParpadearAlarma()
    {
        float initVolume = audioSource.volume;
        //audioSource.Pause();
        for (int i = 0; i < 3; i++)
        {
            fondo.color = errorColor;
            instrucciones.enabled = true;
            audioSource.PlayOneShot(alarma);
            yield return new WaitForSeconds(0.9f);

            if (i == 2) break;
            fondo.color = originalColor;
            instrucciones.enabled = false;
            yield return new WaitForSeconds(0.9f);
        }
        //audioSource.UnPause();
        yield return new WaitForSeconds(1f);
        canvas.gameObject.SetActive(false);
        tutorialEvents.IniciarTutorial.Invoke();
    }



    #region Métodos públicos para triggers en el pasillo
    public void AnimacionPasillo()
    {
        MostrarUI();
        muroSuperior.transform.DOMoveY(muroSuperior.transform.position.y + 80, 12f).OnComplete(() =>
        {
            muroSuperior.gameObject.SetActive(false);
        });
        muroInferior.transform.DOMoveY(muroInferior.transform.position.y - 80, 12f).OnComplete(() =>
        {
            muroInferior.gameObject.SetActive(false);
        });
    }

    public void EsconderEdificio()
    {
        edificio.SetActive(false);
    }

    public void AgrandarPasillo()
    {
        muroSuperior.transform.DOScaleX(280, 1);
        muroInferior.transform.DOScaleX(280, 1);
    }

    void MostrarUI()
    {
        Tareas.Nueva(1.3f, () => tutorialEvents.TerminarTutorial.Invoke());
    }


    #endregion


    #region Métodos para el tutorial jugable
    public IEnumerator DialogosInicioTutorial()
    {
        yield return new WaitForSeconds(0.2f);
        if (Level2DManager.instance != null)
        {
            if (Level2DManager.instance.dialogoRobotIA != null && Level2DManager.instance.dialogoPlayer != null)
            {
                Level2DManager.instance.dialogoRobotIA.Talk(new DialogsData.Dialog("TUT_ROBOT_MOVE_01", 3f));
                yield return null;
                Level2DManager.instance.dialogoRobotIA.Talk(new DialogsData.Dialog("TUT_ROBOT_MOVE_02", 2.5f));
            }
        }
        yield return null;
    }

    public void RecogeExpDialogoEvent()
    {
        Tareas.Nueva(15f, () =>
        {
            if (Level2DManager.instance != null)
            {
                if (Level2DManager.instance.dialogoRobotIA != null && Level2DManager.instance.dialogoPlayer != null)
                {
                    Level2DManager.instance.dialogoRobotIA.Talk("TUT_ROBOT_LEVELUP");
                }
            }
        });
    }
    #endregion
}
