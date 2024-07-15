using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSkill : BaseState
{
    public BattleSkill( BattleCore core ) : base ( core ) { }


    public override void Enter()
    {
        
        _core.SetViewSkill(true);
    }

    public override void Exit()
    {
        _core.SetViewSkill(false);
    }
}
