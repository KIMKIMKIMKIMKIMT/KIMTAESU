using UI;

public class OpenPopupMessage<T> where T : UI_Popup
{
}
    
public class ClosePopupMessage<T> where T : UI_Popup
{
}

public class OpenPanelMessage<T> where T : UI_Panel
{
    
}

public class ViewCutScene
{
    
}

public class LoadServerChart
{
    
}

public class LoadGameData
{
    
}

public class GuildChangeDescMessage
{
    public string Desc;

    public GuildChangeDescMessage(string desc)
    {
        Desc = desc;
    }
}

public class GuildChangeMarkMessage
{
    public int MarkId;

    public GuildChangeMarkMessage(int markId)
    {
        MarkId = markId;
    }
}

public class GuildApplicantMessage
{
    public UI_GuildManagementApplicantItem UIGuildManagementApplicantItem;
    public bool IsApprove;

    public GuildApplicantMessage(UI_GuildManagementApplicantItem uiGuildManagementApplicantItem, bool isApprove)
    {
        UIGuildManagementApplicantItem = uiGuildManagementApplicantItem;
        IsApprove = isApprove;
    }
}

public class GuildExpelMessage
{
    public UI_GuildManagementMemberItem UIGuildManagementMemberItem;

    public GuildExpelMessage(UI_GuildManagementMemberItem uiGuildManagementMemberItem)
    {
        UIGuildManagementMemberItem = uiGuildManagementMemberItem;
    }
}

public class GuildReceivedGuildApplicantMessage
{
    
}

public class GuildChangeGradeMessage
{
    public int Grade;
    public int Gxp;

    public GuildChangeGradeMessage(int grade, int gxp)
    {
        Grade = grade;
        Gxp = gxp;
    }
}

public enum LabAwakeningSettingMessageType
{
    Pattern,
    Stat,
    Grade,
}

public class LabAwakeningSettingMessage
{
    public LabAwakeningSettingMessageType Type;
    public int Value;

    public LabAwakeningSettingMessage(LabAwakeningSettingMessageType type, int value)
    {
        Type = type;
        Value = value;
    }
}
public enum WoodAwakeningSettingMessageType
{
    Stat,
    Grade,
}

public class WoodAwakeningSettingMessage
{
    public WoodAwakeningSettingMessageType Type;
    public int Value;

    public WoodAwakeningSettingMessage(WoodAwakeningSettingMessageType type, int value)
    {
        Type = type;
        Value = value;
    }
}

public class AutoLoginMessage
{
    
}
