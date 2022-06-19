using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinController : MonoBehaviour
{

    [SerializeField]
    private List<Material> materialsToChange;

    [SerializeField]
    private Color weaponModeColor;

    public Transform _liquidFill;
    private Animator _storageAnimator;
    private Animator _animator;
    private List<int> _chosenMaterials;
    private List<Material> _allMaterials;
    private List<Color> _originalColors;
    private GameObject _suctionParticles;

    void Start()
    {
        Initialize();
        EventsPool.ChangePhaseEvent.AddListener(AnimateWeaponMode);
        EventsPool.PlayerChangedMovementEvent.AddListener(Run);
        EventsPool.PickedupObjectEvent.AddListener(PickupProjectile);
    }
    private void OnEnable()
    {
        Initialize();
    }
    private void Initialize()
    {
        _chosenMaterials = new List<int>();
        _allMaterials = new List<Material>();
        _originalColors = new List<Color>();
        _animator = GetComponent<Animator>();
        _liquidFill = GameObject.FindGameObjectWithTag("Fill").transform;
        _storageAnimator = _liquidFill.parent.GetComponent<Animator>();
        _suctionParticles = GetComponentInChildren<ParticleSystem>().gameObject;
        var renderer = transform.Find("Body").GetComponent<Renderer>();
        _allMaterials.AddRange(renderer.materials);
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            if (materialsToChange.Exists(m => renderer.materials[i].name.Contains(m.name)))
                _chosenMaterials.Add(i);
            _originalColors.Add(renderer.materials[i].color);
        }

        _liquidFill.localScale = new Vector3(1, 0, 1);
    }
    public void AnimateHit()
    {
        if (Observer.weaponMode)
            return;
        IEnumerator getHit()
        {
            yield return null;
            float duration = 1f;
            float elapsed = 0;
            for (int i = 0; i < _allMaterials.Count; i++)
            {
                _allMaterials[i].color = Color.red;
            }
            while (elapsed < duration)
            {
                for (int i = 0; i < _allMaterials.Count; i++)
                {
                    _allMaterials[i].color = Color.Lerp(Color.red, _originalColors[i], elapsed / duration);
                }
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            for (int i = 0; i < _allMaterials.Count; i++)
            {
                _allMaterials[i].color = _originalColors[i];
            }
        }
        StartCoroutine(getHit());
    }
    private void Run(bool isRunning)
    {
        try
        {
            _animator.SetBool("Running", isRunning);
        }
        catch { }
    }
    private void AnimateWeaponMode(bool weaponMode)
    {
        IEnumerator weaponmode()
        {
            yield return null;
            float duration = 0.5f;
            float elapsed = 0;
            _storageAnimator.SetBool("WeaponMode", true);
            _suctionParticles.SetActive(false);
            for (int i = 0; i < _allMaterials.Count; i++)
            {
                _allMaterials[i].color = _originalColors[i];
            }
            while (elapsed < duration)
            {
                for (int i = 0; i < _chosenMaterials.Count; i++)
                {
                    _allMaterials[_chosenMaterials[i]].color = Color.Lerp(_originalColors[_chosenMaterials[i]], weaponModeColor, elapsed / duration);
                }
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            for (int i = 0; i < _chosenMaterials.Count; i++)
            {
                _allMaterials[_chosenMaterials[i]].color = weaponModeColor;
            }
        }
        IEnumerator normal()
        {
            yield return null;
            float duration = 1f;
            float elapsed = 0;
            _liquidFill.localScale = new Vector3(1, 0, 1);
            _storageAnimator.SetBool("WeaponMode", false);
            _suctionParticles.SetActive(true);
            while (elapsed < duration)
            {
                for (int i = 0; i < _chosenMaterials.Count; i++)
                {
                    _allMaterials[_chosenMaterials[i]].color = Color.Lerp(weaponModeColor, _originalColors[_chosenMaterials[i]], elapsed / duration);
                }
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            for (int i = 0; i < _chosenMaterials.Count; i++)
            {
                _allMaterials[_chosenMaterials[i]].color =  _originalColors[_chosenMaterials[i]];
            }
        }
        if (weaponMode)
            StartCoroutine(weaponmode());
        else
            StartCoroutine(normal());
    }
    private void PickupProjectile(FillType prj)
    {
        if (prj == FillType.Diamond || Observer.weaponMode)
            return;
        _liquidFill.localScale = new Vector3(1, _liquidFill.localScale.y + (StaticValues.GetFillPercent(prj) / 100f), 1);
    }
}
