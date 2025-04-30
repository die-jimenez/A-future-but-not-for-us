using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    public static UpgradePanelManager instance;
    public GameObject upgradePanel;
    public Color colorMejoraTrucada;
    [SerializeField] int mejorasTomadas;


    public List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (upgradePanel == null) Debug.LogWarning(transform.name + " no tiene referneciado el panelUpgrades");
    }

    //Se ejecuta en los botones de upgrade
    public void Upgrade(int buttonID)
    {
        if (upgradeButtons[buttonID].upgradeData.upgradeType == UpgradeType.NonUpgrade)
        {
            upgradeButtons[buttonID].Block();
            return;
        }
        UpgradeData selectedUpgrade = upgradeButtons[buttonID].upgradeData;
        Level2DManager.instance.player2D.GetComponent<PlayerLevel>().ApplyUpgrade(selectedUpgrade);
        CloseUpgradePanel();
    }

    public void OpenUpgradePanel(List<UpgradeData> upgradeDatas)
    {
        upgradePanel.SetActive(true);
        for (int i = 0; i < upgradeDatas.Count; i++)
        {
            upgradeButtons[i].SearchAndSetData(upgradeDatas[i]);
        }
        if (GameManager.instance != null) Level2DManager.instance.PauseGame();
        ChangeInputPlayer("NotHackeo");
        StartCoroutine(ChangeInputPlayer("Upgrades", 1f));
    }

    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);
        foreach (UpgradeButton upgradeButton in upgradeButtons)
        {
            upgradeButton.Clean();
            upgradeButton.titleTMP.color = Color.white;
        }

        if (Level2DManager.instance.player2D.automaticHack)
        {
            ChangeInputPlayer("NotHackeo");
        }
        else ChangeInputPlayer("Hackeo");

        mejorasTomadas++;
        if (mejorasTomadas == 1) Level2DManager.instance?.dialogoRobotIA.Talk("GM_ROBOT_AFTER_AUTOHACK");
        else if (mejorasTomadas == 2) Level2DManager.instance?.dialogoRobotIA.Talk("GM_ROBOT_AFTER_AUTOSHOOT");
        else if (mejorasTomadas == 3) Level2DManager.instance?.dialogoRobotIA.Talk("GM_ROBOT_AFTER_AUTOMOVE");


        if (GameManager.instance != null) Level2DManager.instance.UnPauseGame();
    }

    void ChangeInputPlayer(string nameOfMap)
    {
        SequenceManager.instance.GetPlayerInput().SwitchCurrentActionMap(nameOfMap);
    }

    IEnumerator ChangeInputPlayer(string nameOfMap, float delay)
    {
        yield return new WaitForSecondsRealtime (delay);
        SequenceManager.instance.GetPlayerInput().SwitchCurrentActionMap(nameOfMap);
    }
}
