using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace GameData
{
    public class CollectionData
    {
        public int CollectionId;

        private ObscuredInt _quantity;

        public int Quantity
        {
            get => _quantity;
            set => _quantity = value;
        }

        private ObscuredInt _lv;

        public int Lv
        {
            get => _lv;
            set => _lv = value;
        }
    }
    
    public class CollectionGameData : BaseGameData
    {
        public override string TableName => "Collection";
        protected override string InDate { get; set; }

        private string CollectionKey(int collectionId) => $"Collection_{collectionId}";
        
        protected override Param MakeInitData()
        {
            Param param = new Param();
            
            foreach (var collectionChart in ChartManager.CollectionCharts.Values)
            {
                CollectionData collectionData = new CollectionData()
                {
                    CollectionId = collectionChart.CollectionId,
                    Quantity = 0,
                    Lv = 0
                };
                
                param.Add(CollectionKey(collectionData.CollectionId), collectionData);
            }

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            foreach (var collectionData in Managers.Game.CollectionDatas.Values)
            {
                param.Add(CollectionKey(collectionData.CollectionId), collectionData);
            }

            return param;
        }

        protected override Param MakeSaveData(List<int> ids)
        {
            var param = new Param();

            ids.ForEach(id =>
            {
                var collectionKey = CollectionKey(id);

                if (param.Contains(collectionKey))
                    return;

                if (!Managers.Game.CollectionDatas.TryGetValue(id, out var collectionData))
                    return;

                param.Add(collectionKey, collectionData);
            });

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();
            
            foreach (var collectionChart in ChartManager.CollectionCharts.Values)
            {
                CollectionData collectionData;
                
                if (!jsonData.ContainsKey(CollectionKey(collectionChart.CollectionId)))
                {
                    collectionData = new CollectionData()
                    {
                        CollectionId = collectionChart.CollectionId,
                        Quantity = 0,
                        Lv = 0
                    };
                    
                    param.Add(CollectionKey(collectionData.CollectionId), collectionData);
                }
                else
                {
                    collectionData = JsonMapper.ToObject<CollectionData>(jsonData[CollectionKey(collectionChart.CollectionId)].ToJson());
                }

                if (Managers.Game.CollectionDatas.ContainsKey(collectionChart.CollectionId))
                    Managers.Game.CollectionDatas[collectionChart.CollectionId] = collectionData;
                else
                    Managers.Game.CollectionDatas.Add(collectionChart.CollectionId, collectionData);
            }
            
            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}