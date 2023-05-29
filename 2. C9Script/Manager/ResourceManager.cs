using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ResourceManager
{
    #region Path

    private const string GoodsIconAtlasPath = "Atlas/Item";
    private const string WeaponIconAtlasPath = "Atlas/Weapon";
    private const string SkillIconAtlasPath = "Atlas/Skill";
    private const string CostumeIconAtlasPath = "Atlas/Costume";
    private const string PetIconAtlasPath = "Atlas/Pet";
    private const string RankIconAtlasPath = "Atlas/Rank";
    private const string PromoIconAtlasPath = "Atlas/Promo";
    private const string MonsterIconAtlasPath = "Atlas/Monster";
    private const string PopupAtlasPath = "Atlas/Popup";
    private const string CollectionAtlasPath = "Atlas/Collection";
    private const string StatAtlasPath = "Atlas/Stat";
    private const string ShopAtlasPath = "Atlas/Shop";
    private const string GuildMarkAtlasPath = "Atlas/GuildMark";
    private const string LabAtlasPath = "Atlas/Lab";

    #endregion

    private readonly Dictionary<string, Sprite> _sprites = new();
    private readonly Dictionary<string, GameObject> _gameObjects = new();
    private readonly Dictionary<string, RuntimeAnimatorController> _characterAnimators = new();
    private readonly Dictionary<string, SpriteAtlas> _atlas = new();
    private readonly Dictionary<string, AudioClip> _audioClips = new();

    public T Load<T>(string path) where T : Object
    {
        var type = typeof(T);

        if (type == typeof(Sprite))
        {
            if (_sprites.TryGetValue(path, out Sprite sprite))
                return sprite as T;

            Sprite sp = Resources.Load<Sprite>(path);

            _sprites.Add(path, sp);
            return sp as T;
        }

        if (type == typeof(GameObject))
        {
            if (_gameObjects.TryGetValue(path, out GameObject obj))
                return obj as T;

            obj = Resources.Load<GameObject>(path);

            _gameObjects.Add(path, obj);

            return obj as T;
        }

        if (type == typeof(RuntimeAnimatorController))
        {
            if (_characterAnimators.TryGetValue(path, out RuntimeAnimatorController animator))
                return animator as T;

            animator = Resources.Load<RuntimeAnimatorController>(path);
            _characterAnimators.Add(path, animator);
            return animator as T;
        }

        if (type == typeof(AudioClip))
        {
            if (_audioClips.TryGetValue(path, out AudioClip audioClip))
                return audioClip as T;

            audioClip = Resources.Load<AudioClip>(path);
            _audioClips.Add(path, audioClip);
            return audioClip as T;
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        return Instantiate(prefab, parent);
    }

    public GameObject Instantiate(GameObject prefab, Transform parent = null)
    {
        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }

    public Monster CreateMonster(string name)
    {
        GameObject go = Instantiate($"Monster/{name}");
        if (go == null)
            return null;

        Monster monster = go.GetComponent<Monster>();
        return monster;
    }

    public Sprite LoadGoodsIcon(string name)
    {
        return LoadSpriteByAtlas(GoodsIconAtlasPath, name);
    }

    public Sprite LoadWeaponIcon(string name)
    {
        return LoadSpriteByAtlas(WeaponIconAtlasPath, name);
    }

    public Sprite LoadSkillIcon(string name)
    {
        return LoadSpriteByAtlas(SkillIconAtlasPath, name);
    }

    public Sprite LoadCostumeIcon(string name)
    {
        return LoadSpriteByAtlas(CostumeIconAtlasPath, name);
    }

    public Sprite LoadPetIcon(string name)
    {
        return LoadSpriteByAtlas(PetIconAtlasPath, name);
    }

    public Sprite LoadCollectionIcon(string name)
    {
        return LoadSpriteByAtlas(CollectionAtlasPath, name);
    }

    public Sprite LoadStatIcon(string name)
    {
        return LoadSpriteByAtlas(StatAtlasPath, name);
    }

    public Sprite LoadShopIcon(string name)
    {
        return LoadSpriteByAtlas(ShopAtlasPath, name);
    }
    
    public Sprite LoadGuildMarkIcon(int guildMarkId)
    {
        return LoadSpriteByAtlas(GuildMarkAtlasPath, $"School_Icon_{guildMarkId:000}");
    }

    public Sprite LoadGuildGradeIcon(int guildGrade)
    {
        return LoadSpriteByAtlas(GuildMarkAtlasPath, $"School_Grade_{guildGrade:000}");
    }

    public Sprite LoadLabIcon(string name)
    {
        return LoadSpriteByAtlas(LabAtlasPath, name);
    }

    public Sprite LoadItemIcon(ItemType itemType, int id)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
            {
                if (!ChartManager.WeaponCharts.TryGetValue(id, out var weaponChart))
                    return null;

                return LoadWeaponIcon(weaponChart.Icon);
            }
            case ItemType.Goods:
            {
                if (!ChartManager.GoodsCharts.TryGetValue(id, out var goodsChart))
                    return null;

                return LoadGoodsIcon(goodsChart.Icon);
            }
            case ItemType.Collection:
            {
                if (!ChartManager.CollectionCharts.TryGetValue(id, out var collectionChart))
                    return null;

                return LoadCollectionIcon(collectionChart.Icon);
            }
            case ItemType.Costume:
            {
                if (!ChartManager.CostumeCharts.TryGetValue(id, out var costumeChart))
                    return null;

                return LoadCostumeIcon(costumeChart.Icon);
            }
            case ItemType.Pet:
            {
                if (!ChartManager.PetCharts.TryGetValue(id, out var petChart))
                    return null;

                return LoadPetIcon(petChart.Icon);
            }
        }

        return null;
    }

    public Sprite LoadRankIcon(int rank)
    {
        switch (rank)
        {
            case 1:
                return LoadSpriteByAtlas(RankIconAtlasPath, "1st");
            case 2:
                return LoadSpriteByAtlas(RankIconAtlasPath, "2nd");
            case 3:
                return LoadSpriteByAtlas(RankIconAtlasPath, "3rd");
            default:
            {
                return rank switch
                {
                    >= 4 and <= 10 => LoadSpriteByAtlas(RankIconAtlasPath, "4~10_Rank"),
                    >= 11 and <= 50 => LoadSpriteByAtlas(RankIconAtlasPath, "11~50_Rank"),
                    >= 51 and <= 100 => LoadSpriteByAtlas(RankIconAtlasPath, "51~100_Rank"),
                    >= 101 and <= 200 => LoadSpriteByAtlas(RankIconAtlasPath, "101~200_Rank"),
                    >= 201 and <= 400 => LoadSpriteByAtlas(RankIconAtlasPath, "201~400_Rank"),
                    _ => LoadSpriteByAtlas(RankIconAtlasPath, "Default_Rank")
                };
            }
        }
    }

    private Sprite LoadSpriteByAtlas(string atlasName, string spriteName)
    {
        if (_atlas.TryGetValue(atlasName, out var atlas))
            return atlas == null ? null : atlas.GetSprite(spriteName);

        atlas = Load<SpriteAtlas>(atlasName);
        _atlas.Add(atlasName, atlas);

        return atlas == null ? null : atlas.GetSprite(spriteName);
    }

    public Sprite LoadMonsterIcon(int monsterId)
    {
        return LoadSpriteByAtlas(MonsterIconAtlasPath, ChartManager.MonsterCharts[monsterId].PrefabName);
    }

    public Sprite LoadPromoIcon(int promoId)
    {
        return ChartManager.PromoDungeonCharts.TryGetValue(promoId, out var promoDungeonChart) ?
            LoadSpriteByAtlas(PromoIconAtlasPath, promoDungeonChart.Icon) : 
            LoadSpriteByAtlas(PromoIconAtlasPath, "Chulwadae_Grade_000");
    }

    public Sprite LoadBg(int worldId)
    {
        if (ChartManager.WorldCharts.TryGetValue(worldId, out var worldChart))
        {
            if (!_sprites.ContainsKey($"Texture/Background/{worldChart.BgName}"))
            {
                _sprites.Add($"Texture/Background/{worldChart.BgName}",
                    Resources.Load<Sprite>($"Texture/Background/{worldChart.BgName}"));
            }

            return _sprites[$"Texture/Background/{worldChart.BgName}"];
        }

        return null;
    }

    public Sprite LoadBg(string bgName)
    {
        if (!_sprites.ContainsKey($"Texture/Background/{bgName}"))
        {
            _sprites.Add($"Texture/Background/{bgName}",
                Resources.Load<Sprite>($"Texture/Background/{bgName}"));
        }

        return _sprites[$"Texture/Background/{bgName}"];
    }

    public Sprite LoadItemGradeBg(Grade grade)
    {
        return LoadSpriteByAtlas(PopupAtlasPath, $"Popup_Bg_001_{grade.ToString()}");
    }

    public Sprite LoadBackGroundIconImage(Grade grade)
    {
        return LoadSpriteByAtlas(PopupAtlasPath, $"Popup_Bg2_001_{grade.ToString()}");
    }

    public Sprite LoadSkillSlot(Grade grade)
    {
        return LoadSpriteByAtlas(SkillIconAtlasPath, $"Skill_Bg_{grade.ToString()}");
    }

    public Sprite LoadSpriteInPopupAtlas(string path)
    {
        return LoadSpriteByAtlas(PopupAtlasPath, path);
    }

    public Sprite LoadSkillSlotBg(Grade grade)
    {
        return LoadSpriteByAtlas(SkillIconAtlasPath, $"Skill_Bg_{grade.ToString()}_Back");
    }

    public Sprite LoadCriticalDamageEffect(double criticalMultiple)
    {
        switch (criticalMultiple)
        {
            case 2:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_001");
            case 4:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_002");
            case 8:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_003");
            case 16:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_004");
            case 32:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_005");
            case 64:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_006");
            case 128:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_007");
            case 256:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_008");
            case 512:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_009");
            case 1024:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_010");
            case 2048:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_011");
            case 4096:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_012");
            case 5009:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_013");
            case 6009:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_014");
            case 7009:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_015");
            case 8009:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_016");
            case 9009:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_017");
            case 10009:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_018");
            case 11009:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_019");
            case 12009:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_020");
            case 13009:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_021");
            case 14009:
                return Load<Sprite>("Texture/CriticalDamageEffect/Critical_Grade_022");
            default:
                return null;
        }
    }

    public Sprite LoadSubGradeIcon(ItemType itemType, int itemId)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
            {
                if (!ChartManager.WeaponCharts.TryGetValue(itemId, out var weaponChart))
                    return null;

                return LoadSubGradeIcon(weaponChart.SubGrade);
            }
            case ItemType.Pet:
            {
                if (!ChartManager.PetCharts.TryGetValue(itemId, out var petChart))
                    return null;

                return LoadSubGradeIcon(petChart.SubGrade);
            }
            default:
                return null;
        }
    }

    public Sprite LoadSubGradeIcon(int subGrade)
    {
        return LoadSpriteByAtlas(PopupAtlasPath, $"SubGrade_{subGrade}");
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }

    public void Destroy(Player player)
    {
        if (player == null)
        {
            return;
        }

        Object.Destroy(player);
    }
}