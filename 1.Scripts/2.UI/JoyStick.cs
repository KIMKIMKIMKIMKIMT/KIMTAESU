using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : SingletonMonoBehaviour<JoyStick>, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    #region Fields
    [SerializeField] private RectTransform _joyStickBG;
    [SerializeField] private RectTransform _handle;

    private Vector3 _moveDir = Vector3.zero;

    [SerializeField, Range(0f, 1f)] public float _handleRange;
    public bool IsDrag { get; private set; }
    #endregion

    #region Unity Methods
    private void Start()
    {
        IsDrag = false;
        _joyStickBG.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _joyStickBG.gameObject.SetActive(false);
        _moveDir = Vector3.zero;
    }
    #endregion

    #region Public Methods
    public void OnPointerDown(PointerEventData eventData)
    {
        _joyStickBG.gameObject.SetActive(true);
        _joyStickBG.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        IsDrag = true;

        _handle.position = eventData.position;
        _handle.localPosition = Vector2.ClampMagnitude(eventData.position - (Vector2)_joyStickBG.position, _joyStickBG.rect.width * _handleRange);
        _moveDir = new Vector3(_handle.localPosition.x, _handle.localPosition.y, 0).normalized;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsDrag = false;

        _joyStickBG.gameObject.SetActive(false);
        _handle.localPosition = Vector2.zero;
        _moveDir = Vector3.zero;
    }

    public Vector3 GetMoveDir()
    {
        return _moveDir;
    }
    public void StopDrag()
    {
        IsDrag = false;
    }
    #endregion
}
