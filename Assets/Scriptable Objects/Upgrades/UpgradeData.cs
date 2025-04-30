using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UpgradeType
{
    WeaponUpgrade,
    WeaponUnlock,
    StatsUpgrade,
    IAUpgrade,
    NonUpgrade,
}


[CreateAssetMenu(fileName = "UpgradeData", menuName = "ScriptableObjects/UpgradeData", order = 1)]
public class UpgradeData : ScriptableObject
{
    [Header("Upgrade Data")]
    [SerializeField] private UpgradeType _upgradeType;
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private IAData _iaData;
    [Header("UI Data")]
    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private string _extraInfo;
    [SerializeField] private Texture2D _icon;

    public UpgradeType upgradeType
    {
        get { return _upgradeType; }
        private set { _upgradeType = value; }
    }

    public WeaponData weaponData
    {
        get { return _weaponData; }
        private set { _weaponData = value; }
    }

    public IAData iaData
    {
        get { return _iaData; }
        private set { _iaData = value; }
    }

    public string title
    {
        get { return _title; }
        private set { _title = value; }
    }

    public string description
    {
        get { return _description; }
        private set { _description = value; }
    }

    public string extraInfo
    {
        get { return _extraInfo; }
        private set { _extraInfo = value; }
    }

    public Texture2D icon
    {
        get { return _icon; }
        private set { _icon = value; }
    }
}
