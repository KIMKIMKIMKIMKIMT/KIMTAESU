using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace UI
{
    public class UI_DamageText : MonoBehaviour
    {
        [SerializeField] private TMP_Text DamageText;
        [SerializeField] private Image CriticalImage;

        private Sequence sequence;

        public void Initialize(string text, double criticalMultiple)
        {
            DamageText.text = text;
            DamageText.DOFade(1, 0);
            CriticalImage.DOFade(1, 0);

            Sprite criticalSprite = Managers.Resource.LoadCriticalDamageEffect(criticalMultiple);

            if (criticalSprite == null)
            {
                CriticalImage.gameObject.SetActive(false);
            }
            else
            {
                CriticalImage.gameObject.SetActive(true);
                CriticalImage.sprite = criticalSprite;
                var criticalImagePos = CriticalImage.transform.localPosition;
                criticalImagePos.x = -(DamageText.GetPreferredValues().x * 0.5f);
                CriticalImage.transform.localPosition = criticalImagePos;
            }

            DamageText.color = Utils.CriticalColor.ContainsKey((int)criticalMultiple) ? Utils.CriticalColor[(int)criticalMultiple] : Color.white;
            var position = transform.position;
            Vector3 pos = new Vector3(position.x, position.y + 1f, position.z);

            if (sequence != null)
                sequence.onComplete = null;

            sequence = DOTween.Sequence().Append(transform.DOScale(new Vector3(1.8f, 1.8f, 1.8f), 0.1f))
                .Append(transform.DOScale(Vector3.one, 0.1f))
                .Append(transform.DOMove(pos, 0.3f))
                .Insert(0.3f, DamageText.DOFade(0, 0.3f)).Join(CriticalImage.DOFade(0, 0.3f));

            sequence.onComplete += () => { Managers.DamageText.Return(this); };
        }

        public void Initialize(string text)
        {
            DamageText.text = text;
            DamageText.DOFade(1, 0);
            CriticalImage.DOFade(1, 0);

            CriticalImage.gameObject.SetActive(false);

            DamageText.color = Color.white;
            var position = transform.position;
            Vector3 pos = new Vector3(position.x, position.y + 1f, position.z);

            if (sequence != null)
                sequence.onComplete = null;

            sequence = DOTween.Sequence()
                .Append(transform.DOMove(pos, 0.8f))
                .Insert(0.3f, DamageText.DOFade(0, 0.8f)).Join(CriticalImage.DOFade(0, 0.8f));

            sequence.onComplete += () => { Managers.DamageText.Return(this); };
        }

        public void Return()
        {
            if (sequence != null)
                sequence.onComplete = null;

            Managers.DamageText.Return(this);
        }
    }
}