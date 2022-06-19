using TMPro;
using UnityEngine;

public class EnemiesLeft : MonoBehaviour
{
    TextMeshProUGUI num;
    private void Awake()
    {
        num = GetComponent<TextMeshProUGUI>();
        num.text = GameManager.Instance.LeftEnemiesToKill.ToString();
        EventsPool.UpdateUIEvent.AddListener(
            () => num.text = GameManager.Instance.LeftEnemiesToKill.ToString());
    }
}
