using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BattleWave : BaseState, IState.IExcute
{
    public BattleWave(BattleCore core) : base(core) { }

    private float   _spawnTime;
    private float   _timer;
    private int     _spawnCnt;
    private int     _maxSpawn;
    private int     _curSpawnCnt;
    
    public override void Enter( )
    {
        SetSpawnCount();
        SetTime();
    }
    public void Excute( )
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    Spawn();
        _core.DecreaseTime(Time.deltaTime);

        bool waveTimeEnd    = _core.CheckWaveTime();
        bool spawnEnd       = _curSpawnCnt <= 0 && _core.CheckMonsterCount();

        if( waveTimeEnd || spawnEnd )
        {
            _core.NextState();
        }
        else if( _curSpawnCnt > 0 )
        {
            _timer += Time.deltaTime;

            if (_timer > _spawnTime)
            {
                _timer = 0;
                Spawn();
            }
        }
    }

    public override void Exit()
    {
        _spawnTime      = 0;
        _timer          = 0;
        _spawnCnt       = 0;
        _maxSpawn       = 0;
        _curSpawnCnt    = 0;
    }

    private void SetSpawnCount()
    {
        _spawnCnt       = _core.Wave;
        _maxSpawn       = (_core.Wave * 10);
        _curSpawnCnt    = _maxSpawn;
    }

    private void SetTime()
    {
        _timer      = 0;
        _spawnTime  = (_core.GameTime / 2) / (_maxSpawn / _spawnCnt);
    }

    private void Spawn( )
    {
        _core.SpawnMonster(_spawnCnt);

        _curSpawnCnt -= _spawnCnt;
        _core.SetMonsterCount(_curSpawnCnt);
    }
}
