using System;
using System.Collections.Generic;
using BackEnd;
using BackEnd.Tcp;
using LitJson;
using Newtonsoft.Json;
using UniRx;

[Serializable]
public record ChatData
{
    public int Rank;
    public int CostumeId;
    public string Nickname;
    public string GuildName;
    public string ChatMessage;
    public bool IsMy;
}

public class ChatManager
{
    private static string ChatGroupName => $"일반채널_Server{Managers.Server.CurrentServer}";
    private static string GuildChatGroupName => "길드채널";

    public ReactiveCollection<ChatData> ChatDatas { get; } = new();
    public ReactiveCollection<ChatData> GuildChatDatas { get; } = new();

    public Action OnFailGetGuildGroupChannelList;

    public void Init()
    {
        SetHandler();
    }

    private void SetHandler()
    {
        // 자신, 다른 유저 채널 입장 이벤트
        Backend.Chat.OnJoinChannel += args =>
        {
            if (args.ErrInfo == ErrorInfo.Success)
            {
                if (!args.Session.IsRemote)
                {
                    ChatDatas.Clear();
                    Debug.Log("채팅 채널에 접속햇습니다.");
                }
                else
                {
                    Debug.Log($"{args.Session.NickName}님이 접속했습니다");
                }
            }
            else
            {
                Debug.Log($"채팅 채널 입장 도중 에러가 발생했습니다!! : {args.ErrInfo.Reason}");
            }
        };

        Backend.Chat.OnJoinGuildChannel += args =>
        {
            if (args.ErrInfo == ErrorInfo.Success)
            {
                if (!args.Session.IsRemote)
                {
                    GuildChatDatas.Clear();
                    Debug.Log("길드 채팅 채널에 접속햇습니다.");
                }
                else
                {
                    Debug.Log($"{args.Session.NickName}님이 접속했습니다");
                }
            }
            else
            {
                Debug.Log($"채팅 채널 입장 도중 에러가 발생했습니다!! : {args.ErrInfo.Reason}");
            }
        };

        // 자신이 채널에 입장 했을때 모든 유저 정보 조회, 최초 1회
        Backend.Chat.OnSessionListInChannel += _ => { };

        // 채널 입장시 채팅 내역 조회 / 최대 30개
        Backend.Chat.OnRecentChatLogs += args =>
        {
            for (var i = args.LogInfos.Count - 1; i >= 0; i--)
            {
                var nickname = args.LogInfos[i].NickName;
                var chatContent = args.LogInfos[i].Message;

                var chatData = JsonConvert.DeserializeObject<ChatData>(chatContent);
                
                if (chatData == null) 
                    continue;
                
                chatData.IsMy = nickname.Equals(Backend.UserNickName);
                chatData.Nickname = nickname;

                switch (args.channelType)
                {
                    case ChannelType.Public:
                        ChatDatas.Add(chatData);
                        break;
                    case ChannelType.Guild:
                        GuildChatDatas.Add(chatData);
                        break;
                }
            }
        };

        // 일반 채팅 수신
        Backend.Chat.OnChat += args =>
        {
            if (args.ErrInfo == ErrorInfo.Success)
            {
                ChatData chatData = JsonConvert.DeserializeObject<ChatData>(args.Message);

                if (chatData != null)
                {
                    chatData.IsMy = !args.From.IsRemote;
                    chatData.Nickname = args.From.NickName;

                    ChatDatas.Add(chatData);

                    if (ChatDatas.Count > 30)
                        ChatDatas.RemoveAt(0);
                }
            }
            else if (args.ErrInfo.Category == ErrorCode.BannedChat)
            {
                if (args.ErrInfo.Detail == ErrorCode.BannedChat)
                {
                    Managers.Message.ShowMessage("메시지를 너무 많이 입력하셧습니다. 잠시 후 시도해주세요.");
                }
            }
        };

        // 길드 채팅 수신
        Backend.Chat.OnGuildChat += args =>
        {
            if (args.ErrInfo == ErrorInfo.Success)
            {
                var chatData = JsonConvert.DeserializeObject<ChatData>(args.Message);

                if (chatData == null)
                    return;

                chatData.IsMy = !args.From.IsRemote;
                chatData.Nickname = args.From.NickName;

                GuildChatDatas.Add(chatData);

                if (GuildChatDatas.Count > 30)
                    GuildChatDatas.RemoveAt(0);
            }
            else if (args.ErrInfo.Category == ErrorCode.BannedChat)
            {
                if (args.ErrInfo.Detail == ErrorCode.BannedChat)
                {
                    Managers.Message.ShowMessage("메시지를 너무 많이 입력하셧습니다. 잠시 후 시도해주세요.");
                }
            }
        };
        
        // 길드 채팅 나갓을때
        Backend.Chat.OnLeaveGuildChannel += args =>
        {
            if (args.ErrInfo == ErrorInfo.Success && !args.Session.IsRemote)
            {
                Debug.Log($"w");
                GuildChatDatas.Clear();
            }
        };
    }

