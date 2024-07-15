using CommonEnum;
using Cysharp.Threading.Tasks;
using ISkill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class StraightShoot : ShootSkill, ICoolTime
{
    private System.IDisposable  _coolTimeDispose;
    private float               _coolTime;
    private float               _maxCoolTime;
    public float GetCoolTime()      => _coolTime;
    public float GetMaxCoolTime()   => _maxCoolTime;

    public override void Initialize( SkillData data )
    {
        base.Initialize( data );
    }

    public override void PlaySkill( Scanner scanner )
    {
        base.PlaySkill( scanner );

        _maxCoolTime = SkillData.coolTime;
        SetCoolTime();
    }

    public override async UniTask Fire(Vector2 direct)
    {
        Vector3 startPos    = _scanner.GetMyPosition();
        var projectile      = await LoadProjectile<BaseProjectile>( SkillData.prefabName , startPos );
        
        if( null == projectile )
        {
            return;
        }

        projectile.Set( SkillData.damage, Camp.Enemy , null, direct );
    }

    public override void Upgrade()
    {
        base.Upgrade();

        _maxCoolTime -= 0.5f;
    }

    public void SetCoolTime( )
    {
        _coolTime = _maxCoolTime;
        _coolTimeDispose = Observable
            .EveryUpdate()
            .Subscribe(l =>
            {
                _coolTime -= Time.deltaTime;
                if(_coolTime <= 0 )
                {
                    Vector3? target = _scanner.GetNearOnce();
                    if( null != target )
                    {
                        Fire(target.Value).Forget();

                        _coolTime = _maxCoolTime;
                    }
                    else
                    {
                        _coolTime = 0;
                    }
                }
                else
                {

                }

            }).AddTo(_scanner);
    }

    public override void Despawn()
    {
        base.Despawn();

        _coolTimeDispose?.Dispose();
        _coolTimeDispose = null;
    }
}
