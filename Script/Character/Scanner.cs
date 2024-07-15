using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class Scanner : MonoBehaviour
{
    private bool    _scanning;
    private float   _coolTime;
    private float   _atkSpeed;
    private float   _range;

    System.IDisposable      _scanDispose;
    private Transform       _target;
    private RaycastHit2D[]  _targets;

    public Transform Target => _target;

    public void Initialize( float cool , float speed , float range )
    {
        _coolTime   = cool;
        _atkSpeed   = speed;
        _range      = range;
        _scanning   = false;

        if (null != _scanDispose) _scanDispose.Dispose();

        _scanDispose = 
            Observable
            .EveryFixedUpdate()
            .Where(l => _scanning == true)
            .Subscribe(l =>
            {
                _target = Scanned();
            })
            .AddTo(this);
    }

    public void Initialize( float range )
    {
        _range = range;
    }

    public void StartScan()
    {
        _scanning = true;
    }

    public void StopScan()
    {
        _scanning = false;
    }

    public Vector3 GetMyPosition()
    {
        return transform.position;
    }

    public Vector2 GetMyDirection()
    {
        return mydir;
    }

    private Vector2 mydir = Vector2.zero;

    public void SetMyDirection( Vector2 direct )
    {
        mydir = direct;
    }

    public Vector3? GetNearOnce()
    {
        Transform target = Scanned();
        if( null == target ) return null;

        return target.position - transform.position;
    }

    private Transform Scanned( )
    {
        var targets = Physics2D.CircleCastAll( transform.position, _range, Vector2.zero, 0 ).ToList();
        
        targets = targets.FindAll( target => target.transform.gameObject.CompareTag("Monster") );
        targets = targets.OrderByDescending(target => target.distance).ToList();

        if (targets.Count > 0)
            return targets[0].transform;
        else
            return null;
    }

    private void OnDestroy()
    {
        _scanDispose?.Dispose();
    }
}
