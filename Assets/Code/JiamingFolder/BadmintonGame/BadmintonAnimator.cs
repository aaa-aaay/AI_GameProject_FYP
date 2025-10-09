using System;
using UnityEngine;

public class BadmintonAnimator : MonoBehaviour
{
    [SerializeField] Animator _animator;

    public void PlayWalkAnimation(bool play)
    {
        _animator.SetBool("walking", play);
    }




}
