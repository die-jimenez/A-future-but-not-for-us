using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement3D : MonoBehaviour
{
    [Header("General")]
    [SerializeField] CharacterController controller;
    [SerializeField] Camera POVCamera;
    [SerializeField] float moveDistance;
    [SerializeField] bool canMove;
    [SerializeField] bool keyHold;
    [SerializeField] bool isZooming;
    [SerializeField] Collider objectInFront;


    [Header("Animacion Inicial")]
    [SerializeField] Vector3 zoomInPos;
    [SerializeField] Vector3 zoomInRot;
    [SerializeField] Color filtroColorPantalla;
    [SerializeField] RawImage pantallaMonitor;
    [SerializeField] RawImage flash;
    [SerializeField] bool playAnimacionInicial;

    [HideInInspector] public bool isOnAnimation { get; private set; }
    RaycastHit[] frontRay = new RaycastHit[1];
    Vector3 defaultCameraPos;
    Vector3 defaultCameraRot;
    bool anyObjectInFront;
    PlayerInput playerInput;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        canMove = true;
        defaultCameraPos = POVCamera.transform.localPosition;
        defaultCameraRot = POVCamera.transform.localRotation.eulerAngles;

        if (playAnimacionInicial)
        {
            StartCoroutine(AnimacionInicial());
        }
    }

    void Update()
    {
        StartCoroutine(CheckFrontCollision());

        if (canMove)
        {
            Move();
        }
    }


    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal_2");
        float vertical = Input.GetAxisRaw("Vertical_2");
        Debug.DrawRay(transform.position - Vector3.up / 2, transform.forward, Color.yellow);

        if (!keyHold && !isOnAnimation)
        {
            //Movimiento
            if (vertical >= 0.1f)
            {
                if (!anyObjectInFront)
                {
                    keyHold = true;
                    isOnAnimation = true;
                    Vector3 moveDirection = transform.forward * vertical;
                    transform.DOMove(transform.position + (moveDirection * moveDistance), 0.4f).OnComplete(() =>
                    {
                        isOnAnimation = false;
                    });
                }
                //if (!anyObjectInFront) controller.Move(moveDirection * moveDistance);
            }
            //Rotacion
            if (horizontal != 0)
            {
                keyHold = true;
                isOnAnimation = true;
                transform.DORotate(new Vector3(0, transform.eulerAngles.y + (90 * horizontal), 0), 0.5f).OnComplete(() =>
                {
                    isOnAnimation = false;
                });
            }
        }
        else
        {
            if (horizontal == 0 && vertical == 0)
            {
                keyHold = false;
            }
        }
    }


    IEnumerator CheckFrontCollision()
    {
        yield return new WaitForSeconds(0.3f);
        LayerMask defaultlayer = 1 << LayerMask.NameToLayer("Default");
        int hits = Physics.RaycastNonAlloc(transform.position - Vector3.up / 2, transform.forward, frontRay, moveDistance * 1.5f, defaultlayer);

        if (hits != 0)
        {
            anyObjectInFront = true;
            objectInFront = frontRay[0].collider;
        }
        else
        {
            anyObjectInFront = false;
            objectInFront = null;
        }

    }



    void ZoomInOrOut(InteractiveObject interactiveScript)
    {
        isOnAnimation = true;
        if (isZooming)
        {
            Zoom(defaultCameraPos, defaultCameraRot, interactiveScript.moveSpeed, () =>
            {
                canMove = true;
                isZooming = false;
                isOnAnimation = false;
            });
        }
        else
        {
            Zoom(interactiveScript.newCameraPos, interactiveScript.newCameraRot, interactiveScript.moveSpeed, () =>
            {
                isZooming = !isZooming;
                isOnAnimation = false;
            });
            canMove = false;
        }
    }

    void Zoom(Vector3 endPos, Vector3 endRot, float _speed, Action onComplete)
    {
        POVCamera.transform.DOLocalMove(endPos, _speed);
        POVCamera.transform.DOLocalRotate(endRot, _speed).OnComplete(() =>
            {
                onComplete();
            });
    }

    IEnumerator AnimacionInicial()
    {
        canMove = false;
        isZooming = true;
        isOnAnimation = true;
        POVCamera.transform.localPosition = zoomInPos;
        POVCamera.transform.localRotation = Quaternion.Euler(zoomInRot);


        //Flash blanco
        Color currentColor = flash.color;
        flash.DOColor(new Color(currentColor.r, currentColor.g, currentColor.b, 1), 0.4f);
        yield return new WaitForSeconds(0.6f);
        flash.DOColor(new Color(currentColor.r, currentColor.g, currentColor.b, 0), 2f).OnComplete(() =>
        {
            flash.gameObject.SetActive(false);
        });
        pantallaMonitor.DOColor(filtroColorPantalla, 1f);
        yield return new WaitForSeconds(2f);

        //Filtro de Pantalla
        //yield return new WaitForSeconds(0.75f);
        //pantallaMonitor.DOColor(filtroColorPantalla, 5f);

        //Camara estatica
        yield return new WaitForSeconds(3);

        //Zoom out
        Zoom(defaultCameraPos, defaultCameraRot, 2.5f, () => { });
        yield return new WaitForSeconds(3);
        canMove = true;
        isZooming = false;
        isOnAnimation = false;

    }

}
