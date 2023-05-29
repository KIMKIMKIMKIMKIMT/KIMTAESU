using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Chart;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class MonsterManager
    {
        private readonly string[] GUILDSPORTS_STRING = new string[]{ "Red", "Blue" };

        private readonly List<Monster> _monsters = new();
        private Monster _bossMonster;
        public Monster[] GuildSportMonster;

        private Camera _gameCamera;

        public Monster PromoBoss;
        public Monster DpsMonster;
        public Monster WorldCupMonster;
        public Monster RaidBossMonster;
        public Monster AllRaidBossMonster;

        public readonly Subject<Monster> OnSpawnRaidBoss = new();
        public readonly Subject<Monster> OnSpawnGuildAllRaidBoss = new();

        private Camera GameCamera
        {
            get
            {
                if (_gameCamera != null)
                    return _gameCamera;

                _gameCamera = Camera.main;
                return _gameCamera;
            }
        }

        public void StartSpawn()
        {
            ClearSpawnMonster();

            switch (Managers.Stage.State.Value)
            {
                case StageState.Normal:
                    SpawnNormal();
                    break;
                case StageState.StageBoss:
                    SpawnStageBoss();
                    break;
                case StageState.Dungeon:
                    SpawnDungeon();
                    break;
                case StageState.Promo:
                    SpawnPromo();
                    break;
                case StageState.Dps:
                    SpawnDps();
                    break;
                case StageState.WorldCupEvent:
                    SpawnWorldCupEvent();
                    break;
                case StageState.Raid:
                    SpawnRaid();
                    break;
                case StageState.GuildRaid:
                    SpawnGuildRaid();
                    break;
                case StageState.GuildAllRaid:
                    SpawnAllRaid();
                    break;
                case StageState.GuildSports:
                    SpawnGuildSports();
                    break;
                    
            }
        }

        public void ClearSpawnMonster()
        {
            _monsters.ForEach(monster =>
            {
                if (monster == null)
                    return;

                Object.Destroy(monster.gameObject);
            });

            _monsters.Clear();

            if (_bossMonster != null)
                Managers.Resource.Destroy(_bossMonster.gameObject);

            if (PromoBoss != null)
                Managers.Resource.Destroy(PromoBoss.gameObject);

            if (DpsMonster != null)
                Managers.Resource.Destroy(DpsMonster.gameObject);

            if (RaidBossMonster != null)
                Managers.Resource.Destroy(RaidBossMonster.gameObject);

            if (AllRaidBossMonster != null)
                Managers.Resource.Destroy(AllRaidBossMonster.gameObject);

            if (GuildSportMonster != null)
            {
                for (int i = 0; i < GuildSportMonster.Length; i++)
                {
                    if (GuildSportMonster[i] != null)
                    {
                        Managers.Resource.Destroy(GuildSportMonster[i].gameObject);
                    }
                }
            }
            
            
                
        }


        private void SpawnNormal()
        {
            var stageChart = ChartManager.StageDataController.StageDataTable[Managers.Stage.StageId.Value];

            var arrLength = stageChart.MonsterIds.Length;

            for (var i = 0; i < arrLength; i++)
            {
                var monsterId = stageChart.MonsterIds[i];

                MonsterChart monsterChart = ChartManager.MonsterCharts[monsterId];

                for (int j = 0; j < stageChart.SpawnMonsterCounts[i]; j++)
                {
                    Monster monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);

                    if (monster == null)
                        continue;

                    monster.transform.position = GetRandomSpawnPosition();
                    monster.Init(monsterId, stageChart.MonsterHps[i], stageChart.RespawnTimes[i], 0,
                        stageChart.GoldValues[i], stageChart.ExpValues[i]);

                    _monsters.Add(monster);
                }
            }
        }

        private void SpawnStageBoss()
        {
            int id = ChartManager.StageBossDataController.StageBossTable[Managers.Stage.StageId.Value].BossId;

            var stageBossChart = ChartManager.StageBossDataController.StageBossTable[Managers.Stage.StageId.Value];

            _bossMonster = Managers.Resource.CreateMonster(ChartManager.MonsterCharts[id].PrefabName);

            if (_bossMonster == null)
            {
                Debug.LogError($"Fail Spawn Boss!!!! : {id}");
                return;
            }

            _bossMonster.transform.position = Managers.GameSystemData.StageBossSpawnPosition;
            _bossMonster.Init(id, stageBossChart.BossHp, 0, stageBossChart.BossAttack);
            _monsters.Add(_bossMonster);
        }

        private void SpawnDungeon()
        {
            if (!ChartManager.DungeonCharts.TryGetValue(Managers.Dungeon.DungeonId.Value, out var dungeonInfoChart))
                return;

            switch (dungeonInfoChart.Id)
            {
                case (int)DungeonType.Hwasengbang:
                {
                    if (!ChartManager.HwasengbangDungeonCharts.TryGetValue(Managers.Dungeon.DungeonStep.Value,
                            out var dungeonChart))
                        return;

                    switch (Managers.Dungeon.HwasengbangDungeonWave)
                    {
                        case 1:
                        {
                            for (int i = 0; i < dungeonChart.MonsterSpawnCount; i++)
                            {
                                foreach (int monsterId in dungeonChart.MonsterIds)
                                {
                                    if (!ChartManager.MonsterCharts.TryGetValue(monsterId, out var monsterChart))
                                        continue;

                                    Monster monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);

                                    if (monster == null)
                                    {
                                        Debug.LogError($"Fail CreaseMonster {monsterChart.PrefabName}");
                                        continue;
                                    }

                                    monster.transform.position = GetRandomSpawnPositionInGameView();
                                    monster.DungeonRewardId = dungeonInfoChart.RewardItemId;
                                    monster.DungeonRewardValue = dungeonChart.MonsterRewardValue;
                                    monster.Init(monsterId, dungeonChart.MonsterHp, dungeonChart.RespawnTime);
                                    _monsters.Add(monster);
                                }
                            }
                        }
                            break;
                        case 2:
                        {
                            if (!ChartManager.MonsterCharts.TryGetValue(dungeonChart.SecondMonsterId,
                                    out var monsterChart))
                            {
                                Debug.LogError($"Monster Chart Fail Load Id : {dungeonChart.SecondMonsterId}");
                                return;
                            }

                            var monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);

                            if (monster == null)
                            {
                                Debug.LogError($"Fail CreateMonster {monsterChart.PrefabName}");
                                return;
                            }

                            monster.transform.position = new Vector2(0, -6);
                            monster.DungeonRewardId = dungeonInfoChart.RewardItemId;
                            monster.DungeonRewardValue = dungeonChart.MonsterRewardValue;
                            monster.Init(dungeonChart.SecondMonsterId, dungeonChart.SecondMonsterHp);
                            _monsters.Add(monster);
                        }
                            break;
                    }
                }
                    break;
                case (int)DungeonType.MarinCamp:
                {
                    if (!ChartManager.MarinCampDungeonCharts.TryGetValue(Managers.Dungeon.DungeonStep.Value,
                            out var dungeonChart))
                        return;

                    for (int i = 0; i < dungeonChart.MonsterIds.Length; i++)
                    {
                        int monsterId = dungeonChart.MonsterIds[i];

                        if (!ChartManager.MonsterCharts.TryGetValue(monsterId, out var monsterChart))
                            continue;

                        Monster monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);

                        if (monster == null)
                        {
                            Debug.LogError($"Fail CreaseMonster {monsterChart.PrefabName}");
                            continue;
                        }

                        monster.transform.position = Managers.GameSystemData.MarinCampBossSpawnPositions[i];
                        monster.DungeonRewardId = dungeonInfoChart.RewardItemId;
                        monster.DungeonRewardValue = dungeonChart.MonsterRewardValue;
                        monster.Init(monsterId, dungeonChart.MonsterHp, 0, dungeonChart.MonsterAttack);
                        _monsters.Add(monster);
                    }
                }
                    break;
                case (int)DungeonType.March:
                {
                    if (!ChartManager.MarchDungeonCharts.TryGetValue(Managers.Dungeon.DungeonStep.Value,
                            out var dungeonChart))
                        return;

                    int wave = Managers.Dungeon.MarchDungeonWave;

                    ObscuredInt[] spawnMonsterCounts;
                    ObscuredInt[] spawnMonsterAreas;

                    switch (wave)
                    {
                        case 0:
                            spawnMonsterCounts = dungeonChart.FirstSpawnMonsterCounts;
                            spawnMonsterAreas = dungeonChart.FirstSpawnAreas;
                            break;
                        case 1:
                            spawnMonsterCounts = dungeonChart.SecondSpawnMonsterCounts;
                            spawnMonsterAreas = dungeonChart.SecondSpawnAreas;
                            break;
                        case 2:
                            spawnMonsterCounts = dungeonChart.ThirdSpawnMonsterCounts;
                            spawnMonsterAreas = dungeonChart.ThirdSpawnAreas;
                            break;
                        default:
                            return;
                    }

                    for (int i = 0; i < spawnMonsterCounts.Length; i++)
                    {
                        if (!ChartManager.MonsterCharts.TryGetValue(dungeonChart.MonsterIds[i], out var monsterChart))
                            continue;

                        Vector2 minPos = Managers.GameSystemData
                            .MarchMonsterSpawnMinPositions[spawnMonsterAreas[i] - 1];
                        Vector2 maxPos = Managers.GameSystemData
                            .MarchMonsterSpawnMaxPositions[spawnMonsterAreas[i] - 1];

                        for (int j = 0; j < spawnMonsterCounts[i]; j++)
                        {
                            float x = Random.Range(minPos.x, maxPos.x);
                            float y = Random.Range(minPos.y, maxPos.y);

                            Monster monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);

                            if (monster == null)
                            {
                                continue;
                            }

                            monster.transform.position = new Vector2(x, y);
                            monster.DungeonRewardValue = dungeonChart.MonsterRewardValues[i];
                            monster.Init(monsterChart.Id, dungeonChart.MonsterHps[i]);

                            _monsters.Add(monster);
                        }
                    }
                }
                    break;
            }
        }

        private void SpawnPromo()
        {
            int entryPromoId = Managers.Game.UserData.PromoGrade + 1;

            if (!ChartManager.PromoDungeonCharts.TryGetValue(entryPromoId,
                    out var promoDungeonChart))
            {
                Debug.LogError($"{entryPromoId} Can't Find PromoDungeonChart");
                return;
            }

            if (!ChartManager.MonsterCharts.TryGetValue(promoDungeonChart.BossId, out var monsterChart))
            {
                Debug.LogError($"{promoDungeonChart.BossId} Can't Find MonsterChart");
                return;
            }

            _bossMonster =
                Managers.Resource.CreateMonster(ChartManager.MonsterCharts[promoDungeonChart.BossId].PrefabName);

            if (_bossMonster == null)
            {
                Debug.LogError($"Fail Spawn Boss!!!! : {promoDungeonChart.BossId}");
                return;
            }

            _bossMonster.transform.position = Managers.GameSystemData.PromoBossSpawnPosition;
            _bossMonster.Init(promoDungeonChart.BossId, promoDungeonChart.BossHp);

            PromoBoss = _bossMonster;

            _monsters.Add(_bossMonster);
        }
        private void SpawnAllRaid()
        {
            
            int id = Managers.AllRaid.Step.Value;

            if (!ChartManager.AllGuildRaidDungeonCharts.TryGetValue(id, out var allRaidDungeonChart))
            {
                Debug.LogError($"{id} Can't Find AllRaidDungeonChart");
                return;
            }

            if (!ChartManager.MonsterCharts.TryGetValue(allRaidDungeonChart.MonsterId, out var monsterChart))
            {
                Debug.LogError($"{allRaidDungeonChart.MonsterId} Can't Find MonsterChart");
                return;
            }

            _bossMonster =
                Managers.Resource.CreateMonster(ChartManager.MonsterCharts[allRaidDungeonChart.MonsterId].PrefabName);

            if (_bossMonster == null)
            {
                Debug.LogError($"Fail Spawn Boss!!!! : {allRaidDungeonChart.MonsterId}");
                return;
            }

            _bossMonster.transform.position = new Vector2(30, 0);
            _bossMonster.Init(allRaidDungeonChart.MonsterId, allRaidDungeonChart.MonsterHp, -1 , allRaidDungeonChart.MonsterAtk);

            AllRaidBossMonster = _bossMonster;

            Managers.Game.MainPlayer.TargetGroupFollow.m_Targets[1].target = AllRaidBossMonster.transform;

            _monsters.Add(_bossMonster);
            OnSpawnGuildAllRaidBoss?.OnNext(AllRaidBossMonster);

            _bossMonster.State.Value = CharacterState.Appear;
            
        }

        private void SpawnGuildSports()
        {
            int monsterId = 619;

            if (!ChartManager.MonsterCharts.TryGetValue(monsterId, out var monsterChart))
            {
                Debug.LogError($"{monsterId} Can't Find MonsterChart");
                return;
            }

            GuildSportMonster = new Monster[2];

            for (int i = 0; i < GuildSportMonster.Length; i++)
            {
                GuildSportMonster[i] = Managers.Resource.CreateMonster(monsterChart.PrefabName + GUILDSPORTS_STRING[i]);
                GuildSportMonster[i].Id = monsterId;
                GuildSportMonster[i].Initialize();
                _monsters.Add(GuildSportMonster[i]);
            }

            GuildSportMonster[0].transform.position = new Vector3(0, -14, 0);
            GuildSportMonster[1].transform.position = new Vector3(0, 8, 0);

        }

        private void SpawnDps()
        {
            int monsterId = Managers.Dps.IsBoss ? 199 : 200;
            
            if (!ChartManager.MonsterCharts.TryGetValue(monsterId, out var monsterChart))
            {
                Debug.LogError($"{monsterId} Can't Find MonsterChart");
                return;
            }

            Monster monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);

            monster.transform.position = Managers.GameSystemData.DpsDungeonMonsterPosition;
            monster.Id = monsterId;

            monster.Initialize();

            monster.Direction.Value = CharacterDirection.Left;

            DpsMonster = monster;
            _monsters.Add(DpsMonster);
        }

        private void SpawnWorldCupEvent()
        {
            if (!ChartManager.MonsterCharts.TryGetValue(102, out var monsterChart))
            {
                Debug.LogError($"{102} Can't Find MonsterChart");
                return;
            }

            if (!ChartManager.WorldCupEventDungeonCharts.TryGetValue(1, out var worldCupEventDungeonChart))
            {
                Debug.LogError($"1 Can't Find WorldCupEventDungeonChart");
                return;
            }

            Monster monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);
            monster.transform.position = new Vector2(20, 0);
            monster.Id = 102;

            monster.DungeonRewardValue = worldCupEventDungeonChart.HitRewardValue;

            monster.Initialize();

            monster.State.Value = CharacterState.None;

            monster.Direction.Value = CharacterDirection.Left;
            WorldCupMonster = monster;

            _monsters.Add(monster);
        }

        public void SpawnRaid()
        {
            if (!ChartManager.RaidDungeonCharts.TryGetValue(Managers.Raid.Step.Value, out var raidDungeonChart))
            {
                Debug.LogError($"Can't Find RaidDungeonChart : {Managers.Raid.Step.Value}");
                return;
            }

            switch (Managers.Raid.Wave.Value)
            {
                case 1:
                {
                    if (!ChartManager.MonsterCharts.TryGetValue(raidDungeonChart.Wave1MonsterId, out var monsterChart))
                    {
                        Debug.LogError(
                            $"Can't Find MonsterChart By Raid_Wave1_MonsterID : {raidDungeonChart.Wave1MonsterId}");
                        return;
                    }

                    foreach (var wave1Portal in raidDungeonChart.Wave1Portals)
                    {
                        var spawnPos = Managers.GameSystemData.RaidWave1SpawnPositions[wave1Portal - 1];

                        var portalObj = Managers.Resource.Instantiate("ETC/Raid_Wave1_Portal");
                        if (portalObj == null)
                        {
                            Debug.LogError("Fail Create ETC/Raid_Wave1_Portal");
                            return;
                        }

                        portalObj.transform.position = spawnPos;

                        Managers.Sound.PlaySfxSound(SfxType.RaidPortal);

                        List<SpriteRenderer> spriteRenderers =
                            portalObj.GetComponentsInChildren<SpriteRenderer>().ToList();

                        Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
                        {
                            if (!Managers.Raid.IsProgress)
                            {
                                Managers.Resource.Destroy(portalObj);
                                return;
                            }

                            var monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);
                            _monsters.Add(monster);
                            var offset = monster.CenterPos - monster.transform.position;
                            monster.transform.position = spawnPos - (Vector2)offset;

                            DOTween.Sequence().Append(monster.transform.DOScale(0, 0))
                                    .Append(monster.transform.DOScale(monsterChart.Scale, 1)).onComplete =
                                () =>
                                {
                                    monster.Init(monsterChart.Id, raidDungeonChart.Wave1MonsterHp,
                                        raidDungeonChart.Wave1RespawnTime, raidDungeonChart.Wave1MonsterAttack);

                                    portalObj.transform.DOScale(0, 1f).onComplete = () =>
                                    {
                                        Managers.Resource.Destroy(portalObj);
                                    };
                                    spriteRenderers.ForEach(sprite => sprite.DOFade(0, 0.95f));
                                };
                        });
                    }
                }
                    break;
                case 2:
                {
                    if (!ChartManager.MonsterCharts.TryGetValue(raidDungeonChart.Wave2MonsterId, out var monsterChart))
                    {
                        Debug.LogError(
                            $"Can't Find MonsterChart By Raid_Wave2_MonsterID : {raidDungeonChart.Wave2MonsterId}");
                        return;
                    }

                    for (int i = 0; i < raidDungeonChart.Wave2Portals.Length; i++)
                    {
                        var wave2Portal = raidDungeonChart.Wave2Portals[i] - 1;
                        float x = Random.Range(Managers.GameSystemData.RaidWave2SpawnMinPositions[wave2Portal].x,
                            Managers.GameSystemData.RaidWave2SpawnMaxPositions[wave2Portal].x);
                        float y = Random.Range(Managers.GameSystemData.RaidWave2SpawnMinPositions[wave2Portal].y,
                            Managers.GameSystemData.RaidWave2SpawnMaxPositions[wave2Portal].y);

                        var spawnPos = new Vector2(x, y);

                        var portalObj = Managers.Resource.Instantiate("ETC/Raid_Wave2_Portal");
                        if (portalObj == null)
                        {
                            Debug.LogError("Fail Create ETC/Raid_Wave2_Portal");
                            return;
                        }

                        portalObj.transform.position = spawnPos;
                        Managers.Sound.PlaySfxSound(SfxType.RaidPortal);

                        List<SpriteRenderer> spriteRenderers =
                            portalObj.GetComponentsInChildren<SpriteRenderer>().ToList();

                        var waveSpawnCount = raidDungeonChart.Wave2SpawnCounts[i];

                        Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
                        {
                            if (!Managers.Raid.IsProgress)
                            {
                                Managers.Resource.Destroy(portalObj);
                                return;
                            }

                            for (int j = 0; j < waveSpawnCount; j++)
                            {
                                var monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);
                                _monsters.Add(monster);
                                var offset = monster.CenterPos - monster.transform.position;
                                monster.transform.position = spawnPos - (Vector2)offset;

                                DOTween.Sequence().Append(monster.transform.DOScale(0, 0))
                                        .Append(monster.transform.DOScale(monsterChart.Scale, 1)).onComplete =
                                    () =>
                                    {
                                        monster.Init(monsterChart.Id, raidDungeonChart.Wave2MonsterHp,
                                            raidDungeonChart.Wave2RespawnTime);

                                        spriteRenderers.ForEach(sprite => sprite.DOFade(0, 0.95f));
                                    };
                            }

                            DOTween.Sequence().AppendInterval(1f).Append(portalObj.transform.DOScale(0, 1f))
                                .onComplete = () => { Managers.Resource.Destroy(portalObj); };
                        });
                    }
                }

                    break;
                case 3:
                {
                    var spawnPos = Managers.GameSystemData.RaidWave3SpawnPosition;

                    if (!ChartManager.MonsterCharts.TryGetValue(raidDungeonChart.Wave3MonsterId, out var monsterChart))
                    {
                        Debug.LogError($"Can't Find Wave3MonsterID {raidDungeonChart.Wave3MonsterId}");
                        return;
                    }

                    var portalObj = Managers.Resource.Instantiate("ETC/Raid_Wave3_Portal");
                    if (portalObj == null)
                    {
                        Debug.LogError("Fail Create ETC/Raid_Wave3_Portal");
                        return;
                    }

                    portalObj.transform.position = spawnPos;
                    Managers.Sound.PlaySfxSound(SfxType.RaidPortal);
                    List<SpriteRenderer> spriteRenderers =
                        portalObj.GetComponentsInChildren<SpriteRenderer>().ToList();

                    Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
                    {
                        if (!Managers.Raid.IsProgress)
                        {
                            Managers.Resource.Destroy(portalObj);
                            return;
                        }

                        var monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);

                        _monsters.Add(monster);
                        RaidBossMonster = monster;
                        var offset = monster.CenterPos - monster.transform.position;
                        monster.transform.position = spawnPos - (Vector2)offset;
                        monster.Direction.Value = CharacterDirection.Left;

                        DOTween.Sequence().Append(monster.transform.DOScale(0, 0))
                                .Append(monster.transform.DOScale(monsterChart.Scale, 1)).onComplete =
                            () =>
                            {
                                monster.Init(monsterChart.Id, raidDungeonChart.Wave3MonsterHp,
                                    -1, raidDungeonChart.Wave3MonsterAttack);
                                monster.Direction.Value = CharacterDirection.Left;

                                Managers.Raid.StartWave3Timer();

                                portalObj.transform.DOScale(0, 1f).onComplete = () =>
                                {
                                    Managers.Resource.Destroy(portalObj);
                                };

                                spriteRenderers.ForEach(sprite => sprite.DOFade(0, 0.95f));
                                OnSpawnRaidBoss?.OnNext(RaidBossMonster);
                            };
                    });
                }
                    break;
            }
        }

        public void SpawnGuildRaid()
        {
            if (!ChartManager.GuildRaidDungeonCharts.TryGetValue(Managers.GuildRaid.Step.Value,
                    out var guildRaidDungeonChart))
            {
                Debug.LogError($"Can't Find GuildRaidDungeonChart : {Managers.GuildRaid.Step.Value}");
                return;
            }

            switch (Managers.GuildRaid.Wave.Value)
            {
                case 1:
                {
                    if (!ChartManager.MonsterCharts.TryGetValue(guildRaidDungeonChart.Wave1MonsterId,
                            out var monsterChart))
                    {
                        Debug.LogError(
                            $"Can't Find MonsterChart By Raid_Wave1_MonsterID : {guildRaidDungeonChart.Wave1MonsterId}");
                        return;
                    }

                    foreach (var wave1Portal in guildRaidDungeonChart.Wave1Portals)
                    {
                        var spawnPos = Managers.GameSystemData.RaidWave1SpawnPositions[wave1Portal - 1];

                        var portalObj = Managers.Resource.Instantiate("ETC/Raid_Wave1_Portal");
                        if (portalObj == null)
                        {
                            Debug.LogError("Fail Create ETC/Raid_Wave1_Portal");
                            return;
                        }

                        portalObj.transform.position = spawnPos;

                        Managers.Sound.PlaySfxSound(SfxType.RaidPortal);

                        List<SpriteRenderer> spriteRenderers =
                            portalObj.GetComponentsInChildren<SpriteRenderer>().ToList();

                        Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
                        {
                            if (!Managers.GuildRaid.IsProgress)
                            {
                                Managers.Resource.Destroy(portalObj);
                                return;
                            }

                            var monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);
                            _monsters.Add(monster);
                            var offset = monster.CenterPos - monster.transform.position;
                            monster.transform.position = spawnPos - (Vector2)offset;

                            DOTween.Sequence().Append(monster.transform.DOScale(0, 0))
                                    .Append(monster.transform.DOScale(monsterChart.Scale, 1)).onComplete =
                                () =>
                                {
                                    monster.Init(monsterChart.Id, guildRaidDungeonChart.Wave1MonsterHp,
                                        guildRaidDungeonChart.Wave1Respawn, guildRaidDungeonChart.Wave1MonsterAttack);

                                    portalObj.transform.DOScale(0, 1f).onComplete = () =>
                                    {
                                        Managers.Resource.Destroy(portalObj);
                                    };
                                    spriteRenderers.ForEach(sprite => sprite.DOFade(0, 0.95f));
                                };
                        });
                    }
                }
                    break;
                case 2:
                {
                    if (!ChartManager.MonsterCharts.TryGetValue(guildRaidDungeonChart.Wave2MonsterId,
                            out var monsterChart))
                    {
                        Debug.LogError(
                            $"Can't Find MonsterChart By Raid_Wave2_MonsterID : {guildRaidDungeonChart.Wave2MonsterId}");
                        return;
                    }

                    for (int i = 0; i < guildRaidDungeonChart.Wave2Portals.Length; i++)
                    {
                        var wave2Portal = guildRaidDungeonChart.Wave2Portals[i] - 1;
                        float x = Random.Range(Managers.GameSystemData.RaidWave2SpawnMinPositions[wave2Portal].x,
                            Managers.GameSystemData.RaidWave2SpawnMaxPositions[wave2Portal].x);
                        float y = Random.Range(Managers.GameSystemData.RaidWave2SpawnMinPositions[wave2Portal].y,
                            Managers.GameSystemData.RaidWave2SpawnMaxPositions[wave2Portal].y);

                        var spawnPos = new Vector2(x, y);

                        var portalObj = Managers.Resource.Instantiate("ETC/Raid_Wave2_Portal");
                        if (portalObj == null)
                        {
                            Debug.LogError("Fail Create ETC/Raid_Wave2_Portal");
                            return;
                        }

                        portalObj.transform.position = spawnPos;
                        Managers.Sound.PlaySfxSound(SfxType.RaidPortal);

                        List<SpriteRenderer> spriteRenderers =
                            portalObj.GetComponentsInChildren<SpriteRenderer>().ToList();

                        var waveSpawnCount = guildRaidDungeonChart.Wave2SpawnCounts[i];

                        Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
                        {
                            if (!Managers.GuildRaid.IsProgress)
                            {
                                Managers.Resource.Destroy(portalObj);
                                return;
                            }

                            for (int j = 0; j < waveSpawnCount; j++)
                            {
                                var monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);
                                _monsters.Add(monster);
                                var offset = monster.CenterPos - monster.transform.position;
                                monster.transform.position = spawnPos - (Vector2)offset;

                                DOTween.Sequence().Append(monster.transform.DOScale(0, 0))
                                        .Append(monster.transform.DOScale(monsterChart.Scale, 1)).onComplete =
                                    () =>
                                    {
                                        monster.Init(monsterChart.Id, guildRaidDungeonChart.Wave2MonsterHp,
                                            guildRaidDungeonChart.Wave2Respawn);

                                        spriteRenderers.ForEach(sprite => sprite.DOFade(0, 0.95f));
                                    };
                            }

                            DOTween.Sequence().AppendInterval(1f).Append(portalObj.transform.DOScale(0, 1f))
                                .onComplete = () => { Managers.Resource.Destroy(portalObj); };
                        });
                    }
                }

                    break;
                case 3:
                {
                    var spawnPos = Managers.GameSystemData.RaidWave3SpawnPosition;

                    if (!ChartManager.MonsterCharts.TryGetValue(guildRaidDungeonChart.Wave3MonsterId,
                            out var monsterChart))
                    {
                        Debug.LogError($"Can't Find Wave3MonsterID {guildRaidDungeonChart.Wave3MonsterId}");
                        return;
                    }

                    var portalObj = Managers.Resource.Instantiate("ETC/Raid_Wave3_Portal");
                    if (portalObj == null)
                    {
                        Debug.LogError("Fail Create ETC/Raid_Wave3_Portal");
                        return;
                    }

                    portalObj.transform.position = spawnPos;
                    Managers.Sound.PlaySfxSound(SfxType.RaidPortal);
                    List<SpriteRenderer> spriteRenderers =
                        portalObj.GetComponentsInChildren<SpriteRenderer>().ToList();

                    Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
                    {
                        if (!Managers.GuildRaid.IsProgress)
                        {
                            Managers.Resource.Destroy(portalObj);
                            return;
                        }

                        var monster = Managers.Resource.CreateMonster(monsterChart.PrefabName);

                        _monsters.Add(monster);
                        RaidBossMonster = monster;
                        var offset = monster.CenterPos - monster.transform.position;
                        monster.transform.position = spawnPos - (Vector2)offset;
                        monster.Direction.Value = CharacterDirection.Left;

                        DOTween.Sequence().Append(monster.transform.DOScale(0, 0))
                                .Append(monster.transform.DOScale(monsterChart.Scale, 1)).onComplete =
                            () =>
                            {
                                monster.Init(monsterChart.Id, guildRaidDungeonChart.Wave3MonsterHp,
                                    -1, guildRaidDungeonChart.Wave3MonsterAttack);
                                monster.Direction.Value = CharacterDirection.Left;

                                Managers.GuildRaid.StartWave3Timer();

                                portalObj.transform.DOScale(0, 1f).onComplete = () =>
                                {
                                    Managers.Resource.Destroy(portalObj);
                                };

                                spriteRenderers.ForEach(sprite => sprite.DOFade(0, 0.95f));
                                OnSpawnRaidBoss?.OnNext(RaidBossMonster);
                            };
                    });
                }
                    break;
            }
        }

        public Vector2 GetRandomSpawnPosition()
        {
            float posX = Random.Range(Managers.GameSystemData.MinSpawnPosition.x,
                Managers.GameSystemData.MaxSpawnPosition.x);
            float posY = Random.Range(Managers.GameSystemData.MinSpawnPosition.y,
                Managers.GameSystemData.MaxSpawnPosition.y);

            return new Vector2(posX, posY);
        }

        public Vector2 GetRandomSpawnPositionInGameView()
        {
            var minPos = GameCamera.ViewportToWorldPoint(GameCamera.rect.min);
            var maxPos = GameCamera.ViewportToWorldPoint(GameCamera.rect.max);

            if (minPos.x < Managers.GameSystemData.MinSpawnPosition.x)
                minPos.x = Managers.GameSystemData.MinSpawnPosition.x;

            if (minPos.y < Managers.GameSystemData.MinSpawnPosition.y)
                minPos.y = Managers.GameSystemData.MinSpawnPosition.y;

            if (maxPos.x > Managers.GameSystemData.MaxSpawnPosition.x)
                maxPos.x = Managers.GameSystemData.MaxSpawnPosition.x;

            if (maxPos.y > Managers.GameSystemData.MaxSpawnPosition.y)
                maxPos.y = Managers.GameSystemData.MaxSpawnPosition.y;

            return new Vector2(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y));
        }

        public Monster FindTargetMonster(Vector3 pos)
        {
            Monster targetMonster = null;
            float minDistance = float.MaxValue;

            _monsters.ForEach(monster =>
            {
                if (monster == null)
                    return;

                if (monster.IsDead)
                    return;

                float distance = Vector3.Distance(pos, monster.CenterPos);
                if (distance >= minDistance)
                    return;

                targetMonster = monster;
                minDistance = distance;
            });

            return targetMonster;
        }

        public List<Monster> FindTargetMonsters(Vector3 pos, CharacterDirection characterDirection, float attackRange)
        {
            List<Monster> targetMonsters = new List<Monster>();

            _monsters.ForEach(monster =>
            {
                if (monster == null)
                    return;

                if (monster.IsDead)
                    return;

                Vector3 direction = monster.CenterPos - pos;

                switch (characterDirection)
                {
                    case CharacterDirection.Left:
                    {
                        if (direction.x >= 0)
                            return;
                        break;
                    }
                    case CharacterDirection.Right:
                    {
                        if (direction.x < 0)
                            return;
                        break;
                    }
                }

                float distance = Vector3.Distance(pos, monster.CenterPos);
                if (distance <= attackRange)
                    targetMonsters.Add(monster);
            });

            return targetMonsters;
        }

        public List<Monster> FindTargetMonsters(Vector3 pos, float attackRange)
        {
            List<Monster> targetMonsters = _monsters.FindAll(monster =>
            {
                if (monster == null)
                    return false;

                if (!monster.gameObject.activeSelf)
                    return false;

                if (monster.IsDead)
                    return false;

                if (Vector3.Distance(pos, monster.CenterPos) > attackRange)
                    return false;

                return true;
            });

            return targetMonsters;
        }

        public Monster FindRandomMonsterInGameView()
        {
            var monsters = _monsters.FindAll(monster =>
            {
                if (monster == null)
                    return false;

                if (monster.IsDead)
                    return false;

                Vector3 screenPos = GameCamera.WorldToViewportPoint(monster.CenterPos);

                if (screenPos.x > 0 && screenPos.x < 1 &&
                    screenPos.y > 0 && screenPos.y < 1)
                    return true;

                return false;
            });

            return monsters.Count <= 0 ? null : monsters[Random.Range(0, monsters.Count)];
        }

        public Monster FindRandomMonster()
        {
            var monsters = _monsters.FindAll(monster =>
            {
                if (monster == null)
                    return false;

                if (monster.IsDead)
                    return false;

                return true;
            });

            return monsters.Count <= 0 ? null : monsters[Random.Range(0, monsters.Count)];
        }

        public List<Monster> FindMonstersInGameView()
        {
            List<Monster> monsters = new List<Monster>();

            _monsters.ForEach(monster =>
            {
                if (monster == null)
                    return;

                if (monster.IsDead)
                    return;

                Vector3 screenPos = GameCamera.WorldToViewportPoint(monster.CenterPos);

                if (screenPos.x > 0 && screenPos.x < 1 &&
                    screenPos.y > 0 && screenPos.y < 1)
                    monsters.Add(monster);
            });

            return monsters;
        }

        public void SetAllMonsterStateNone()
        {
            SetAllMonsterState(CharacterState.None);
        }

        public void SetAllMonsterState(CharacterState state)
        {
            _monsters.ForEach(monster =>
            {
                if (monster == null)
                    return;

                monster.State.Value = state;
            });

            if (_bossMonster != null)
                _bossMonster.State.Value = state;
        }

        public bool IsAllDeadMonster()
        {
            bool isAllDead = true;

            _monsters.ForEach(monster =>
            {
                if (monster == null)
                    return;

                if (!monster.IsDead)
                    isAllDead = false;
            });

            return isAllDead;
        }

        public void AllKillMonster()
        {
            _monsters.ForEach(monster =>
            {
                if (monster == null)
                    return;

                monster.Damage(monster.Hp.Value);
            });
        }
    }
}