using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using Object = UnityEngine.Object;
using CommonEnum;

public class AssetManager : MonoSingleton<AssetManager>
{
    private enum Label
    {
        Preload,
    }

    static private Dictionary<string, Object>  _assets;
    static private List<string>                _primaryList;
    public long                  TotalDownloadSize { private set; get; } = 0L;

    static public async UniTask Initialize( )
    {
        _assets = new Dictionary<string, Object>( );

        Release();

        await Instance.InitializeAssets();
    }

    /// <summary>
    /// Get Assets
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    static public async UniTask<T> GetAsset<T>( string key ) where T : Object
    {
        T result = null;

        if( _assets.TryGetValue( key, out var obj ) )
        {
            result = obj as T;
        }
        else
        {
            result = await Instance.LoadAssetAsync<T>(key);
            return result;
        }

        return result is T ? result : default;
    }

    static public async UniTask<T> GetTable<T>( Table table ) where T : TableData
    {
        string data = table.ToString();
        T result    = await GetAsset<T>(data);

        return result;
    }

    /// <summary>
    /// Initialize Addressable & Check for update
    /// </summary>
    /// <returns></returns>
    private async UniTask InitializeAssets()
    {
        await Addressables.InitializeAsync();

        await CheckForCatalog();
    }

    #region [ Check for download ]

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private async UniTask CheckForCatalog()
    {
        List<string> checkList = await Addressables.CheckForCatalogUpdates();
        
        await CheckCatalogForUpdate(checkList);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="checkList"></param>
    /// <returns></returns>
    private async UniTask CheckCatalogForUpdate( List<string> checkList )
    {
        if( 0 < checkList.Count )
        {
            await Addressables.UpdateCatalogs(checkList);
        }

        await SetDownLoadSize();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private async UniTask SetDownLoadSize()
    {
        var result          = await Addressables.LoadResourceLocationsAsync("Bundle");

        _primaryList        = result.Select(l => l.PrimaryKey).ToList();
        TotalDownloadSize   = await Addressables.GetDownloadSizeAsync(_primaryList);
    }
    #endregion

    #region [ Load ]
    private async UniTask<T> LoadAssetAsync<T>( string key ) where T : Object
    {
        if (true == _assets.ContainsKey(key))
            return default;

        T load  = null;
        load    = await Addressables.LoadAssetAsync<T>(key);

        if( null != load )
            _assets.Add(key, load);

        return load;
    }

    static public async UniTask LoadAllAssetByLabel( )
    {
        List<string> labels     = Enum.GetNames(typeof(Label)).ToList();
        List<UniTask> taskList  = new List<UniTask>();

        for( int i = 0; i < labels.Count; i++ )
        {
            taskList.Add(Instance.LoadAssetByLabel(labels[i]));
        }

        foreach (var task in taskList)
            await task;
    }

    private async UniTask LoadAssetByLabel( string label )
    {
        var assets = await Addressables.LoadResourceLocationsAsync(label, typeof(Object));

        var handle = await Addressables.LoadAssetsAsync<Object>(assets, (result) =>
        {
            Debug.Log($"Load Asset : {result.name}");

            if (false == _assets.ContainsKey(result.name))
                _assets.Add(result.name, result);
        });

        Addressables.Release(handle);
    }
    #endregion

    #region [ Release ]
    static public void Release( )
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }
    #endregion
}

