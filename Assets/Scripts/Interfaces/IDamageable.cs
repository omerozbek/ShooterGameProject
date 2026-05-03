/// <summary>
/// Defines the contract for any object that can receive damage and be reset.
/// Weapons reference this interface instead of the concrete Target class so
/// that any damageable object (multiple target types, destructible walls, etc.)
/// could be used without changing weapon code — Open/Closed Principle.
/// </summary>
public interface IDamageable
{
    /// <summary>Subtracts the given amount from this object's health.</summary>
    /// <param name="amount">Damage points to apply (always positive).</param>
    void TakeDamage(float amount);

    /// <summary>Restores the object to its starting state (full health).</summary>
    void Reset();

    /// <summary>
    /// True when health has reached zero and the object can no longer
    /// receive damage until Reset() is called.
    /// </summary>
    bool IsDestroyed { get; }
}
