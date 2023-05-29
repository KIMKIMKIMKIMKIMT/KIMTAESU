using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private readonly List<RewardedAd> _videoAds = new();
    private readonly Dictionary<string, RewardedAd> _adVideos = new();

    private const string _googleTestVideoID = "ca-app-pub-3940256099942544/5224354917";
    private const string _iosVideoID = "ca-app-pub-3940256099942544/1712485313";

    private bool _showVideo;
    
    private int _showIndex;

    public bool IsShowingAd;

    private readonly string[] _googleVideoIds = {
#if !UNITY_EDITOR
        "ca-app-pub-3542043612981554/1599753360",
        "ca-app-pub-3542043612981554/2171232808",
        "ca-app-pub-3542043612981554/9858151136",
        "ca-app-pub-3542043612981554/3642428278",
        "ca-app-pub-3542043612981554/7466919375",
        "ca-app-pub-3542043612981554/8260346045",
        "ca-app-pub-3542043612981554/6947264371",
        "ca-app-pub-3542043612981554/9286433357",
        "ca-app-pub-3542043612981554/8407631801",
        "ca-app-pub-3542043612981554/4763938251"
#endif
    };

    private Action _rewardCallback;

    private RewardedAd _showVideosAd;
    private string _showAppId;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (Managers.Manager.ProjectType == ProjectType.Live)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    foreach (var googleVideoId in _googleVideoIds)
                    {
                        if (_adVideos.ContainsKey(googleVideoId))
                            _adVideos[googleVideoId] = new RewardedAd(googleVideoId);
                        else
                            _adVideos.Add(googleVideoId, new RewardedAd(googleVideoId));
                    }

                    break;
                case RuntimePlatform.IPhonePlayer:
                    _videoAds.Add(new RewardedAd(_iosVideoID));
                    break;
                default:
                    _videoAds.Add(new RewardedAd(_googleTestVideoID));
                    break;
            }

            foreach (var video in _adVideos)
                Handle(video.Value);
        }
        else
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    _videoAds.Add(new RewardedAd(_googleTestVideoID));
                    break;
                case RuntimePlatform.IPhonePlayer:
                    _videoAds.Add(new RewardedAd(_iosVideoID));
                    break;
                default:
                    _videoAds.Add(new RewardedAd(_googleTestVideoID));
                    break;
            }

            _videoAds.ForEach(Handle);
        }


        Load();
    }

    private void Handle(RewardedAd videoAd)
    {
        videoAd.OnAdClosed += HandleOnAdClosed;
        videoAd.OnUserEarnedReward += HandleOnUserEarnedReward;
    }

    private void Load()
    {
        if (Managers.Manager.ProjectType == ProjectType.Live)
        {
            foreach (var video in _adVideos)
            {
                AdRequest request = new AdRequest.Builder().Build();
                video.Value.LoadAd(request);
            }
        }
        else
        {
            _videoAds.ForEach(videoAd =>
            {
                AdRequest request = new AdRequest.Builder().Build();
                videoAd.LoadAd(request);
            });
        }
    }

    private RewardedAd ReloadAd()
    {
        RewardedAd videoAd;

        if (Managers.Manager.ProjectType == ProjectType.Live)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    videoAd = new RewardedAd(_showAppId);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    videoAd = new RewardedAd(_iosVideoID);
                    break;
                default:
                    videoAd = new RewardedAd(_googleTestVideoID);
                    break;
            }
        }
        else
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    videoAd = new RewardedAd(_googleTestVideoID);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    videoAd = new RewardedAd(_iosVideoID);
                    break;
                default:
                    videoAd = new RewardedAd(_googleTestVideoID);
                    break;
            }
        }

        Handle(videoAd);
        AdRequest request = new AdRequest.Builder().Build();
        videoAd.LoadAd(request);
        return videoAd;
    }

    //오브젝트 참조해서 불러줄 함수
    public bool Show(Action rewardCallback)
    {
        _rewardCallback = rewardCallback;

        if (Managers.Game.UserData.IsAdSkip())
        {
            _rewardCallback?.Invoke();
            return true;
        }

        if (_showVideo)
            return false;
        
        _showVideo = true;
        
        RewardedAd videoAd = null;

        if (Managers.Manager.ProjectType == ProjectType.Live)
        {
            int index = 0;
            
            if (_showIndex >= _googleVideoIds.Length)
                _showIndex = 0;
            
            while (index < _adVideos.Count)
            {
                string appId = _googleVideoIds[_showIndex];

                _showIndex += 1;
                if (_showIndex >= _googleVideoIds.Length)
                    _showIndex = 0;
                
                if (_adVideos.ContainsKey(appId) && _adVideos[appId].IsLoaded())
                {
                    _showAppId = appId;
                    videoAd = _adVideos[appId];
                    break;
                }

                ++index;
            }
        }
        else
            videoAd = _videoAds.Find(rewardedAd => rewardedAd.IsLoaded());

        // 준비된 광고 없음
        if (videoAd == null)
        {
            Managers.Message.ShowMessage("광고가 준비되지 않았습니다.");
            _showVideo = false;
        }
        else
        {
            IsShowingAd = true;
            _showVideosAd = videoAd;
            _rewardCallback = rewardCallback;

            if (_videoAds.Contains(videoAd))
                _videoAds.Remove(videoAd);
            StartCoroutine(ShowRewardAd());
            return true;
        }

        return false;
    }

    private IEnumerator ShowRewardAd()
    {
        if (_showVideosAd == null)
            yield break;

        while (!_showVideosAd.IsLoaded())
        {
            yield return null;
        }

        _showVideosAd.Show();
    }

    //광고가 종료되었을 때
    private void HandleOnAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleOnAdClosed");
        _showVideo = false;
        IsShowingAd = false;
        if (Managers.Manager.ProjectType == ProjectType.Live)
        {
            if (_adVideos.ContainsKey(_showAppId))
                _adVideos[_showAppId] = ReloadAd();
            else
                _adVideos.Add(_showAppId, ReloadAd());
        }
        else
        {
            _showVideosAd = ReloadAd();
            _videoAds.Add(_showVideosAd);
        }
    }

    //광고를 끝까지 시청하였을 때
    private void HandleOnUserEarnedReward(object sender, Reward args)
    {
        Debug.Log("HandleOnUserEarnedReward");
        StartCoroutine(CoReward());
        _showVideo = false;
    }

    private IEnumerator CoReward()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("CoReward");
        _rewardCallback?.Invoke();
        _rewardCallback = null;
    }
}