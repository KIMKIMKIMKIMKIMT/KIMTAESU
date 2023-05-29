using UniRx;
using UnityEngine;

namespace SignInSample.Character
{
    public class HpBar : MonoBehaviour
    {
        [SerializeField] private Transform HpBarTr;
        
        private BaseCharacter _ownerCharacter;

        public void Init(BaseCharacter ownerCharacter)
        {
            _ownerCharacter = ownerCharacter;
        }
        
        private void Start()
        {
            if (Utils.IsHwasengbangDungeon())
            {
                gameObject.SetActive(false);
                return;
            }
            
            if (_ownerCharacter == null)
            {
                _ownerCharacter = transform.parent.GetComponent<BaseCharacter>();
                if (_ownerCharacter == null)
                    return;
            }

            _ownerCharacter.Hp.Subscribe(hp =>
            {
                float value = _ownerCharacter.MaxHp == 0 ? 0f : (float)(hp / _ownerCharacter.MaxHp);
                UpdateHpBar(value);
            });
        }

        private void UpdateHpBar(float value)
        {
            Vector3 localScale = HpBarTr.localScale;
            HpBarTr.localScale = new Vector3(value, localScale.y, localScale.z);
        }
        
        public void On()
        {
            gameObject.SetActive(true);            
        }
        
        public void Off()
        {
            gameObject.SetActive(false);
        }
    }
}