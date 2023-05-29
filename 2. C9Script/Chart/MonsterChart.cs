using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class MonsterChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString Name;
        public MonsterType Type;
        public ObscuredFloat Scale;
        public ObscuredFloat MoveSpeed;
        public ObscuredInt DetectionType;
        public ObscuredString PrefabName;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Monster_Id"].ToString(), out int id);
            Id = id;
            
            Name = jsonData["Monster_Name"].ToString();
            Enum.TryParse(jsonData["Monster_Type"].ToString(), out Type);
            
            float.TryParse(jsonData["Monster_Scale"].ToString(), out float scale);
            Scale = scale;
            
            float.TryParse(jsonData["Monster_Move_Speed"].ToString(), out float moveSpeed);
            MoveSpeed = moveSpeed;
            
            int.TryParse(jsonData["Monster_Detection_Type"].ToString(), out int detectionType);
            DetectionType = detectionType;
            
            PrefabName = jsonData["Res_Name"].ToString();

            ChartManager.MonsterCharts[GetID()] = this;
        }
    }
}