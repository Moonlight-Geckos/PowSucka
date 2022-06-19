using System.Collections;
using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    private void Start()
    {
        int selectedSkin = PlayerStorage.SkinSelected;
        foreach(SkinItem s in DataHolder.Instance.AllSkins)
        {
            if(s.skinNumber == selectedSkin)
            {
                ChangeSkin(s);
                break;
            }
        }
        EventsPool.UpdateSkinEvent.AddListener(ChangeSkin);
    }
    private void ChangeSkin(SkinItem item)
    {
        IEnumerator change()
        {
            yield return null;
            try
            {
                Destroy(transform.GetChild(0).gameObject);
            }
            catch { }
            yield return new WaitForEndOfFrame();
            GameObject skin = Instantiate(item.skinObject);
            skin.transform.parent = transform;
            skin.transform.localPosition = Vector3.zero;
        }
        StartCoroutine(change());
    }
}