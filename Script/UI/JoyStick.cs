using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ICharacter;

public class JoyStick : MonoBehaviour, IPointerDownHandler , IDragHandler, IPointerUpHandler
{
    [SerializeField] RectTransform  _bgRect;
    [SerializeField] Image          _handler;

    private float   _range = 75;
    private Vector2 _direction;
    private bool    _input;

    public bool     Move => _input;
    public Vector3  Direction => _direction;

    public void SetPlayer( BaseCharacter character )
    {
        Observable.EveryFixedUpdate()
            .Subscribe(l =>
            {
                IMove move = character as IMove;
                if(_input)
                {
                    if (null == move)
                    {
                        Debug.Log("Is not available Move!!");
                        return;
                    }

                    move.Move(_direction);
                }
                else
                {
                    move.Stop();
                }
            })
            .AddTo(this);

        this.ObserveEveryValueChanged(l => _input)
            .Subscribe(l =>
            {
                if (_input) 
                {
                    character.PlayAnimation("Run");
                }
                else
                {
                    character.PlayAnimation("Idle");
                }
            })
            .AddTo(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var input = eventData.position - _bgRect.anchoredPosition;
        var intputPos = input.magnitude < _range ? input : input.normalized * _range;

        _handler.rectTransform.anchoredPosition = intputPos;

        _direction = intputPos / _range;

        _input = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var input       = eventData.position - _bgRect.anchoredPosition;
        var intputPos   = input.magnitude < _range ? input : input.normalized * _range;

        _handler.rectTransform.anchoredPosition = intputPos;

        _direction = intputPos / _range;

        _input = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _handler.rectTransform.anchoredPosition = Vector2.zero;

        _input = false;
    }
}
