using System;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGamePanel : UI_Panel
{
    [SerializeField] private Transform SkillPanelTr;
    [SerializeField] private Transform ChattingPanelTr;
    [SerializeField] private Transform GainItemPanelTr;

    [SerializeField] private Button DownButton;
    [SerializeField] private Button UpButton;

    private float _baseSkillPanelPos;
    private float _baseChattingPanelPos;
    private float _baseGainItemPanelPos;
    
    private float _downSkillPanelPos;
    private float _downChattingPanelPos;
    private float _downGainItemPanelPos;

    private float _downValue = 270;

    private Camera _uiCamera;

    private void Awake()
    {
        _uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();

        _baseSkillPanelPos = SkillPanelTr.localPosition.y;
        _baseChattingPanelPos = ChattingPanelTr.localPosition.y;
        _baseGainItemPanelPos = GainItemPanelTr.localPosition.y;

        _downSkillPanelPos = (SkillPanelTr.localPosition.y - _downValue) ;
        _downChattingPanelPos = (ChattingPanelTr.localPosition.y - _downValue) ;
        _downGainItemPanelPos = (GainItemPanelTr.localPosition.y - _downValue) ;
        
        DownButton.gameObject.SetActive(false);
        UpButton.gameObject.SetActive(false);
        
        DownButton.BindEvent(() =>
        {
            SkillPanelTr.DOLocalMoveY(_downSkillPanelPos, 1);
            ChattingPanelTr.DOLocalMoveY(_downChattingPanelPos, 1);
            GainItemPanelTr.DOLocalMoveY(_downGainItemPanelPos, 1);

            DownButton.gameObject.SetActive(false);
            UpButton.gameObject.SetActive(true);
        });
        
        UpButton.BindEvent(() =>
        {
            SkillPanelTr.DOLocalMoveY(_baseSkillPanelPos, 1);
            ChattingPanelTr.DOLocalMoveY(_baseChattingPanelPos, 1);
            GainItemPanelTr.DOLocalMoveY(_baseGainItemPanelPos, 1);
            DownButton.gameObject.SetActive(true);
            UpButton.gameObject.SetActive(false);
        });
    }
}