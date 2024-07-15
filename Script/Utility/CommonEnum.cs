namespace CommonEnum
{
    public enum Table
    {
        PlayerTableData     = 0,
        MonsterTableData    = 2,
        ItemTableData       = 3,
        SkillTableData      = 4
    }

    public enum SceneType
    {
        Survival,
    }

    #region [ CHARACTER ]
    public enum StatType
    {
        None    = 0,
        Hp      = 1,
        Attack,
        Speed,
        AtkSpeed,
        Max,
    }

    public enum AttackCombo
    {
        Stand, Attack1, Attack2, Attack3, Max
    }

    #region [ SKILL ]

    public enum SkillType
    {
        Damage,
        Buff,
        Debuff,
    }

    public enum SkillTarget
    {
        None,
        Self,
        Enemy,
    }

    public enum SkillRate
    {
        Common,Rare,Epic,Legend
    }

    #endregion
    #endregion

    #region [ BATTLE ]
    public enum BattleState
    {
        None    = 0,
        Stand   = 1,
        Start   = 2,
        Wave    = 3,
        WaveEnd = 4,
        Skill   = 5,
        End     = 6,
        MAX
    }

    public enum Camp
    {
        None,
        Ally,
        Enemy,
    }

    #endregion

    #region [ ITEM ]
    public enum ItemType
    {
        EXP,
        GOLD,
    }
    #endregion
}