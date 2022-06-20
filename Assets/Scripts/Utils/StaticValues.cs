using System;
using UnityEngine;
using System.Collections.Generic;

public static class StaticValues
{
    private static byte _bulletFillPercent = 2;
    private static byte _bombFillPercent = 4;
    private static byte _rocketFillPercent = 4;
    private static byte _gooFillPercent = 5;

    public readonly static int EnemyLayer = LayerMask.NameToLayer("Enemy");
    public readonly static int DevilLayer = LayerMask.NameToLayer("Devil");
    public readonly static int VacuumLayer = LayerMask.NameToLayer("Vacuum");
    public readonly static int BlackHoleLayer = LayerMask.NameToLayer("Blackhole");
    public readonly static int PlayerLayer = LayerMask.NameToLayer("Player");
    public readonly static int LaserLayer = LayerMask.NameToLayer("Laser");
    public readonly static int FlamesLayer = LayerMask.NameToLayer("Flame");
    public readonly static int ProjectileLayer = LayerMask.NameToLayer("Vacuum");
    public readonly static int MinigunLayer = LayerMask.NameToLayer("Minigun");
    public readonly static int PlayerProjectileLayer = LayerMask.NameToLayer("PlayerProjectile");
    public readonly static int GooLayer = LayerMask.NameToLayer("Goo");

    public readonly static Dictionary<Tuple<FillType, FillType>, ShootingType> Combinations = new Dictionary<Tuple<FillType, FillType>, ShootingType>();
    public static byte GetFillPercent(FillType prj)
    {
        if (prj == FillType.Rocket) return _rocketFillPercent;
        else if (prj == FillType.Bomb)  return _bombFillPercent;
        else if (prj == FillType.Goo)   return _gooFillPercent;
        else   return _bulletFillPercent;
    }
    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        Combinations.Add(new Tuple<FillType, FillType>(FillType.Goo, FillType.Bullet),
            ShootingType.Flames);
        Combinations.Add(new Tuple<FillType, FillType>(FillType.Bullet, FillType.Goo),
            ShootingType.Flames);

        Combinations.Add(new Tuple<FillType, FillType>(FillType.Bomb, FillType.Bullet),
            ShootingType.Laser);
        Combinations.Add(new Tuple<FillType, FillType>(FillType.Bullet, FillType.Bomb),
            ShootingType.Laser);

        Combinations.Add(new Tuple<FillType, FillType>(FillType.Rocket, FillType.Bullet),
            ShootingType.Blackhole);
        Combinations.Add(new Tuple<FillType, FillType>(FillType.Bullet, FillType.Rocket),
            ShootingType.Blackhole);

        Combinations.Add(new Tuple<FillType, FillType>(FillType.Goo, FillType.Bomb),
            ShootingType.Minigun);
        Combinations.Add(new Tuple<FillType, FillType>(FillType.Bomb, FillType.Goo),
            ShootingType.Minigun);

        Combinations.Add(new Tuple<FillType, FillType>(FillType.Rocket, FillType.Goo),
            ShootingType.Rockets);
        Combinations.Add(new Tuple<FillType, FillType>(FillType.Goo, FillType.Rocket),
            ShootingType.Rockets);

        Combinations.Add(new Tuple<FillType, FillType>(FillType.Rocket, FillType.Bomb),
            ShootingType.Goo);
        Combinations.Add(new Tuple<FillType, FillType>(FillType.Bomb, FillType.Rocket),
            ShootingType.Goo);
    }
}
