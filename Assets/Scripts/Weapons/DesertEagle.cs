using UnityEngine;

/// <summary>
/// Desert Eagle semi-automatic pistol.
///
/// Stats:
///   RPM                : 60
///   Magazine           : 10
///   Accuracy           : 90%  → effectiveCone = 10° × 0.10 = 1.0° max deflection
///   Damage             : 40
///   Reload             : 4 s
///   Max deflection     : 10°  (tight cone — precision pistol)
/// </summary>
public class DesertEagle : WeaponBase
{
    public override bool IsAutomatic => false;

    protected override void Awake()
    {
        weaponName = "Desert Eagle";
        rpm = 60f;
        magazineCapacity = 10;
        accuracy = 0.9f;
        damage = 40f;
        reloadDuration = 4f;
        maxDeflectionAngle = 10f;  // effectiveCone = 10° × (1−0.9) = 1°

        base.Awake();
        InitialiseAmmo();
    }
}
