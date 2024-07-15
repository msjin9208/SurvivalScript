using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class MainUI : MonoSingleton<MainUI>
{
    public enum UIType
    {
       Survival,
       Max,
    }

    #region > SETTING <
    Camera                              _mainCamera;
    Canvas                              _canvas;
    EventSystem                         _eventSystem;
    #endregion

    #region > TRANS <
    RectTransform                       _rectTransform;

    GameObject                          _uiGo;
    GameObject                          _popGo;
    #endregion

    #region > PARAMETER <
    readonly string                     _resPath = "UIs";
    #endregion

    #region > DATA <
    BaseSceneUI[]                       _sceneUI;
    #endregion


    private void Initailize( )
    {
        Instance._sceneUI = new BaseSceneUI[(int)UIType.Max];

        InitCamera( );
        InitCanvas( );
        InitCanvasScaler( );
        InitGraphicRaycaster( );
        InitTransform( );
    }

    private void InitCamera( )
    {
        _mainCamera                 = gameObject.AddComponent<Camera>( );
        _mainCamera.clearFlags      = CameraClearFlags.Nothing;
        _mainCamera.orthographic    = true;
    }

    private void InitCanvas( )
    {
        _canvas             = gameObject.AddComponent<Canvas>( );

        _canvas.worldCamera = _mainCamera;
        _canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
    }

    private void InitGraphicRaycaster()
    {
        var graph           = gameObject.AddComponent<GraphicRaycaster>();
    }
    private void InitCanvasScaler()
    {
        var scaler                  = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode          = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.screenMatchMode      = CanvasScaler.ScreenMatchMode.Expand;
        scaler.referenceResolution  = new Vector2( 1080 , 1920 );
    }

    private void InitTransform( )
    {
        _uiGo = new GameObject("UIs");
        {
            _uiGo.transform.SetParent( Instance.transform );

            RectTransform uiTransform = _uiGo.AddComponent<RectTransform>( );
            {
                uiTransform.offsetMin = Vector2.zero;
                uiTransform.offsetMax = Vector2.zero;
                uiTransform.anchorMin = Vector2.zero;
                uiTransform.anchorMax = Vector2.one;
            }

            _uiGo.transform.localPosition = Vector3.zero;
            _uiGo.transform.localScale = Vector3.one;
        }

        _popGo = new GameObject("Popup");
        {
            _popGo.transform.SetParent( Instance.transform );

            RectTransform popupTransform = _popGo.AddComponent<RectTransform>( ) ;
            {
                popupTransform.offsetMin = Vector2.zero;
                popupTransform.offsetMax = Vector2.zero;
                popupTransform.anchorMin = Vector2.zero;
                popupTransform.anchorMax = Vector2.one;
            }

            _popGo.transform.localPosition = Vector3.zero;
            _popGo.transform.localScale= Vector3.one;
        }

        GameObject go = new GameObject( "EventSystem" );
        {
            _eventSystem = go.AddComponent<UnityEngine.EventSystems.EventSystem>( );

            go.AddComponent<StandaloneInputModule>( );

            _eventSystem.transform.SetParent( Instance.transform );
        }
    }

    private async UniTask<T> MakebySceneUI<T>( UIType type ) where T : BaseSceneUI
    {
        string name = $"{type}SceneUI";

        BaseSceneUI ui = await ResourcePoolManager.Instance.Get<BaseSceneUI>(name, _uiGo.transform);

        if( null == ui )
            return null;

        _sceneUI[(int)type] = ui;

        return ui as T;
    }


    #region > STATIC <
    static public void Initialize( ) 
    {
        Instance.Initailize( );
    }

    static public void Destroy( ) 
    {
        if( null == Instance )
            return;

        GameObject.Destroy( Instance.gameObject );
    }

    static public async UniTask<T> GetSceneUI<T>( UIType type ) where T : BaseSceneUI
    {
        BaseSceneUI ui = Instance._sceneUI[(int)type];
        if( null == ui )
        {
            ui = await Instance.MakebySceneUI<T>( type );
        }
        else
        {
            ui.transform.SetParent( Instance._uiGo.transform );
        }

        RectTransform rectTransform = null;
        if (true == ui.TryGetComponent(out rectTransform))
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale    = Vector3.one;
        }

        return ui as T;
    }
    #endregion
}
