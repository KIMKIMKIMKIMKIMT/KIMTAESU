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
        // ����ϱ��� SDK �ʱ�ȭ.
        MobileAds.Initialize(initStatus => { });
        Init();
    }
    public void Init()
    {

        this.rewardedAd = new RewardedAd(testunitID);
        //����ε�
        AdRequest request = new AdRequest.Builder().Build();

        this.rewardedAd.LoadAd(request);

        //���� �ε� �Ϸ� �Ǹ� ȣ��
        this.rewardedAd.OnAdLoaded += OnAdLoaded;
        //���� �ε� ���� �ϸ� ȣ��
        this.rewardedAd.OnAdFailedToLoad += OnAdFailedToLoad;
        //���� ǥ�õ� �� ȣ��
        this.rewardedAd.OnAdOpening += OnAdOpening;
        //���� ǥ�ð� ���� ���� �� ȣ��
        this.rewardedAd.OnAdFailedToShow += OnAdFailedToShow;
        //���� �ݾ��� �� ȣ��
        this.rewardedAd.OnAdClosed += OnAdClosed;
        //���� ��û �� ������ �޾ƾ� �� �� ȣ��
        this.rewardedAd.OnUserEarnedReward += OnUserEarnedReward;




    }
    public void OnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("���� �ε� ����");
    }

    public void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("���� �ε� ����");
        Init();
    }
    public void OnAdOpening(object sender, EventArgs args)
    {
        Debug.Log("���� ǥ�� ����");
    }
    public void OnAdFailedToShow(object sender, EventArgs args)
    {
        Debug.Log("���� ǥ�� ����");
    }
    public void OnAdClosed(object sender, EventArgs args)
    {
        Debug.Log("���� ����");
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
