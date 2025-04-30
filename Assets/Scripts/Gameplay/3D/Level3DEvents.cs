using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;


public class Level3DEvents : MonoBehaviour
{
    [System.Serializable]
    public class MultiLanguageAudio 
    {
        public AudioClip es;
        public AudioClip en;
    }




    public static Level3DEvents instance;
    //public enum states

    [Header("Dialogos General")]
    public DialogueWindow dialogoCelular;
    public AudioSource audioSource;
    [SerializeField] AudioClip notificacion;

    [Header("Dialogos Ia")]
    [SerializeField] MultiLanguageAudio tienesUnNuevoMensaje;
    [SerializeField] MultiLanguageAudio esteMensajeFueEscrito;
    [SerializeField] MultiLanguageAudio quieresResponder;
    [SerializeField] MultiLanguageAudio mensajeEnviado;


    [Header("Dialogos Pareja")]
    [SerializeField] List<MultiLanguageAudio> audiosPareja = new List<MultiLanguageAudio>();


    [Header("Dialogos Personaje")]
    [SerializeField] List<MultiLanguageAudio> audiosPersonaje = new List<MultiLanguageAudio>();


    [Header("Colliders para responder")]
    [SerializeField] Collider trigerToReply;

    [Header("Dormir")]
    [SerializeField] MultiLanguageAudio audioMimirForzado;
    [SerializeField] AudioClip apagarLuz;
    [SerializeField] RawImage fondoNegro;
    [SerializeField] GameObject sommier;

    [Header("Eventos")]
    [Space(5)]
    public UnityEvent LlegaPrimerMensaje = new UnityEvent();


    string[] mensajesPareja = {
        "3D_GIRL_01",
        "3D_GIRL_02",
        "3D_GIRL_03",
        "3D_GIRL_04",
        "3D_GIRL_05",
        "3D_GIRL_06",
        "3D_GIRL_07",
        "3D_GIRL_08",
        "3D_IA_NOTIFICATION_01"
    };

    string[] mensajesPersonaje = {
        "3D_CHR_01",
        "3D_CHR_02",
        "3D_CHR_03",
        "3D_CHR_04",
        "3D_CHR_05",
        "3D_IA_NOTIFICATION_02",
    };




    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    void Start()
    {
        LlegaPrimerMensaje.AddListener(() =>
        {
            StartCoroutine(RecibirMensaje());
        });
        LlegaPrimerMensaje.Invoke();

        MimirForzado();
    }


    public void ResponderMensajes()
    {
        StartCoroutine(ResponderMensajesCor());

    }


