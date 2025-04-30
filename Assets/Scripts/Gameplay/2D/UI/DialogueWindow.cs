using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Game.Languages;

[RequireComponent(typeof(AudioSource), typeof(Animator))]
public class DialogueWindow : MonoBehaviour
{
    //Este script esta basado en DialogueWindow que se usa en la parte 2D

    //Publicas
    [Header("Elementos del dialogo")]
    public DialogsData currentDialogs;
    public TMP_Text textMP;
    [SerializeField] AudioClip openOrCloseSound;

    [Header("Opciones")]
    public Color backgroundColor;
    [SerializeField] bool closeOnEachDialog;

    [Header("Debug")]
    public bool isDialogOpen;

    //Privadas
    Utilidades.Timer timer;
    const float kMaxTextTime = 0.1f;
    public static int TextSpeed = 3;
    private string currentText = " ";
    AudioSource audioSource;
    Animator animator;

    //Guarda los dialogos de posible cuando se utiliza TalkRandomly();
    [SerializeField] List<DialogsData.Dialog> dialogsPool = new List<DialogsData.Dialog>();

    //Almacena los dialogos cuando ya hay uno abierto, al cerrar el dialogo, ejecuta el siguiente
    //QUEUE ES UNA CLASE DE C#, es una lista pero mas limitada
    public Queue<DialogsData.Dialog> dialogQueue = new Queue<DialogsData.Dialog>();



    void Start()
    {
        timer = new Utilidades.Timer(1f);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (animator == null)
        {
            Debug.LogError("No Animator Controller on DialogueWindow: " + gameObject.name);
        }
    }



    public void Show(string text)
    {
        textMP.text = "";
        isDialogOpen = true;
        animator?.SetBool("Open", true);
        currentText = text;
    }

    public void Close()
    {
        animator?.SetBool("Open", false);
        isDialogOpen = false;
    }

    public void LoadDialogsList(DialogsData data)
    {
        dialogsPool.Clear();
        currentDialogs = data;
        foreach (DialogsData.Dialog dialog in data.dialogs)
        {
            dialogsPool.Add(dialog);
        }
    }

    //Esta funcion re-adapta el sistema de dialogos para que funcione con textos importados de excell (que es más nuevo), en vez textos en el propio codigo
    string Get_dialog_text_using_ID(DialogsData.Dialog dialog_with_id)
    {
        if (textMP == null)
        {
            Debug.LogWarning(transform.name + " no tiene asignado su TextMeshPro en el inspector");
            return "text error #7";
        }

        if (textMP.TryGetComponent(out TextSearcher searcher))
        {
            //dialog_with_id.text = searcher.GetUpdatedText(dialog_with_id.id);
            string text = searcher.GetUpdatedText(dialog_with_id.id);
            return text;
        }
        else
        {
            Debug.LogWarning("El Text no tiene TextSearcher.cs");
            return "text error #2";
        }
    }

    public void Talk(DialogsData.Dialog _dialog)
    {
        if (_dialog.multipleTexts)
        {
            StartCoroutine(TalkCor(_dialog));
            return;
        }
        //Creo un nuevo dialogo para no modificar los scriptable object (anque no siempre es por scriptable object)
        DialogsData.Dialog dialogEdited = new DialogsData.Dialog(_dialog.id, Get_dialog_text_using_ID(_dialog), _dialog.duration);
        StartCoroutine(TalkCor(dialogEdited));
    }

    
    [Tooltip("Reproduce un audio con el texto")]
    public void Talk(DialogsData.Dialog _dialog, AudioClip _audio)
    {
        if (_dialog.multipleTexts)
        {
            StartCoroutine(TalkCor(_dialog));
            return;
        }
        //Creo un nuevo dialogo para no modificar los scriptable object (anque no siempre es por scriptable object)
        DialogsData.Dialog dialogEdited = new DialogsData.Dialog(_dialog.id, Get_dialog_text_using_ID(_dialog), _dialog.duration);
        StartCoroutine(TalkWithAudioCor(dialogEdited, _audio));
    }


