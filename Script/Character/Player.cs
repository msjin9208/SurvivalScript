using ICharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UniRx.Triggers;
using System.Linq;

public class Player : BaseCharacter, IAttacker , IMove, IHealth
{
    [SerializeField] 
    protected Rigidbody2D           _rigid;

    protected Scanner               _scanner;
    private System.IDisposable      _attackDispose;

    protected bool                  _isActive       = false;
    private bool                    _isDeath        = false;
    protected bool                  _invincibility  = false;

    public Subject<BaseItem>        GetItem { private set; get; }
    public Dictionary<int, BaseSkill> SkillDic { private set; get; }

    public bool HasSkill(int index)
        => SkillDic.ContainsKey(index);
    public BaseSkill GetSkill(int index)
        => SkillDic.GetValueOrDefault(index);

    public override void Initialize()
    {
        base.Initialize();

        GetItem     = new Subject<BaseItem>();
        SkillDic    = new Dictionary<int, BaseSkill>();

        SetMove();
        SetAttack();
    }

    public void Move( Vector2 direct )
    {
        if ( true == _isActive )
            return;

        PlayAnimation("Run");

        Vector2 moveSet = direct * (Speed * Time.fixedDeltaTime);
        Vector2 curSet = _rigid.position;
        _rigid.MovePosition( curSet +  moveSet );
        if( direct.x != 0 )
        {
            int look                = direct.x > 0 ? 1 : -1;
            transform.localScale    = new Vector3(look, 1, 1);
        }
    }

    public void Stop()
    {

    }

    public virtual void SetAttack( )
    {
        if( TryGetComponent(out _scanner))
        {

        }
        else
        {
            _scanner = gameObject.AddComponent<Scanner>();
        }

        _scanner.Initialize(3f);
    }

    private void SetMove()
    {
        bool move = false;
        this.UpdateAsObservable()
            .Subscribe(l =>
            {
                float x = Input.GetAxisRaw("Horizontal");
                float y = Input.GetAxisRaw("Vertical");
                move    = x != 0 || y != 0;
                
                if (move)
                {
                    Vector2 target = new Vector2(x, y).normalized;

                    _scanner.SetMyDirection(target);
                    Move(target);
                }
                else if( false == move && false == _isActive )
                {
                    PlayAnimation("Idle");
                }
            })
            .AddTo(this);
    }

    private void SetDash()
    {
        this.UpdateAsObservable()
            .Subscribe(l =>
            {

            })
            .AddTo(this);
    }

    public void SetHealth()
    {
        this.ObserveEveryValueChanged(l => HP)
            .TakeUntilDisable(this)
            .Subscribe(hp =>
            {
                _isDeath = hp <= 0;
                if( _isDeath )
                {
                    Death().Forget();
                }
            })
            .AddTo(this);
    }

    public async UniTask Death()
    {
        PlayAnimation("Death");

        GetItem.Dispose();
    }

    public void AddSkill<T>( T skill ) where T : BaseSkill 
    {
        SkillDic.Add(skill.Index, skill);

        skill.PlaySkill(_scanner);
    }

    public virtual void OnDamage(int decrease)
    {
        _stat.Add(CommonEnum.StatType.Hp, -decrease);
    }

    public virtual void OnHeal(int add)
    {
        _stat.Add(CommonEnum.StatType.Hp, add);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (false == collision.gameObject.CompareTag("Item"))
            return;
        if (false == collision.gameObject.TryGetComponent(out BaseItem item))
            return;

        GetItem.OnNext(item);
    }
}
