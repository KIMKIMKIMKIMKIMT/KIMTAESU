using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostumeCollectionManager
{
    #region Fields
    public int SetCount;
    #endregion

    #region Public Methods
    public void CostumeCollectActive()
    {
        int count = 0;
        int check = 0;
        foreach (var chartData in ChartManager.CostumCollectionCharts.Values)
        {
            for (int i = 0; i < chartData.RequireCostumes.Length; i++)
            {
                if (Managers.Game.CostumeDatas[chartData.RequireCostumes[i]].Awakening == -1)
                {
                    check++;
                }
            }

            if (check == 0)
            {
                count++;
                Managers.Game.CostumeSetDatas[chartData.Id] = true;
            }
            else
            {
                Managers.Game.CostumeSetDatas[chartData.Id] = false;
                check = 0;
            }
        }

        SetCount = count;

        Managers.Game.CalculateStat();
    }
    #endregion

    #region Private Methods
    #endregion
}
