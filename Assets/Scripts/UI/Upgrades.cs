using System;
using TMPro;
using UnityEngine;



public class Upgrades : MonoBehaviour
{
    [Serializable]
    class UIItem
    {
        public TextMeshProUGUI price;
        public TextMeshProUGUI level;
    }
    [SerializeField]
    private UIItem healthItem;

    [SerializeField]
    private UIItem damageItem;

    private void Awake()
    {
        EventsPool.UpdateUIEvent.AddListener(UpdateUI);
    }
    private void UpdateUI()
    {
        healthItem.level.text = "Lvl. " + PlayerStorage.HealthUpgradeLevel.ToString();
        healthItem.price.text = (15 + 15 * PlayerStorage.HealthUpgradeLevel).ToString();

        damageItem.level.text = "Lvl. " + PlayerStorage.DamageUpgradeLevel.ToString();
        damageItem.price.text = (15 + 15 * PlayerStorage.DamageUpgradeLevel).ToString();
    }
    public void UpgradeHealth()
    {
        int pr = 15 + (15 * PlayerStorage.HealthUpgradeLevel);
        if(pr <= PlayerStorage.CoinsCollected)
        {
            PlayerStorage.HealthUpgradeLevel += 1;
            PlayerStorage.CoinsCollected -= pr;
            EventsPool.UpdateUIEvent.Invoke();

        }
    }
    public void UpgradeDamage()
    {
        int pr = 15 + (15 * PlayerStorage.DamageUpgradeLevel);
        if (pr <= PlayerStorage.CoinsCollected)
        {
            PlayerStorage.DamageUpgradeLevel += 1;
            PlayerStorage.CoinsCollected -= pr;
            EventsPool.UpdateUIEvent.Invoke();

        }
    }
}
