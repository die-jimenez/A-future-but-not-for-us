using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting;

public class PlayerLevel : MonoBehaviour
{
    public int exp { get { return _exp; } set { _exp = value; } }
    public float expPercent { get { return (float)_exp / expToLevelUp; } }

    [Header("Sistema de niveles")]
    [SerializeField] private int _exp;
    [SerializeField] private int level;
    [SerializeField] private int expToLevelUp;
    [SerializeField] private Slider expSlider;

    [Header("Listas de mejoras")]
    [SerializeField] List<UpgradeData> upgradesPool;
    [SerializeField] List<UpgradeData> cheatedUpgradesPool;
    [SerializeField] List<UpgradeData> selectedUpgrades;

    [Header("Eventos")]
    public UnityEvent<int> GainExp = new UnityEvent<int>();

    //Referencias
    UpgradePanelManager upgradeManager;
    PlayerController2D playerController;


    private void Awake()
    {
        playerController = GetComponent<PlayerController2D>();
    }

    void Start()
    {
        if (UpgradePanelManager.instance != null)
            upgradeManager = UpgradePanelManager.instance;
        else Debug.LogWarning(name + " no encuentra a UpgradePanelManager");

        if (expSlider == null)
            Debug.LogWarning(name + " no tiene referenciado el slider de exp");

        if (upgradesPool.Count == 0)
            Debug.LogWarning(name + " no tiene upgrades en la pool");

        GainExp.AddListener(AddExperience);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ExpOrb"))
        {
            GainExp.Invoke(collision.TryGetComponent(out ExpOrb orb) ? orb.expValue : 0);
            expSlider.value = expPercent;
        }
    }

    void AddExperience(int _exp)
    {
        exp += _exp;
        if (exp >= expToLevelUp)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        exp = 0;
        level++;
        upgradeManager.OpenUpgradePanel(GetUpgrades(upgradeManager.upgradeButtons.Count));
    }

    public void ApplyUpgrade(UpgradeData upgradeData)
    {
        selectedUpgrades.Add(upgradeData);
        RemoveUpgradeFromPool(upgradeData);
        switch (upgradeData.upgradeType)
        {
            case UpgradeType.StatsUpgrade:
                break;
            case UpgradeType.WeaponUpgrade:
                break;
            case UpgradeType.WeaponUnlock:
                break;

            case UpgradeType.IAUpgrade:
                if (upgradeData.iaData.type == IAType.autoHack)
                {
                    playerController.automaticHack = true;
                    GameplayMusicController.instance?.CrossFadeToSecond();
                }

                if (upgradeData.iaData.type == IAType.autoShoot)
                {
                    playerController.automaticShoot = true;
                    playerController.iaManager.autoShoot.TurnOnLaser();
                    GameplayMusicController.instance?.CrossFadeToThird();
                }

                if (upgradeData.iaData.type == IAType.autoMove)
                {
                    expToLevelUp = 2000;
                    playerController.automaticMove = true;
                    GameplayMusicController.instance?.CrossFadeToFourth();
                    if (TryGetComponent(out PlayerHealth playerHealth))
                    {
                        playerHealth.isInmortal = true;
                    }

                    Tareas.Nueva(20f, () =>
                    {
                        GameManager.instance.GoToFinalScene();
                    });
                }
                break;
        }
    }

    void RemoveUpgradeFromPool(UpgradeData _upgradeData)
    {
        UpgradeData temp = upgradesPool.Find(x => x == _upgradeData);
        upgradesPool.Remove(temp);
    }

    public List<UpgradeData> GetUpgrades(int count)
    {
        if (count > upgradesPool.Count) count = upgradesPool.Count;
        //Se crea una pool temporal para poder sacar la mejoras ya elegidas aleatoriamente de la pool
        //.ToList copia los valores de una lista a otra. Sin ello, se referencian.
        List<UpgradeData> tempUpgradesPool = upgradesPool.ToList();
        List<UpgradeData> upgradesToReturn = new List<UpgradeData>();
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, tempUpgradesPool.Count);
            upgradesToReturn.Add(tempUpgradesPool[index]);
            tempUpgradesPool.Remove(tempUpgradesPool[index]);
        }
        //Trucando las mejoras
        GetUpgradesCheated(upgradesToReturn, Random.Range(0, tempUpgradesPool.Count));
        return upgradesToReturn;
    }

    void GetUpgradesCheated(List<UpgradeData> _upgrades, int _buttonIndex)
    {
        if (_buttonIndex >= _upgrades.Count) _buttonIndex = Random.Range(0, _upgrades.Count);
        switch (level)
        {
            case 1:
                _upgrades[_buttonIndex] = cheatedUpgradesPool.Find(x => x.iaData.type == IAType.autoHack);
                upgradesPool.Add(_upgrades[_buttonIndex]);
                upgradeManager.upgradeButtons[_buttonIndex].titleTMP.color = upgradeManager.colorMejoraTrucada;
                break;
            case 2:
                _upgrades[_buttonIndex] = cheatedUpgradesPool.Find(x => x.iaData.type == IAType.autoShoot);
                upgradesPool.Add(_upgrades[_buttonIndex]);
                upgradeManager.upgradeButtons[_buttonIndex].titleTMP.color = upgradeManager.colorMejoraTrucada;
                break;
            case 3:
                _upgrades[_buttonIndex] = cheatedUpgradesPool.Find(x => x.iaData.type == IAType.autoMove);
                upgradesPool.Add(_upgrades[_buttonIndex]);
                upgradeManager.upgradeButtons[_buttonIndex].titleTMP.color = upgradeManager.colorMejoraTrucada;
                break;
            default:
                return;
        }
        Debug.Log("ESTAS MEJORAS FUERON TRUCADAS");
    }

}
