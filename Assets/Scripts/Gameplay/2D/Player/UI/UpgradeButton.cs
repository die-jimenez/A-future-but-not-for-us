using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Languages;

public class UpgradeButton : MonoBehaviour
{
    public UpgradeData upgradeData;
    public TextMeshProUGUI titleTMP;
    [SerializeField] TextMeshProUGUI descriptionTMP;
    [SerializeField] TextMeshProUGUI extraInfo; 
    [SerializeField] RawImage iconRawImage;

    [Header("Input y eventos")]
    public InputActionReference keyInput;
    [SerializeField] UnityEvent CodePress = new UnityEvent();




    private void Start()
    {
        if (titleTMP == null) Debug.LogWarning(transform.name + " no tiene la referencia del titulo");
        if (descriptionTMP == null) Debug.LogWarning(transform.name + " no tiene la referencia de la descripcion");
    }

    private void OnEnable()
    {
        keyInput.action.started += OnKeyInputStarted;
    }

    private void OnDisable()
    {
        keyInput.action.started -= OnKeyInputStarted;
    }

    public void SearchAndSetData(UpgradeData _upgradeData)
    {
        upgradeData = _upgradeData;//Esto sirve para que se vea en el inspector
        string _title = titleTMP.TryGetComponent(out TextSearcher search1) ? search1.GetUpdatedText(_upgradeData.title) : "";
        string _description = descriptionTMP.TryGetComponent(out TextSearcher search2) ? search2.GetUpdatedText(_upgradeData.description) : "";
        Debug.Log(_title);

        titleTMP.text = _title;
        descriptionTMP.text = _description;
        //iconRawImage.texture = upgradeData.icon;
    }

    public void Clean()
    {
        titleTMP.text = "Tittle";
        descriptionTMP.text = "Description";
        iconRawImage = null;
        upgradeData = null;
        titleTMP.color = Color.white;
        descriptionTMP.color = Color.white;
        GetComponent<Button>().interactable = true;
    }

    public void Block()
    {
        Button button = GetComponent<Button>();
        Color _color = button.colors.disabledColor;
        button.interactable = false;
        titleTMP.text = "Error " + Random.Range(101, 999).ToString() + "!";
        descriptionTMP.text = "at System Threading .Tasks.HandleNonSuccess()";
        titleTMP.color = _color;
        descriptionTMP.color = _color;

        if(UpgradePanelManager.instance.upgradePanel != null)
        {
            if (UpgradePanelManager.instance.upgradePanel.TryGetComponent(out CanvasShake script))
            {
                //script.PlayShake();
            }
        }
    }

    private void OnKeyInputStarted(InputAction.CallbackContext context)
    {
        CodePress.Invoke();
    }
}
