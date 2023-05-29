using System.Collections.Generic;
using System.Linq;
using UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class UIManager
{
    private int _orderIndex = 1;
    private int _topOrderIndex = 51;

    //private readonly Stack<UI_Popup> _popups = new();
    private readonly List<UI_Popup> _popups = new();
    private readonly Dictionary<string, GameObject> _uiDict = new();

    private GameObject _root;

    public int PopupCount => _popups.Count;

    private GameObject Root
    {
        get
        {
            if (_root != null)
                return _root;

            _root = GameObject.Find("@UI_Root");
            if (_root == null)
                _root = new GameObject { name = "@UI_Root" };

            return _root;
        }
    }

    private Camera _uiCamera;

    private Camera UICamera
    {
        get
        {
            if (_uiCamera != null)
                return _uiCamera;

            _uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
            return _uiCamera;
        }
    }

    public void SetCanvas(UI_Base uiBase, bool sort = true)
    {
        Canvas canvas = uiBase.GetComponent<Canvas>();

        if (canvas == null)
            return;

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.overrideSorting = true;
        canvas.worldCamera = UICamera;
        canvas.sortingLayerName = "UI";

        if (uiBase.isTop)
            canvas.sortingOrder = sort ? _topOrderIndex++ : 0;
        else
            canvas.sortingOrder = sort ? _orderIndex++ : 0;
    }

    public void SetScene(UI_Scene uiScene)
    {
        uiScene.transform.SetParent(Root.transform);
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/Item/{name}");

        if (prefab == null)
        {
            Debug.LogError($"Null Item {name}");
        }

        GameObject go = Managers.Resource.Instantiate(prefab);
        if (parent != null)
            go.transform.SetParent(parent);

        go.transform.localScale = Vector3.one;
        go.transform.localPosition = prefab.transform.position;

        return go.GetComponent<T>();
    }

    public void ShowPowerSave()
    {
        GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/UI_PowerSavePopup");
        if (prefab == null)
            return;

        Managers.Resource.Instantiate(prefab);
    }

    public T ShowPopupUI<T>(string name = null, Transform parent = null, bool isCheckServer = true) where T : UI_Popup
    {
        #if !UNITY_EDITOR
        if (isCheckServer)
            Managers.Server.CheckServerStatus();
        #endif
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        T popup;

        if (!_uiDict.TryGetValue(name, out GameObject go))
        {
            GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{name}");

            go = Managers.Resource.Instantiate($"UI/Popup/{name}");
            
            go.OnEnableAsObservable().Subscribe(_ =>
            {
                MessageBroker.Default.Publish(new OpenPopupMessage<T>());
            });

            go.OnDisableAsObservable().Subscribe(_ =>
            {
                MessageBroker.Default.Publish(new ClosePopupMessage<T>());
            });

            popup = go.GetComponent<T>();

            _popups.Add(popup);

            go.transform.SetParent(parent != null ? parent : Root.transform);

            go.transform.localScale = Vector3.one;
            go.transform.localPosition = prefab.transform.position;

            _uiDict.Add(name, go);
            
            
            MessageBroker.Default.Publish(new OpenPopupMessage<T>());
        }
        else
        {
            go.SetActive(true);
            popup = go.GetComponent<T>();
            _popups.Add(popup);
        }

        popup.Open();

        return popup;
    }

    public void DeletePopup<T>()
    {
        string  name = typeof(T).Name;

        if (_uiDict.ContainsKey(name))
        {
            Managers.Resource.Destroy(_uiDict[name]);
            _uiDict.Remove(name);
        }
    }

    public T FindPopup<T>() where T : UI_Popup
    {
        return _popups.FirstOrDefault(x => x.GetType() == typeof(T)) as T;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popups.Count == 0)
            return;

        if (!_popups.Contains(popup))
            return;

        _popups.Remove(popup);
        popup.gameObject.SetActive(false);

        if (popup.isTop)
            _topOrderIndex--;
        else
            _orderIndex--;
    }

    public void ClosePopupUI()
    {
        if (_popups.Count == 0)
            return;

        UI_Popup popup = _popups[_popups.Count - 1];
        popup.gameObject.SetActive(false);
        _popups.Remove(popup);

        if (popup.isTop)
            _topOrderIndex--;
        else
            _orderIndex--;
    }

    public void CloseAllPopupUI()
    {
        _popups.ForEach(popup =>
        {
            popup.gameObject.SetActive(false);
            
            if (popup.isTop)
                _topOrderIndex--;
            else
                _orderIndex--;
        });
        
        _popups.Clear();
    }

    public void Clear()
    {
        _popups.ForEach(popup =>
        {
            if (popup == null)
                return;
            
            Managers.Resource.Destroy(popup.gameObject);
        });
        
        _popups.Clear();
        _uiDict.Clear();

        _orderIndex = 1;
    }

    public void ShowGainItems(Dictionary<(ItemType, int), double> gainItemDatas)
    {
        if (gainItemDatas.Count <= 0)
            return;
        
        var uiGainItemPopup = ShowPopupUI<UI_GainItemPopup>();

        if (uiGainItemPopup == null)
            return;

        uiGainItemPopup.SetItem(gainItemDatas);
    }
}