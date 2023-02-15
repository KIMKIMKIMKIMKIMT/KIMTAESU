using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class RewardAds : MonoBehaviour
{
    private RewardedAd rewardedAd;

    private readonly string testunitID = "ca-app-pub-3940256099942544/5224354917";


    private void Start()
    {
        // 모바일광고 SDK 초기화.
        MobileAds.Initialize(initStatus => { });
        Init();
    }
    public void Init()
    {

        this.rewardedAd = new RewardedAd(testunitID);
        //광고로드
        AdRequest request = new AdRequest.Builder().Build();

        this.rewardedAd.LoadAd(request);

        //광고 로드 완료 되면 호출
        this.rewardedAd.OnAdLoaded += OnAdLoaded;
        //광고 로드 실패 하면 호출
        this.rewardedAd.OnAdFailedToLoad += OnAdFailedToLoad;
        //광고가 표시될 때 호출
        this.rewardedAd.OnAdOpening += OnAdOpening;
        //광고 표시가 실패 했을 때 호출
        this.rewardedAd.OnAdFailedToShow += OnAdFailedToShow;
        //광고를 닫았을 때 호출
        this.rewardedAd.OnAdClosed += OnAdClosed;
        //광고 시청 후 보상을 받아야 할 때 호출
        this.rewardedAd.OnUserEarnedReward += OnUserEarnedReward;




    }
    public void OnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("광고 로드 성공");
    }

    public void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("광고 로드 실패");
        Init();
    }
    public void OnAdOpening(object sender, EventArgs args)
    {
        Debug.Log("광고 표시 성공");
    }
    public void OnAdFailedToShow(object sender, EventArgs args)
    {
        Debug.Log("광고 표시 실패");
    }
    public void OnAdClosed(object sender, EventArgs args)
    {
        Debug.Log("광고 닫음");
        Init();
    }
    public void OnUserEarnedReward(object sender, Reward reward)
    {
        UIMgr.Instance.MainUI.ShopUI.Reward();
        GameMgr.Instance.QuestAddCnt(eQUEST.ShowAds);
    }
    public bool ShowRewardAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
            return true;
        }

        return false;
    }
}
