using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class StatSystem : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] _runes;

    bool    _inPlayer = false;
    Color   _onColor;
    Color   _offColor;

    readonly private float  _despawnTime = 30f;
    private float           _remainTime;

    System.IDisposable _onTheRunes;
    System.IDisposable _despawn;

    public bool SystemOn { private set; get; }

    public void Initialize()
    {
        _inPlayer = false;
        _offColor = new Color(1, 1, 1, 0);

        Off();
    }

    public void Play()
    {
        Dispose();

        _remainTime = _despawnTime;
        SystemOn    = false;
        _onColor    = new Color(1, 1, 1, 0);

        gameObject.SetActive(true);

        _despawn = Observable
            .Interval(System.TimeSpan.FromSeconds(_remainTime))
            .Subscribe(l =>
            {
                Despawn();
            })
            .AddTo(this);

        _onTheRunes = Observable
            .EveryFixedUpdate()
            .TakeUntilDisable(this)
            .Where(l => _inPlayer)
            .Where(l => SystemOn == false)
            .Subscribe(l =>
            {
                float alpha = _onColor.a;
                if (alpha >= 1)
                {
                    SystemOn = true;
                    return;
                }

                _onColor = new Color(1, 1, 1, alpha += Time.fixedDeltaTime);
                On();
            })
            .AddTo(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _onColor    = new Color(1, 1, 1, 0);
        _inPlayer   = true;
        SystemOn    = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerAway();
    }

    public void Despawn()
    {
        Dispose();
        PlayerAway();

        gameObject.SetActive(false);
    }

    private void Dispose()
    {
        if (null != _onTheRunes) 
        {
            _onTheRunes.Dispose();
            _onTheRunes = null;
        }

        if( null != _despawn )
        {
            _despawn.Dispose();
            _despawn = null;
        }
    }

    private void PlayerAway()
    {
        _inPlayer   = false;
        SystemOn    = false;
        Off();
    }

    private void On( )
    {
        for (int i = 0; i < _runes.Length; i++)
            _runes[i].color = _onColor;
    }

    private void Off()
    {
        for (int i = 0; i < _runes.Length; i++)
            _runes[i].color = _offColor;
    }
}
