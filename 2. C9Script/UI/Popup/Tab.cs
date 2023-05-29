using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public record Tab
{
    public UI_Panel Panel;
    public Button Button;
    public TMP_Text Text;
    public GameObject SelectObj;
    
    private readonly Color _nonSelectTabTextColor = new(124 / 255f, 114 / 255f, 103 / 255f);

    public void Init(Action<Tab> buttonEvent)
    {
        Panel.gameObject.SetActive(false);
        Button.BindEvent(() => buttonEvent?.Invoke(this));
        SelectObj.SetActive(false);
        Text.color = _nonSelectTabTextColor;
    }

    public void Open()
    {
        SelectObj.SetActive(true);
        Text.color = Color.white;
        Panel.Open();
    }

    public void Close()
    {
        Panel.gameObject.SetActive(false);
        SelectObj.SetActive(false);
        Text.color = _nonSelectTabTextColor;
    }
}