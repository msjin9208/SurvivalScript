using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Starter : MonoBehaviour
{
    private async void Start()
    {
        await InitializeManagers();
        IntializeEnviroment();

        AppScene.Move(CommonEnum.SceneType.Survival);
    }

    private async UniTask InitializeManagers( )
    {
        AppScene.Initialize();
        MainUI.Initialize();

        await AssetManager.Initialize();
        await AssetManager.LoadAllAssetByLabel();

        ResourcePoolManager.Initialize();
    }

    private void IntializeEnviroment()
    {
        Screen.sleepTimeout         = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
    }
}
