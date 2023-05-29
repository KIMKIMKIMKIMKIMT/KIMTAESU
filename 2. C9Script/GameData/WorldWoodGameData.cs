using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public class WorldWoodData
    {
        public int Id;

        private ObscuredInt _level;

        public int Level
        {
            get => _level;
            set => _level = value;
        }

        [JsonIgnore]
        public bool IsAcquired => Level > 0;
    }

    [Serializable]
    public class WorldWoodAwakneingData
    {
        public int StatId;
        public Grade Grade;
        public double StatValue;

        public WorldWoodAwakneingData()
        {
            StatId = 0;
            Grade = Grade.None;
            StatValue = 0;
        }
        
            

    }

    public class WorldWoodGameData : BaseGameData
    {
        public override string TableName => "Wood";

        protected override string InDate { get; set; }

        private String WoodKey(int id) => $"Wood_{id.ToString()}";
        private String WoodAwakeningKey(int awakeningId) => $"Awakening_{awakeningId.ToString()}";

        protected override Param MakeInitData()
        {
            Param param = new Param();

            WorldWoodData woodData;
            WorldWoodAwakneingData woodAwakeningData;

            woodData = new WorldWoodData()
            {
                Id = 0,
                Level = 0
            };

            param.Add("Id", 0);
            param.Add("Level", 0);

            //param.Add("Awakening_1", new WorldWoodAwakneingData());
            //param.Add("Awakening_2", new WorldWoodAwakneingData());
            //param.Add("Awakening_3", new WorldWoodAwakneingData());
            //param.Add("Awakening_4", new WorldWoodAwakneingData());
            //param.Add("Awakening_5", new WorldWoodAwakneingData());

            foreach (var data in ChartManager.WoodAwakeningDataCharts.Values)
            {
                woodAwakeningData = new WorldWoodAwakneingData();
                param.Add(WoodAwakeningKey(data.Id), JsonConvert.SerializeObject(woodAwakeningData));
                Managers.Game.WoodAwakeningDatas.Add(data.Id, woodAwakeningData);
            }

            Managers.Game.WoodsDatas.Add(0, woodData);

            

            return param;
        }


        protected override Param MakeSaveData()
        {
            Param param = new Param();

            //foreach (var woodata in Managers.Game.WoodsDatas.Values)
            //{
            //    param.Add(WoodKey(woodata.Id), woodata.ToJson());
            //}
            param.Add("Id", Managers.Game.WoodsDatas[0].Id);
            param.Add("Level", Managers.Game.WoodsDatas[0].Level);

            foreach (var data in Managers.Game.WoodAwakeningDatas)
            {
                param.Add(WoodAwakeningKey(data.Key), JsonConvert.SerializeObject(data.Value));
            }

            return param;
        }

        //protected override Param MakeSaveData(int id)
        //{
        //    Param param = new Param();

        //    if (Managers.Game.WoodsDatas.TryGetValue(id, out var woodData))
        //    {
        //        param.Add(WoodKey(woodData.Id), woodData.ToJson());
        //    }

        //    return param;
        //}

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            WorldWoodData woodData = new();
            
            
            if (!jsonData.ContainsKey("Id"))
            {
                woodData = new WorldWoodData()
                {
                    Id = 0,
                    Level = 0
                };

                param.Add("Id", woodData.Id);
                param.Add("Level", woodData.Level);
            }
            else
            {
                woodData.Id = int.Parse(jsonData["Id"].ToString());
                woodData.Level = int.Parse(jsonData["Level"].ToString());
            }

            foreach (var data in ChartManager.WoodAwakeningDataCharts.Keys)
            {
                WorldWoodAwakneingData worldWoodAwakeningData;

                if (!jsonData.ContainsKey(WoodAwakeningKey(data)))
                {
                    worldWoodAwakeningData = new WorldWoodAwakneingData();

                    param.Add(WoodAwakeningKey(data), JsonConvert.SerializeObject(worldWoodAwakeningData));
                }
                else
                {
                    worldWoodAwakeningData =
                        JsonConvert.DeserializeObject<WorldWoodAwakneingData>(jsonData[WoodAwakeningKey(data)].ToString());
                }

                if (!Managers.Game.WoodAwakeningDatas.ContainsKey(data))
                    Managers.Game.WoodAwakeningDatas.Add(data, worldWoodAwakeningData);
                else
                    Managers.Game.WoodAwakeningDatas[data] = worldWoodAwakeningData;
            }


            if (Managers.Game.WoodsDatas.Count <= 0)
            {
                Managers.Game.WoodsDatas.Add(0, woodData);
            }
            else
            {
                Managers.Game.WoodsDatas[0] = woodData;
            }

            if (param.Count > 0)
            {
                SaveGameData(param);
            }
        }


    }
}


