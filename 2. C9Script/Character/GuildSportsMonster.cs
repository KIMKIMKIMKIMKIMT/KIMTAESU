using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eANIM_GUILDSPORTS_PARK
{
    Idle_1,
    Idle_2,
    Idle_3,
    Victory,
}

public class GuildSportsMonster : Monster
{
    #region Fields

    private bool _isFirstHit;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _isFirstHit = false;
    }
    #endregion

    #region Public Methods
    public override void Damage(double damage, double criticalMultiple = 0, int teamIndex = -1)
    {
        base.Damage(damage, criticalMultiple, teamIndex);

        if (!_isFirstHit)
        {
            _isFirstHit = true;
            GuildSportsSetAnim(eANIM_GUILDSPORTS_PARK.Idle_1);
        }
    }

    public void GuildSportsSetAnim(eANIM_GUILDSPORTS_PARK state)
    {
        _animator.SetTrigger(state.ToString());
    }
    #endregion

    #region Private methods
    #endregion
}
