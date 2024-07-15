
using System.Collections;
using UnityEngine;
using CommonEnum;
using Cysharp.Threading.Tasks;

namespace ICharacter
{
    public struct AddStats
    {
        public int attack;
        public int hp;
        public float speed;
        public float atkSpeed;

        public AddStats( int atk, int hp , float spd, float atkSpd )
        {
            this.attack     = atk;
            this.hp         = hp;
            this.speed      = spd;
            this.atkSpeed   = atkSpd;
        }
    }

    [System.Serializable]
    public class BaseStat
    {
        public int      Attack { private set; get; }
        public int      HP { private set; get; }
        public float    Speed { private set; get; }
        public float    AtkSpeed { private set; get; }

        public BaseStat(int hp, int atk, float spd, float atkSpd)
        {
            this.HP         = hp;
            this.Attack     = atk;
            this.Speed      = spd;
            this.AtkSpeed   = atkSpd;
        }

        public void Add(StatType type, float value)
        {
            switch (type)
            {
                case StatType.Hp:
                    {
                        HP += (int)value;

                        if (HP <= 0)
                            HP = 0;
                    }
                    break;
                case StatType.Attack:   Attack += (int)value; break;
                case StatType.Speed:    Speed += value; break;
                case StatType.AtkSpeed: AtkSpeed -= (value * 0.1f); break;
            }
        }
    }

    public interface IStat
    {
        public void SetStat( BaseStat stat );
    }

    public interface IHealth
    {
        public void SetHealth();
        public UniTask Death();
        public void OnDamage( int decrease );
        public void OnHeal(int add);
    }

    public interface IMove
    {
        public void Move(Vector2 direct);
        public void Stop();
    }

    public interface IAttacker
    {
        public void SetAttack();
    }

    public interface IMelee
    {
        public void Melee( );
        public void Melee(Vector2 direct);
        public void Hit();
    }

    public interface IDash
    {
        public float GetDashCoolTime();
        public float GetDashMaxCoolTime();
        public UniTask Dash(Vector2 direct, float time);
        public void DashMove(Vector2 direct);
    }

    public interface IChase
    {
        public void ToChase( BaseCharacter target );
    }
}
