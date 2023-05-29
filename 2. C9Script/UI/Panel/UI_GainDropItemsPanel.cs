using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UniRx;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class UI_GainDropItemsPanel : UI_Panel
{
    public record GainDropItemData
    {
        public int GoodsId1;
        public double GoodsValue1;
        
        public int GoodsId2;
        public double GoodsValue2;

        public GainDropItemData(int goodsId1, double goodsValue1)
        {
            GoodsId1 = goodsId1;
            GoodsValue1 = goodsValue1;
        }
        
        public GainDropItemData(int goodsId1, double goodsValue1, int goodsId2, double goodsValue2)
        {
            GoodsId1 = goodsId1;
            GoodsValue1 = goodsValue1;
            GoodsId2 = goodsId2;
            GoodsValue2 = goodsValue2;
        }
    }
    
    [SerializeField] private Transform UIGainDropItemRoot;

    private const int MaxItemCount = 5;
    private readonly Queue<UI_GainDropItem> _uiGainDropItems = new();
    private readonly Queue<GainDropItemData> _gainDropItemDatas = new();

    private void Start()
    {
        UIGainDropItemRoot.DestroyInChildren();
        
        MessageBroker.Default.Receive<GainDropItemData>().Subscribe(_ =>
        {
            UpdateGainDropItems(_);
            //_gainDropItemDatas.Enqueue(_);
        });
    }

    private void Update()
    {
        if (_gainDropItemDatas.Count <= 0)
            return;
        
        UpdateGainDropItems(_gainDropItemDatas.Dequeue());
    }

    private void UpdateGainDropItems(GainDropItemData gainDropItemData)
    {
        UI_GainDropItem uiGainDropItem;

        if (_uiGainDropItems.Count < MaxItemCount)
        {
            uiGainDropItem = Managers.UI.MakeSubItem<UI_GainDropItem>(UIGainDropItemRoot);
        }
        else
            uiGainDropItem = _uiGainDropItems.Dequeue();
        
        _uiGainDropItems.Enqueue(uiGainDropItem);
        uiGainDropItem.transform.localScale = Vector3.one;
        uiGainDropItem.transform.SetAsLastSibling();
        uiGainDropItem.Set(gainDropItemData);

        for (int i = 0; i < _uiGainDropItems.Count; i++)
        {
            int siblingIndex = _uiGainDropItems.ElementAt(i).transform.GetSiblingIndex();

            int value = _uiGainDropItems.Count - 1 - siblingIndex;
            Vector3 scale = new Vector3(1 - value * 0.1f, 1 - value * 0.1f, 1 - value * 0.1f);

            if (_uiGainDropItems.ElementAt(i) != uiGainDropItem)
            {
                _uiGainDropItems.ElementAt(i).transform.localScale = scale;
                _uiGainDropItems.ElementAt(i).SetFadeSequence();
            }

            // var lerp = (MaxItemCount - (MaxItemCount - siblingIndex)) / (float)MaxItemCount;
            // var scale = Mathf.Lerp(1f, 0.5f, lerp);
            // _uiGainDropItems.ElementAt(i).transform.localScale = scale;
            //
            // var delay = Mathf.Lerp(1f, 0.5f, lerp);
            // var alpha = Mathf.Lerp(1f, 0.5f, lerp);
            // _uiGainDropItems.ElementAt(i).FadeOut(delay, alpha);
        }
    }
}