using System;
using Chart;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace SignInSample.Character
{
    public class WorldCupMonster : Monster
    {
        public int Phase;
        
        private Color _phase1Color = new Color(1f, 0.82f, 0.82f);
        private Color _phase2Color = new Color(1f, 0.41f, 0.41f);
        private Color _phase3Color = new Color(1f, 0.28f, 0.28f);

        private WorldCupEventDungeonChart _worldCupEventDungeonChart;

        public override void Initialize()
        {
            base.Initialize();

            ChartManager.WorldCupEventDungeonCharts.TryGetValue(1, out _worldCupEventDungeonChart);

            MaxHp = _worldCupEventDungeonChart.MonsterHp;
            Hp.Value = MaxHp;

            Phase = 1;
        }

        public override void Damage(double damage, double criticalMultiple = 0, int teamIndex = -1)
        {
            base.Damage(damage, criticalMultiple);

            switch (Phase)
            {
                case 1:
                {
                    if (MaxHp * _worldCupEventDungeonChart.Phase1Hp > Hp.Value)
                    {
                        Phase = 2;
                        _spriteRenderers.ForEach(sprite =>
                        {
                            sprite.color = _phase1Color;
                        });
                        MoveSpeed.Value = _monsterChart.MoveSpeed +
                                          _monsterChart.MoveSpeed * _worldCupEventDungeonChart.Phase1Value;
                    }
                }
                    break;
                case 2:
                {
                    if (MaxHp * _worldCupEventDungeonChart.Phase2Hp > Hp.Value)
                    {
                        Phase = 3;
                        _spriteRenderers.ForEach(sprite =>
                        {
                            sprite.color = _phase2Color;
                        });
                        MoveSpeed.Value = _monsterChart.MoveSpeed +
                                          _monsterChart.MoveSpeed * _worldCupEventDungeonChart.Phase2Value;
                    }
                }
                    break;
                case 3:
                {
                    if (MaxHp * _worldCupEventDungeonChart.Phase3Hp > Hp.Value)
                    {
                        Phase = 4;
                        _spriteRenderers.ForEach(sprite =>
                        {
                            sprite.color = _phase3Color;
                        });
                        MoveSpeed.Value = _monsterChart.MoveSpeed +
                                          _monsterChart.MoveSpeed * _worldCupEventDungeonChart.Phase3Value;
                    }
                }
                    break;
            }
        }

        protected override void HitEffect()
        {
            _spriteRenderers.ForEach(sprite =>
            {
                if (sprite == null)
                    return;

                sprite.color = Color.red;
                Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
                {
                    if (sprite == null)
                        return;

                    switch (Phase)
                    {
                        case 1:
                            sprite.color = Color.white;
                            break;
                        case 2:
                            sprite.color = _phase1Color;
                            break;
                        case 3:
                            sprite.color = _phase2Color;
                            break;
                        case 4:
                            sprite.color = _phase3Color;
                            break;
                    }
                }).AddTo(_hitColorComposite);
            });
        }
    }
}