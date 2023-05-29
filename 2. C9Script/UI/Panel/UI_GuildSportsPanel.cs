using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using TMPro;

public class UI_GuildSportsPanel : UI_Panel
{
    #region Fields
    [SerializeField] private Button _startBtn;
    [SerializeField] private Button _closeBtn;

    [SerializeField] private TMP_Text _titleTxt;
    [SerializeField] private TMP_Text _guideTxt;
    [SerializeField] private TMP_Text _entryTxt;
    [SerializeField] private TMP_Text _entryCountTxt;

    [SerializeField] private Image _goodsImg;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _startBtn.BindEvent(OnClickGuildSportsStart);
        _closeBtn.BindEvent(Close);

        StartSetUI();
    }
    #endregion

    #region Public Methods
    public override void Open()
    {
        base.Open();

        SetUI();
    }
    #endregion

    #region Private Methods
    private void SetUI()
    {
        _entryCountTxt.text = Managers.Game.GoodsDatas[(int)Goods.GuildSportsTicket].Value.ToString();
    }

    private void StartSetUI()
    {
        _goodsImg.sprite = Managers.Resource.LoadGoodsIcon(ChartManager.GoodsCharts[(int)Goods.GuildSportsTicket].Icon);

        _titleTxt.text = ChartManager.GetString("Guild_Sports_Name");
        _guideTxt.text = ChartManager.GetString("Guild_Sports_Desc");
        _entryTxt.text = ChartManager.GetString("Dungeon_Start");
    }

    private void OnClickGuildSportsStart()
    {
        var afterGuildJoinTime = Utils.GetNow() - Managers.Guild.MyGuildMemberData.GuildJoinTime;

        if (afterGuildJoinTime.TotalSeconds < 60 * 60 * 24)
        {
            var openRaidTime = Managers.Guild.MyGuildMemberData.GuildJoinTime.AddHours(24);
            var openGapTime = openRaidTime - Utils.GetNow();

            if (openGapTime.Hours > 0)
                Managers.Message.ShowMessage($"{openGapTime.Hours}시간 후 참여할 수 있습니다.");
            else if (openGapTime.Minutes > 0)
                Managers.Message.ShowMessage($"{openGapTime.Minutes}분 후 참여할 수 있습니다.");
            else
                Managers.Message.ShowMessage($"{openGapTime.Seconds}초 후 참여할 수 있습니다.");

            return;
        }

        if (Managers.Game.GoodsDatas[(int)Goods.GuildSportsTicket].Value <= 0)
        {
            Managers.Message.ShowMessage(ChartManager.GetString("Ticket_Condition_not_have"));
            return;
        }

        Managers.Guild.GetGuildList(Managers.GuildSports.GuildSportsStart, true);

    }
    #endregion
}
