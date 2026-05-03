using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages every on-screen text element. Other systems call the public methods
/// below — they never touch UI components directly — keeping UI concerns isolated.
///
/// Uses TextMeshPro (TMPro) which is Unity's modern, built-in text system.
/// All text references must be assigned in the Inspector.
///
/// UI Layout (see setup guide for exact positioning):
///   Top-left     : Weapon name
///   Below weapon : Ammo count
///   Top-right    : Target health
///   Bottom-centre: Event log (temporary messages, auto-clears)
///   Centre-screen: Crosshair "+"
///   Centre-top   : Reloading indicator
/// </summary>
public class UIManager : MonoBehaviour
{
    // ─── Inspector References ─────────────────────────────────────────────────
    // Each field maps to one Text - TextMeshPro component on the Canvas.

    [Header("Weapon Info")]
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Target Info")]
    [SerializeField] private TextMeshProUGUI targetHealthText;

    [Header("Event Log")]
    [SerializeField] private TextMeshProUGUI eventText;

    [Header("Reload Indicator")]
    [SerializeField] private TextMeshProUGUI reloadingText;

    [Header("Crosshair")]
    [SerializeField] private TextMeshProUGUI crosshairText;

    [Header("Settings")]
    [Tooltip("Seconds before the event log message disappears.")]
    [SerializeField] private float eventDisplayDuration = 2.5f;

    private Coroutine _clearEventRoutine;
    private Coroutine _clearHealthRoutine;

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        // The crosshair is a static "+" that never changes
        if (crosshairText != null)
        {
            crosshairText.text     = "+";
            crosshairText.fontSize = 36;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PUBLIC API — called by WeaponBase, WeaponSystem, Target
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Updates the weapon-name panel (top-left).</summary>
    public void UpdateWeaponName(string name)
    {
        if (weaponNameText != null)
            weaponNameText.text = $"Silah: {name}";
    }

    /// <summary>Updates the ammo counter (top-left, below weapon name).</summary>
    public void UpdateAmmo(int current, int max)
    {
        if (ammoText != null)
            ammoText.text = $"Mermi: {current} / {max}";
    }

    /// <summary>
    /// Shows the target health temporarily, then clears it after eventDisplayDuration.
    /// Behaves identically to ShowEvent — each new hit/reset restarts the timer.
    /// </summary>
    public void UpdateTargetHealth(float health, bool destroyed)
    {
        if (targetHealthText == null) return;

        targetHealthText.text = destroyed
            ? "Hedef: KULLANAMAZ DURUMDA"
            : $"Hedef Sağlık: {health:0} / 100";

        if (_clearHealthRoutine != null)
            StopCoroutine(_clearHealthRoutine);

        _clearHealthRoutine = StartCoroutine(ClearHealthAfterDelay());
    }

    /// <summary>
    /// Shows a temporary message in the event log (bottom-centre).
    /// If a previous message is still showing, it is replaced immediately.
    /// The message auto-clears after eventDisplayDuration seconds.
    /// </summary>
    public void ShowEvent(string message)
    {
        if (eventText == null) return;

        eventText.text = message;

        // Cancel any pending clear-timer so it does not wipe the new message early
        if (_clearEventRoutine != null)
            StopCoroutine(_clearEventRoutine);

        _clearEventRoutine = StartCoroutine(ClearEventAfterDelay());
    }

    /// <summary>
    /// Shows or hides the "RELOADING..." indicator (centre-top).
    /// Pass true when reload starts, false when it finishes or is cancelled.
    /// </summary>
    public void ShowReloading(bool isReloading)
    {
        if (reloadingText != null)
            reloadingText.text = isReloading ? "Yeniden dolduruluyor." : string.Empty;
    }

    // ─────────────────────────────────────────────────────────────────────────

    private IEnumerator ClearEventAfterDelay()
    {
        yield return new WaitForSeconds(eventDisplayDuration);

        if (eventText != null)
            eventText.text = string.Empty;
    }

    private IEnumerator ClearHealthAfterDelay()
    {
        yield return new WaitForSeconds(eventDisplayDuration);

        if (targetHealthText != null)
            targetHealthText.text = string.Empty;
    }
}