    [Tooltip("Permite enviar varios dialogos en solo una linea pero sin control de su duracion. El separador es |- ")]
    public void Talk(string dialogID)
    {
        float duracion = 2.5f;
        DialogsData.Dialog dialogCompleto = new DialogsData.Dialog(dialogID, duracion);
        string textoCompleto = Get_dialog_text_using_ID(dialogCompleto);

        //Identifica si el texto, no es multi dialogos
        if (!textoCompleto.Contains("|-"))
        {
            //Si el texto es muy largo, dura más
            if (textoCompleto.Length > 24) duracion = 3.5f;
            Talk(new DialogsData.Dialog(dialogID, duracion));
            return;
        }

        //Si es multi dialogo... divide Dialogo en multiples dialogos, les asigna su texto y evitar que Talk() re-busque el texto sin dividir 
        string[] textoDivididos = textoCompleto.Split("|-");
        foreach (string textosDividido in textoDivididos)
        {
            //Cambia la duración si el texto es muy largo
            if (textosDividido.Length > 24) duracion = 3.5f;
            //El "true" impide que rebusque los dailogos por el id
            DialogsData.Dialog dialogosDividido = new DialogsData.Dialog(dialogID, textosDividido, duracion, true);
            Talk(dialogosDividido);
        }
    }



    public void TalkOnPause(DialogsData.Dialog _dialog)
    {
        if (_dialog.multipleTexts)
        {
            StartCoroutine(TalkCor(_dialog));
            return;
        }
        //Creo un nuevo dialogo para no modificar los scriptable object (anque no siempre es por scriptable object)
        DialogsData.Dialog dialogEdited = new DialogsData.Dialog(_dialog.id, Get_dialog_text_using_ID(_dialog), _dialog.duration);
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        StartCoroutine(ProcessDialogsOnPause(dialogEdited));
    }


    public void TalkOnPause<T>(DialogsData.Dialog _dialog, T _var)
    {
        //Creo un nuevo dialogo para no modificar los scriptable object (anque no siempre es por scriptable object)
        string text = Get_dialog_text_using_ID(_dialog).Replace("-x-", _var.ToString());
        DialogsData.Dialog dialogEdited = new DialogsData.Dialog(_dialog.id, text, _dialog.duration);

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        StartCoroutine(ProcessDialogsOnPause(dialogEdited));
    }

    public void TalkRandomly()
    {
        if (dialogsPool.Count == 0)
        {
            Debug.LogWarning("Un personaje intento hablar pero no le quedan dialogos");
            return;
        }
        int randomIndex = Random.Range(0, dialogsPool.Count);
        DialogsData.Dialog nextDialog = dialogsPool[randomIndex];
        dialogsPool.Remove(dialogsPool[randomIndex]);
        Talk(nextDialog);
    }

    public void ClearDialogQueue()
    {
        dialogQueue.Clear();
    }

    public void ChangeTextAndAddVariable<T>(string dialogID, T _var)
    {
        DialogsData.Dialog temp = new DialogsData.Dialog(dialogID);
        string _text = Get_dialog_text_using_ID(temp).Replace("-x-", _var.ToString());
        textMP.text = _text;
    }






    #region Corrutinas privadas para hablar 
    //Las corrutinas no pueden ser ejecutadas en eventos desde el inspector, asi que cree metodos publicos void que las ejecutan
    private IEnumerator TalkCor(DialogsData.Dialog _dialog)
    {
        // Si un diálogo ya está abierto, añadir el nuevo diálogo a la cola
        if (isDialogOpen)
        {
            dialogQueue.Enqueue(_dialog);
            yield break;
        }
        Show(_dialog.text);
        yield return new WaitForSeconds(_dialog.duration);

        if (!closeOnEachDialog)//Se mantiene el dialogo abierto hasta que diga todos los dialogos
        {
            HandleContinuousDialog();
        }
        else //Se abre y cierra con cada dialogo en cola
        {
            Close();
            yield return new WaitForSeconds(0.5f);
            HandleCloseEachDialog();
        }
    }


    [Tooltip("Nace de TalkCoroutine pero agrega la reproduccion del audio")]
    private IEnumerator TalkWithAudioCor(DialogsData.Dialog _dialog, AudioClip _audio)
    {
        // Si un diálogo ya está abierto, añadir el nuevo diálogo a la cola
        if (isDialogOpen)
        {
            _dialog.audio = _audio;
            dialogQueue.Enqueue(_dialog);
            yield break;
        }

        audioSource.clip = _audio;
        audioSource.Play();
        Show(_dialog.text);
        yield return new WaitForSeconds(_dialog.duration - 0.2f);

        if (!closeOnEachDialog) //Se mantiene el dialogo abierto hasta que diga todos los dialogos
        {
            HandleContinuousDialog();
        } 
        else //Se abre y cierra con cada dialogo en cola
        {
            Close();
            yield return new WaitForSecondsRealtime(0.5f);
            HandleCloseEachDialog();
        }
    }


