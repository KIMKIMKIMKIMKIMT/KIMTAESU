using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgObject : MonoBehaviour
{
    #region Fields
    [SerializeField] private BG_Controller _ctrl;

    public int Index { get; private set; }
    #endregion

    #region Unity Methods
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _ctrl.BGLoop();
        }
    }
    #endregion

    #region Public Methods
    public void SetIndex(int index)
    {
        Index = index;
    }
    #endregion
}
