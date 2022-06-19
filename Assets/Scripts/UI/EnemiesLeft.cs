using TMPro;
using UnityEngine;

public class EnemiesLeft : MonoBehaviour
{
    TextMeshProUGUI num;
    Observer _instance;
    private void Awake()
    {
        _instance = Observer.Instance;
        num = GetComponent<TextMeshProUGUI>();
        num.text = Observer.Instance.LeftEnemiesToKill.ToString();
        EventsPool.UpdateUIEvent.AddListener(
            () => num.text = _instance.LeftEnemiesToKill.ToString());
    }
}
