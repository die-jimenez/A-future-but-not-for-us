using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ChangeNonVariableButtonTexture : MonoBehaviour
{
    public enum Button { one, two, three};
    public Button button; 
    RawImage rawImage;


    private void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void OnEnable()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }

        if (SequenceManager.instance != null)
        {
            rawImage.texture = SequenceManager.instance.codesData[((int)button)].currentTexture;
        }
    }
}
