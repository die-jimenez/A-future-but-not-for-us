using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[RequireComponent(typeof(RawImage))]
public class Code : MonoBehaviour
{
    public enum CodeOrder { fisrt, second, third };

    [Header("Configuracion")]
    public CodeOrder order;
    [SerializeField] bool iniciarOcultos;

    [Header("Keycodes")]
    public List<string> keys = new List<string>();


    [Header("Input key inicial")]
    [Tooltip("Con esto el codigo obtiene una key/inputAction al iniciar. Sirve para secuencias no controlados por terceros como el hackerWeapon")]
    public InputActionReference starterKeyInput;
    [SerializeField] bool hasStarterKeyInput;

    [Header("Eventos")]
    public UnityEvent CodePressed = new UnityEvent();
    HackSequence sequence;
    RawImage image;

    private void OnEnable()
    {
        //Cuando ya haya encontrado sus path keys
        if (keys.Count > 0)
        {
            SequenceManager.instance?.ChangeTexture(this);
        }

    }

    void Start()
    {
        image = GetComponent<RawImage>();
        CodePressed.AddListener(Disnable);
        if (hasStarterKeyInput) GetDefaultKeys();

        //No esta del todo bien esta funcion en este script, porque cada code sobrescribe la busqueda de textura 
        SequenceManager.instance?.ChangeTexture(this);

        if (transform.parent.TryGetComponent(out HackSequence _sequence))
        {
            sequence = _sequence;
        }
        else Debug.LogWarning(transform.name + " no encontro la refernecia del Hacksequence en su padre");
        if (iniciarOcultos) Disnable();

    }

    public void Disnable()
    {
        gameObject.SetActive(false);
    }

    public void ChangeImage(Texture2D _texture)
    {
        image.texture = _texture;
    }

    public void ChangeKeys(List<string> _keys)
    {
        keys = _keys;
    }

    void GetDefaultKeys()
    {
        foreach (InputBinding _key in starterKeyInput.action.bindings)
        {
            keys.Add(_key.effectivePath.Replace("/", "").Replace('<', '/').Replace('>', '/'));
        }
    }




}

