using System;
using System.Collections;
using GameData;
using UniRx;
using UnityEngine;

namespace DefaultNamespace
{
    public class DpsDungeonManager
    {
        public ReactiveProperty<double> TotalDps = new(0);

        public float LimitTime;
        public ReactiveProperty<float> RemainTime = new(0);

        public bool IsProgress;

        public bool IsBoss;

        public void StartDps()
        {
            Managers.Game.MainPlayer.State.Value = CharacterState.None;

            FadeScreen.FadeOut(() =>
            {
                Managers.UI.CloseAllPopupUI();
                
                MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EnterDungeon, (int)DungeonType.Dps, 1));
                
                Managers.Stage.State.Value = StageState.Dps;
                Managers.Stage.SetBg(ChartManager.DungeonCharts[(int)DungeonType.Dps].WorldId);

                Managers.Monster.StartSpawn();

                Managers.Game.MainPlayer.transform.position = Managers.GameSystemData.DpsDungeonPlayerPosition;

                LimitTime = ChartManager.SystemCharts[SystemData.DpsDungeon_LimitTime].Value;
                RemainTime.Value = LimitTime;

                TotalDps.Value = 0;

                FadeScreen.FadeIn(() =>
                {
                    IsProgress = true;
                    MainThreadDispatcher.StartCoroutine(CoRemainTimeTimer());
                    Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                    Managers.Game.MainPlayer.Direction.Value = CharacterDirection.Right;
                }, 0.5f);
            }, 0f, 0.2f);
        }

        private IEnumerator CoRemainTimeTimer()
        {
            while (true)
            {
                yield return null;

                if (!IsProgress)
                    yield break;

                RemainTime.Value -= Time.deltaTime;

                if (RemainTime.Value <= 0)
                {
                    EndDpsDungeon();
                    yield break;
                }
            }
        }

        public void EndDpsDungeon(bool isGiveUp = false)
        {
            if (!IsProgress)
                return;

            IsProgress = false;
            Managers.Game.MainPlayer.State.Value = CharacterState.None;

            if (isGiveUp)
                Managers.UI.ShowPopupUI<UI_FailDungeonPopup>();
            else
                Managers.UI.ShowPopupUI<UI_DpsResultPopup>();

            Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
            {
                FadeScreen.FadeOut(() =>
                {
                    Managers.Game.MainPlayer.SetAllSkillCoolTime();
                    Managers.UI.CloseAllPopupUI();
                    Managers.Stage.State.Value = StageState.Normal;
                    Managers.Monster.StartSpawn();

                    if (!isGiveUp)
                    {
                        if (IsBoss)
                        {
                            if (TotalDps.Value > Managers.Game.UserData.DpsBossDungeonHighestScore)
                            {
                                Managers.Game.UserData.DpsBossDungeonHighestScore = TotalDps.Value;
                                GameDataManager.UserGameData.SaveGameData();
                            }
                        }
                        else
                        {
                            if (TotalDps.Value > Managers.Game.UserData.DpsDungeonHighestScore)
                            {
                                if (Managers.Game.UserData.DpsDungeonHighestScore <= 0)
                                    InAppActivity.SendEvent("First_dps");

                                Managers.Game.UserData.DpsDungeonHighestScore = TotalDps.Value;
                                GameDataManager.UserGameData.SaveGameData();
                            }
                        }
                    }

                    FadeScreen.FadeIn(() => { Managers.Game.MainPlayer.State.Value = CharacterState.Idle; });
                }, 0.5f, 0.2f);
            });
        }
    }
}