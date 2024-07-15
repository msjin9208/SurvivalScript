using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public enum DataType
    {
        Level,
        EXP,
        Gold,
    }

    private int _level;
    private int _exp;
    private int _expMax;
    private int _gold;
    private int _statPoint;

    public int Level => _level;
    public int Exp => _exp;
    public int Gold => _gold;
    public int ExpMax => _expMax;
    public int StatPoint => _statPoint;

    public PlayerData()
    {
        _level  = 1;
        _exp    = 0;
        _expMax = 1;
        _gold   = 0;
    }

    public void Add( DataType type, int value )
    {
        switch( type )
        {
            case DataType.Level:
                {
                    _level += value;
                }
                break;
            case DataType.EXP:
                {
                    _exp += value;

                    if (_exp >= _expMax)
                    {
                        _exp        = 0;
                        _level      += 1;
                        _expMax     = _level * 10;
                        _statPoint  += 1;
                    }
                }
                break;
            case DataType.Gold:
                {
                    _exp += value;
                }
                break;
        }
    }

    public void Decrease( DataType type, int value ) 
    {
        switch (type)
        {
            case DataType.Level:
                {
                    _level -= value;
                }
                break;
            case DataType.EXP:
                {
                    _exp -= value;
                }
                break;
            case DataType.Gold:
                {
                    _exp -= value;
                }
                break;
        }
    }
}
