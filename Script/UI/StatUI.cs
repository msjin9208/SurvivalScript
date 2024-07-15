using CommonEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utility;

public class StatUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI    _titleTxt;
    [SerializeField] TextMeshProUGUI    _valueTxt;
    [SerializeField] TextMeshProUGUI    _addTxt;
    [SerializeField] Button             _btn;

    private float _value;

    public void Init( StatType type, float value , UnityAction<StatType, float> addCb )
    {
        Reset();

        UIUtility.SetText(_titleTxt, type.ToString());
        UIUtility.SetText(_valueTxt, value.ToString());
        

        _btn.onClick.RemoveAllListeners();
        _btn.onClick.AddListener(() => 
        {
            _value += 1;
            if( null != addCb )
            {
                addCb.Invoke(type, _value);
                UIUtility.SetText(_addTxt, _value.ToString());
            }
        });
    }

    public void Reset()
    {
        _value = 0;

        UIUtility.SetText(_addTxt, _value.ToString());
    }
}
