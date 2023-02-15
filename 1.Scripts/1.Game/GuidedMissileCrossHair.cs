using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissileCrossHair : MonoBehaviour
{
    #region Fields
    private Vector3 _scale = new Vector3(0.8f, 0.8f, 1);
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        transform.localScale = _scale * BuffSkillController.Buff_BulletScale;
        Invoke("ActiveOff", 1.5f);
    }
    #endregion

    #region Public Methods
    public void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
