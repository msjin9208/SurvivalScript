using ISkill;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SkillSystemUI : MonoBehaviour
{
    [SerializeField] Button _closeBtn;

    [SerializeField] SkillUI[] _skillUIs;

    private Subject<int> Select { set; get; }

    public void Initialize()
    {
        SetActive(false);
    }

    public void SetActive( bool active )
    {
        gameObject.SetActive( active );
    }

    public void ViewSkills( UI_SkillData[] skills, UnityAction<int> onComplete )
    {
        if( null != Select )
        {
            Select.Dispose();
        }
     
        Select = new Subject<int>();
        Select.Subscribe( result => 
        {
            onComplete(result);
            SetActive(false);
        } );

        SetSkillUI(skills);

        SetActive(true);

        _closeBtn.onClick.RemoveAllListeners();
        _closeBtn.onClick.AddListener(() =>
        {
            Select.OnNext(0);
        });
    }

    private void SetSkillUI( UI_SkillData[] skills )
    {
        for (int i = 0; i < _skillUIs.Length; i++)
        {
            if (skills.Length > i)
                _skillUIs[i].Set(skills[i], Select);
        }
    }
}
