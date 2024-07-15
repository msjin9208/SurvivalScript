using CommonEnum;
using Cysharp.Threading.Tasks;
using ISkill;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utility;

[System.Serializable]
public class SkillUI : MonoBehaviour
{
    [SerializeField] Image              _bg;
    [SerializeField] TextMeshProUGUI    _titleTxt;
    [SerializeField] TextMeshProUGUI    _subjectTxt;
    [SerializeField] Button             _selectBtn;
    public void Set( UI_SkillData data , Subject<int> select )
    {
        _selectBtn.onClick.RemoveAllListeners();
        _selectBtn.onClick.AddListener(() =>
        {
            select.OnNext(data.index);
        });

        SetBg(data.rate);
        
        SetTitle(data.title);
        SetSubject(data.subject, data.has, data.level);
    }

    private void SetBg( SkillRate rate )
    {
        string atlas    = "UIAtlas";
        string bg       = string.Empty;
        switch(rate)
        {
            case SkillRate.Common:  bg = "Skill_Brown"; break;
            case SkillRate.Rare:    bg = "Skill_Green"; break;
            case SkillRate.Epic:    bg = "Skill_Red"; break;
            case SkillRate.Legend:  bg = "Skill_Yellow"; break;
        }

        UIUtility.SetImage(_bg, atlas, bg).Forget( );
    }

    private void SetTitle( string title )
    {
        UIUtility.SetText(_titleTxt, title);
    }

    private void SetSubject(string subject, bool has, int level)
    {
        if (has)
        {
            UIUtility.SetText(_subjectTxt, string.Format($"Lv{level} : UPGRADE!"));
        }
        else
        {
            UIUtility.SetText(_subjectTxt, subject);
        }
    }
}
