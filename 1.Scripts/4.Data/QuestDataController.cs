using System.Collections;
using System.Collections.Generic;
using System;

public enum eQUEST
{
    EnterGame,
    ChapterPlay,
    SurvivalPlay,
    GemUse50,
    Play10,
    ShowAds,
    Upgrade,

    Max
}

public class QuestData
{
    public int Index;
    public eQUEST Key;
    public string Header;
    public int ClearCnt;
    public eASSET_TYPE RewardType;
    public int RewardCnt;

    public QuestData(int index, eQUEST key, string header, int clearCnt, eASSET_TYPE reward, int cnt)
    {
        Index = index;
        Key = key;
        Header = header;
        ClearCnt = clearCnt;
        RewardType = reward;
        RewardCnt = cnt;
    }
}

public class QuestDataController
{

    public enum eQUEST_STRUCT
    {
        Index,
        Key,
        Header,
        ClearCnt,
        Reward,
        Cnt
    }

    public Dictionary<eQUEST, QuestData> QuestDic;

    public readonly string PATH = "ExcelData/QuestData";

    public QuestDataController()
    {
        QuestDic = new Dictionary<eQUEST, QuestData>();

        string[,] data = ExcelDataLoader.Instance.LoadTextData(PATH);

        QuestData qeustData;
        for (int i = 0; i < data.GetLength(0); i++)
        {
            qeustData = new QuestData(
                int.Parse(data[i, (int)eQUEST_STRUCT.Index]),
                (eQUEST)Enum.Parse(typeof(eQUEST), data[i, (int)eQUEST_STRUCT.Key]),
                data[i, (int)eQUEST_STRUCT.Header],
                int.Parse(data[i, (int)eQUEST_STRUCT.ClearCnt]),
                (eASSET_TYPE)Enum.Parse(typeof(eASSET_TYPE), data[i, (int)eQUEST_STRUCT.Reward]),
                int.Parse(data[i, (int)eQUEST_STRUCT.Cnt])
                );

            QuestDic.Add(qeustData.Key, qeustData);
        }
    }
}
