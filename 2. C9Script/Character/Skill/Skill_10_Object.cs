using UnityEngine;

public class Skill_10_Object : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    public void Boom()
    {
        Animator.SetTrigger("Boom");
    }

    public void OnAnimationEvent_End()
    {
        gameObject.SetActive(false);
    }
}