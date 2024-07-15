using CommonEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseItem : MonoBehaviour
{
    public UnityAction<ItemType, int> Get;

    protected int       _value;
    protected ItemType  _type;

    public virtual void Initialize(Item item) { }
    public virtual void Despawn() { }
}