    public void ConnectChat()
    {
        if (Backend.Chat.IsChatConnect(ChannelType.Public))
            return;

        Backend.Chat.GetGroupChannelList(ChatGroupName, bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog("Fail GetGroupChannelList", bro);
                return;
            }

            JsonData jsonData = bro.FlattenRows();

            for (int i = 0; i < jsonData.Count; i++)
            {
                int.TryParse(jsonData[i]["joinedUserCount"].ToString(), out int joinedUserCount);

                if (joinedUserCount >= 190)
                    continue;

                string serverAddress = jsonData[i]["serverAddress"].ToString();
                string serverPort = jsonData[i]["serverPort"].ToString();
                string inDate = jsonData[i]["inDate"].ToString();

                Backend.Chat.JoinChannel(ChannelType.Public, serverAddress,
                    ushort.Parse(serverPort), ChatGroupName, inDate, out var errorInfo);
                Debug.Log($"JoinChannel ErrorInfo : {errorInfo}");

                break;
            }
        });
    }

    public void ConnectGuildChat()
    {
        if (Backend.Chat.IsChatConnect(ChannelType.Guild))
            return;

        Backend.Chat.GetGroupChannelList(GuildChatGroupName, bro =>
        {
            if (!bro.IsSuccess())
            {
                Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);
                
                // PreconditionFailed
                // notGuildMember 사전 조건을 만족하지 않습니다.

                if (statusCode.Equals("412") && errorCode.Contains("PreconditionFailed") &&
                    message.Contains("notGuildMember"))
                {
                    OnFailGetGuildGroupChannelList?.Invoke();
                }
                
                Managers.Backend.FailLog("Fail GetGroupChannelList", bro);
                return;
            }

            JsonData jsonData = bro.FlattenRows();

            for (int i = 0; i < jsonData.Count; i++)
            {
                int.TryParse(jsonData[i]["joinedUserCount"].ToString(), out int joinedUserCount);

                if (joinedUserCount >= 190)
                    continue;

                string serverAddress = jsonData[i]["serverAddress"].ToString();
                string serverPort = jsonData[i]["serverPort"].ToString();
                string inDate = jsonData[i]["inDate"].ToString();

                Backend.Chat.JoinChannel(ChannelType.Guild, serverAddress,
                    ushort.Parse(serverPort), GuildChatGroupName, inDate, out var errorInfo);
                Debug.Log($"JoinChannel ErrorInfo : {errorInfo}");

                break;
            }
        });
    }

    public void RemoveChatData(string nickname)
    {
        List<ChatData> chatDatas = new List<ChatData>();

        foreach (var chatData in ChatDatas)
        {
            if (chatData.Nickname.Equals(nickname))
                chatDatas.Add(chatData);
        }

        chatDatas.ForEach(chatData => ChatDatas.Remove(chatData));

        chatDatas.Clear();
        
        foreach (var chatData in GuildChatDatas)
        {
            if (chatData.Nickname.Equals(nickname))
                chatDatas.Add(chatData);
        }

        chatDatas.ForEach(chatData => GuildChatDatas.Remove(chatData));
    }
}