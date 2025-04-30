using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DeathScreenManager : MonoBehaviour
{
    public static DeathScreenManager instance;
    [SerializeField] Volume volume;

    [Header("Robot")]
    [SerializeField] SpriteRenderer spriteRendererRobot;
    [SerializeField] RawImage rawImageRobot;
    [SerializeField] DialogueWindow robotDialog;
    [SerializeField] GameObject ondaExpansiva;

    [Header("Secuancias")]
    [SerializeField] HackSequence secuenciaPersonaje;
    [SerializeField] RawImage botonRevivir;
    [SerializeField] InputActionReference revivirKeyInput;
    [SerializeField] UnityEvent CodePress = new UnityEvent();

    [Header("Player")]
    [SerializeField] Color temporalHealthColor;
    PlayerHealth playerHealth;



    void Start()
    {
        robotDialog = rawImageRobot.GetComponentInChildren<DialogueWindow>();
        if (Level2DManager.instance.player2D.TryGetComponent(out PlayerHealth script))
        {
            playerHealth = script;
            playerHealth.Die.AddListener(PlayAnimation);
        }
    }


    void PlayAnimation()
    {
        volume.gameObject.SetActive(true);

        if (volume != null)
        {
            DOTween.To(() => volume.weight, x => volume.weight = x, 1, 2f)
                .OnStart(() =>
                {
                    BorrarSequenciaAnterior();
                    ChangeInputToDeathScreen();
                })
                .OnUpdate(() =>
                {
                    rawImageRobot?.gameObject.SetActive(true);
                    DrawRobotOnRawImage();
                })
                .OnComplete(() =>
                {
                    Level2DManager.instance?.PauseGame();
                    spriteRendererRobot?.gameObject.SetActive(false);
                    rawImageRobot?.gameObject.SetActive(true);
                    DrawRobotOnRawImage();
                    StartCoroutine(RobotDialogos());
                });
        }
    }

    void BorrarSequenciaAnterior()
    {
        if (SequenceManager.instance.sequenceStarted != null)
        {
            secuenciaPersonaje = SequenceManager.instance.sequenceStarted;
            SequenceManager.instance.ClearCurrentSequence();
            secuenciaPersonaje.gameObject.SetActive(false);
        }

    }

    IEnumerator RobotDialogos()
    {
        int tiempo = 30;
        if (Level2DManager.instance.player2D.automaticMove)
        {
            robotDialog?.TalkOnPause(new DialogsData.Dialog("DTH_ROBOT_AUTOMOVE_01", 3.5f));
            yield return new WaitForSecondsRealtime(4f);
            robotDialog?.TalkOnPause(new DialogsData.Dialog("DTH_ROBOT_AUTOMOVE_02", 3.5f));
            yield return new WaitForSecondsRealtime(4f);
            robotDialog?.TalkOnPause(new DialogsData.Dialog("DTH_ROBOT_AUTOMOVE_03", 3.5f));
            yield return new WaitForSecondsRealtime(3.7f);
            Revivir();
            yield break;
        }

        robotDialog?.TalkOnPause(new DialogsData.Dialog("DTH_ROBOT_01", 3.5f));
        yield return new WaitForSecondsRealtime(4f);
        robotDialog?.TalkOnPause(new DialogsData.Dialog("DTH_ROBOT_02", 3.5f));
        yield return new WaitForSecondsRealtime(4f);
        robotDialog?.TalkOnPause(new DialogsData.Dialog("DTH_ROBOT_03", 4f));
        yield return new WaitForSecondsRealtime(1.5f);
        HabilitarBotonRevivir();
        yield return new WaitForSecondsRealtime(3f);
        robotDialog?.TalkOnPause(new DialogsData.Dialog("DTH_ROBOT_04", 31f), tiempo);
        yield return new WaitForSecondsRealtime(0.5f);

        for (int i = tiempo; i > 0; i--)
        {
            yield return new WaitForSecondsRealtime(1);
            robotDialog?.ChangeTextAndAddVariable("DTH_ROBOT_COUNTDOWN", i);
        }

        botonRevivir.gameObject.SetActive(false);
        ChangeInputPlayer("NotHackeo");
        yield return new WaitForSecondsRealtime(1);
        robotDialog?.TalkOnPause(new DialogsData.Dialog("DTH_ROBOT_DEAD_01", 3.5f));
        yield return new WaitForSecondsRealtime(4);
        robotDialog?.TalkOnPause(new DialogsData.Dialog("DTH_ROBOT_DEAD_02", 3.5f));
        yield return new WaitForSecondsRealtime(4);
        Level2DManager.instance?.UnPauseGame();
        GameManager.instance?.GoToMainMenuScene();
    }


    void DrawRobotOnRawImage()
    {
        // Obtén la posición en el espacio global (world space) del SpriteRenderer
        Vector3 worldPosition = spriteRendererRobot.transform.position;

        // Convierte la posición global a coordenadas de pantalla
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Convierte las coordenadas de pantalla a coordenadas locales del Canvas
        RectTransform canvasRectTransform = GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            screenPosition,
            null, // No necesitas pasar una cámara porque es Screen Space Overlay
            out Vector2 localPosition
        );

        // Asigna la posición al RawImage
        rawImageRobot.rectTransform.anchoredPosition = localPosition;
    }

    void ChangeInputPlayer(string nameOfMap)
    {
        SequenceManager.instance.GetPlayerInput().SwitchCurrentActionMap(nameOfMap);
    }

    void ChangeInputToDeathScreen()
    {
        ChangeInputPlayer("DeathScreen");
    }

    void HabilitarBotonRevivir()
    {
        revivirKeyInput.action.started += OnKeyInputRevivir;
        botonRevivir?.gameObject.SetActive(true);
    }

    void Revivir()
    {
        revivirKeyInput.action.started -= OnKeyInputRevivir;
        ChangeInputPlayer("Hackeo");
        botonRevivir?.gameObject.SetActive(false);
        rawImageRobot?.gameObject.SetActive(false);
        spriteRendererRobot?.gameObject.SetActive(true);
        robotDialog.Close();
        Level2DManager.instance.UnPauseGame();
        volume.weight = 0;
        StopAllCoroutines();
        StartCoroutine(playerHealth?.RestoreLife(temporalHealthColor));
        ondaExpansiva?.SetActive(true);

        if (Level2DManager.instance.dialogoRobotIA != null)
        {
            Tareas.Nueva(5, () =>
            {
                Level2DManager.instance.dialogoRobotIA.Talk("DTH_ROBOT_REVIVE");
            });
        }


    }


    private void OnKeyInputRevivir(InputAction.CallbackContext context)
    {
        Revivir();
    }
}