    [Tooltip("Nace de TalkCoroutine pero las corrutinas usan waitsForSecondRealTime para cuando la TimeScale = 0")]
    private IEnumerator ProcessDialogsOnPause(DialogsData.Dialog _dialog)
    {
        // Si un diálogo ya está abierto, añadir el nuevo diálogo a la cola
        if (isDialogOpen)
        {
            dialogQueue.Enqueue(_dialog);
            yield break;
        }
        Show(_dialog.text);
        yield return new WaitForSecondsRealtime(_dialog.duration - 0.2f);

        if (!closeOnEachDialog) //Se mantiene el dialogo abierto hasta que diga todos los dialogos
        {
            HandleContinuousDialog();
        }
        else //Se abre y cierra con cada dialogo en cola
        {
            Close();
            yield return new WaitForSecondsRealtime(0.3f);
            HandleCloseEachDialog();
        }
    }
    #endregion





    #region Eventos de animacion en animator
    private void HandleContinuousDialog()
    {
        //Muestra el siguiente diálogo en la cola... si hay
        if (dialogQueue.Count > 0)
        {
            //El orden de ejecución aca es importante, RESPETALO!
            var nextDialog = dialogQueue.Dequeue();
            isDialogOpen = false;
            Talk(nextDialog, nextDialog.audio);
            StartCoroutine(DisplayText());
        }
        else if (dialogQueue.Count == 0) Close();
    }

    private void HandleCloseEachDialog()
    {
        //Muestra el siguiente diálogo en la cola... si hay
        if (dialogQueue.Count > 0)
        {
            var nextDialog = dialogQueue.Dequeue();
            Talk(nextDialog);
        }
    }

    public void PlayOpenSound()
    {
        audioSource.PlayOneShot(openOrCloseSound);
    }

    public void OnDialogueOpen()
    {
        StartCoroutine(DisplayText());
    }

    public void OnDialogueClosed()
    {
        StopAllCoroutines();
        textMP.text = "";
    }

    private IEnumerator DisplayText()
    {
        /* La logica es... Se imprime el texto del color que tiene en el componente textMeshPro, pero luego,
         * en loop, se va reescribiendo todo el texto, pero desplazando la etiqueta de color del fondo a la derecha.
         * Vease, las letras no se pintan a blanco, sino que la etiqueta que les cambia de color para que sean invisibles, se va corriendo.
        */
        if (textMP == null)
        {
            Debug.LogError("Text is not linked in DialogueWindow: " + gameObject.name);
            yield return null;
        }

        //Agrego un espacio para evitar bugs en donde no se muestra la ultima letra
        string originalText = currentText + " ";
        string displayedText = "";
        textMP.text = "";

        for (int currentCharIndex = 0; currentCharIndex < originalText.Length; currentCharIndex++)
        {
            //Comprueba si el char inicia una tag de texto
            if (originalText[currentCharIndex] == '<')
            {
                //Busca donde termina la etiqueta y avanza el loop hasta el siguiente char
                currentCharIndex = FindClosingTagIndex(originalText, currentCharIndex);
            }
            textMP.text = originalText;
            string hexColor = ColorUtility.ToHtmlStringRGBA(backgroundColor);
            displayedText = textMP.text.Insert(currentCharIndex, "<color=#" + hexColor + ">");
            textMP.text = displayedText;
            yield return new WaitForSecondsRealtime(kMaxTextTime / TextSpeed);
        }
        yield return null;
    }

    int FindClosingTagIndex(string _text, int _currentCharIndex)
    {
        for (int i = _currentCharIndex + 1; i < _text.Length; i++)
        {
            if (_text[i] == '>')
            {
                //Si hay otra etiqueta...
                if (_text[i + 1] == '<')
                {
                    return FindClosingTagIndex(_text, i);
                }
                else return i + 1;
            }
        }
        //Si no ecnuentra el signo de cierre, regresa el index inicial
        return _currentCharIndex;
    }
    #endregion








}
