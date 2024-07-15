using ICharacter;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using CommonEnum;

public class Knight : Player, IMelee, IDash
{
    [SerializeField] Transform      _attackArea;

    private AttackCombo             _attackCombo;
    private float                   _comboTime;

    private Subject<AttackCombo>    _attackSubject;
    private ReactiveProperty<bool>  _inputAttack;

    private Vector2                 _attackRange;
    private float                   _dashCoolTime;
    readonly private float          _dashMaxCoolTime = 3f;

    readonly private float          _dashSpeed = 10f;

    public override void Initialize()
    {
        base.Initialize();

        _dashCoolTime = _dashMaxCoolTime;
    }

    public override void SetAttack()
    {
        base.SetAttack();

        _isActive       = false;
        _comboTime      = 0;
        _attackCombo    = AttackCombo.Attack1;
        _attackSubject  = new Subject<AttackCombo>();
        _inputAttack    = new ReactiveProperty<bool>(false);
        _attackRange    = new Vector2( 1.5f, 1.5f );

        _attackSubject.Subscribe( l => Melee() ).AddTo(this);

        this.UpdateAsObservable()
            .Where( l => Input.GetKeyDown(KeyCode.Space) )
            .Where( l => _attackCombo < AttackCombo.Max )
            .ThrottleFirstFrame(3)
            .Subscribe(l =>
            {
                _comboTime = 0.5f;
                Melee();
            })
            .AddTo(this);

        Observable.EveryUpdate()
            .Subscribe(l =>
            {
                if (_dashCoolTime > 0)
                {
                    _dashCoolTime -= Time.deltaTime;
                    return;
                }
                else
                {
                    _dashCoolTime = 0;
                }
            })
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(l => Input.GetKeyDown(KeyCode.LeftShift))
            .Subscribe(l => 
            {
                if ( _dashCoolTime > 0 )
                    return;

                _isActive       = true;
                _invincibility  = true;

                float x = Input.GetAxisRaw("Horizontal");
                float y = Input.GetAxisRaw("Vertical");

                Vector2 direct = new Vector2(x, y);

                float time = PlayAnimationWithTrigger("Dash");

                Dash(direct, 0.6f).Forget();
            })
            .AddTo(this);

        
        this.UpdateAsObservable()
            .Subscribe(l =>
            {
                if ( _attackCombo > AttackCombo.Stand )
                {
                    _comboTime -= _comboTime > 0 ? Time.deltaTime : 0;
                    if( _comboTime <= 0 )
                    {
                        _attackCombo    = AttackCombo.Stand;
                        _isActive       = false;
                    }
                }

            }).AddTo(this);
        
    }

    public float GetDashCoolTime() => _dashCoolTime;
    public float GetDashMaxCoolTime() => _dashMaxCoolTime;

    public async UniTask Dash( Vector2 direct, float time )
    {
        
        float duration  = time;

        if (direct == Vector2.zero)
        {
            _isActive = false;
            return;
        }

        DashMove(direct);

        await UniTask.WaitForSeconds(duration);

        _rigid.velocity = Vector2.zero;
        _dashCoolTime   = _dashMaxCoolTime;
        _invincibility  = false;
        _isActive       = false;
    }

    public void DashMove(Vector2 direct)
    {
        _rigid.velocity = direct.normalized * _dashSpeed;

        if (direct.x != 0)
        {
            int look = direct.x > 0 ? 1 : -1;
            transform.localScale = new Vector3(look, 1, 1);
        }
    }

    public override void OnDamage(int decrease)
    {
        if (true == _invincibility)
            return;

        base.OnDamage(decrease);

        PlayAnimation("Hit");
    }

    public override void OnHeal(int add)
    {
        base.OnHeal(add);
    }

    public void Melee( )
    {
        Debug.Log("Attack");

        _isActive       = true;
        _attackCombo    += 1;
        if (_attackCombo >= AttackCombo.Max)
            return;

        float time = PlayAnimationWithTrigger(_attackCombo.ToString());

        Hit();
    }

    public void Melee(Vector2 direct)
    {

    }

    public void Hit()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(_attackArea.position, _attackRange, 0);

        if ( null != hit )
        {
            foreach ( var collider in hit )
            {
                Monster monster = null;
                if( true == collider.TryGetComponent(out monster) )
                {
                    IHealth health = monster;
                    health?.OnDamage(Attack);

                    OnHit(monster.transform.position).Forget();
                }
            }
        }
    }

    private async UniTask OnHit( Vector2 target, string res = "Hit_1" )
    {
        var hit = await ResourcePoolManager.Instance.Get<AutoDespawn>(res);
        hit.transform.position = target;

        hit.Play();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_attackArea.position, _attackRange);
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
