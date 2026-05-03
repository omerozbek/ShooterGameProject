using System.Collections;
using UnityEngine;

/// <summary>
/// Abstract base class for all weapons. Handles fire-rate, cone-of-fire accuracy,
/// ammo management, and reloading. Subclasses set their stats and declare fire mode.
///
/// Cone-of-fire accuracy model:
///   effectiveCone = maxDeflectionAngle × (1 − accuracy)
///   A random angle within [0, effectiveCone] is applied each shot in a random
///   azimuth around the aim ray. Hit/miss is decided by physics, not a coin flip.
///
/// Ray visualisation (visible in Scene view, and Game view with Gizmos enabled):
///     White  = perfect aim direction (before deflection)
///     Green  = deflected ray that hit the target
///     Red    = deflected ray that missed
///     Yellow = deflected ray that hit an already-destroyed target
/// </summary>
public abstract class WeaponBase : MonoBehaviour, IShootable, IReloadable
{
    // Stats — set by subclass before calling base.Awake()

    protected string weaponName;
    protected float rpm;
    protected int magazineCapacity;
    protected float accuracy;
    protected float damage;
    protected float reloadDuration;

    /// Half angle of the bullet cone in degrees
    protected float maxDeflectionAngle;

    /// Seconds each debug ray stays visible
    protected float rayDisplayDuration = 0.5f;

    private int _currentAmmo;
    private bool _isReloading;
    private float _nextFireTime;
    private bool _hasLoggedEmpty;

    public string WeaponName => weaponName;
    public int CurrentAmmo => _currentAmmo;
    public int MagazineCapacity => magazineCapacity;
    public bool IsReloading => _isReloading;

    public abstract bool IsAutomatic { get; }

    protected UIManager uiManager;

    // ────────────────────────────────────────────────────────────────────────

    protected virtual void Awake()
    {
        uiManager = Object.FindFirstObjectByType<UIManager>();
    }

    protected void InitialiseAmmo()
    {
        _currentAmmo = magazineCapacity;
    }

    // ── IShootable ──────────────────────────────────────────────────────────

    public bool CanShoot()
    {
        return !_isReloading && _currentAmmo > 0 && Time.time >= _nextFireTime;
    }

    public void Shoot(Ray aimRay, LayerMask layerMask, float range)
    {
        if (_isReloading) return;

        if (_currentAmmo <= 0)
        {
            if (!_hasLoggedEmpty)
            {
                _hasLoggedEmpty = true;
                string msg = $"[{weaponName}] Sarjor bos! [R] tusuna basin.";
                Debug.Log(msg);
                uiManager?.ShowEvent(msg);
            }
            return;
        }

        if (Time.time < _nextFireTime) return;

        // ── Fire ────────────────────────────────────────────────────────────
        _nextFireTime = Time.time + (60f / rpm);
        _currentAmmo--;
        _hasLoggedEmpty = false;

        // ── Cone-of-fire deflection ─────────────────────────────────────────
        float effectiveCone = maxDeflectionAngle * (1f - accuracy);
        float deflectionDegrees = Random.Range(0f, effectiveCone);
        Ray deflectedRay = DeflectRay(aimRay, deflectionDegrees);

        Debug.Log($"[{weaponName}] Ateş edildi. Sapma: {deflectionDegrees:F2} derece , Mermi: {_currentAmmo}/{magazineCapacity}");
        uiManager?.ShowEvent($"[{weaponName}] Ateş edildi. Mermi: {_currentAmmo}/{magazineCapacity}");
        uiManager?.UpdateAmmo(_currentAmmo, magazineCapacity);

        // White line = perfect aim direction (before deflection)
        Debug.DrawRay(aimRay.origin, aimRay.direction * range, Color.white, rayDisplayDuration);

        // ── Raycast with deflected direction ────────────────────────────────
        if (Physics.Raycast(deflectedRay, out RaycastHit hit, range, layerMask))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            if (target != null && !target.IsDestroyed)
            {
                target.TakeDamage(damage);
                // Green line = deflected ray that hit
                Debug.DrawRay(deflectedRay.origin, deflectedRay.direction * hit.distance, Color.green, rayDisplayDuration);
                string hitMsg = $"[{weaponName}] HEDEF VURULDU. Hasar: {damage} | Sapma: {deflectionDegrees:F2}°";
                Debug.Log(hitMsg);
                uiManager?.ShowEvent(hitMsg);
            }
            else if (target != null && target.IsDestroyed)
            {
                // Yellow line = hit an already-destroyed target
                Debug.DrawRay(deflectedRay.origin, deflectedRay.direction * hit.distance, Color.yellow, rayDisplayDuration);
                Debug.Log($"[{weaponName}] Hedef zaten kullanamaz. [T] ile yenileyin.");
            }
        }
        else
        {
            // Red line = missed entirely
            Debug.DrawRay(deflectedRay.origin, deflectedRay.direction * range, Color.red, rayDisplayDuration);
            string missMsg = $"[{weaponName}] ISKALAMDI. Sapma: {deflectionDegrees:F2} derece";
            Debug.Log(missMsg);
            uiManager?.ShowEvent(missMsg);
        }

        // ── Magazine empty notification ─────────────────────────────────────
        if (_currentAmmo <= 0)
        {
            string emptyMsg = $"[{weaponName}] Şarjör boşaldı";
            Debug.Log(emptyMsg);
            uiManager?.ShowEvent(emptyMsg);
            _hasLoggedEmpty = true;
        }
    }

    // ── IReloadable ─────────────────────────────────────────────────────────

    public void StartReload()
    {
        if (_isReloading) return;
        if (_currentAmmo == magazineCapacity) return;
        StartCoroutine(ReloadCoroutine());
    }

    public void CancelReload()
    {
        if (!_isReloading) return;
        StopAllCoroutines();
        _isReloading = false;
        uiManager?.ShowReloading(false);
        Debug.Log($"[{weaponName}] Şarjör değiştirme iptal edildi.");
    }

    // ── Private helpers ─────────────────────────────────────────────────────

    private Ray DeflectRay(Ray baseRay, float angleDegrees)
    {
        if (angleDegrees <= 0f) return baseRay;

        Vector3 forward = baseRay.direction.normalized;
        Vector3 worldUp = Mathf.Abs(Vector3.Dot(forward, Vector3.up)) < 0.99f
                          ? Vector3.up : Vector3.right;
        Vector3 perpendicular = Vector3.Cross(forward, worldUp).normalized;

        float randomAzimuth = Random.Range(0f, 360f);
        perpendicular = Quaternion.AngleAxis(randomAzimuth, forward) * perpendicular;

        Vector3 deflectedDir = Quaternion.AngleAxis(angleDegrees, perpendicular) * forward;
        return new Ray(baseRay.origin, deflectedDir);
    }

    private IEnumerator ReloadCoroutine()
    {
        _isReloading = true;

        string startMsg = $"[{weaponName}] Şarjör dolduruluyor. ({reloadDuration}s)";
        Debug.Log(startMsg);
        uiManager?.ShowEvent(startMsg);
        uiManager?.ShowReloading(true);

        yield return new WaitForSeconds(reloadDuration);

        _currentAmmo = magazineCapacity;
        _isReloading = false;
        _hasLoggedEmpty = false;

        string doneMsg = $"[{weaponName}] Şarjör dolduruldu. Mermi: {_currentAmmo}/{magazineCapacity}";
        Debug.Log(doneMsg);
        uiManager?.ShowEvent(doneMsg);
        uiManager?.ShowReloading(false);
        uiManager?.UpdateAmmo(_currentAmmo, magazineCapacity);
    }
}
