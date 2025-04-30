using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(IAManager))]
public class IAHacker : MonoBehaviour
{
    [SerializeField] IAManager controller;
    [SerializeField] HackSequence hackSequence;
    [Range(0.05f, 1f)] public float timeToStartHacking;
    [Range(0.05f, 1f)] public float timePerHackCode;

    private Utilidades.Timer timerPerHackCode;
    private Utilidades.Timer timerToStartHacking;

    private bool isHacking;


    // Start is called before the first frame update
    void Start()
    {
        controller = controller == null ? GetComponent<IAManager>() : controller;

        if (hackSequence == null)
        {
            if (controller.weapon.TryGetComponent(out HackerWeapon _weapon))
            {
                hackSequence = _weapon.hackSequence;
            }
            else Debug.LogWarning("IAHacker necesita la referencia de un hackSequence");
        }
        if (controller.weapon != null)
        {
            controller.weapon.Shoot.AddListener(ResetHacking);
        }
        timerPerHackCode = new Utilidades.Timer(timePerHackCode);
        timerToStartHacking = new Utilidades.Timer(timeToStartHacking);

    }

    public void AutoHack()
    {
        if (hackSequence.isHackCompleted) return;

        if (!isHacking)
        {
            timerToStartHacking.Play();
            timerToStartHacking.ExecuteOnFinish(()=> isHacking = true);
            return;
        }
        else
        {
            timerPerHackCode.Play();
            timerPerHackCode.ExecuteOnFinish(() => hackSequence.AutoPress());
        }
    }

    void ResetHacking()
    {
        isHacking = false;
        timerPerHackCode.Reset();
        timerToStartHacking.Reset();
        hackSequence.ResetHackSequence();
    }
}
