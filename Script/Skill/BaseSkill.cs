using UnityEngine;
using Cysharp.Threading.Tasks;
using ISkill;
using CommonEnum;

public class BaseSkill
{
    protected Scanner _scanner;
    public SkillData SkillData { private set; get; }

    public int Index    => SkillData.index;
    public int MaxLevel => SkillData.maxLevel;
    public SkillRate Rate => SkillData.rate;
    public string SkillName => SkillData.skillName;
    public int Level { private set; get; }

    public virtual void Initialize( SkillData data )
    {
        SkillData = data;

        Level = 1;
    }

    public virtual void PlaySkill( Scanner scanner )
    {
        _scanner = scanner;
    }

    public virtual void Upgrade()
    {
        Level += 1;

        if(Level > MaxLevel) Level = MaxLevel;
    }

    protected async UniTask<T> LoadProjectile<T>( string prefab, Vector3 pos ) where T : Component
    {
        T obj = await ProjectileFactory.Instance.Get( prefab, pos ) as T;

        return obj;
    }

    public virtual void Despawn()
    {

    }
}
