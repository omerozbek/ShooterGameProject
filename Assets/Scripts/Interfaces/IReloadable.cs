/// <summary>
/// Defines the contract for any object that supports magazine reloading.
/// Separating this from IShootable respects the Interface Segregation Principle:
/// not every shootable thing necessarily needs manual reloading.
/// </summary>
public interface IReloadable
{
    /// <summary>
    /// Starts the timed reload sequence. Does nothing if already reloading
    /// or the magazine is already full.
    /// </summary>
    void StartReload();

    /// <summary>
    /// Immediately aborts an in-progress reload (e.g. when the player
    /// switches weapons mid-reload).
    /// </summary>
    void CancelReload();

    /// <summary>True while the reload timer is running.</summary>
    bool IsReloading { get; }
}
