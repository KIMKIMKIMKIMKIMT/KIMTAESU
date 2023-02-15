using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEditor;
using Firebase;
using Firebase.Database;
using Newtonsoft.Json;

public enum eDATA_GROUP
{
    Users,
    RankData,
    Ranking
}

public class PlayerDataMgr : DontDestroy<PlayerDataMgr>
{
    #region Fields
    private DatabaseReference _dataBase;
    public DatabaseReference Database { get { return _dataBase; } }
   

    public PlayerData PlayerData { get; private set; }
    public RankData RankData { get; private set; }

    private readonly string DATABASE_URL = "https://tanksurvival-b3df0-default-rtdb.firebaseio.com/";

    public bool IsLogin { get; private set; }
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        InitFirebaseDatabase();
        IsLogin = false;
    }
    #endregion

    #region Public Methods
    public void PlayerDataSet()
    {
        PlayerData = new PlayerData();
    }
    public void SetRankData()
    {
        RankData.UserNickname = PlayerData.UserNickname;
        RankData.Time = PlayerData.TopRecord;
        SaveRankData();
    }
    public async void GetPlayerData()
    {
        
        await _dataBase.Child(eDATA_GROUP.Users.ToString()).Child(AccountMgr.Instance.UID).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        PlayerData player = JsonConvert.DeserializeObject<PlayerData>(snapshot.GetRawJsonValue());

                        PlayerData = new PlayerData(player);
                        RankData = new RankData(player.UserNickname, player.TopRecord);
                    }
                    else
                    {
                        PlayerData = new PlayerData();
                        RankData = new RankData();
                    }
                }
            }
        });
        IsLogin = true;
        SaveData();
        SaveRankData();
    }

    public async Task<List<RankData>> GetRankingData()
    {
        List<RankData> rankList = new List<RankData>();
        await _dataBase.Child(eDATA_GROUP.Ranking.ToString()).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot data in snapshot.Children)
                    {
                        RankData rankData = JsonConvert.DeserializeObject<RankData>(data.GetRawJsonValue());
                        rankList.Add(rankData);
                    }

                }
            }
        });

        return rankList;
    }
    public async void SaveData()
    {
        string jsonData = JsonConvert.SerializeObject(PlayerData);
        await _dataBase.Child(eDATA_GROUP.Users.ToString()).Child(AccountMgr.Instance.UID).SetRawJsonValueAsync(jsonData).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {

            }
        });
    }
    public async void SaveRankData()
    {
        string jsonData = JsonConvert.SerializeObject(RankData);
        await _dataBase.Child(eDATA_GROUP.RankData.ToString()).Child(AccountMgr.Instance.UID).SetRawJsonValueAsync(jsonData).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {

            }
        });
    }

    public int GetEquipToLevel(eATKWEAPON weapon, int index)
    {
        return PlayerData.WeaponLevel[(int)weapon][index];
    }
    public int GetEquipToLevel(eHPEQUIP equip, int index)
    {
        return PlayerData.EquipLevel[(int)equip][index];
    }

    public void SetTankToEquip(eATKWEAPON weapon, int index)
    {
        PlayerData.TankInfo[PlayerData.CurTank].WeaponType = weapon;
        PlayerData.TankInfo[PlayerData.CurTank].CurrentWeaponIndex = index;
        
        if (weapon == eATKWEAPON.None)
        {
            PlayerData.TankInfo[PlayerData.CurTank].CurrentWeaponLevel = 0;
        }
        else
        {
            PlayerData.TankInfo[PlayerData.CurTank].CurrentWeaponLevel = PlayerData.WeaponLevel[(int)weapon][index];
        }
    }
    public void SetTankToEquip(eHPEQUIP equip, int index)
    {
        PlayerData.TankInfo[PlayerData.CurTank].EquipType = equip;
        PlayerData.TankInfo[PlayerData.CurTank].CurrentEquipIndex = index;
        if (equip == eHPEQUIP.None)
        {
            PlayerData.TankInfo[PlayerData.CurTank].CurrentEquipLevel = 0;
        }
        else
        {
            PlayerData.TankInfo[PlayerData.CurTank].CurrentEquipLevel = PlayerData.EquipLevel[(int)equip][index];
        }
    }

    public void InitTankToWeapon()
    {
        PlayerData.TankInfo[PlayerData.CurTank].WeaponType = eATKWEAPON.None;
        PlayerData.TankInfo[PlayerData.CurTank].CurrentWeaponIndex = -1;
        PlayerData.TankInfo[PlayerData.CurTank].CurrentWeaponLevel = 0;
    }
    public void InitTankToEquip()
    {
        PlayerData.TankInfo[PlayerData.CurTank].EquipType = eHPEQUIP.None;
        PlayerData.TankInfo[PlayerData.CurTank].CurrentEquipIndex = -1;
        PlayerData.TankInfo[PlayerData.CurTank].CurrentEquipLevel = 0;
    }
    public void SetFusionEquip(eATKWEAPON weapon, int index, eATKWEAPON fusionWeapon, int newIndex)
    {
        for (int i = 0; i < (int)eTANK.Max; i++)
        {
            if (PlayerData.TankInfo[(eTANK)i].WeaponType == weapon && PlayerData.TankInfo[(eTANK)i].CurrentWeaponIndex == index)
            {
                PlayerData.TankInfo[(eTANK)i].WeaponType = fusionWeapon;
                PlayerData.TankInfo[(eTANK)i].CurrentWeaponIndex = newIndex;
                UIMgr.Instance.MainUI.InvenUI.SetCurrentEquip(fusionWeapon, newIndex, GetEquipToLevel(fusionWeapon, newIndex));
            }
        }
    }
    public void SetFusionEquip(eHPEQUIP equip, int index, eHPEQUIP fusionEquip, int newIndex)
    {
        for (int i = 0; i < (int)eTANK.Max; i++)
        {
            if (PlayerData.TankInfo[(eTANK)i].EquipType == equip && PlayerData.TankInfo[(eTANK)i].CurrentEquipIndex == index)
            {
                PlayerData.TankInfo[(eTANK)i].EquipType = fusionEquip;
                PlayerData.TankInfo[(eTANK)i].CurrentEquipIndex = newIndex;
                UIMgr.Instance.MainUI.InvenUI.SetCurrentEquip(fusionEquip, newIndex, GetEquipToLevel(fusionEquip, newIndex));
            }
        }
    }
    #endregion

    #region Firebase Database
    private void InitFirebaseDatabase()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DATABASE_URL);
        _dataBase = FirebaseDatabase.DefaultInstance.RootReference;
    }
    #endregion
}

