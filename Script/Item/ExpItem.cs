using CommonEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExpItem : BaseItem
{
    [SerializeField] SpriteRenderer     _sprite;

    public override void Initialize(Item item)
    {
        _value  = item.Value;
        _type   = item.Type;
    }

    public override void Despawn()
    {
        gameObject.SetActive(false);

        if (null != Get)
            Get.Invoke(_type, _value);
    }
}
