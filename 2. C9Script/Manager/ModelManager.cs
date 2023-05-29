using GameData;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    public static ModelManager Instance;

    [SerializeField] public PlayerModel Ranker1Model;
    [SerializeField] public PlayerModel Ranker2Model;
    [SerializeField] public PlayerModel Ranker3Model;
    [SerializeField] public PlayerModel PlayerModel;
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void ResetPlayerModel()
    {
        PlayerModel.SetCostume(Managers.Game.EquipDatas[EquipType.ShowCostume]);
        PlayerModel.SetWeapon(Managers.Game.EquipDatas[EquipType.Weapon]);
    }

    public void SetAllRankerModelAnimation(string animationName)
    {
        Ranker1Model.SetAnimation(animationName);
        Ranker2Model.SetAnimation(animationName);
        Ranker3Model.SetAnimation(animationName);
    }
}