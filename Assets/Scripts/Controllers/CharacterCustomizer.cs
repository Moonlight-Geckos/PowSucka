using UnityEngine;
public class CharacterCustomizer : MonoBehaviour
{
    private Animator _animator;
    public void Run(bool isRunning)
    {
        if(_animator == null)
            _animator = GetComponent<Animator>();
        _animator.SetBool("Running", isRunning);
    }
}