#region Data Class
public enum eTANK
{
    Tank_0,
    Tank_1,
    Tank_2,

    Max
}


[Serializable]
public class PlayerData
{
    public enum eTANK_STATUS
    {
        ATK,
        HP
    }

    public Dictionary<eQUEST, int> QuestState;
    public Dictionary<eQUEST, bool> QuestRewardCheck;

    public Dictionary<eTANK, TankData> TankInfo;
    
    public eTANK CurTank;
    //public RankData Rank;

    public string UserNickname;
    public int UserLevel;
    public float UserExp;
    public float UserLevelUpExp;
    public float TopRecord;
    public float EneterPlayTime;
    
    public int Gem;
    public int Gold;
    
    public bool[] Tanks;
    public bool BgmToggle;
    public bool SfxToggle;
    public bool InGameSoundToggle;

    public List<int>[] WeaponLevel;
    public List<int>[] EquipLevel;
    public int WeaponScroll;
    public int EquipScroll;
    public TankData GetCurrentTankData()
    {
        return TankInfo[CurTank];
    }
    public void AddEquip(eATKWEAPON weapon, int level)
    {
        WeaponLevel[(int)weapon].Add(level);
    }
    public void AddEquip(eHPEQUIP equip, int level)
    {
        EquipLevel[(int)equip].Add(level);
    }
    public void AddScroll(eSCROLL_TYPE type, int index)
    {
        switch (type)
        {
            case eSCROLL_TYPE.Weapon:
                WeaponScroll += index;
                break;
            case eSCROLL_TYPE.Equip:
                EquipScroll += index;
                break;
        }
    }
    public void EquipLevelUp(eATKWEAPON weapon, int index)
    {
        WeaponLevel[(int)weapon][index]++;
    }
    public void EquipLevelUp(eHPEQUIP equip, int index)
    {
        EquipLevel[(int)equip][index]++;
    }

    public void RemoveEquip(eATKWEAPON weapon, int index)
    {
        WeaponLevel[(int)weapon].RemoveAt(index);
    }
    public void RemoveEquip(eHPEQUIP equip, int index)
    {
        EquipLevel[(int)equip].RemoveAt(index);
    }
    public void RemoveScroll(eSCROLL_TYPE type, int index)
    {
        switch (type)
        {
            case eSCROLL_TYPE.Weapon:
                WeaponScroll -= index;
                break;
            case eSCROLL_TYPE.Equip:
                EquipScroll -= index;
                break;
        }
    }
    public int GetEquipLevel(eATKWEAPON weapon, int index)
    {
        return WeaponLevel[(int)weapon][index];
    }
    public int GetEquipLevel(eHPEQUIP equip, int index)
    {
        return EquipLevel[(int)equip][index];
    }
    
