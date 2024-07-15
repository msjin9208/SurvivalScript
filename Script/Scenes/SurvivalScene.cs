using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ICharacter;
using ISkill;

public class SurvivalScene : BaseScene
{
    private SurvivalSceneUI _ui;
    private BattleCore      _core;

    private System.IDisposable _trackingCamera;

    public override void Init()
    {
        base.Init();
    }

    public override void DataReset()
    {
        base.DataReset();
    }

    public override async void Enter()
    {
        base.Enter();

        InitGame();

        if( null == _ui )
        {
            await OnLoadUI();
        }

        await InitCore();
        InitCamera();
        InitUI();

        _core.NextState();
    }

    public override void Exit()
    {
        _trackingCamera?.Dispose();
        GamePause?.Dispose();

        base.Exit();
    }

    public override async UniTask OnLoadUI()
    {
        _ui = await MainUI.GetSceneUI<SurvivalSceneUI>(MainUI.UIType.Survival);
        _ui?.Init();
    }

    private async UniTask InitCore()
    {
        if( null == _core )
        {
            GameObject core = new GameObject("[CORE]");
            _core           = core.AddComponent<BattleCore>();
        }

        await _core.InitCore();
        InitPlayer().Forget();
    }

    #region [ PLAYER ]
    private async UniTask InitPlayer()
    {
        BaseCharacter character = _core.Player;

        IDash dash = character as IDash;
        if ( null != dash )
        {
            CoolTimeUI coolTimeUI = await _ui.CreateCool();
            coolTimeUI.SetIndex(0);
            coolTimeUI.SetRatingColor(CommonEnum.SkillRate.Common);
            coolTimeUI.SetSkillName("Dash");

            this.ObserveEveryValueChanged(l => dash.GetDashCoolTime())
                .Subscribe(time => 
                {
                    float max = dash.GetDashMaxCoolTime();
                    coolTimeUI.FillSubject.OnNext( time / max );
                })
                .AddTo(character);
        }
    }
    #endregion

    #region [ CAMERA ]
    private void InitCamera()
    {
        SetCamera();
    }
    
    private void SetCamera( )
    {
        _trackingCamera = Observable.EveryFixedUpdate()
            .Subscribe(l => 
            {
                Vector3 position                = _core.Player.transform.position;
                Vector3 nextPosition            = new Vector3(position.x, position.y, -10f);
                Camera.main.transform.position  = nextPosition;
            });
    }
    #endregion

    #region [ UI ]
    private void InitUI()
    {
        this.ObserveEveryValueChanged(l => _core.GameTime)
            .Subscribe(tick => _ui.Timer.OnNext(tick))
            .AddTo(_core.gameObject);

        Player player = _core.Player;

        int playerHp = player.HP;
        _ui.SetPlayerMaxHp(playerHp);

        this.ObserveEveryValueChanged(l => player.HP)
            .Subscribe(hp => _ui.HpValue.OnNext(hp))
            .AddTo(_core.Player.gameObject);

        this.ObserveEveryValueChanged(l => _core.MaxExp)
            .Subscribe(max => _ui.MaxExpValue.OnNext(max))
            .AddTo(_ui);

        this.ObserveEveryValueChanged(l => _core.Exp)
            .Subscribe(exp => _ui.ExpValue.OnNext(exp))
            .AddTo(_ui);

        this.ObserveEveryValueChanged(l => _core.Level)
            .Subscribe(level => _ui.LevelValue.OnNext(level))
            .AddTo(_ui);

        this.ObserveEveryValueChanged(l => _core.OpenStatSystemUI)
            .Where(open => open == true)
            .Subscribe(l => 
            {
                GamePause.OnNext(true);
                _ui.OpenStatUI( player.Stat, ( stat )=> 
                {
                    GamePause.OnNext(false);
                    _core.StatSystemComplete(stat);
                });
            } )
            .AddTo(_ui);

        this.ObserveEveryValueChanged(l => _core.Wave)
            .Subscribe( wave =>
            {
                _ui.ViewWave(wave);
            } )
            .AddTo(_ui);

        this.ObserveEveryValueChanged( l => _core.RemainSpawnCnt )
            .Subscribe(cnt =>
            {
                _ui.ViewMonsterCount(cnt);
            })
            .AddTo(_ui);

        this.ObserveEveryValueChanged(l => _core.ViewSkill)
            .Where(view => true == view)
            .Subscribe(l =>
            {
                GamePause.OnNext(true);

                UI_SkillData[] skills = _core.CreateRandomSkill();
                _ui.ViewSkillUI(skills, (result) =>
                {
                    GamePause.OnNext(false);

                    AddSkill(result);
                });
            })
            .AddTo(_ui);
    }

    private void AddSkill( int index )
    {
        BaseSkill skill = _core.AddSkillToPlayer(index);
        SetCoolTime(skill).Forget();

        GamePause.OnNext(false);
        _core.StartWave();
    }

    private async UniTask SetCoolTime( BaseSkill skill )
    {
        ICoolTime coolTime = skill as ICoolTime;
        if( null != coolTime )
        {
            var ui = await _ui.CreateCool();

            ui.SetIndex(skill.Index);
            ui.SetRatingColor(skill.Rate);
            ui.SetSkillName(skill.SkillName);

            this.ObserveEveryValueChanged(l => coolTime.GetCoolTime())
                .Subscribe(time =>
                {
                    float max = coolTime.GetMaxCoolTime();

                    ui.FillSubject.OnNext( time / max );
                })
                .AddTo(ui);
        }
    }

    #endregion

    #region [ GAME ]
    public Subject<bool> GamePause { private set; get; }

    private void InitGame()
    {
        GamePause = new Subject<bool>();
        GamePause.Subscribe( value => Pause(value) );
    }

    private void Pause( bool pause )
    {
        Time.timeScale = pause ? 0 : 1;
    }
    #endregion
}
