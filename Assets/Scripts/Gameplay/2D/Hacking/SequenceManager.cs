using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SequenceManager : MonoBehaviour
{
    //Configuracion
    public static SequenceManager instance;

    [System.Serializable]
    public class CodeData
    {
        [SerializeField] Texture2D _currentTexture;
        public Texture2D def_texture;
        public Texture2D alt_texture;
        public List<string> keys;

        public Texture2D currentTexture
        {
            get { return _currentTexture != null ? _currentTexture : def_texture; }
            set { _currentTexture = value; }
        }

    }
    public CodeData[] codesData { get { return _codesData; } set { _codesData = value; } }


    //-------------------------------------------------------------------------------------
    [Header("Configuracion automatica")]
    [SerializeField] PlayerInput hackInput;//Siento que SequenceManager no deberia tener el player Input pero ya es tarde
    [SerializeField] CodeData[] _codesData = new CodeData[3];

    [Tooltip("Si es 'null' no se ha introducido el primer codigo de cualqueir secuencia")]
    public HackSequence sequenceStarted;
    public Code currentCodeToDebug;

    //-------------------------------------------------------------------------------------
    [Tooltip("Secuencia controladas")]
    [Space(5)] public List<HackSequence> sequencesActive = new List<HackSequence>();

    InputAction press1;
    InputAction press2;
    InputAction press3;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        hackInput = GetComponent<PlayerInput>();
        press1 = hackInput.actions["Press1"];
        press2 = hackInput.actions["Press2"];
        press3 = hackInput.actions["Press3"];
        GetKeysFromInputSystem();
    }

    private void OnEnable()
    {
        if (press1 != null) press1.started += PressCurrentCode;
        if (press1 != null) press1.started += ChangeControlTexture;

        if (press2 != null) press2.started += PressCurrentCode;
        if (press2 != null) press2.started += ChangeControlTexture;

        if (press3 != null) press3.started += PressCurrentCode;
        if (press3 != null) press3.started += ChangeControlTexture;

    }

    private void OnDisable()
    {
        if (press1 != null) press1.started -= PressCurrentCode;
        if (press1 != null) press1.started -= ChangeControlTexture;

        if (press2 != null) press2.started -= PressCurrentCode;
        if (press2 != null) press2.started -= ChangeControlTexture;

        if (press3 != null) press3.started -= PressCurrentCode;
        if (press3 != null) press3.started -= ChangeControlTexture;
    }

    void Start()
    {
        StartCoroutine(Debugger());
        if (GameManager.instance?.controls == GameManager.Controls.original)
        {
            SetDefaultTextures();
        }
        else if (GameManager.instance?.controls == GameManager.Controls.alternative)
        {
            SetAltTextures();
        }

    }


    void Update()
    {

    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------
    void PressCurrentCode(InputAction.CallbackContext context)
    {
        // Si no hay secuencia iniciada
        if (sequenceStarted == null)
        {
            TryStartNewSequence(context);
        }
        else
        {
            // Si ya hay una secuencia iniciada, procesar el siguiente código
            ContinueActiveSequence(context);
        }
    }

    #region Refactorizacion de PressCurrentCode();
    void TryStartNewSequence(InputAction.CallbackContext context)
    {
        foreach (HackSequence sequence in sequencesActive)
        {
            var firstCode = sequence.GetFisrtCode();
            if (!firstCode.enabled) return;

            if (CodeMatchesKeys(firstCode, context.control.path))
            {
                currentCodeToDebug = sequence.NextCode();//Solo sirve para debug en el inspector
                firstCode.CodePressed.Invoke();
                sequence.SetNewCurrentCode();
                sequenceStarted = sequence;
            }
        }

        // Si es una secuencia con un solo codigo
        if (sequenceStarted?.codesInGame.Length == 1)
        {
            LastCodePresed();
        }
    }

    void ContinueActiveSequence(InputAction.CallbackContext context)
    {
        HackSequence _sequence = sequenceStarted;
        var currentCode = _sequence.GetCurrentCode();
        if (!currentCode.enabled) return;

        if (CodeMatchesKeys(currentCode, context.control.path))
        {
            currentCodeToDebug = _sequence.NextCode();//Solo sirve para debug en el inspector
            currentCode.CodePressed.Invoke();
            _sequence.SetNewCurrentCode();

            // Si es el último código, completar la secuencia
            if (currentCode == _sequence.GetLastCode())
            {
                LastCodePresed();
            }
            return;
        }

        //Si se presiono el codigo equivocado
        FailSequence();
    }

    void LastCodePresed()
    {
        if (sequenceStarted == null)
        {
            Debug.LogWarning($"{gameObject.name} no tiene referencia de ninguna referencia iniciada (Es un dinamica)");
        }
        sequenceStarted.HackingCompleted.Invoke();
        ResetSequenceState();
    }

    void ResetSequenceState()
    {
        currentCodeToDebug = null;
        sequenceStarted = null;
    }

    bool CodeMatchesKeys(Code code, string controlPath)
    {
        return code.keys.Contains(controlPath);
    }

    void FailSequence()
    {
        sequenceStarted.HackingFailed.Invoke();
        currentCodeToDebug = null;
        sequenceStarted = null;
    }
    #endregion
    //-----------------------------------------------------------------------------------------------------------------------------------------------



    void ChangeControlTexture(InputAction.CallbackContext context)
    {
        if (context.control.name.All(char.IsDigit))
        {
            SetDefaultTextures();
            GameManager.instance?.SetControls(GameManager.Controls.original);
        }
        else
        {
            if (context.control.name.Contains("numpad"))
            {
                SetDefaultTextures();
                GameManager.instance?.SetControls(GameManager.Controls.original);
            }
            else
            {
                GameManager.instance?.SetControls(GameManager.Controls.alternative);
                SetAltTextures();
            }
        }

        ChangeTextures();
    }

    void ChangeTextures()
    {
        foreach (HackSequence _sequence in sequencesActive)
        {
            foreach (Code code in _sequence.codesInGame)
            {
                ChangeTexture(code);
            }
        }
    }

    public void ChangeTexture(Code _code)
    {
        foreach (CodeData codeData in codesData)
        {
            if (_code.keys.SequenceEqual(codeData.keys))
            {
                _code.ChangeImage(codeData.currentTexture);
            }
        }
    }

    void SetDefaultTextures()
    {
        foreach (CodeData codeData in codesData)
        {
            codeData.currentTexture = codeData.def_texture;
        }
    }

    void SetAltTextures()
    {
        foreach (CodeData codeData in codesData)
        {
            codeData.currentTexture = codeData.alt_texture;
        }
    }






    IEnumerator Debugger()
    {
        yield return new WaitForEndOfFrame();
        if (sequencesActive.Count == 0)
        {
            Debug.LogWarning("SuquenceManager necesita al menos una 'codeSequence'. Revisa de referenciar este manager en las secuencias");
        }
    }

    public void ClearCurrentSequence()
    {
        sequenceStarted.HackingFailed.Invoke();
    }


    void GetKeysFromInputSystem()
    {
        //Debo modificar asi la cadena de string para que coincida exactamente con el "context.control.path"
        foreach (InputBinding _key in press1.bindings)
        {
            codesData[0].keys.Add(_key.effectivePath.Replace("/", "").Replace('<', '/').Replace('>', '/'));
        }
        foreach (InputBinding _key in press2.bindings)
        {
            codesData[1].keys.Add(_key.effectivePath.Replace("/", "").Replace('<', '/').Replace('>', '/'));
        }
        foreach (InputBinding _key in press3.bindings)
        {
            codesData[2].keys.Add(_key.effectivePath.Replace("/", "").Replace('<', '/').Replace('>', '/'));
        }
    }

    public PlayerInput GetPlayerInput()
    {
        return hackInput;
    }


    //Se usa en el CodePress de los Code de los Upgrade
    public void PressUpgradeCode(HackSequence _sequence)
    {
        _sequence.HackingCompleted.Invoke();
        currentCodeToDebug = null;
        sequenceStarted = null;
    }








    /* PressCurrentCode antes de refactorizar (Lo estoy guardadno por si falla)

  void PressCurrentCode(InputAction.CallbackContext context)
  {
      //Los eventos de las secuencias y codes se ejecutan en el manager para centrarlizar en un solo script la ejecucion de todos estos eventos
      //y así no tenerlos dispersos en varios, lo cual, tras analizarlo dos veces distintas, es la mejor opcion

      //Si aún no se ha empezado a "hackear" ninguna secuencia... PRIMER CODIGO
      if (sequenceStarted == null)
      {
          foreach (HackSequence _sequence in sequencesActive)
          {
              if (_sequence.GetFisrtCode().enabled)
              {
                  foreach (string _key in _sequence.GetFisrtCode().keys)
                  {
                      if (_key == context.control.path)
                      {
                          //Debug.Log(context.control.name);

                          _sequence.GetFisrtCode().CodePressed.Invoke();
                          currentCodeToDebug = _sequence.NextCode();
                          _sequence.SetNewCurrentCode();
                          sequenceStarted = _sequence;
                          return;
                      }
                  }
              }
          }
      }
      //Una vez se empezo a "hakear" una secuencia... SEGUNDO O TERCER CODIGO
      else
      {
          if (sequenceStarted.GetCurrentCode().enabled)
          {
              HackSequence _sequence = sequenceStarted;
              foreach (string _key in _sequence.GetCurrentCode().keys)
              {
                  if (_key == context.control.path)
                  {
                      _sequence.GetCurrentCode().CodePressed.Invoke();
                      //Si es el último codigo para terminar el hackeo
                      if (_sequence.GetCurrentCode() == _sequence.LastCode())
                      {
                          _sequence.HackingCompleted.Invoke();
                          currentCodeToDebug = null;
                          sequenceStarted = null;
                      }
                      currentCodeToDebug = _sequence.NextCode();
                      _sequence.SetNewCurrentCode();
                      return;
                  }
              }
          }
      }

      //Si no se presiono la codigo correcto
      if (sequenceStarted != null)
      {
          sequenceStarted.HackingFailed.Invoke();
          currentCodeToDebug = null;
          sequenceStarted = null;
          return;
      }
  }

  */







}
