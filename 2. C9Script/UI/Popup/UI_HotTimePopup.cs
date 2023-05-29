using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_HotTimePopup : UI_Popup
{
    [SerializeField] private Image HotTimeImage;

    [SerializeField] private Sprite HotTimeSprite1;
    [SerializeField] private Sprite HotTimeSprite2;

    [SerializeField] private TMP_Text HotTimeText;
    [SerializeField] private TMP_Text WeekText;
    [SerializeField] private TMP_Text GoldRateText;
    [SerializeField] private TMP_Text GoldBarRateText;
    [SerializeField] private TMP_Text TicketRateText;

    [SerializeField] private Button CloseButton;

    private void Start()
    {
        CloseButton.BindEvent(ClosePopup);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();       
        }
    }

    public override void Open()
    {
        base.Open();

        SetUI();
    }

    private void SetUI()
    {
        switch (Utils.GetNow().DayOfWeek)
        {
            case DayOfWeek.Monday:
            case DayOfWeek.Tuesday:
            case DayOfWeek.Wednesday:
            case DayOfWeek.Thursday:
            {
                HotTimeImage.sprite = HotTimeSprite1;
                HotTimeText.color = Color.white;
                HotTimeText.text = "평일 핫타임";
                WeekText.color = Color.white;
                WeekText.text = "(월~목)";
                GoldRateText.text = "획득량\n50%증가";
                GoldBarRateText.text = "획득량\n50%증가";
                TicketRateText.text = "드롭율\n10%증가";
            }
                break;
            case DayOfWeek.Friday:
            case DayOfWeek.Saturday:
            case DayOfWeek.Sunday:
            {
                HotTimeImage.sprite = HotTimeSprite2;
                HotTimeText.color = Color.red;
                HotTimeText.text = "주말 핫타임";
                WeekText.color = Color.red;
                WeekText.text = "(금,토,일)";
                GoldRateText.text = "획득량\n100%증가";
                GoldBarRateText.text = "획득량\n100%증가";
                TicketRateText.text = "드롭율\n20%증가";
            }
                break;
        }
    }
}