using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ISkill;
using System.Linq;
using System;

public class SkillManager
{
    private List<Skill> _skillList;

    public void Initialize( )
    {
        LoadData().Forget();
    }

    private async UniTask LoadData()
    {
        var skillData   = await AssetManager.GetTable<SkillTableData>(CommonEnum.Table.SkillTableData);
        _skillList      = skillData.SkillList.ToList();
    }

    public Skill[] GetRandomSkill( int cnt = 3 )
    {
        Skill[] skills = new Skill[cnt];

        for( int i = 0; i < skills.Length; i++ )
        {
            skills[i] = GetRandomSkill();
        }

        return skills;
    }

    private Skill GetRandomSkill()
    {
        int index = UnityEngine.Random.Range(0, _skillList.Count);

        return _skillList[index];
    }

    public T CreateSkill<T>( int index ) where T : BaseSkill
    {
        var tableData = _skillList.Find( element => element.Index == index );
        if (tableData.Index <= 0)
            return null;

        SkillData data;
        data.index          = tableData.Index;
        data.damage         = tableData.Damage;
        data.targetCount    = tableData.TargetCount;
        data.coolTime       = tableData.CoolTime;
        data.prefabName     = tableData.Prefab;
        data.skillTarget    = tableData.Target;
        data.skillType      = tableData.SkillType;
        data.rate           = tableData.Rate;
        data.maxLevel       = tableData.MaxLevel;
        data.skillName      = tableData.SkillName;

        Type type   = Type.GetType(tableData.SkillName);
        T skill     = System.Activator.CreateInstance( type ) as T;

        skill.Initialize(data);

        return skill;
    }
}
