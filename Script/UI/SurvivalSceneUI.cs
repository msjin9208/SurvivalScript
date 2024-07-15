using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;
using Utility;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using ICharacter;
using ISkill;

public class SurvivalSceneUI : BaseSceneUI
{
    #region [ PLAYER UI ]

    [Header("HP")]
    [SerializeField] HPBar              _hpBar;
    [Space(10)]
    [Header("TIMER")]
    [SerializeField] TextMeshProUGUI    _timerTxt;
    [Space(10)]
    [Header("PAD")]
    [SerializeField] JoyStick           _pad;
    [Space(10)]
    [Header("LEVEL")]
    [SerializeField] Image              _expImage;
    [SerializeField] TextMeshProUGUI    _levelTxt;
    [SerializeField] TextMeshProUGUI    _expText;
    [SerializeField] GameObject         _levelUpGo;
    [Space(10)]
    [Header("STAT")]
    [SerializeField] StatSystemUI       _statSystemUI;
    [Space(10)]
    [Header("WAVE")]
    [SerializeField] TextMeshProUGUI    _waveTxt;
    [SerializeField] TextMeshProUGUI    _monsterCntTxt;
    [Space(10)]
    [Header("SKILL")]
    [SerializeField] SkillSystemUI      _skillSystemUI;
    [Space(10)]

    [SerializeField] Transform          _coolRoot;
    

    public Subject<float>   Timer { private set; get; }
    public Subject<int>     HpValue { private set; get; }
    public Subject<Vector3> HpPosition { private set; get; }
    public Subject<int>     LevelValue { private set; get; }
    public Subject<int>     ExpValue { private set; get; }
    public Subject<int>     MaxExpValue { private set; get; }

    public Vector3          PlayerDirection => _pad.Direction;
    public bool             PlayerMove => _pad.Move;


    private int _maxExp;

    private List<CoolTimeUI> _coolTimeUIList;
    #endregion

    public override void Init()
    {
        base.Init();

        InitTimer();
        InitHPBar();
        InitLevel();
        InitExp();
        InitStatSystemUI();
        InitViewSkill();
        InitCoolTimeUI();
    }

    private void InitTimer()
    {
        Timer?.Dispose();
        Timer = new Subject<float>();
        Timer.Subscribe(tick => ViewTime(tick)).AddTo(this);
    }

 

    public override void DataReset()
    {
        base.DataReset();
    }

    public override void SetActive(bool active)
    {
        base.SetActive(active);
    }

    #region [ TIMER ]

    private void ViewTime( float tick )
    {
        string time = string.Empty;
        if( tick <= 0 )
        {
            time = "FINISH";
        }
        else
        {
            int min = Mathf.FloorToInt(tick / 60);
            int sec = Mathf.FloorToInt(tick % 60);
            time    = $"{min}m {sec}s";
        }

        Utility.UIUtility.SetText(_timerTxt, time);
    }
    #endregion

    #region [ HP ]
    private void InitHPBar()
    {
        if (null == _hpBar)
        {
            Debug.Log("HP Bar is null");
            return;
        }

        _hpBar.Initialize();

        HpValue?.Dispose();
        HpValue = new Subject<int>();
        HpValue.Subscribe(hp => UpdateHp(hp)).AddTo(this);
    }

    private void UpdateHp( int hp )
    {
        _hpBar.HpSubject.OnNext(hp);
    }

    public void SetPlayerMaxHp(int maxHp)
    {
        _hpBar.SetMaxHp(maxHp);
    }
    #endregion

    #region [ LEVEL ]
    private void InitLevel()
    {
        _levelUpGo.SetActive(false);

        if( null != LevelValue )
        {
            LevelValue.Dispose();
            LevelValue = null;
        }

        LevelValue = new Subject<int>();
        LevelValue.Subscribe( level => UpdateLevel(level)).AddTo(this);
    }

    private void UpdateLevel(int level)
    {
        string result = string.Format($"Lv. {level}");
        UIUtility.SetText(_levelTxt, result);

        if( level != 1 )
            PlayLevelUp().Forget();
    }
    private async UniTask PlayLevelUp()
    {
        _levelUpGo.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        _levelUpGo.SetActive(false);
    }
    #endregion

    #region [ EXP ]
    private void InitExp()
    {
        ExpValue?.Dispose();
        ExpValue = new Subject<int>();
        ExpValue.Subscribe(exp => UpdateExp(exp)).AddTo(this);

        MaxExpValue?.Dispose();
        MaxExpValue = new Subject<int>();
        MaxExpValue.Subscribe(max => RefreshExp(max)).AddTo(this);

        _expImage.fillAmount = 0;
    }
    private void UpdateExp( int value )
    {
        float result            = (float)value / (float)_maxExp;
        _expImage.fillAmount    = result;

        UIUtility.SetText(_expText, string.Format($"{value} / {_maxExp}"));
    }
    private void RefreshExp(int maxExp)
    {
        _maxExp = maxExp;
    }
    #endregion

    #region [ STAT ]

    private void InitStatSystemUI()
    {
        _statSystemUI.Close();
    }

    public void OpenStatUI( BaseStat stat, UnityAction<AddStats?> complete )
    {
        Debug.Log("Open Stat UI");
        _statSystemUI.Open(stat, complete );
    }


    #endregion

    #region [ WAVE ]
    public void ViewWave( int wave )
    {
        string view = string.Format($" WAVE : {wave}");
        UIUtility.SetText( _waveTxt, view ); 
    }
    #endregion

    public void ViewMonsterCount(int cnt)
    {
        string view = string.Format($"MON : {cnt}");
        UIUtility.SetText(_monsterCntTxt, view);
    }

    #region [ COOL ]
    private void InitCoolTimeUI()
    {
        _coolTimeUIList = new List<CoolTimeUI>();
    }
    public async UniTask<CoolTimeUI> CreateCool( )
    {
        CoolTimeUI ui = await ResourcePoolManager.Instance.Get<CoolTimeUI>("CoolTimeUI", _coolRoot);

        if (null == ui)
            return null;

        ui.Initialize();

        _coolTimeUIList.Add( ui );

        RectTransform rect = null;
        _coolRoot.TryGetComponent(out rect);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        return ui;
    }
    #endregion

    #region [ SKILL ]

    public void InitViewSkill()
    {
        _skillSystemUI.Initialize();
        _skillSystemUI.gameObject.SetActive(false);
    }

    public void ViewSkillUI( UI_SkillData[] skill , UnityAction<int> onComplete )
    {
        _skillSystemUI.ViewSkills(skill, onComplete);
    }

    #endregion
}
