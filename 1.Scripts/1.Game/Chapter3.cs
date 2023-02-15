using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter3 : MonoBehaviour
{
    #region Fields
    [SerializeField] private CameraMove _cam;

    private WaitForSeconds _wait = new WaitForSeconds(20f);
    private WaitForSeconds _bossWait = new WaitForSeconds(2);

    int _time;
    #endregion

    #region Unity Methods
    private void Start()
    {
        StopAllCoroutines();
        StartCoroutine(Cor_DefaultWave());
    }
    private void Update()
    {
        _time = (int)BattleMgr.Instance.PlayTime;
    }
    #endregion

    #region Public Methods
    public void GetItemBoxSpawn()
    {
        int xRandom = Random.Range(0, 2) == 0 ? -1 : 1;
        int yRandom = Random.Range(0, 2) == 0 ? -1 : 1;

        float x = Random.Range(0f, 16f);
        Vector3 pos = new Vector3(x * xRandom + BattleMgr.Instance.Player.transform.position.x, Random.Range(x > 8 ? 0 : 8, 16f) * yRandom + BattleMgr.Instance.Player.transform.position.y);
        GameItemBox box = PoolMgr.Instance.GetItemBox(0);
        box.transform.position = pos;
    }
    public void GeneralEnemySpawn(eENEMY_TYPE type, float index = 1)
    {
        for (int i = 0; i < 30; i++)
        {
            int xRandom = Random.Range(0, 2) == 0 ? -1 : 1;
            int yRandom = Random.Range(0, 2) == 0 ? -1 : 1;

            float x = Random.Range(0f, 16f);

            Vector3 pos = new Vector3(x * xRandom + BattleMgr.Instance.Player.transform.position.x, Random.Range(x > 8 ? 0 : 8, 16f) * yRandom + BattleMgr.Instance.Player.transform.position.y);
            Enemy enemy = PoolMgr.Instance.GetEnemy((int)type);
            enemy.transform.position = pos;
            enemy.EnemyDataUpgrade(index);
        }
    }
    public void LongDistanceEnemySpawn(eENEMY_TYPE type, float index = 1)
    {
        int xRandom = Random.Range(0, 2) == 0 ? -1 : 1;
        int yRandom = Random.Range(0, 2) == 0 ? -1 : 1;

        float x = Random.Range(0f, 16f);

        Vector3 pos = new Vector3(x * xRandom + BattleMgr.Instance.Player.transform.position.x, Random.Range(x > 8 ? 0 : 8, 16f) * yRandom + BattleMgr.Instance.Player.transform.position.y);
        Enemy enemy = PoolMgr.Instance.GetEnemy((int)type);
        enemy.transform.position = pos;
        enemy.EnemyDataUpgrade(index);
    }

    public void MiniBossSpawn(float index = 1)
    {
        int xRandom = Random.Range(0, 2) == 0 ? -1 : 1;
        int yRandom = Random.Range(0, 2) == 0 ? -1 : 1;

        float x = Random.Range(0f, 16f);

        Vector3 pos = new Vector3(x * xRandom + BattleMgr.Instance.Player.transform.position.x, Random.Range(x > 8 ? 0 : 8, 16f) * yRandom + BattleMgr.Instance.Player.transform.position.y);
        Enemy enemy = PoolMgr.Instance.GetEnemy((int)eENEMY_TYPE.MiniBoss1);
        enemy.transform.position = pos;
        enemy.EnemyDataUpgrade(index);
    }
    #endregion
    #region Coroutines
    private IEnumerator Cor_DefaultWave()
    {

        while (_time < 300)
        {
            int ran = Random.Range(0, 20);
            if (ran == 0)
            {
                GetItemBoxSpawn();
            }

            GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1, 6);

            if (_time == 60)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1, 6);
            }

            if (_time == 120)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1, 6);
                _wait = new WaitForSeconds(5f);
            }

            if (_time == 150)
            {
                InGameUIMgr.Instance._gameUI.Warning();
                _cam.SetCameraSize(8f);
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1, 6);
                MiniBossSpawn();
            }

            if (_time == 180)
            {
                for (int i = 0; i < 15; i++)
                {
                    LongDistanceEnemySpawn(eENEMY_TYPE.LongDistanceEnemy1, 6);
                }
            }

            if (_time == 245)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1, 6);
                for (int i = 0; i < 15; i++)
                {
                    LongDistanceEnemySpawn(eENEMY_TYPE.LongDistanceEnemy1, 6);
                }
            }

            if (_time == 265)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1, 6);
            }
            if (_time == 280)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1, 6);
            }
            if (_time == 290)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1, 6);
            }
            yield return _wait;
        }

        BattleMgr.Instance.BossRaid(true);
        InGameUIMgr.Instance._gameUI.ShowBoomEffect();
        BattleMgr.Instance.Player.BoomTrigger(1000);

        yield return _bossWait;
        PoolMgr.Instance.GetFence(15, 15);

        Enemy boss1 = PoolMgr.Instance.GetEnemy(6);
        boss1.EnemyDataUpgrade(6);
        boss1.transform.position = new Vector2(BattleMgr.Instance.Player.transform.position.x, BattleMgr.Instance.Player.transform.position.y + 3.5f);


        while (BattleMgr.Instance.IsBossRaid)
        {
            yield return null;
        }
        PoolMgr.Instance.FenceObjOff();

        while (_time < 600)
        {
            int ran = Random.Range(0, 20);
            if (ran == 0)
            {
                GetItemBoxSpawn();
            }

            GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 6);

            if (_time == 330)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 6);
            }

            if (_time == 360)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 6);
            }
            if (_time == 390)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 3);
            }

            if (_time == 450)
            {
                InGameUIMgr.Instance._gameUI.Warning();
                _cam.SetCameraSize(9f);
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1, 6);
                MiniBossSpawn();
            }

            if (_time == 455)
            {
                for (int i = 0; i < 15; i++)
                {
                    LongDistanceEnemySpawn(eENEMY_TYPE.LongDistanceEnemy1, 6);
                }
            }

            if (_time == 465)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 6);
            }

            if (_time == 480)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1, 6);
            }
            if (_time == 520)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1, 6);
            }
            if (_time == 550)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 6);
            }
            if (_time == 580)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 6);
            }
            yield return _wait;
        }

        BattleMgr.Instance.BossRaid(true);
        InGameUIMgr.Instance._gameUI.ShowBoomEffect();
        BattleMgr.Instance.Player.BoomTrigger(1000);

        yield return _bossWait;
        PoolMgr.Instance.GetFence(15, 15);

        Enemy boss2 = PoolMgr.Instance.GetEnemy(7);
        boss2.EnemyDataUpgrade(6);
        boss2.transform.position = new Vector2(BattleMgr.Instance.Player.transform.position.x, BattleMgr.Instance.Player.transform.position.y + 3.5f);


        while (BattleMgr.Instance.IsBossRaid)
        {
            yield return null;
        }
        PoolMgr.Instance.FenceObjOff();

        while (_time < 900)
        {
            int ran = Random.Range(0, 20);
            if (ran == 0)
            {
                GetItemBoxSpawn();
            }
            if (_time < 800)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 6);
            }


            if (_time == 630)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 6);
            }

            if (_time == 660)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 6);
                for (int i = 0; i < 20; i++)
                {
                    LongDistanceEnemySpawn(eENEMY_TYPE.LongDistanceEnemy2, 6);
                }

            }
            if (_time == 700)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 6);
            }
            if (_time == 720)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2, 6);
            }
            if (_time == 750)
            {
                InGameUIMgr.Instance._gameUI.Warning();
                _cam.SetCameraSize(10f);
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3, 6);
                MiniBossSpawn();
            }

            if (_time == 760)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3, 6);
            }

            if (_time == 780)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3, 6);
            }

            if (_time == 800)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3, 6);
                _wait = new WaitForSeconds(2.5f);
            }
            if (_time > 800)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3, 6);
            }
            yield return _wait;
        }

        BattleMgr.Instance.BossRaid(true);
        InGameUIMgr.Instance._gameUI.ShowBoomEffect();
        BattleMgr.Instance.Player.BoomTrigger(1000);

        yield return _bossWait;
        PoolMgr.Instance.GetFence(15, 15);

        Enemy boss3 = PoolMgr.Instance.GetEnemy(8);
        boss3.transform.position = new Vector2(BattleMgr.Instance.Player.transform.position.x, BattleMgr.Instance.Player.transform.position.y + 3.5f);
        boss3.EnemyDataUpgrade(6);

        while (BattleMgr.Instance.IsBossRaid)
        {
            yield return null;
        }
        BattleMgr.Instance.GameOver(true);
    }
    #endregion
}
