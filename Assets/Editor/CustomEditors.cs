using UnityEngine;
using UnityEditor;

public class CustomEditors : MonoBehaviour
{
    [CustomEditor(typeof(GameManager)), InitializeOnLoad]
    public class GameManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Clear Player Storage"))
            {
                PlayerPrefs.DeleteAll();
                EventsPool.UpdateUIEvent.Invoke();
                Debug.Log("Cleared all values!");
            }
            if (GUILayout.Button("GIVE ME SOME DAMN MONEEEYYYY"))
            {
                PlayerStorage.CoinsCollected = PlayerStorage.CoinsCollected + 100;
                EventsPool.UpdateUIEvent.Invoke();
                Debug.Log("Greedy..");
            }
        }
    }
}
