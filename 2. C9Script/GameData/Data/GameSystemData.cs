using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MonsterSpawnData", menuName = "Scriptable Object/Monster Spawn Data", order = int.MaxValue)]
public class GameSystemData : ScriptableObject
{
    [Header("기본 카메라 사이즈")] public float BaseCameraSize;
    [Header("피버모드 카메라 사이즈")] public float FeverCameraSize;
    
    [Space(30)]
    
    [Header("몬스터 스폰 최소 위치")] public Vector2 MinSpawnPosition;
    [Header("몬스터 스폰 최대 위치")] public Vector2 MaxSpawnPosition;
    
    [Space(30)]

    [Header("스테이지 보스 플레이어 시작 위치")] public Vector2 StageBossPlayerPosition;
    [Header("스테이지 보스 스폰 위치")] public Vector2 StageBossSpawnPosition;

    [Space(30)] 
    
    [Header("재입대 던전 카메라 사이즈")] public float MilitaryDungeonCameraSize;
    
    [Space(30)]
    [Header("승급전 보스 플레이어 시작 위치")] public Vector2 PromoBossPlayerPosition;
    [Header("승급전 보스 스폰 위치")] public Vector2 PromoBossSpawnPosition;
    [Header("승급전 카메라 시작 값")] public float PromoCameraSize;


    [Space(30)] [Header("해병대 캠프 몬스터 스폰 위치")]
    public Vector2[] MarinCampBossSpawnPositions;

    [Space(30)]
    [Header("행군 카메라 사이즈")] public float MarchCameraSize;
    [Header("행군 플레이어 스폰 위치")] public Vector2 MarchPlayerSpawnPosition;
    [Header("행군 몬스터 스폰 최소 위치")] public Vector2[] MarchMonsterSpawnMinPositions;
    [Header("행군 몬스터 스폰 최대 위치")] public Vector2[] MarchMonsterSpawnMaxPositions;

    [Space(30)] [Header("DPS 던전 플레이어 위치")]
    public Vector2 DpsDungeonPlayerPosition;
    [Header("DPS 던전 몬스터 위치")]
    public Vector2 DpsDungeonMonsterPosition;

    [Header("Worldcup 몬스터 스폰 위치")] 
    public Vector2 WorldcupMonsterPosition;

    [Header("레이드 1웨이브 스폰 위치")] 
    public List<Vector2> RaidWave1SpawnPositions;
    
    [Header("레이드 2웨이브 최소 스폰 위치")]
    public List<Vector2> RaidWave2SpawnMinPositions;
    
    [Header("레이드 2웨이브 최대 스폰 위치")]
    public List<Vector2> RaidWave2SpawnMaxPositions;
    
    [Header("레이드 3웨이브 스폰 위치")]
    public Vector2 RaidWave3SpawnPosition;

    [Header("몬스터 DetectionType 4번 이동 위치")] 
    public List<Vector2> MonsterRunDestinations;
}