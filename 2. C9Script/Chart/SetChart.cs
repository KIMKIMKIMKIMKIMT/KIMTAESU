using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class SetChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString Name;
        public ObscuredInt WeaponId;
        public ObscuredInt PetId;
        public ObscuredInt CostumeId;
        public ObscuredInt StatType1;
        public ObscuredDouble StatValue1;
        public ObscuredInt StatType2;
        public ObscuredDouble StatValue2;
        public ObscuredInt StatType3;
        public ObscuredDouble StatValue3;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Set_Id"].ToString(), out int id);
            Id = id;
            
            Name = jsonData["Set_Name"].ToString();
            
            int.TryParse(jsonData["Weapon_Id"].ToString(), out int weaponId);
            WeaponId = weaponId;
            
            int.TryParse(jsonData["Pet_Id"].ToString(), out int petId);
            PetId = petId;
            
            int.TryParse(jsonData["Costume_Id"].ToString(), out int costumeId);
            CostumeId = costumeId;
            
            int.TryParse(jsonData["Stat_Type_1"].ToString(), out int statType1);
            StatType1 = statType1;
            
            double.TryParse(jsonData["Stat_Type_1_Value"].ToString(), out double statValue1);
            StatValue1 = statValue1;
            
            int.TryParse(jsonData["Stat_Type_2"].ToString(), out int statType2);
            StatType2 = statType2;
            
            double.TryParse(jsonData["Stat_Type_2_Value"].ToString(), out double statValue2);
            StatValue2 = statValue2;
            
            int.TryParse(jsonData["Stat_Type_3"].ToString(), out int statType3);
            StatType3 = statType3;
            
            double.TryParse(jsonData["Stat_Type_3_Value"].ToString(), out double statValue3);
            StatValue3 = statValue3;

            ChartManager.SetCharts[GetID()] = this;
        }
    }
}