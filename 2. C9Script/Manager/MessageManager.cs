
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Pool;
    using Object = UnityEngine.Object;

    public class MessageManager
    {
        private Transform _root;
        public Transform Root => _root;

        private ObjectPool<TMP_Text> _messagePool;

        private readonly Dictionary<MessageType, string> _messageDic = new()
        {
            { MessageType.FailLogin , "로그인에 실패 했습니다!! "},
            { MessageType.LackReinforceMaterial, "강화 재료가 부족합니다!" },
            { MessageType.FailDungeonUIByStageState, "컨텐츠 진행중에는 이동할 수 없습니다." }
        };

        private readonly Dictionary<MessageType, TMP_Text> _playingMessage = new();
        private List<string> _playingMessageList = new();

        public void Init()
        {
            if (_root == null)
            {
                _root = GameObject.Find("MessageCanvas").transform;
                Object.DontDestroyOnLoad(_root.gameObject);
            }

            _messagePool = new ObjectPool<TMP_Text>(
                CreateMessage,
                messageText => messageText.gameObject.SetActive(true),
                messageText => messageText.gameObject.SetActive(false)
                );
        }

        public void ShowMessage(string message, Action onCompleteCallback = null)
        {
            if (_playingMessageList.Contains(message))
                return;
            
            _playingMessageList.Add(message);
            TMP_Text messageText = _messagePool.Get();

            messageText.text = message;
            Sequence sequence = DOTween.Sequence().AppendInterval(0.3f)
                .Append(messageText.transform.DOMove(messageText.transform.position + (Vector3.up * 100f), 2f))
                .Join(messageText.DOFade(0f, 2.3f));
            
            sequence.onComplete += () =>
            {
                _playingMessageList.Remove(message);
                messageText.alpha = 1f;
                messageText.transform.localPosition = Vector3.zero;
                onCompleteCallback?.Invoke();
                _messagePool.Release(messageText);
            };

        }

        public void ShowMessage(MessageType messageType)
        {
            if (!_messageDic.ContainsKey(messageType))
                return;
            
            if (_playingMessage.ContainsKey(messageType))
                return;

            ShowMessage(_messageDic[messageType], () =>
            {
                _playingMessage.Remove(messageType);
            });
        }

        private TMP_Text CreateMessage()
        {
            return Managers.Resource.Instantiate("ETC/MessageText", Root).GetComponent<TMP_Text>();
        }
    }
