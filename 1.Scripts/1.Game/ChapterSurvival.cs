using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterSurvival : MonoBehaviour
{
    #region Fields
    [SerializeField] private CameraMove _cam;

    private WaitForSeconds _wait = new WaitForSeconds(5f);
    private WaitForSeconds _forWait = new WaitForSeconds(0.5f);
    private WaitForSeconds _waveWait = new WaitForSeconds(5f);

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
        int hp = 10;
        while (true)
        {
            int ran = Random.Range(0, 20);

            if (ran == 0)
            {
                GetItemBoxSpawn();
            }

            if (_time < 300)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy1);
            }

            if (_time == 60)
            {
                MiniBossSpawn(5);
                for (int i = 0; i < 10; i++)
                {
                    LongDistanceEnemySpawn(eENEMY_TYPE.LongDistanceEnemy1);
                }
            }
            if (_time == 120)
            {
                _cam.SetCameraSize(8f);
                for (int i = 0; i < 2; i++)
                {
                    yield return null;
                    yield return null;
                    yield return null;
                    yield return null;
                    MiniBossSpawn(5);
                    GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2);
                }
            }
            if (_time == 150)
            {
                for (int i = 0; i < 30; i++)
                {
                    yield return null;
                    yield return null;
                    yield return null;
                    yield return null;
                    yield return null;
                    LongDistanceEnemySpawn(eENEMY_TYPE.LongDistanceEnemy1);
                }
            }
            if (_time == 200)
            {
                for (int i = 0; i < 3; i++)
                {
                    yield return null;
                    yield return null;
                    yield return null;
                    yield return null;
                    GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy2);
                }

            }
            if (_time == 230)
            {
                for (int j = 0; j < 30; j++)
                {
                    yield return null;
                    yield return null;
                    yield return null;
                    yield return null;
                    yield return null;
                    LongDistanceEnemySpawn(eENEMY_TYPE.LongDistanceEnemy2);
                }
            }
            if (_time == 250)
            {
                _cam.SetCameraSize(9f);
                for (int i = 0; i < 10; i++)
                {
                    yield return null;
                    yield return null;
                    yield return null;
                    yield return null;
                    yield return null;
                    GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3);
                }
            }
            if (_time == 300)
            {
                _wait = new WaitForSeconds(2.5f);
                MiniBossSpawn();
                for (int i = 0; i < 10; i++)
                {
                    GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3);
                }
            }
            else if (_time < 360)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3, 2);
            }
            else if (_time < 420)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3, 3);
            }
            else if (_time < 480)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3, 4);
            }
            else if (_time < 540)
            {
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3, 5);
            }
            else if (_time < 600)
            {
                
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3, 6);
            }
            else if (_time > 660)
            {
                hp++;
                GeneralEnemySpawn(eENEMY_TYPE.ShortDistanceEnemy3, hp);
            }
            yield return _wait;

        }

    }
    #endregion
}
