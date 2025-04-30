using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayerDialogs2D : MonoBehaviour
{
    public DialogueWindow dialogueWindow;
    [SerializeField] DialogsData fightingData;


    Utilidades.Timer timerRandomDialogs;
    Utilidades.Timer talkingDurating;




    void Start()
    {
        timerRandomDialogs = new Utilidades.Timer(dialogueWindow.currentDialogs.frecuencyTime);
        dialogueWindow.LoadDialogsList(fightingData);
    }

    void Update()
    {
        Debug.Log(Level2DManager.instance.state);

        if (Level2DManager.instance.state.Equals(Level2DManager.State.Game))
        {
            timerRandomDialogs.Play();
            timerRandomDialogs.ExecuteOnFinish(() =>
            {
                dialogueWindow.TalkRandomly();
            });

        }
    }

    IEnumerator PlayerTalk()
    {
        yield return new WaitForSeconds(dialogueWindow.currentDialogs.frecuencyTime);
        dialogueWindow.TalkRandomly();
    }


}
