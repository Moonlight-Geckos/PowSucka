using UnityEngine;

public class GooRandomizer : MonoBehaviour
{
    private float _rand;
    private Material _material;
    private void OnEnable()
    {
        Randomize();
    }
    private void Randomize()
    {
        if (_material == null)
            _material = GetComponent<Renderer>().material;
        _rand = Random.Range(-3f, 3f);
        _material.SetFloat("_RandomNum", _rand);
    }
}
