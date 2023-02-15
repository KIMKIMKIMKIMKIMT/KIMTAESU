using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMgr : SingletonMonoBehaviour<PoolMgr>
{
    #region Fields
    [SerializeField] private Base_BulletPool _baseBulletPool;
    [SerializeField] private EnemyPool _enemyPool;
    [SerializeField] public GemPool _gemPool;
    private EffectPool _effectPool;
    private DmgTxtPool _dmgTxtPool;
    [SerializeField] private EnemyBulletPool _enemyBulletPool;
    [SerializeField] private BossRaidFencePool _bossRaidFencePool;
    [SerializeField] private GameItemBoxPool _gameItemBoxPool;
    private Transform _player;
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        _effectPool = GetComponentInChildren<EffectPool>();
        _dmgTxtPool = GetComponentInChildren<DmgTxtPool>();
        _player = BattleMgr.Instance.Player.transform;
    }


    #endregion

    #region Public Methods
    public Base_Bullet GetBaseBullet(int index)
    {
        return _baseBulletPool.GetFromPool(_baseBulletPool.transform, index);
    }

    public Enemy GetEnemy(int index)
    {
        return _enemyPool.GetFromPool(_enemyPool.transform, index);
    }
    public void GemSpawn(int index, Vector3 position)
    {
        Gem gem = _gemPool.GetFromPool(index);
        gem.transform.position = position;
        gem.transform.SetParent(_gemPool.transform);
    }

    public Effect GetEffect(int index)
    {
        return _effectPool.GetFromPool(_effectPool.transform, index);
    }

    public DmgTxt GetDmgTxt(int index)
    {
        return _dmgTxtPool.GetFromPool(_dmgTxtPool.transform, index);
    }

    public EnemyBullet GetEnemyBullet(int index)
    {
        return _enemyBulletPool.GetFromPool(_enemyBulletPool.transform, index);
    }
    public GameItemBox GetItemBox(int index)
    {
        return _gameItemBoxPool.GetFromPool(_gameItemBoxPool.transform, index);
    }
    public void GetFence(int width,int height)
    {
        Vector3 playerPos = _player.position;

        float evenOffset = (width % 2 == 0) ? 0.5f : 0; //짝수일때 예외처리1

        for (int i = 0; i < width; i++)
        {
            BossRaidFence fence1 = _bossRaidFencePool.GetFromPool(_bossRaidFencePool.transform, 0);
            fence1.transform.position = new Vector3(i - width / 2 + evenOffset, (float)height / 2) + playerPos;

            BossRaidFence fence2 = _bossRaidFencePool.GetFromPool(_bossRaidFencePool.transform, 0);
            fence2.transform.position = new Vector3(i - width / 2 + evenOffset, -(float)height / 2) + playerPos;
        }

        evenOffset = (height % 2 == 0) ? 0.5f : 0; //짝수일때 예외처리2

        for (int i = 0; i < height; i++)
        {
            BossRaidFence fence3 = _bossRaidFencePool.GetFromPool(_bossRaidFencePool.transform, 0);
            fence3.transform.position = new Vector3((float)width / 2, i - height / 2 + evenOffset) + playerPos;

            BossRaidFence fence4 = _bossRaidFencePool.GetFromPool(_bossRaidFencePool.transform, 0);
            fence4.transform.position = new Vector3(-(float)width / 2, i - height / 2 + evenOffset) + playerPos;
        }
    }
    public void FenceObjOff()
    {
        _bossRaidFencePool.AllObjActiveOff();
    }
    public void EnemyBulletObjAllOff()
    {
        _enemyBulletPool.AllObjActiveOff();
    }
    #endregion
}
