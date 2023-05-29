using UnityEngine;


public class Effect : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    [SerializeField] private bool IsReturn = true;

    public string EffectName;

    private void Update()
    {
        if (Animator == null)
        {
            Destroy(gameObject);
            return;
        }

        if (Animator.IsEndCurrentAnimation())
        {
            if (IsReturn)
                Managers.Effect.ReturnEffect(EffectName, this);
            else
                gameObject.SetActive(false);
        }
    }
}