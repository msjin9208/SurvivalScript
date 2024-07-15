using UnityEngine;
using UniRx;

public class AutoDespawn : MonoBehaviour
{
    public void Play(float time = 1f, bool destroy = false)
    {
        Observable.Interval(System.TimeSpan.FromSeconds(time)).TakeUntilDisable(this).Subscribe(l =>
        {
            if (true == destroy)
            {
                GameObject.Destroy(gameObject);
            }
            else
            {
                ResourcePoolManager.Instance.Despawn(gameObject);
            }
        }).AddTo(this);
    }
}
