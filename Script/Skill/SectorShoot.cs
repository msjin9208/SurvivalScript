using CommonEnum;
using Cysharp.Threading.Tasks;
using ISkill;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


public class SectorShoot : ShootSkill, ICoolTime
{
    readonly private float      _sectorAngle = 90f;

    private System.IDisposable  _coolTimeDispose;
    private float               _coolTime;
    private float               _maxCoolTime;
    private int                 _targetCnt;

    public float GetCoolTime() => _coolTime;
    public float GetMaxCoolTime() => _maxCoolTime;

    public override void Initialize( SkillData data )
    {
        base.Initialize( data );

        _maxCoolTime    = SkillData.coolTime;
        _targetCnt      = SkillData.targetCount;
    }

    public override void PlaySkill(Scanner scanner)
    {
        base.PlaySkill(scanner);

        _coolTime = _maxCoolTime;
        SetCoolTime();
    }

    public void SetCoolTime()
    {
        _coolTimeDispose = Observable
            .EveryUpdate()
            .Subscribe(l =>
            {
                _coolTime -= Time.deltaTime;
                if (_coolTime <= 0)
                {
                    ShootSector();
                    
                    _coolTime = _maxCoolTime;
                }
                else
                {

                }

            }).AddTo(_scanner);
    }

    public override void Upgrade()
    {
        base.Upgrade();

        _maxCoolTime    -= 1f;
        _targetCnt      += 1;
    }

    private void ShootSector( )
    {
        Vector2 myDir       = _scanner.GetMyDirection();
        float shootAngle    = Mathf.Atan2(myDir.y, myDir.x) * Mathf.Rad2Deg;

        for( int i = 0; i < _targetCnt; i++ )
        {
            float angle     = shootAngle - _sectorAngle / 2 + i * _sectorAngle / (_targetCnt - 1);
            Vector2 direct  = Quaternion.Euler(0f, 0f, angle) * Vector2.right;

            Fire(direct).Forget();
        }
    }

    public override async UniTask Fire(Vector2 direct)
    {
        Vector3 startPos = _scanner.GetMyPosition();
        var projectile = await LoadProjectile<BaseProjectile>(SkillData.prefabName, startPos);

        if (null == projectile)
        {
            return;
        }

        projectile.Set(SkillData.damage, Camp.Enemy, null, direct);
    }

    public override void Despawn()
    {
        base.Despawn();
    }
}
