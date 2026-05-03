using UnityEngine;

/// <summary>
/// AK-47 fully automatic assault rifle.
///
/// Stats:
///   RPM                : 400
///   Magazine           : 30
///   Accuracy           : 60%  → effectiveCone = 20° × 0.40 = 8.0° max deflection
///   Damage             : 5
///   Reload             : 5 s
///   Max deflection     : 20°  (wide cone — automatic spray)
/// </summary>
public class AK47 : WeaponBase
{
    public override bool IsAutomatic => true;

    protected override void Awake()
    {
        weaponName = "AK-47";
        rpm = 400f;
        magazineCapacity = 30;
        accuracy = 0.6f;
        damage = 5f;
        reloadDuration = 5f;
        maxDeflectionAngle = 20f;  // effectiveCone = 20° × (1−0.6) = 8°

        base.Awake();
        InitialiseAmmo();
    }
}
