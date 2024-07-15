using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStand : BaseState
{
    public BattleStand(BattleCore core) : base(core){ }

    readonly private float _baseTime    = 30f;
    readonly private float _addTime     = 10f;

    public override void Enter()
    {
        float waveTime = GetWaveTime();

        _core.AddWave();
        _core.SetTime( waveTime );
        _core.NextState();
    }

    public override void Exit()
    {
        
    }

    private float GetWaveTime()
    {
        int wave = _core.Wave;

        return _baseTime + (_addTime * wave);
    }
}
