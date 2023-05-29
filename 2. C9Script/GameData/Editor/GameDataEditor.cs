#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class GameDataEditor : EditorWindow
{
    #region Override Property(DateTime)

    private string _serverTime;

    private string ServerTime
    {
        get => Managers.Game.ServerTime.ToString();
        set
        {
            if (!DateTime.TryParse(value, out var dateTime))
                return;

            Managers.Game.ServerTime = dateTime;
        }
    }

    private string _resetTime;

    private string ResetTime
    {
        get => Managers.Game.UserData.ResetTime.ToString();
        set
        {
            if (!DateTime.TryParse(value, out var dateTime))
                return;
            
            Managers.Game.UserData.ResetTime = dateTime;
        }
    }

    private string _guildAttendanceTime;

    private string GuildAttendanceTime
    {
        get => _guildAttendanceTime;
        set
        {
            _guildAttendanceTime = value;

            if (!DateTime.TryParse(_guildAttendanceTime, out var dateTime))
                return;

            Managers.Game.UserData.GuildAttendanceTime = dateTime;
        }
    }

    private string _guildJoinCoolTime;

    private string GuildJoinCoolTime
    {
        get => _guildJoinCoolTime;
        set
        {
            _guildJoinCoolTime = value;

            if (!DateTime.TryParse(_guildJoinCoolTime, out var dateTime))
                return;

            Managers.Game.UserData.GuildJoinCoolTime = dateTime;
        }
    }

    private string _eventAttendanceCoolTime;

    private string EventAttendanceCoolTime
    {
        get => Managers.Game.UserData.EventAttendanceTime.ToString();
        set
        {
            _eventAttendanceCoolTime = value;

            if (!DateTime.TryParse(_eventAttendanceCoolTime, out var dateTime))
                return;

            Managers.Game.UserData.EventAttendanceTime = dateTime;
        }
    }

    #endregion

    enum DBTab
    {
        Tool,
        User,
        Goods,
        Collection,
        Costume,
        Weapon,
        Pet,
        Wood,
        Skill,
        Lab_Research,
        Lab_Awakening,
    }

    private int _currentTab;
    private static Vector2 _scrollPos = new Vector2();

    [MenuItem("Tools/GameDataEditor")]
    private static void Init()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("게임 실행중에만 작동 합니다.");
            return;
        }

        GameDataEditor window = (GameDataEditor)GetWindow(typeof(GameDataEditor));
        window.Show();

        window.titleContent.text = "데이터 에디터";

        window.minSize = new Vector2(800, 600);
        window.maxSize = new Vector2(1920, 1080);
    }

    private void OnGUI()
    {
        List<string> tabs = new();
        
        foreach (var dbTab in Enum.GetValues(typeof(DBTab)))
            tabs.Add(dbTab.ToString());

        _currentTab = GUILayout.Toolbar(_currentTab, tabs.ToArray());
        
        Color originColor = EditorStyles.boldLabel.normal.textColor;
        
        EditorStyles.boldLabel.normal.textColor = Color.cyan;
        GUILayout.Space(10f);
        GUILayout.Label(((DBTab)_currentTab).ToString(), EditorStyles.boldLabel);
        
        switch ((DBTab)_currentTab)
        {
            case DBTab.Tool:
                ShowTool();
                break;
            case DBTab.User:
                InitUserData();
                ShowUserData();
                break;
            case DBTab.Goods:
                ShowGoodsData();
                break;
            case DBTab.Collection:
                ShowCollectionData();
                break;
            case DBTab.Weapon:
                ShowWeaponData();
                break;
            case DBTab.Pet:
                ShowPetData();
                break;
            case DBTab.Wood:
                ShowWoodData();
                break;
            case DBTab.Costume:
                ShowCostumeData();
                break;
            case DBTab.Skill:
                ShowSkillData();
                break;
            case DBTab.Lab_Research:
                ShowLabResearchData();
                break;
            case DBTab.Lab_Awakening:
                ShowLabAwakening();
                break;
        }

        EditorStyles.boldLabel.normal.textColor = originColor;
    }

    private void ShowTool()
    {
        ServerTime = EditorGUILayout.TextField("서버 시간", ServerTime);
    }

    private void InitUserData()
    {
        GuildAttendanceTime = Managers.Game.UserData.GuildAttendanceTime.ToString();
        GuildJoinCoolTime = Managers.Game.UserData.GuildJoinCoolTime.ToString();
    }

    /// <summary>
    /// DB 이름 : User
    /// </summary>
    private void ShowUserData()
    {
        Managers.Game.UserData.Level = EditorGUILayout.IntField("레벨", Managers.Game.UserData.Level);
        Managers.Game.UserData.MaxReachStage = EditorGUILayout.IntField("최고 스테이지", Managers.Game.UserData.MaxReachStage);
        ResetTime = EditorGUILayout.TextField("리셋 시간", ResetTime);
        EventAttendanceCoolTime = EditorGUILayout.TextField("이벤트 출석 리셋 시간", EventAttendanceCoolTime);
        Managers.Game.UserData.PromoGrade = EditorGUILayout.IntField("승급 등급", Managers.Game.UserData.PromoGrade);
        Managers.Game.UserData.Dungeon1ClearStep =
            EditorGUILayout.IntField("화생방 최대 단계", Managers.Game.UserData.Dungeon1ClearStep);
        Managers.Game.UserData.Dungeon2ClearStep =
            EditorGUILayout.IntField("철병대 최대 단계", Managers.Game.UserData.Dungeon2ClearStep);
        Managers.Game.UserData.Dungeon3ClearStep =
            EditorGUILayout.IntField("행군 최대 단계", Managers.Game.UserData.Dungeon3ClearStep);
        Managers.Game.UserData.ProgressGuideQuestId =
            EditorGUILayout.IntField("가이드 퀘스트 단계", Managers.Game.UserData.ProgressGuideQuestId);
        GuildAttendanceTime = EditorGUILayout.TextField("길드 출석체크 시간", GuildAttendanceTime);
        GuildJoinCoolTime = EditorGUILayout.TextField("길드 가입 쿨타임", GuildJoinCoolTime);
    }

    /// <summary>
    /// DB 이름 : Goods
    /// </summary>
    private void ShowGoodsData()
    {
        var keys = Managers.Game.GoodsDatas.Keys.ToList();

        foreach (var goodsKey in keys)
        {
            string goodsName = ChartManager.GetString(ChartManager.GoodsCharts[goodsKey].Name);
            Managers.Game.GoodsDatas[goodsKey].Value =
                EditorGUILayout.DoubleField(goodsName, Managers.Game.GoodsDatas[goodsKey].Value);
        }
    }

    /// <summary>
    /// DB 이름 : Collection
    /// </summary>
    private void ShowCollectionData()
    {
        var keys = Managers.Game.CollectionDatas.Keys.ToList();

        _scrollPos = GUILayout.BeginScrollView(_scrollPos);
        
        foreach (var collectionKey in keys)
        {
            string collectionName = ChartManager.GetString(ChartManager.CollectionCharts[collectionKey].CollectionName);
            EditorStyles.boldLabel.normal.textColor = Color.green; 
            GUILayout.Label(collectionName, EditorStyles.boldLabel);
            
            Managers.Game.CollectionDatas[collectionKey].Lv =
                EditorGUILayout.IntField("레벨", Managers.Game.CollectionDatas[collectionKey].Lv);
            Managers.Game.CollectionDatas[collectionKey].Quantity =
                EditorGUILayout.IntField("갯수", Managers.Game.CollectionDatas[collectionKey].Quantity);
            
            GUILayout.Space(15);
        }
        
        GUILayout.EndScrollView();
    }

    private void ShowCostumeData()
    {
        var keys = Managers.Game.CostumeDatas.Keys.ToList();
        
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        foreach (var costumeKey in keys)
        {
            string costumeName = ChartManager.GetString(ChartManager.CostumeCharts[costumeKey].Name);
            EditorStyles.boldLabel.normal.textColor = Color.green; 
            GUILayout.Label(costumeName, EditorStyles.boldLabel);

            Managers.Game.CostumeDatas[costumeKey].Quantity =
                EditorGUILayout.IntField("갯수", Managers.Game.CostumeDatas[costumeKey].Quantity);
            Managers.Game.CostumeDatas[costumeKey].Awakening =
                EditorGUILayout.IntField("각성", Managers.Game.CostumeDatas[costumeKey].Awakening);
            GUILayout.Space(15);
        }
        
        GUILayout.EndScrollView();
    }
    
    private void ShowWeaponData()
    {
        var keys = Managers.Game.WeaponDatas.Keys.ToList();
        
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        foreach (var weaponKey in keys)
        {
            string weaponName = ChartManager.GetString(ChartManager.WeaponCharts[weaponKey].Name);
            EditorStyles.boldLabel.normal.textColor = Color.green; 
            GUILayout.Label(weaponName, EditorStyles.boldLabel);

            Managers.Game.WeaponDatas[weaponKey].Level =
                EditorGUILayout.IntField("레벨", Managers.Game.WeaponDatas[weaponKey].Level);
            
            Managers.Game.WeaponDatas[weaponKey].Quantity =
                EditorGUILayout.IntField("갯수", Managers.Game.WeaponDatas[weaponKey].Quantity);
            
            GUILayout.Space(15);
        }
        
        GUILayout.EndScrollView();
    }
    
    private void ShowPetData()
    {
        var keys = Managers.Game.PetDatas.Keys.ToList();
        
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        foreach (var petKey in keys)
        {
            string petName = ChartManager.GetString(ChartManager.PetCharts[petKey].PetName);
            EditorStyles.boldLabel.normal.textColor = Color.green; 
            GUILayout.Label(petName, EditorStyles.boldLabel);

            Managers.Game.PetDatas[petKey].Level =
                EditorGUILayout.IntField("레벨", Managers.Game.PetDatas[petKey].Level);
            
            Managers.Game.PetDatas[petKey].Quantity =
                EditorGUILayout.IntField("갯수", Managers.Game.PetDatas[petKey].Quantity);
            
            GUILayout.Space(15);
        }
        
        GUILayout.EndScrollView();
    }

    private void ShowWoodData()
    {
        var wooddata = Managers.Game.WoodsDatas[0];

        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        var keys = Managers.Game.WoodAwakeningDatas.Keys.ToList();

        if (wooddata.Id != 0)
        {
            string woodName = ChartManager.WoodCharts[wooddata.Id].Grade.ToString();
            EditorStyles.boldLabel.normal.textColor = Color.green;
            GUILayout.Label(woodName, EditorStyles.boldLabel);
        }

        Managers.Game.WoodsDatas[0].Id =
        EditorGUILayout.IntField("Id", Managers.Game.WoodsDatas[0].Id);

        Managers.Game.WoodsDatas[0].Level =
        EditorGUILayout.IntField("레벨", Managers.Game.WoodsDatas[0].Level);

        foreach (var key in keys)
        {
            string awakeningName = "Awakening_" + key;
            EditorStyles.boldLabel.normal.textColor = Color.green;
            GUILayout.Label(awakeningName, EditorStyles.boldLabel);

            Managers.Game.WoodAwakeningDatas[key].StatId =
                EditorGUILayout.IntField("스탯", Managers.Game.WoodAwakeningDatas[key].StatId);
            Managers.Game.WoodAwakeningDatas[key].Grade =
                (Grade)EditorGUILayout.IntField("등급", (int)Managers.Game.WoodAwakeningDatas[key].Grade);
        }

        GUILayout.Space(15);
        GUILayout.EndScrollView();
    }

    private void ShowSkillData()
    {
        var keys = Managers.Game.SkillDatas.Keys.ToList();
        
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        foreach (var skillKey in keys)
        {
            string skillName = ChartManager.GetString(ChartManager.SkillCharts[skillKey].Name);
            EditorStyles.boldLabel.normal.textColor = Color.green; 
            GUILayout.Label(skillName, EditorStyles.boldLabel);

            Managers.Game.SkillDatas[skillKey].Level =
                EditorGUILayout.IntField("레벨", Managers.Game.SkillDatas[skillKey].Level);

            GUILayout.Space(15);
        }
        
        GUILayout.EndScrollView();
    }

    private void ShowLabResearchData()
    {
        var keys = Managers.Game.LabResearchDatas.Keys.ToList();
        
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        foreach (var key in keys)
        {
            string labResearchName = ChartManager.GetString(ChartManager.LabSkillCharts[key].LabSkillType.ToString());
            EditorStyles.boldLabel.normal.textColor = Color.green; 
            GUILayout.Label(labResearchName, EditorStyles.boldLabel);

            Managers.Game.LabResearchDatas[key].Level =
                EditorGUILayout.IntField("레벨", Managers.Game.LabResearchDatas[key].Level);
            
            GUILayout.Space(15);
        }
        
        GUILayout.EndScrollView();
    }

    private void ShowLabAwakening()
    {
        var keys = Managers.Game.LabAwakeningDatas[Managers.Game.LabEquipPresetId].Keys;
        
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        foreach (var key in keys)
        {
            EditorStyles.boldLabel.normal.textColor = Color.green;
            GUILayout.Label($"{key}번", EditorStyles.boldLabel);
            
            Managers.Game.LabAwakeningDatas[Managers.Game.LabEquipPresetId][key].PatternId = 
                EditorGUILayout.IntField("문양",  Managers.Game.LabAwakeningDatas[Managers.Game.LabEquipPresetId][key].PatternId);
            
            Managers.Game.LabAwakeningDatas[Managers.Game.LabEquipPresetId][key].Grade = 
                (Grade)EditorGUILayout.IntField("등급",  (int)Managers.Game.LabAwakeningDatas[Managers.Game.LabEquipPresetId][key].Grade);
            
            Managers.Game.LabAwakeningDatas[Managers.Game.LabEquipPresetId][key].StatId = 
                EditorGUILayout.IntField("스탯",  Managers.Game.LabAwakeningDatas[Managers.Game.LabEquipPresetId][key].StatId);
            
            Managers.Game.LabAwakeningDatas[Managers.Game.LabEquipPresetId][key].StatValue = 
                EditorGUILayout.DoubleField("증가값",  (int)Managers.Game.LabAwakeningDatas[Managers.Game.LabEquipPresetId][key].StatValue);
            
            GUILayout.Space(15);
        }
        
        GUILayout.EndScrollView();
    }
}

#endif