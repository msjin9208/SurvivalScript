using Cysharp.Threading.Tasks;
using Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

namespace Resource
{
    public class Pool
    {
        private GameObject _prefab;
        private GameObject _parent;

        private List<GameObject> _list;

        public Pool( GameObject prefab, GameObject parent )
        {
            _prefab = prefab;
            _parent = parent;

            _list = new List<GameObject>();
        }

        public GameObject Get( )
        {
            if (null == _prefab)
            {
                Debug.Log("Prefab is null !!!");
                return null;
            }

            return Find();
        }

        public void Destroy()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                Object.Destroy( _list[i] );
            }

            _list.Clear();
        }

        private GameObject Find( )
        {
            GameObject go = null;

            for( int i = _list.Count - 1; i >= 0; i-- )
            {
                go = _list[i];

                if( null == go )
                {
                    _list.RemoveAt(i);
                }
                else if( false == go.activeInHierarchy && _parent.transform == go.transform.parent )
                {
                    return go;
                }
            }

            return Add( );
        }

        private GameObject Add( )
        {
            if( null == _prefab )
            {
                Debug.Log("Prefab is Null !!!");
                return default;
            }

            GameObject clone = GameObject.Instantiate( _prefab );

            clone.SetActive(false);

            Transform transform = clone.transform;

            transform.SetParent(_parent.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale    = Vector3.one;

            clone.name = clone.name.Replace("(Clone)", "");

            _list.Add( clone );

            return clone;
        }
    }
}


public class ResourcePoolManager : MonoSingleton<ResourcePoolManager>
{
    static private Dictionary<string, Pool> _resourceDic;

    static public void Initialize()
    {
        _resourceDic = new Dictionary<string, Pool>();
    }

    public async UniTask<Sprite> GetSprite(string atlas , string name)
    {
        var atlasSprite = await AssetManager.GetAsset<SpriteAtlas>(atlas);
        if (null == atlasSprite)
            return null;

        Sprite sprite = atlasSprite.GetSprite(name);

        return sprite;
    }

    public async UniTask<T> Get<T>( string key , Transform parent = null, bool posInit = false, bool active = true )
    {
        if( true == string.IsNullOrEmpty(key) )
        {
            Debug.Log($"{key} is not available");
            return default;
        }

        Pool pool = null;

        if( false == _resourceDic.TryGetValue( key, out pool ) )
        {
            pool =  await MakePoolByAssetsData(key);
        }

        GameObject get = pool.Get();

        if( null == get )
        {
            return default;
        }
        else
        {
            Transform getTransform = get.transform;

            getTransform.SetParent(parent == null ? getTransform : parent);

            if( posInit )
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale    = Vector3.one;
            }

            if (get.TryGetComponent(out T component))
            {
                get.SetActive(active);

                return component;
            }
            else
            {
                return default;
            }
        }
    }

    private async UniTask<Pool> MakePoolByAssetsData( string key )
    {
        GameObject load = await AssetManager.GetAsset<GameObject>(key);

        _resourceDic[key] = new Pool( load, gameObject );

        return _resourceDic[key];
    }

    public void Despawn( GameObject obj )
    {
        obj.transform.SetParent(transform);
        obj.SetActive(false);
    }
}
