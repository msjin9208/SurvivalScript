using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BaseMap : MonoBehaviour
{
    private float   _sizeX;
    private float   _sizeY;

    private Vector3 _prevPlayerPos;
    private Vector3 _nextPlayerPos;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (false == collision.CompareTag("Area"))
            return;

        Vector3 mapPos = transform.position;

        float x = Mathf.Abs(_nextPlayerPos.x - mapPos.x);
        float y = Mathf.Abs(_nextPlayerPos.y - mapPos.y);

        Vector3 direction = (collision.transform.position - _nextPlayerPos).normalized;

        float dirX = direction.x < 0 ? -1 : 1;
        float dirY = direction.y < 0 ? -1 : 1;

        if (x > y)
        {
            transform.Translate(Vector3.right * dirX * _sizeX);
        }
        else if (x < y)
        {
            transform.Translate(Vector3.up * dirY * _sizeY);
        }

        _prevPlayerPos = _nextPlayerPos;
    }

    public void SetMove( GameObject target )
    {
        _sizeX = 40;
        _sizeY = 40;
        _prevPlayerPos  = Vector3.zero;

        this.ObserveEveryValueChanged(l => target.transform.position)
            .Subscribe( position =>
            {
                _nextPlayerPos = position;
            })
            .AddTo(this);
    }

    private void MoveTo( Vector3 position )
    {
        //Vector3 mapPos = transform.position;

        //float x = Mathf.Abs( position.x - mapPos.x );
        //float y = Mathf.Abs( position.y - mapPos.y );

        //Vector3 direction = (_prevPlayerPos - position).normalized;

        //float dirX = direction.x < 0 ? -1 : 1;
        //float dirY = direction.y < 0 ? -1 : 1;

        //if( x > y )
        //{
        //    transform.Translate(Vector3.right * dirX * _sizeX);
        //}
        //else if( x < y )
        //{
        //    transform.Translate(Vector3.up * dirY * _sizeY);
        //}
    }
}
