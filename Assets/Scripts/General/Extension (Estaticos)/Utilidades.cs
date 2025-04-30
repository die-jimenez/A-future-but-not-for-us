using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Utilidades
{

    public class Cronometro
    {
        public float time;

        public void Play()
        {
            time += Time.deltaTime;
        }
        public void Reset()
        {
            time = 0;
        }
    }

    [Serializable]
    public class Timer
    {
        public float startTime;
        public float time;

        public Timer(float startTime)
        {
            this.startTime = startTime; 
            this.time = startTime;
        }

        public void SetStartTime(float _time)
        {
            startTime = _time;
        }
        public void Play()
        {
            time -= Time.deltaTime;
        }
        public void Reset()
        {
            time = startTime;
        }
        public bool finished()
        {
            if (time <= 0) return true;
            else return false;
        }
        public void ExecuteOnFinish(Action action)
        {
            if (finished())
            {
                if (action == null)
                {
                    //"new System.Diagnostics... te indica quien ejecuta la funcion
                    Debug.LogWarning(new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType +
                    ": inicio un timer con una Action al terminar pero la Action es null");
                }
                else
                {
                    action();
                }
                Reset();
            }
        }
    }
}
