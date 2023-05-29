using UnityEngine;
using UnityEngine.UI;


public class UpgradeEffect : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Play()
    {
        if (gameObject.activeSelf)
            return;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_animator.IsEndCurrentAnimation())
            gameObject.SetActive(false);
    }
}