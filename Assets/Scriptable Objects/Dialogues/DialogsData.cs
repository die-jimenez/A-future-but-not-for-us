using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "DialogsData", menuName = "ScriptableObjects/DialogsData", order = 3)]
public class DialogsData : ScriptableObject
{
    public List<Dialog> dialogs;
    [Range(10f, 60f)]public float frecuencyTime;
    public bool loop;



    [System.Serializable]
    public class Dialog
    {
        [SerializeField] private string _id;
        [SerializeField] private string _text;
        [SerializeField] private float _duration;
        [SerializeField] private AudioClip _audioEs;
        [SerializeField] private AudioClip _audioEn;
        [SerializeField] private bool _multipleTexts;


        public string id
        {
            get { return _id; }
            private set { _id = value; }
        }

        public string text
        {
            get { return _text; }
            private set { _text = value; }
        }

        public float duration
        {
            get { return _duration; }
            private set { _duration = value; }
        }

        //EL AUDIO TIENE TODO PUBLICO POR AHORA
        public AudioClip audio
        {
            get { return _audioEn; }
            set { _audioEn = value; }
        }
        //Esta variable impide que rebusque los dailogos por el id
        public bool multipleTexts
        {
            get { return _multipleTexts; }
            private set { _multipleTexts = value; }
        }




        public Dialog(string __id)
        {
            id = __id;
        }
        public Dialog(string __id, float __duration)
        {
            id = __id;
            duration = __duration;   
        }
        public Dialog(string __id, string __text, float __duration)
        {
            id = __id;
            text = __text;
            duration = __duration;
        }
        public Dialog(string __id, string __text, float __duration, bool __multipleTexts)
        {
            id = __id;
            text = __text;
            duration = __duration;
            multipleTexts = __multipleTexts;
        }

        public Dialog(string __text, float __duration, AudioClip __audio)
        {
            text = __text;
            duration = __duration;
            audio = __audio;
        }

        /*
        [Tooltip("Deprecated")]
        public Dialog(float __duration, string __text)
        {
            text = __text;
            duration = __duration;
        }
        */
    }

}
