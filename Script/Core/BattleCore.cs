using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonEnum;
using UniRx;
using ICharacter;
using System.Linq;
using UnityEngine.Rendering;
using IState;
using ISkill;

public class BattleCore : MonoBehaviour
{
    #region [ ROOT ]
    private GameObject _playerRoot;
    private GameObject _monsterRoot;
    #endregion

    #region [ MAP ]
    [SerializeField] private MapManager         _mapManager;
    public bool                                 OpenStatSystemUI => _mapManager.StatSystemOn;
    #endregion

    #region [ STATE DATA ]
    private Dictionary<BattleState, BaseState>  _stateDic;
    private BattleState                         _curState;
    private System.IDisposable                  _stateExcute;
    #endregion

    #region [ PLAYER DATA ]
    public Player       Player { private set; get; }
    private PlayerData  PlayerData { set; get; }
    public int          Level => PlayerData.Level;
    public int          Exp => PlayerData.Exp;
    public int          MaxExp => PlayerData.ExpMax;
    public int          Gold => PlayerData.Gold;

    public Subject<Vector3> MoveCharacter { private set; get; }
    #endregion

    #region [ MONSTER DATA ]
    private List<Monster> _monsterList;

    public int MonsterCnt => _monsterList.Count;
    #endregion

    #region [ TIME DATA ]
   

    private float   _gameTime;
    public float    GameTime => _gameTime;
    #endregion

    #region [ WAVE DATA ]
    private int     _wave;
    public int      Wave => _wave;
    public int      RemainSpawnCnt { private set; get; }
    #endregion

    #region [ ITEM ]
    private ItemFactory         _itemFactory;
    private Subject<Vector3>    _createItemSubject;
    #endregion

    #region [ SKILL ]
    private SkillManager    _skillMgr;
    public bool             ViewSkill { private set; get; } = false;
    #endregion


    public async UniTask InitCore( )
    {
        InitData();
        InitState();
        InitRoot();
        InitSkill();
        
        await Utility.TaskUtility.WaitTasks(InitFactory(), SetPlayer() ,InitMap());
    }

    private void InitData()
    {
        PlayerData      = new PlayerData();
        _monsterList    = new List<Monster>();

        SetWave();
    }

    private void InitRoot()
    {
        _playerRoot     = new GameObject("PlayerRoot");
        _monsterRoot    = new GameObject("MonsterRoot");
    }


    #region [ STATE ]
    private void InitState()
    {
        _stateDic = new Dictionary<BattleState, BaseState>();

        _stateDic[BattleState.Stand]    = new BattleStand(this);
        _stateDic[BattleState.Start]    = new BattleStart(this);
        _stateDic[BattleState.Wave]     = new BattleWave(this);
        _stateDic[BattleState.WaveEnd]  = new BattleWaveEnd(this);
        _stateDic[BattleState.Skill]    = new BattleSkill(this);
        _stateDic[BattleState.End]      = new BattleEnd(this);
    }
    public void NextState( )
    {
        ChangeState( _curState + 1 );
    }
    public void ChangeState( BattleState state )
    {
        Debug.Log($"Next State : {state}");

        BattleState next = state;
        BattleState prev = _curState;

        if (BattleState.MAX <= next)
        {
            next = BattleState.Stand;
        }

        if (BattleState.None != prev)
        {
            _stateDic[prev].Exit();
        }

        _curState = next;
        _stateDic[_curState].Enter();

        if (null != _stateExcute)
            _stateExcute.Dispose();

        IExcute excute = _stateDic[_curState] as IExcute;
        if (null != excute)
        {
            _stateExcute = Observable
                .EveryUpdate()
                .Subscribe(l =>
                {
                    excute.Excute();
                }).AddTo(this);
        }
    }
    #endregion

    #region [ TIME ]
    public bool DecreaseTime(float time)
    {
        _gameTime -= time;

        return _gameTime <= 0;
    }
   
    public void SetTime( float time )
    {
        _gameTime = time;
    }
    #endregion

    #region [ WAVE ]
    public void StartWave()
    {
        ChangeState(BattleState.Stand);
    }

    private void SetWave()
    {
        _wave = 0;
    }
    public void AddWave(int add = 1)
    {
        _wave += add;
    }

    public void SetMonsterCount( int cnt ) 
    {
        RemainSpawnCnt = cnt;
    }

    public bool CheckWaveTime()
    {
        return _gameTime <= 0;
    }

    public bool CheckMonsterCount()
    {
        return _monsterList.Count <= 0;
    }

    public bool CheckGameEnd()
    {
        return _gameTime > 0 && _monsterList.Count <= 0;
    }
    #endregion

    #region [ MAP ]
    private async UniTask InitMap()
    {
        _mapManager = await ResourcePoolManager.Instance.Get<MapManager>("Map_1", transform);

        _mapManager.Initialize();
        _mapManager.SetCharacter(Player.gameObject);

        this.ObserveEveryValueChanged(l => Level)
            .Where(level => level > 1)
            .Subscribe(l => _mapManager.SpawnStatSystem(Player.transform.position))
            .AddTo(_mapManager);
    }
    public void StatSystemComplete( AddStats? stat )
    {
        if( null != stat )
        {
            Player.AddStat(stat.Value);
        }

        _mapManager.DespawnStatSystem();
    }
    #endregion

