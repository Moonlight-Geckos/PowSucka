using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemController : MonoBehaviour
{
    protected bool _canBehave = true;
    public void Initialize(bool c) => _canBehave = c;
    private void Awake()
    {
        var system = GetComponent<ParticleSystem>().main;
        system.stopAction = ParticleSystemStopAction.Callback;
    }
    void OnParticleSystemStopped()
    {
        GetComponent<IDisposable>()?.Dispose();
    }
}