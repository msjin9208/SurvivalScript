using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    static public class UIUtility
    {
        static readonly string _imagePath = "Image/";

        static public void SetText(TextMeshProUGUI text, string str)
        {
            text.text = str;
        }

        static public async UniTask SetImage(Image img, string atlas ,string res )
        {
            Sprite sprite = await ResourcePoolManager.Instance.GetSprite(atlas, res);
            if (null == sprite)
            {
                Debug.Log($"Image setting is not Available {res}");
                return;
            }

            img.sprite = sprite;
        }
    }

}
