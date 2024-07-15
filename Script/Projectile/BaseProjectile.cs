using CommonEnum;
using ICharacter;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigid;
    

    private Camp        _targetCamp;
    private UnityAction _despawn;
    private int         _dmg;
    private float       _speed;

    private Vector2             _startPoint;
    private System.IDisposable  _moveDispose;

    public void Set( int dmg, Camp targetCamp , UnityAction despawn, Vector2 direction , float speed = 10f)
    {
        _dmg            = dmg;
        _targetCamp     = targetCamp;
        _despawn        = despawn;
        _startPoint     = transform.position;
        _speed          = speed;

        transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);

        _moveDispose =
            Observable
            .EveryFixedUpdate()
            .TakeUntilDisable(this)
            .Subscribe(l =>
            {
                float distance = Vector2.Distance(_startPoint, transform.position);
                if ( 10f < distance )
                {
                    _despawn?.Invoke();
                    Despawn();
                }

                _rigid.velocity = direction * speed;
            })
            .AddTo(this);
    }

    public void Despawn()
    {
        _moveDispose?.Dispose();
        gameObject.SetActive(false);
    }

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = _targetCamp == Camp.Ally ? "Player" : "Monster";

        if (false == collision.CompareTag(tag))
        {
            return;
        }

        if( false == collision.TryGetComponent(out BaseCharacter target) )
        {
            return;
        }

        if( target.HP > 0 )
        {
            IHealth ihealth = target as IHealth;
            ihealth?.OnDamage(_dmg);

            _despawn?.Invoke();
            Despawn();
        }
    }
}
