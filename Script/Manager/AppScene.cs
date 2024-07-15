using System;
using System.Collections.Generic;
using UnityEngine;
using CommonEnum;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class AppScene
{
    static private AppScene instance;

    private Dictionary<SceneType, BaseScene>    _sceneDic;
    private BaseScene                           _currentScene;
    private SceneType                           _currentType;

    static public void Initialize( )
    {
        instance            = new AppScene( );
        instance._sceneDic  = new Dictionary<SceneType , BaseScene>( );

        Add( SceneType.Survival );
    }

    static public bool Add( SceneType sceneType )
    {
        Type type = Type.GetType( $"{sceneType}Scene" );

        if( null == type )
        {
            Debug.Log( $"{sceneType} type is Null !!! " );
            return false;
        }
        else
        {
            object create = System.Activator.CreateInstance( type );
            if( null != create && create is BaseScene )
            {
                BaseScene scene = (BaseScene) create;
                scene.Init( );

                instance._sceneDic.Add( sceneType, scene );
            }

            return true;
        }
    }

    static public void Move( SceneType sceneType ) 
    {
        if( null != instance._currentScene )
        {
            instance._currentScene.Exit( );
            instance._currentScene = null;
        }

        OnLoadScene( sceneType );
    }

    static private void OnLoadScene( SceneType scene )
    {
        SceneManager.LoadSceneAsync( scene.ToString( ) ).completed += ( asycnOp ) =>
        {
            if( false == instance._sceneDic.TryGetValue( scene , out instance._currentScene ) )
            {
                instance._currentScene = null;
            }
            else
            {
                instance._currentType = scene;

                instance._currentScene.Enter( );
            }
        };
    }

    static public void Back( )
    {
        Move( instance._currentType - 1 );
    }
}