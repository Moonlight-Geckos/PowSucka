using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemController : MonoBehaviour
{
    protected float _damage = 0;
    public void Initialize(float d) => _damage = d;
    private void Awake()
    {
        var system = GetComponent<ParticleSystem>().main;
        system.stopAction = ParticleSystemStopAction.Callback;
    }
    void OnParticleSystemStopped()
    {
        _damage = 0;
        GetComponent<IDisposable>()?.Dispose();
    }
}