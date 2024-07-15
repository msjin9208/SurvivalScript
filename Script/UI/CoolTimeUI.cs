using CommonEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class CoolTimeUI : MonoBehaviour
{
    [SerializeField] Image              _frame;
    [SerializeField] Image              _filled;
    [SerializeField] GameObject         _isOn;
    [SerializeField] TextMeshProUGUI    _skillName;

    public Subject<float> FillSubject { private set; get; }

    public int Index { private set; get; }

    public void Initialize()
    {
        if (null != FillSubject)
        {
            FillSubject.Dispose();
            FillSubject = null;
        }

        FillSubject = new Subject<float>();
        FillSubject.Subscribe(value => Fill(value)).AddTo(this);
    }

    public void SetIndex( int index )
    {
        this.Index = index;
    }

    public void SetSkillName( string name )
    {
        UIUtility.SetText(_skillName, name);
    }

    public void SetRatingColor( SkillRate rate )
    {
        Color color = new Color();
        switch (rate)
        {
            case SkillRate.Common:  color = Color.white; break;
            case SkillRate.Rare:    color = Color.blue; break;
            case SkillRate.Epic:    color = Color.red; break;
            case SkillRate.Legend:  color = Color.yellow; break;
        }

        _frame.color = color;
    }

    private void Fill(float value)
    {
        _filled.fillAmount = value;

        _isOn.SetActive(value <= 0);
    }
}
