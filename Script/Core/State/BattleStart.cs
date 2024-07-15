using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStart : BaseState
{
    public BattleStart(BattleCore core) : base(core) { }
    public override void Enter()
    {
        _core.NextState();   
    }
 
    public override void Exit()
    {
    }
}
