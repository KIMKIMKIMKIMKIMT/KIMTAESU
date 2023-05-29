using DG.Tweening;
using UniRx;
using UnityEngine;

public class Pet : MonoBehaviour
{
    [SerializeField] private SpriteRenderer PetSprite;

    private void Start()
    {
        Managers.Stage.State.Subscribe(state =>
        {
            gameObject.SetActive(state != StageState.Pvp);
        }).AddTo(gameObject);
        
        transform.DOLocalMoveY(transform.localPosition.y + 0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void SetPet(int petId)
    {
        if (!ChartManager.PetCharts.TryGetValue(petId, out var petChart))
            return;

        PetSprite.sprite = Managers.Resource.LoadPetIcon(petChart.Icon);
    }
}
