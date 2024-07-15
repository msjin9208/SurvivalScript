using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleWaveEnd : BaseState
{
    public BattleWaveEnd(BattleCore core) : base(core) { }

    public override void Enter()
    {
        bool complete = _core.CheckGameEnd();

        if ( true == complete ) 
        {
            _core.ChangeState( CommonEnum.BattleState.Skill );
        }
        else
        {
            _core.ChangeState( CommonEnum.BattleState.End );
        }
    }
  
    public override void Exit()
    {
        
    }
}
