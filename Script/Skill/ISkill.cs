using Cysharp.Threading.Tasks;
using CommonEnum;
using System.Numerics;

namespace ISkill
{
    public struct SkillData
    {
        public int          index;
        public int          damage;
        public int          targetCount;
        public float        coolTime;
        public string       prefabName;
        public SkillTarget  skillTarget;
        public SkillType    skillType;
        public SkillRate    rate;

        public int          maxLevel;
        public string       skillName;
    }

    public struct UI_SkillData
    {
        public int          index;
        public int          level;
        public SkillRate    rate;
        public string       title;
        public string       subject;
        public bool         has;
    }

    public interface ICoolTime
    {
        public float GetCoolTime();
        public float GetMaxCoolTime();
        public void SetCoolTime();
    }
}
