using System;
using UnityEngine;

public static class StaticValues
{
    private static byte _bulletFillPercent = 2;
    private static byte _bombFillPercent = 3;
    private static byte _rocketFillPercent = 5;

    public static byte GetFillPercent(Type prj)
    {
        if(prj == typeof(Rocket))
        {
            return _rocketFillPercent;
        }
        else if(prj == typeof(Bomb))
        {
            return _bombFillPercent;
        }
        else
        {
            return _bulletFillPercent;
        }
    }
}
