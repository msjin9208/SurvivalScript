using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IState
{
    public interface IExcute
    {
        public void Excute();
    }
}

public abstract class BaseState
{
    protected BattleCore _core;
    public BaseState(BattleCore core) 
    {
        _core = core;
    }
    public abstract void Enter();
    public abstract void Exit();
}
