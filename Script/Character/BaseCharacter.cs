using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ICharacter;

public class BaseCharacter : MonoBehaviour, IStat
{
    protected BaseStat _stat;

    public int      Attack => _stat.Attack;
    public int      HP => _stat.HP;
    public float    Speed => _stat.Speed;
    public float    AtkSpeed => _stat.AtkSpeed;

    public BaseStat Stat => _stat;

    [SerializeField]
    private Animator _animator;

    public virtual void Initialize( )
    {
        InitAnimator();
    }

    private void InitAnimator( )
    {
        
    }

    public float PlayAnimation( string animName )
    {
        _animator.Play( animName );
        
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public float PlayAnimationWithTrigger( string trigger )
    {
        _animator.SetTrigger(trigger);
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }
    

    public void SetStat( BaseStat stat )
    {
        _stat = stat;
    }

    public void AddStat( AddStats stat )
    {
        _stat.Add(CommonEnum.StatType.Attack, stat.attack);
        _stat.Add(CommonEnum.StatType.Hp, stat.hp);
        _stat.Add(CommonEnum.StatType.Speed, stat.speed);
        _stat.Add(CommonEnum.StatType.AtkSpeed, AtkSpeed);
    }
}
