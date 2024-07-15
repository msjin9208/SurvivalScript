using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonEnum;
using Cysharp.Threading.Tasks;
using ISkill;

public abstract class ShootSkill : BaseSkill
{
    public abstract UniTask Fire( Vector2 direct );
}