    #region [ FACTORY ]
    private async UniTask InitFactory()
    {
        GameObject go       = new GameObject("Factory");
        var monsterFactory  = go.AddComponent<MonsterFactory>();
        var bulletFactory   = go.AddComponent<ProjectileFactory>();
        _itemFactory        = go.AddComponent<ItemFactory>();

        await Utility.TaskUtility.WaitTasks(monsterFactory.Initialize(), _itemFactory.Initialize());
        bulletFactory.Initialize();

        go.transform.SetParent(transform);
    }

    public void SpawnMonster( int cnt )
    {
        for( int i = 0; i < cnt; i++ )
        {
            SetMonster(1).Forget();
        }
    }

    public async UniTask SetMonster( int index )
    {
        Vector2 randPosition    = _mapManager.GetRandomPositionForSpawn(Player.transform.position);
        var monster             = await MonsterFactory.Instance.SpawnToPosition<Monster>(randPosition, 1, Player, _monsterRoot.transform);

        if (null == monster)
            return;

        _monsterList.Add(monster);

        monster.Despawn = DespawnMonster;

        monster
            .ObserveEveryValueChanged(l => l.IsDeath)
            .TakeUntilDisable(monster.gameObject)
            .Where( death => death == true )
            .Subscribe(l =>
            {
                if (null == _createItemSubject)
                {
                    _createItemSubject = new Subject<Vector3>();
                    _createItemSubject.Subscribe(target => CreateItem(target).Forget());
                }

                _createItemSubject.OnNext( monster.transform.position );
            })
            .AddTo(monster.gameObject);
    }

    private void DespawnMonster( Monster monster )
    {
        monster.Despawn = null;

        _monsterList.Remove(monster);

        MonsterFactory.Instance.Despawn(monster.gameObject);
    }
    #endregion

    #region [ PLAYER ]
    public async UniTask<BaseCharacter> SetPlayer( )
    {
        if( null == Player )
        {
            Player = await ResourcePoolManager.Instance.Get<Player>("Knight", _playerRoot.transform);
            Player?.Initialize();
        }

        IStat stat = Player as IStat;
        stat.SetStat(await GetPlayerStat(1));

        IHealth iheath = Player as IHealth;
        iheath?.SetHealth();

        Player.GetItem.Subscribe(item => ItemDespawn(item));

        MoveCharacter = new Subject<Vector3>();
        MoveCharacter.Subscribe(pos => Player.Move(pos)).AddTo(Player.gameObject);

        return Player;
    }

    private async UniTask<BaseStat> GetPlayerStat( int index )
    {
        var playerData  = await AssetManager.GetTable<PlayerTableData>(Table.PlayerTableData);
        var list        = playerData.PlayerStatList.ToList();
        var data        = list.Find(data => data.Index == index);

        BaseStat stat   = new BaseStat(data.HP, data.Attack, data.Speed, data.AttackSpeed);

        return stat;
    }

    #endregion

    #region [ ITEM ]
    
    private async UniTask CreateItem( Vector3 position )
    {
        var item = await _itemFactory.CreateItem(1);

        item.Get                = GetItem;
        item.transform.position = position;

        item.gameObject.SetActive(true);
    }

    private void ItemDespawn( BaseItem item )
    {
        item.Despawn();
    }
    private void GetItem( ItemType type, int value )
    {
        switch(type)
        {
            case ItemType.EXP:
                {
                    PlayerData.Add(PlayerData.DataType.EXP, value);
                }
                break;
            case ItemType.GOLD:
                {
                    PlayerData.Add(PlayerData.DataType.Gold, value);
                }
                break;
        }
    }
    #endregion

    #region [ SKILL ]

    private void InitSkill( )
    {
        _skillMgr = new SkillManager();
        _skillMgr.Initialize();
    }

    public UI_SkillData[] CreateRandomSkill( )
    {
        var skills              = _skillMgr.GetRandomSkill();
        UI_SkillData[] uiData   = new UI_SkillData[skills.Length];

        for( int i = 0; i < skills.Length; i++)
        {
            var skill = skills[i];

            UI_SkillData data;
            data.index      = skill.Index;
            data.title      = skill.Title;
            data.subject    = skill.Subject;
            data.rate       = skill.Rate;
            data.has        = Player.HasSkill(data.index);
            data.level      = data.has ? Player.GetSkill(data.index).Level : 0;

            uiData[i] = data;
        }

        return uiData;
    }

    public void SetViewSkill( bool view )
    {
        ViewSkill = view;
    }

    public BaseSkill AddSkillToPlayer( int index )
    {
        BaseSkill skill = Player.GetSkill(index);
        if (skill != null) 
        {
            skill.Upgrade();
        }
        else
        {
            skill = _skillMgr.CreateSkill<BaseSkill>(index);

            Player.AddSkill(skill);
        }

        return skill;
    }

    #endregion
}
