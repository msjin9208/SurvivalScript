using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image _filledImage;

    public Subject<int>     HpSubject { private set; get; }
    public Subject<Vector3> Position { private set; get; }

    private int             _maxHp;

    public void Initialize()
    {
        HpSubject = new Subject<int>();
        HpSubject.Subscribe(hp => UpdateHp(hp)).AddTo(this);
    }

    public void SetMaxHp( int hp )
    {
        _maxHp = hp;
    }

    private void UpdateHp( int hp )
    {
        float result            = (float)hp / (float)_maxHp;
        _filledImage.fillAmount = result;
    }
}
