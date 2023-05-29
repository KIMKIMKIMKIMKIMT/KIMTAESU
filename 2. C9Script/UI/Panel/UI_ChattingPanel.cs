using System;
using System.Collections;
using BackEnd;
using DG.Tweening;
using LitJson;
using Newtonsoft.Json;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChattingPanel : UI_Panel
{
    [SerializeField] private TMP_Text NoticeText;

    [SerializeField] private TMP_Text ChatText;

    [SerializeField] private Button ChatButton;

    [SerializeField] private Image MaskImage;
    
    private Camera _uiCamera;

    private string[] _notices;
    private int _noticeIndex;
    private float _noticeMinX;
    private float _noticeMaxX;

    private void Start()
    {
        _uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
        Debug.Log($"NoticeText Local Position : {NoticeText.transform.localPosition}");
        Debug.Log($"NoticeText Size : {NoticeText.GetPreferredValues()}");

        ChatText.text = "접속하신 걸 환영합니다.";

        Managers.Chat.ChatDatas.ObserveAdd().Subscribe(chatData =>
        {
            ChatText.text = chatData.Value.ChatMessage;
        });
        
        ChatButton.BindEvent(OnClickChat);

        string[] tests = new[]
        {
            "test1",
            "test2",
            "test3",
        };

        string jsonTest = JsonConvert.SerializeObject(tests);

        Backend.Notice.NoticeList(1, bro =>
        {
            if (!bro.IsSuccess())
            {
                if (!bro.IsSuccess())
                {
                    Managers.Backend.FailLog(bro);
                    return;
                }
            }
            
            Backend.Notice.NoticeOne(bro.FlattenRows()[0]["inDate"].ToString(), bro =>
            {
                if (!bro.IsSuccess())
                {
                    Managers.Backend.FailLog(bro);
                    return;
                }

                string content = bro.GetFlattenJSON()["row"]["content"].ToString();

                NoticeText.transform.localPosition = Vector2.zero;

                string[] sss = new[]
                {
                    "테스트",
                    "테스트2"
                };

                _notices = JsonMapper.ToObject<string[]>(content);

                if (_notices.Length <= 0)
                    NoticeText.text = string.Empty;
                else
                {
                   _noticeMinX = MaskImage.transform.localPosition.x - MaskImage.rectTransform.rect.width * 0.5f;
                   _noticeMaxX = MaskImage.transform.localPosition.x + MaskImage.rectTransform.rect.width * 0.5f;
                   MainThreadDispatcher.StartCoroutine(CoNoticeSlider());
                }
            });
        });

        Observable.Timer(TimeSpan.FromHours(1)).Subscribe(_ =>
        {
            Backend.Notice.NoticeList(1, bro =>
            {
                if (!bro.IsSuccess())
                {
                    if (!bro.IsSuccess())
                    {
                        Managers.Backend.FailLog(bro);
                        return;
                    }
                }
            
                Backend.Notice.NoticeOne(bro.FlattenRows()[0]["inDate"].ToString(), bro =>
                {
                    if (!bro.IsSuccess())
                    {
                        Managers.Backend.FailLog(bro);
                        return;
                    }

                    string content = bro.GetFlattenJSON()["row"]["content"].ToString();

                    NoticeText.transform.localPosition = Vector2.zero;

                    _notices = JsonMapper.ToObject<string[]>(content);
                });
            });
        });

    }

    private void OnClickChat()
    {
        Managers.UI.ShowPopupUI<UI_ChatPopup>();
    }

    private IEnumerator CoNoticeSlider()
    {
        void SetNotice()
        {
            if (_notices.Length <= 0)
            {
                NoticeText.text = string.Empty;
                return;
            }

            if (_noticeIndex >= _notices.Length)
                _noticeIndex = 0;
            
            NoticeText.text = _notices[_noticeIndex++];
            NoticeText.transform.localPosition = new Vector2(_noticeMaxX + NoticeText.GetPreferredValues().x * 0.5f, 0);
        }

        SetNotice();
        
        while (true)
        {
            yield return null;
    
            NoticeText.transform.Translate(Vector3.left * 0.5f * Time.deltaTime);

            if (NoticeText.transform.localPosition.x + NoticeText.GetPreferredValues().x * 0.5f < _noticeMinX)
            {
                SetNotice();
                yield return new WaitForSeconds(1f);
            }
        }
    }
}