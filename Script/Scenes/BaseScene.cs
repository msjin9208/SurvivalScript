using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IScene
{
    void Init( );
    void Enter( );
    void Exit( );
    void DataReset( );
}

public class BaseScene : IScene
{
    public virtual void Init( )
    {
        
    }

    public virtual void Enter( )
    {
        
    }

    public virtual void Exit( )
    {

    }

    public virtual void DataReset( )
    {

    }

    public virtual async UniTask OnLoadUI( )
    {
        
    }
}
