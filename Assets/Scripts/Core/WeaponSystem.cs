using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Central controller that bridges player input with the active weapon and targets.
///
/// Key bindings:
///   S     → Shoot   (semi-auto = wasPressedThisFrame, full-auto = isPressed)
///   Space → Switch weapon
///   R     → Reload active weapon
///   T     → Reset ALL targets in the scene
///
/// Shooting pipeline:
///   WeaponSystem builds the aim Ray from the camera centre and passes it to
///   WeaponBase.Shoot() along with the layer mask and range. WeaponBase applies
///   its own cone-of-fire deflection and raycasts to resolve the hit internally.
/// </summary>
public class WeaponSystem : MonoBehaviour
{
    [Header("Drag weapons from the hierarchy")]
    [SerializeField] private DesertEagle desertEagle;
    [SerializeField] private AK47 ak47;

    [Header("Shooting")]
    [Tooltip("Maximum raycast distance")]
    [SerializeField] private float shootRange = 200f;

    [SerializeField] private LayerMask shootLayerMask = ~0;

    private List<WeaponBase> _weapons;
    private int _activeIndex;
    private WeaponBase _activeWeapon;
    private UIManager _ui;
    private Target[] _allTargets;

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _ui = Object.FindFirstObjectByType<UIManager>();
        _weapons = new List<WeaponBase> { desertEagle, ak47 };
    }

    private void Start()
    {
        _allTargets = Object.FindObjectsByType<Target>(FindObjectsSortMode.None);
        _activeIndex = 0;
        _activeWeapon = _weapons[_activeIndex];
        AnnounceActiveWeapon();
    }

    private void Update()
    {
        var keyBoard = Keyboard.current;
        if (keyBoard == null) return;

        HandleShoot(keyBoard);
        HandleWeaponSwitch(keyBoard);
        HandleReload(keyBoard);
        HandleTargetReset(keyBoard);
    }

    // ─────────────────────────────────────────────────────────────────────────

    private void HandleShoot(Keyboard keyBoard)
    {
        // Build the aim ray once — straight from camera centre forward
        Ray aimRay = BuildAimRay();

        if (_activeWeapon.IsAutomatic)
        {
            if (keyBoard.sKey.isPressed)
                _activeWeapon.Shoot(aimRay, shootLayerMask, shootRange);
        }
        else
        {
            if (keyBoard.sKey.wasPressedThisFrame)
                _activeWeapon.Shoot(aimRay, shootLayerMask, shootRange);
        }
    }

    private void HandleWeaponSwitch(Keyboard keyBoard)
    {
        if (!keyBoard.spaceKey.wasPressedThisFrame) return;

        _activeWeapon.CancelReload();
        _activeIndex = (_activeIndex + 1) % _weapons.Count;
        _activeWeapon = _weapons[_activeIndex];
        AnnounceActiveWeapon();
    }

    private void HandleReload(Keyboard keyBoard)
    {
        if (keyBoard.rKey.wasPressedThisFrame)
            _activeWeapon.StartReload();
    }

    private void HandleTargetReset(Keyboard keyBoard)
    {
        if (!keyBoard.tKey.wasPressedThisFrame) return;

        foreach (Target target in _allTargets)
            target.Reset();
    }

    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Constructs a ray from the camera centre pointing straight forward.
    /// WeaponBase then deflects this ray according to its cone-of-fire.
    /// </summary>
    private Ray BuildAimRay()
    {
        Camera cam = Camera.main;
        if (cam == null) return default;

        return new Ray(cam.transform.position, cam.transform.forward);
    }

    private void AnnounceActiveWeapon()
    {
        string msg = $"Silah değiştirildi -> Aktif silah: {_activeWeapon.WeaponName} " +
                     $"Mermi: {_activeWeapon.CurrentAmmo}/{_activeWeapon.MagazineCapacity}";
        Debug.Log(msg);
        _ui?.ShowEvent(msg);
        _ui?.UpdateWeaponName(_activeWeapon.WeaponName);
        _ui?.UpdateAmmo(_activeWeapon.CurrentAmmo, _activeWeapon.MagazineCapacity);
        _ui?.ShowReloading(_activeWeapon.IsReloading);
    }
}
