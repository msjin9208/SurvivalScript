using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class ProjectileFactory : MonoBehaviour
{
    public static ProjectileFactory Instance { private set; get; }

    private List<BaseProjectile> _projectile;

    public void Initialize( )
    {
        Instance    = this;
        _projectile = new List<BaseProjectile>();
    }

    public async UniTask<BaseProjectile> Get( string key, Vector3 position )
    {
        var projectile = await Find(key);

        if ( null != position ) 
        {
            projectile.transform.position = position;
        }

        projectile.gameObject.SetActive(true);

        return projectile;
    }

    private async UniTask<BaseProjectile> Find( string key )
    {
        BaseProjectile projectile = _projectile.Find(obj => obj.gameObject.activeSelf == false);
        if( null == projectile )
        {
            projectile = await MakeProjectile(key);
        }

        return projectile;
    }

    private async UniTask<BaseProjectile> MakeProjectile( string key )
    {
        BaseProjectile projectile = await ResourcePoolManager.Instance.Get<BaseProjectile>(key,transform);

        if( null != projectile )
        {
            _projectile.Add(projectile);
        }

        return projectile;
    }

    public void DespawnAll()
    {
        for (int i = 0; i < _projectile.Count; i++)
        {
            ResourcePoolManager.Instance.Despawn(_projectile[i].gameObject);
        }

        _projectile.Clear();
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
