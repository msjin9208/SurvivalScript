using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public class MapManager : MonoBehaviour
{
    [SerializeField] BaseMap[]  _mapList;

    private StatSystem          _statSystem;
    public bool                 StatSystemOn => _statSystem.SystemOn;

    public void Initialize( )
    {
        InitStatSystem().Forget();
    }

    private async UniTask InitStatSystem()
    {
        _statSystem = await ResourcePoolManager.Instance.Get<StatSystem>("StatSystem", transform, active:false);
        _statSystem.Initialize();
    }

    public void SpawnStatSystem( Vector2 standard )
    {
        Vector2 target                  = GetRandomPositionForSpawn(standard);
        _statSystem.transform.position  = target;

        _statSystem.Play();
    }

    public void DespawnStatSystem()
    {
        _statSystem.Despawn();
    }

    public void SetCharacter( GameObject player )
    {
        for (int i = 0; i < _mapList.Length; i++)
            _mapList[i].SetMove( player );
    }


    public Vector2 GetRandomPositionForSpawn( Vector2 standard )
    {
        float distance  = Random.Range(10, 30);
        int direction   = Random.Range(0, 360);
        float x         = Mathf.Cos(direction * Mathf.Deg2Rad) * distance;
        float y         = Mathf.Sin(direction * Mathf.Deg2Rad) * distance;

        Vector2 get     = standard + new Vector2(x, y);

        return get;
    }
}
