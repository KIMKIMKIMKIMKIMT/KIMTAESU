using UnityEngine;

public class GlobalEffect : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        transform.position = Vector3.zero;
    }

    private void Update()
    {
        if (_animator == null)
            return;

        if (_animator.IsEndCurrentAnimation())
            Destroy(gameObject);
    }
}