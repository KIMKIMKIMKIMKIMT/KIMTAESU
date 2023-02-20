using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Controller : MonoBehaviour
{
    #region Fields
    [SerializeField] private BgObject[] _tiles;
    
    private List<BgObject> _bgList;

    private Vector3[] _gridPos;
    private Vector2[] _offset;

    private float _width;
    private float _height;
    public bool IsInit { get; private set; }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _tiles = GetComponentsInChildren<BgObject>();
        _bgList = new List<BgObject>();
        for (int i = 0; i < _tiles.Length; i++)
        {
            _bgList.Add(_tiles[i]);
        }
        _gridPos = new Vector3[9];
        _offset = new Vector2[9] {
            new Vector2(-1.5f, 1.5f), new Vector2(0, 1.5f), new Vector2(1.5f, 1.5f),
            new Vector2(-1.5f, 0), new Vector2(0, 0), new Vector2(1.5f, 0),
            new Vector2(-1.5f, -1.5f), new Vector2(0, -1.5f), new Vector2(1.5f, -1.5f)
        };

        BoxCollider2D standard = _tiles[0].GetComponent<BoxCollider2D>();
        _width = standard.size.x;
        _height = standard.size.y;

        for (int i = 0; i < _bgList.Count; i++)
        {
            _gridPos[i] = new Vector2(_width * _offset[i].x, _height * _offset[i].y);
            _tiles[i].SetIndex(i);
            _tiles[i].transform.localPosition = _gridPos[i];
        }
    }

    private void Start()
    {
        IsInit = true;
    }
    #endregion

    #region Public Methods
    public void BGLoop()
    {
        if (!IsInit)
        {
            return;
        }

        BgObject currentTile = null;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < _tiles.Length; i++)
        {
            float dis = Vector3.Distance(BattleMgr.Instance.Player.transform.position, _tiles[i].transform.position);
            if (dis <= minDistance)
            {
                minDistance = dis;
                currentTile = _tiles[i];
            }
        }


        BgObject temp = _tiles[4];
        _tiles[4] = currentTile;
        _tiles[currentTile.Index] = temp;

        for (int i = 0; i < _tiles.Length; i++)
        {
            _tiles[i].SetIndex(i);
        }


        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] != currentTile)
            {
                _tiles[i].transform.localPosition = new Vector2(currentTile.transform.localPosition.x + _width * _offset[i].x, currentTile.transform.localPosition.y + _height * _offset[i].y);
            }
        }
    }
    #endregion
}