    IEnumerator RecibirMensaje()
    {
        yield return new WaitForSeconds(7);

        audioSource.PlayOneShot(notificacion);
        yield return new WaitForSeconds(0.3f);
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPareja[0], 5.5f), GetCurrentAudio(tienesUnNuevoMensaje));
        yield return new WaitForSeconds(1f);
        
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPareja[1], 3f), GetCurrentAudio(audiosPareja[0]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPareja[2], 3.3f), GetCurrentAudio(audiosPareja[1]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPareja[3], 3.5f), GetCurrentAudio(audiosPareja[2]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPareja[4], 2f), GetCurrentAudio(audiosPareja[3]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPareja[5], 2.5f), GetCurrentAudio(audiosPareja[4]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPareja[6], 4.5f), GetCurrentAudio(audiosPareja[5]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPareja[7], 2.5f), GetCurrentAudio(audiosPareja[6]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPareja[8], 4f), GetCurrentAudio(esteMensajeFueEscrito));

        //Espera que todos los mensajes ahayan terminado 
        yield return new WaitForSeconds(30);
        DialogsData.Dialog _quieresResponder = new DialogsData.Dialog("3D_IA_QUESTION", 300f);
        dialogoCelular.Talk(_quieresResponder, GetCurrentAudio(quieresResponder));
        
        //Activa el collider para poder responder
        yield return new WaitForSeconds(1);
        trigerToReply.gameObject.SetActive(true);
        PlayerCanInteractAgain();
    }


    public IEnumerator ResponderMensajesCor()
    {
        //Cambia el layer del celular para que no se pueda responder mas
        trigerToReply.gameObject.SetActive(false);
        dialogoCelular.Close();

        yield return new WaitForSeconds(1f);
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPersonaje[0], 3f), GetCurrentAudio(audiosPersonaje[0]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPersonaje[1], 2.5f), GetCurrentAudio(audiosPersonaje[1]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPersonaje[2], 3f), GetCurrentAudio(audiosPersonaje[2]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPersonaje[3], 3.5f), GetCurrentAudio(audiosPersonaje[3]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPersonaje[4], 2.5f), GetCurrentAudio(audiosPersonaje[4]));
        dialogoCelular.Talk(new DialogsData.Dialog(mensajesPersonaje[5], 4f), GetCurrentAudio(esteMensajeFueEscrito));

        dialogoCelular.Talk(new DialogsData.Dialog("3D_DOTS", 0.8f));
        dialogoCelular.Talk(new DialogsData.Dialog("3D_DOTS", 0.8f));
        dialogoCelular.Talk(new DialogsData.Dialog("3D_DOTS", 0.8f));
        dialogoCelular.Talk(new DialogsData.Dialog("3D_IA_ANSWER", 2.5f), GetCurrentAudio(mensajeEnviado));

        yield return new WaitForSeconds(20f);
        int LayerInteractuable = LayerMask.NameToLayer("Interactable");
        sommier.layer = LayerInteractuable;

    }


    public void MimirForzado()
    {
        float tiempoInicio = 90;

        //Canclea todas las corrutinas
        Tareas.Nueva(tiempoInicio, () =>
        {
            StopAllCoroutines();
            dialogoCelular.StopAllCoroutines();

            audioSource.Stop();
            dialogoCelular.ClearDialogQueue();
            dialogoCelular.Close();
        });

        Tareas.Nueva(tiempoInicio + 1, () => dialogoCelular.Talk(new DialogsData.Dialog("3D_IA_END", 2.5f)));
        Tareas.Nueva(tiempoInicio + 1.5f, () => audioSource.PlayOneShot(GetCurrentAudio(audioMimirForzado)));
        Tareas.Nueva(tiempoInicio + 4.7f, () => { audioSource.PlayOneShot(apagarLuz); });
        Tareas.Nueva(tiempoInicio + 5, () =>
        {
            fondoNegro.gameObject.SetActive(true);
            fondoNegro.DOFade(1, 0);
        });
        Tareas.Nueva(tiempoInicio + 10, () =>
        {
            GameManager.instance.GoToCredits();
        });
    }

    public void Mimir()
    {
        fondoNegro.gameObject.SetActive(true);
        fondoNegro.DOFade(1, 3);
        Tareas.Nueva(5, () =>
        {
            GameManager.instance.GoToCredits();
        });
    }

    AudioClip GetCurrentAudio(MultiLanguageAudio _audios)
    {
        if (GameManager.instance == null)
        {
            Debug.Log(transform.name + "No encontro al GameManager");
            return _audios.en;
        }

        if (GameManager.instance.language == GameManager.Language.En)
            return _audios.en;
        else if (GameManager.instance.language == GameManager.Language.Es)
            return _audios.es;

        return _audios.en;
    }

    void PlayerCanInteractAgain()
    {
        //El sistema para evitar que el jugador pueda seguir hackeando en el juego 2D es cambiando el actionMap del player Input
        //con el nuevo cambio de que el mundo 3D tambien usa el SequenceManager, ese otro playerInput tambien afecta al mundo en 3D
        //Eso hace que no puedas interactuar. Con esto lo arreglo... pero tambien estoy dejando que hackeees en el juego 2D, lo que no es la idea, pero ni modo
        SequenceManager.instance.GetPlayerInput().SwitchCurrentActionMap("Hackeo");
    }
}
