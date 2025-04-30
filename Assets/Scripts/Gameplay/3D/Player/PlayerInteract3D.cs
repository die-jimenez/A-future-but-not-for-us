using Game.Languages;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteract3D : MonoBehaviour
{

    [Header("Interaccion")]
    [SerializeField] TextMeshProUGUI interactionText;
    [SerializeField] Canvas interactionCanvas;
    [SerializeField] HackSequence interactionSequence;


    [SerializeField]  GameObject interactiveObject;
    [SerializeField]  InteractiveObject interactiveScript;
    [SerializeField] bool anyIntercativeInFront;


    PlayerMovement3D playerMovement;
    RaycastHit[] frontRay = new RaycastHit[1];




    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement3D>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(CheckFrontCollision());

        if (canShowCanvas())
        {
            ShowInteractiveCanvas();
        }
        else HideInteractiveCanvas();

        //if (canInteract())
        //{
        //    interactionCanvas.gameObject.SetActive(true);
        //}
        //else
        //{
        //    interactionCanvas.gameObject.SetActive(false);
        //}
    }




    IEnumerator CheckFrontCollision()
    {
        yield return new WaitForSeconds(0.3f);
        LayerMask interableLayer = 1 << LayerMask.NameToLayer("Interactable");
        int hits = Physics.RaycastNonAlloc(transform.position - Vector3.up / 2, transform.forward, frontRay, 1f, interableLayer);

        if (hits != 0)
        {
            anyIntercativeInFront = true;
            interactiveObject = frontRay[0].transform.gameObject;
        }
        else
        {
            anyIntercativeInFront = false;
            interactiveObject = null;
            interactiveScript = null;
        }

    }


    bool canInteract()
    {
        if (!anyIntercativeInFront) return false;
        if (playerMovement.isOnAnimation) return false;

        if (interactiveObject.TryGetComponent(out InteractiveObject script))
        {
            interactiveScript = script;
            return true;
        }
        else return false;
    }

    bool canShowCanvas()
    {
        if (!anyIntercativeInFront) return false;
        else return true;
    }

    void ShowInteractiveCanvas()
    {
        interactionCanvas.gameObject.SetActive(true);
        interactionText.gameObject.SetActive(true);
        interactionSequence.gameObject.SetActive(true);
        interactionSequence.SetActiveAllCodes();

        GetInteractiveScript();
        interactionText.text = Get_dialog_text_using_ID(interactiveScript.textoID);

    }

    void HideInteractiveCanvas()
    {
        interactionCanvas.gameObject.SetActive(false);
        //interactionCanvasText.text = Get_dialog_text_using_ID(interactiveScript.textoID);
    }

    //Esta funcion re-adapta el sistema de dialogos para que funcione con textos importados de excell (que es más nuevo), en vez textos en el propio codigo
    string Get_dialog_text_using_ID(string _id)
    {
        if (interactionText == null)
        {
            Debug.LogWarning(transform.name + " no tiene asignado su TextMeshPro en el inspector");
            return "text error #7";
        }

        if (interactionText.TryGetComponent(out TextSearcher searcher))
        {
            return searcher.GetUpdatedText(_id); ;
        }
        else
        {
            Debug.LogWarning("El Text no tiene TextSearcher.cs");
            return "text error #2";
        }
    }




    public void Interact()
    {
        if (!canInteract()) return;
        GetInteractiveScript();

        switch (interactiveScript.interactionType)
        {
            case InteractiveObject.InteractionType.SendMessage:
                StartCoroutine(Level3DEvents.instance.ResponderMensajesCor());
                break;

            case InteractiveObject.InteractionType.Sleep:
                Level3DEvents.instance.Mimir();
                break;
        }
    }

    void GetInteractiveScript()
    {
        if (interactiveScript != null) return;
        if (interactiveObject.TryGetComponent(out InteractiveObject script))
        {
            interactiveScript = script;
        }
    }


}
