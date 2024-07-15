using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    private List<Item>                  _itemDataList;
    private Dictionary<int, BaseItem>   _itemDic;

    public async UniTask Initialize()
    {
        await LoadData();
        _itemDic = new Dictionary<int, BaseItem>();
    }

    private async UniTask LoadData()
    {
        var itemData    = await AssetManager.GetTable<ItemTableData>(CommonEnum.Table.ItemTableData);
        _itemDataList   = itemData.ItemList.ToList();
    }

    public async UniTask CreateItem( int index, int value )
    {

    }

    public async UniTask<BaseItem> CreateItem( int index )
    {
        Item? data = _itemDataList.Find( item => item.Index == index );
        if (null == data)
            return null;

        
        var item = await ResourcePoolManager.Instance.Get<BaseItem>( data.Value.Prefab, transform );

        item.Initialize( data.Value );

        return item;
    }
}
