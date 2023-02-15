using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMissileCrossHair : MonoBehaviour
{
    #region FIelds
    private Tweener _tweener;
    private Vector3 _scale = new Vector3(2, 2, 1);
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _tweener = GetComponent<Tweener>();
    }
    #endregion

    #region Public Methods
    public void SetCrossHair(Vector3 start, Vector3 final)
    {
        _tweener.From = start;
        _tweener.To = final;

        _tweener.StartTween(() => { Invoke("SetActive", 0.2f); } );

        transform.localScale = _scale * BuffSkillController.Buff_BulletScale;
    }

    private void SetActive()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
