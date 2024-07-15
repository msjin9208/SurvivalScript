using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ICharacter;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class Monster : BaseCharacter, IMove , IChase , IHealth
{
    [SerializeField] Rigidbody2D    _rigid;
    private bool                    _isDeath;

    public bool                     IsDeath => _isDeath;

    public UnityAction<Monster>     Despawn;

    public override void Initialize()
    {
        base.Initialize();

        _isDeath            = false;
        Despawn             = null;
    }

    #region [ CHASE ]

    public void ToChase( BaseCharacter target )
    {
        PlayAnimation("Run");

        Observable.EveryFixedUpdate()
            .Where( l => _isDeath == false )
            .Subscribe(l =>
            {
                Move(target.transform.position);
            })
            .AddTo(this);
    }

    #endregion

    #region [ MOVE ]
    public void Move(Vector2 direct)
    {
        float speed = 0;
        if (Vector2.Distance(direct, _rigid.position) > 10)
        {
            speed = 3;
        }
        else
        {
            speed = 1;
        }

        Vector2 dir     = direct - _rigid.position;
        Vector2 move    = dir.normalized * speed * Time.fixedDeltaTime;

        _rigid.MovePosition(_rigid.position + move);

        int look = move.x > 0 ? 1 : -1;
        transform.localScale = new Vector3(look, 1, 1);
    }

    public void Stop( ) { }

    #endregion

    #region [ ATK ]

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsDeath) return;
        if (false == collision.gameObject.CompareTag("Player")) return;
        if (false == collision.gameObject.TryGetComponent<BaseCharacter>(out var target)) return;

        IHealth ihealth = target as IHealth;
        ihealth?.OnDamage(Attack);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    
    }
    

    #endregion

    #region [ HP ]
    public void SetHealth()
    {
        this.ObserveEveryValueChanged(l => HP)
            .TakeUntilDisable(this)
            .Subscribe(hp => 
            {
                _isDeath = hp <= 0;
                if (_isDeath)
                {
                    Death( ).Forget( );
                }
            })
            .AddTo(this);
    }

    public async UniTask Death()
    {
        PlayAnimation("Death");

        await UniTask.Delay(System.TimeSpan.FromSeconds(2f));

        Despawn?.Invoke( this );
    }

    public void OnDamage(int decrease)
    {
        _stat.Add(CommonEnum.StatType.Hp, -decrease);
    }
    public void OnHeal(int add)
    {
        _stat.Add(CommonEnum.StatType.Hp, add);
    }
    #endregion
}
