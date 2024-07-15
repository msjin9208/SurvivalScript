using CommonEnum;
using ICharacter;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StatSystemUI : MonoBehaviour
{
    [SerializeField] List<StatUI> _statUIList;

    [SerializeField] Button _confirmBtn;
    [SerializeField] Button _closeBtn;
    [SerializeField] Button _resetBtn;

    private AddStats _addStat;
    
    public void Open( BaseStat stat, UnityAction<AddStats?> complete )
    {
        _addStat = new AddStats(0, 0, 0, 0);
        int index = 0;
        for (int i = (int)StatType.Hp; i < (int)StatType.Max; i++)
        {
            StatType type   = (StatType)i;
            float value     = 0;

            switch( type )
            {
                case StatType.Hp:       value = stat.HP; break;
                case StatType.Attack:   value = stat.Attack; break;
                case StatType.Speed:    value = stat.Speed; break;
                case StatType.AtkSpeed: value = stat.AtkSpeed; break;
            }

            _statUIList[index].Init( type, value , Add );
            index++;
        }

        _closeBtn.onClick.RemoveAllListeners();
        _closeBtn.onClick.AddListener(() => 
        {
            Close();
            complete?.Invoke( null );
        });

        _confirmBtn.onClick.RemoveAllListeners();
        _confirmBtn.onClick.AddListener(() => 
        {
            complete?.Invoke(_addStat);
            
            Close();
        });

        _resetBtn.onClick.RemoveAllListeners();
        _resetBtn.onClick.AddListener(() =>
        {
            for (int i = 0; i < _statUIList.Count; i++)
                _statUIList[i].Reset();

            _addStat = new AddStats(0, 0, 0, 0);
        });

        gameObject.SetActive(true);
    }

    private void Add( StatType type , float value )
    {
        int atk         = _addStat.attack;
        int hp          = _addStat.hp;
        float spd       = _addStat.speed;
        float atkSpd    = _addStat.atkSpeed;
        
        switch(type)
        {
            case StatType.Hp:       hp = (int)value; break;
            case StatType.Attack:   atk = (int)value; break;
            case StatType.Speed:    spd = value; break;
            case StatType.AtkSpeed: atkSpd = value; break;
        }

        _addStat = new AddStats(atk,hp,spd,atkSpd);
    }

    public void Close( ) 
    {
        gameObject.SetActive(false);
    }
}
