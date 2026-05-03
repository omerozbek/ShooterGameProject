using UnityEngine;

/// <summary>
/// Defines the contract for any object capable of firing along an aim ray.
/// The ray is provided by the caller (WeaponSystem) so the weapon can apply
/// its own cone-of-fire deflection and then raycast to resolve the hit.
/// </summary>
public interface IShootable
{
    /// <summary>
    /// Fires one round along aimRay, applying weapon-specific deflection.
    /// The deflected ray is cast against layerMask up to range units.
    /// </summary>
    void Shoot(Ray aimRay, LayerMask layerMask, float range);

    /// <summary>
    /// Returns true when all firing conditions are met:
    /// not reloading, ammo > 0, fire-rate cooldown elapsed.
    /// </summary>
    bool CanShoot();
}