    public PlayerData()
    {
        QuestState = new Dictionary<eQUEST, int>();
        QuestRewardCheck = new Dictionary<eQUEST, bool>();
        TankData tankData;

        TankInfo = new Dictionary<eTANK, TankData>();
        for (int i = 0; i < (int)eTANK.Max; i++)
        {
            tankData = new TankData();
            TankInfo.Add((eTANK)i, tankData);
        }
        CurTank = eTANK.Tank_0;

        for (int i = 0; i < (int)eQUEST.Max; i++)
        {
            QuestState.Add((eQUEST)i, 0);
            QuestRewardCheck.Add((eQUEST)i, false);
        }
        UserLevel = 1;
        UserExp = 0;
        UserLevelUpExp = 100;
        TopRecord = 0;
        EneterPlayTime = 0;
        UserNickname = null;
        
        Gem = 10000;
        Gold = 10000;

        Tanks = new bool[2];
        for (int i = 0; i < Tanks.Length; i++)
        {
            Tanks[i] = false;
        }
        CurTank = eTANK.Tank_0;

        BgmToggle = true;
        SfxToggle = true;
        InGameSoundToggle = true;

        WeaponLevel = new List<int>[(int)eATKWEAPON.Max];

        for (int i = 0; i < (int)eATKWEAPON.Max; i++)
        {
            WeaponLevel[i] = new List<int>();
        }

        EquipLevel = new List<int>[(int)eHPEQUIP.Max];

        for (int i = 0; i < (int)eHPEQUIP.Max; i++)
        {
            EquipLevel[i] = new List<int>();
        }

        WeaponScroll = 0;
        EquipScroll = 0;
    }

    public PlayerData(PlayerData serverData)
    {
        QuestState = new Dictionary<eQUEST, int>();
        foreach (KeyValuePair<eQUEST, int> pair in serverData.QuestState)
        {
            QuestState.Add(pair.Key, pair.Value);
        }

        QuestRewardCheck = new Dictionary<eQUEST, bool>();
        foreach (KeyValuePair<eQUEST, bool> pair in serverData.QuestRewardCheck)
        {
            QuestRewardCheck.Add(pair.Key, pair.Value);
        }

        
        TankInfo = new Dictionary<eTANK, TankData>();
        foreach (KeyValuePair<eTANK, TankData> pair in serverData.TankInfo)
        {
            TankInfo.Add(pair.Key, pair.Value);
        }

        CurTank = serverData.CurTank;
        UserLevel = serverData.UserLevel;
        UserExp = serverData.UserExp;
        UserLevelUpExp = serverData.UserLevelUpExp;
        TopRecord = serverData.TopRecord;
        EneterPlayTime = serverData.EneterPlayTime;
        UserNickname = serverData.UserNickname;
        
        Gem = serverData.Gem;
        Gold = serverData.Gold;

        Tanks = new bool[2];
        for (int i = 0; i < Tanks.Length; i++)
        {
            Tanks[i] = serverData.Tanks[i];
        }

        BgmToggle = serverData.BgmToggle;
        SfxToggle = serverData.SfxToggle;
        InGameSoundToggle = serverData.InGameSoundToggle;

        WeaponLevel = new List<int>[(int)eATKWEAPON.Max];

        for (int i = 0; i < (int)eATKWEAPON.Max; i++)
        {
            WeaponLevel[i] = new List<int>();
        }

        for (int i = 0; i < serverData.WeaponLevel.Length; i++)
        {
            try
            {
                if (serverData.WeaponLevel[i] != null)
                {
                    for (int j = 0; j < serverData.WeaponLevel[i].Count; j++)
                    {
                        WeaponLevel[i].Add(serverData.WeaponLevel[i][j]);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        EquipLevel = new List<int>[(int)eHPEQUIP.Max];

        for (int i = 0; i < (int)eHPEQUIP.Max; i++)
        {
            EquipLevel[i] = new List<int>();
        }

        for (int i = 0; i < serverData.EquipLevel.Length; i++)
        {
            try
            {
                if (serverData.EquipLevel[i] != null)
                {
                    for (int j = 0; j < serverData.EquipLevel[i].Count; j++)
                    {
                        EquipLevel[i].Add(serverData.EquipLevel[i][j]);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        WeaponScroll = serverData.WeaponScroll;
        EquipScroll = serverData.EquipScroll;
    }
}
#endregion