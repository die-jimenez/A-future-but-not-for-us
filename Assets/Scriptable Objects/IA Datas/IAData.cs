using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IAType
{
    autoHack,
    autoShoot,
    autoMove
};


[CreateAssetMenu(fileName = "UpgradeData", menuName = "ScriptableObjects/IAData", order = 2)]
public class IAData : ScriptableObject
{
    public IAType type;
}
