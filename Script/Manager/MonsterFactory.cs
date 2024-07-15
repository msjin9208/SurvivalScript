using CommonEnum;
using Cysharp.Threading.Tasks;
using ICharacter;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterFactory : MonoBehaviour
{
    public static MonsterFactory Instance { private set; get; }

    private List<MonsterStat>   _monsterStatDataList;
    private List<Monster>       _monsterList;

    public async UniTask Initialize()
    {
        Instance = this;

        _monsterList = new List<Monster>();
        await LoadMonsterStatus();
    }

    private async UniTask LoadMonsterStatus()
    {
        var status              = await AssetManager.GetTable<MonsterTableData>(Table.MonsterTableData);
        _monsterStatDataList    = status.MonsterStatList.ToList();
    }

    public async UniTask<T> SpawnToPosition<T>( Vector2 position , int index , BaseCharacter target, Transform parent) where T : BaseCharacter
    {
        MonsterStat stat = _monsterStatDataList.Find(stat => stat.Index == index);
        if( stat.Index <= 0 )
            return null;

        T monster                   = await LoadMonster<T>(stat.PrefabName, parent);
        monster.transform.position  = position;

        monster.gameObject.SetActive(true);
        monster.Initialize();

        SetMonsterData( monster, stat );
        DoBehaivor( monster, target );

        return monster as T;
    }

    public void Despawn( GameObject obj )
    {
        obj.SetActive( false );
    }

    private async UniTask<T> LoadMonster<T>(string key, Transform parent) where T : BaseCharacter
    {
        BaseCharacter character = FindOffMonster();

        if( null == character )
        {
            character = await ResourcePoolManager.Instance.Get<BaseCharacter>(key, parent);
            if (null != character)
                _monsterList.Add(character as Monster);
        }


        return character as T;
    }

    private BaseCharacter FindOffMonster()
    {
        BaseCharacter character = _monsterList.Find(monster => monster.gameObject.activeSelf == false);

        return character;
    }

    private void SetMonsterData( BaseCharacter character , MonsterStat stat )
    {
        IStat iStat = character as IStat;
        if( null != iStat)
        {
            BaseStat newStat = new BaseStat(stat.HP, stat.Attack, stat.Speed, stat.AtkSpeed);

            iStat.SetStat(newStat);
        }

        IHealth iHealth = character as IHealth;
        if (null != iHealth)
            iHealth.SetHealth();
    }

    private void DoBehaivor( BaseCharacter character , BaseCharacter target )
    {
        if ( null == target || character is Player )
            return;

        IChase chase = character as IChase;
        if( null != chase )
            chase.ToChase(target);
    }
}
