using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;


public class HackSequence : MonoBehaviour
{
    public bool isHackCompleted { get { return _isHackCompleted; } set { _isHackCompleted = value; } }


    #region Inspector
    [Header("Codigos de la secuencia")]

    [Tooltip("Referencia a los -codigos- que maneja esta -secuencia-")]
    public Code[] codesInGame = new Code[3];

    [Tooltip("Indica cual ha de ser el proximo -codigo- para lograr el hackeo")]
    [SerializeField] Code currentCode;

    //-------------------------------------------------------------------------------------
    [Space(10)]
    [Header("Opciones de la secuencia")]
    [Space(5)][SerializeField] bool keepStarterSequence;
    [SerializeField] bool resetWithErrors;


    [Space(10)]
    [Header("Eventos")]
    public UnityEvent HackingCompleted;
    public UnityEvent HackingFailed;
    #endregion



    bool _isHackCompleted;



    private void OnEnable()
    {
        SequenceManager.instance?.sequencesActive.Add(this);
        if (codesInGame.Length > 0) currentCode = codesInGame[0];

    }

    private void OnDisable()
    {
        SequenceManager.instance?.sequencesActive.Remove(this);
    }


    void Start()
    {
        FindChildrenReference();
        HackingCompleted.AddListener(() => isHackCompleted = true);

        //Se repite en el Start por si en "OnEnable" no se ha inicilizado la primera vez
        if (codesInGame.Length > 0) currentCode = codesInGame[0];
        if (!SequenceManager.instance?.sequencesActive.Contains(this) ?? false)
        {
            SequenceManager.instance?.sequencesActive.Add(this);
        }
        //------------------------------------------------------------------------------

        if (resetWithErrors)
        {
            HackingFailed.AddListener(() =>
            {
                ResetHackSequence();
                DisplayHackingSequence();
            });
        }
    }


    //---------------------------------------------------------------------------------------------------------------
    #region Métodos publicos para SequenceManager
    public void DisplayHackingSequence()
    {
        gameObject.SetActive(true);
        if (keepStarterSequence)
        {
            SetActiveAllCodes();
        }
        else
        {
            ChangeHackSequence();
            SetActiveAllCodes();
        }
        ResetHackSequence();
    }

    public void SetActiveAllCodes()
    {
        for (int i = 0; i < codesInGame.Length; i++)
        {
            codesInGame[i].gameObject.SetActive(true);
        }
    }

    public void ResetHackSequence()
    {
        isHackCompleted = false;
        currentCode = codesInGame[0];
    }

    public void ChangeHackSequence()
    {
        List<int> randomCodeOrder = Matematicas.GenerarNumerosConsecutivosAleatorios(0, 2);
        for (int i = 0; i < codesInGame.Length; i++)
        {
            if (SequenceManager.instance != null)
            {
                codesInGame[i].ChangeImage(SequenceManager.instance.codesData[randomCodeOrder[i]].currentTexture);
                codesInGame[i].ChangeKeys(SequenceManager.instance.codesData[randomCodeOrder[i]].keys);
            }

        }
    }

    public Code NextCode()
    {
        for (int i = 0; i < codesInGame.Length; i++)
        {
            if (currentCode == codesInGame[i])
            {
                if (i == codesInGame.Length - 1)
                {
                    return codesInGame[0];
                }
                return codesInGame[i + 1];
            }
        }
        return null;
    }

    public Code GetFisrtCode()
    {
        return codesInGame[0];
    }

    public Code GetCurrentCode()
    {
        return currentCode;
    }

    public void SetNewCurrentCode()
    {
        currentCode = NextCode();
    }

    public bool isTheLastCode()
    {
        if (currentCode == codesInGame[codesInGame.Length - 1]) return true;
        else return false;
    }

    public Code GetLastCode()
    {
        return codesInGame[codesInGame.Length - 1];
    }

    public void AutoPress()
    {
        currentCode.CodePressed.Invoke();
        if (currentCode == codesInGame[2])
        {
            HackingCompleted.Invoke();
        }
        currentCode = NextCode();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------------------



    //---------------------------------------------------------------------------------------------------------------
    #region Métodos privados
    void FindChildrenReference()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Code script))
            {
                codesInGame[i] = script;
            }
            else Debug.LogError(transform.name + "... no encontró la referencia del código -" + i + 1 + "-");
        }
    }
    #endregion
    //---------------------------------------------------------------------------------------------------------------

}
