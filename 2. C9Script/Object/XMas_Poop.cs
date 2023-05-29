using UniRx;
using UnityEngine;

public class XMas_Poop : MonoBehaviour
{
    [SerializeField] private Sprite BaseSprite;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private float _gravity;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _gravity = _rigidbody2D.gravityScale;

        Managers.XMasEvent.IsPlaying.Subscribe(isPlaying =>
        {
            _rigidbody2D.bodyType = isPlaying ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
        }).AddTo(gameObject);
    }

    private void OnEnable()
    {
        _animator.enabled = false;
        _rigidbody2D.gravityScale = _gravity;
        _spriteRenderer.sprite = BaseSprite;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.tag)
        {
            case "XMasPlatform":
                Managers.Sound.PlaySfxSound(SfxType.BoomSnow);
                Managers.XMasEvent.Score.Value += 1;
                _animator.enabled = true;
                _rigidbody2D.gravityScale = 0;
                _rigidbody2D.velocity = Vector2.zero;
                break;
            case "XMasPlayer":
                Managers.XMasEvent.End();
                break;
        }
    }

    public void OnAnimationEvent_Return()
    {
        Managers.XMasEvent.ReturnXMasPoop(this);
    }
}