using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    #region Fields
    [HideInInspector] public Animator _animator;
    private Dictionary<string, float> _dicAnimClipLength = new Dictionary<string, float>();
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        RuntimeAnimatorController ac = _animator.runtimeAnimatorController;

        //for (int i = 0; i < ac.animationClips.Length; i++)
        //{
        //    _dicAnimClipLength.Add(ac.animationClips[i].name, ac.animationClips[i].length);
        //}
    }
    #endregion

    #region Private Methods
    #endregion

    #region Public Methods
    //public float GetAnimationClipLength(string animationName)
    //{
    //    return _dicAnimClipLength[animationName];
    //}

    public void Play(string animState)
    {
        _animator.SetTrigger(animState);
    }
    #endregion
}
