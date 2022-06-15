using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterController : MonoBehaviour, IDamagable
{
    [SerializeField]
    private float maxHealth = 100;

    private HealthBar _healthBar;
    private List<Material> _materials;
    private List<Color> _originalColors;
    private float _currentHP;
    private void Awake()
    {
        _materials = new List<Material>();
        _originalColors = new List<Color>();
        _animator = transform.Find("Character").GetComponent<Animator>();
        _healthBar = transform.parent.GetComponentInChildren<HealthBar>();

        foreach (Renderer rend in _animator.GetComponentsInChildren<Renderer>())
        {
            _materials.AddRange(rend.materials);
            foreach (Material material in rend.materials)
            {
                _originalColors.Add(material.color);
            }
        }

        _currentHP = maxHealth;
    }

    private Animator _animator;
    public void Run(bool isRunning)
    {
        _animator.SetBool("Running", isRunning);
    }
    public void GetDamage(float damage)
    {
        _currentHP -= damage;
        _healthBar.UpdateValue(_currentHP/100f);
        StopAllCoroutines();
        AnimateHit();
    }
    private void AnimateHit()
    {
        IEnumerator getHit()
        {
            yield return null;
            float duration = 1f;
            float elapsed = 0;
            for(int i = 0;i < _materials.Count; i++)
            {
                _materials[i].color =  Color.red;
            }
            while (elapsed < duration)
            {
                for (int i = 0; i < _materials.Count; i++)
                {
                    _materials[i].color = Color.Lerp(Color.red, _originalColors[i], elapsed / duration);
                }
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        StartCoroutine(getHit());
    }
}