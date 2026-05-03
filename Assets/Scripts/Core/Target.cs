using UnityEngine;

/// <summary>
/// Represents the shooting-range target. Implements IDamageable so any weapon
/// can damage it without coupling to this specific class.
///
/// Behaviour:
///   • Starts with 100 HP.
///   • Changes colour to red when destroyed (visual cue with no models needed).
///   • Pressing [T] calls Reset(), returning it to full health and white colour.
///   • Optionally oscillates left/right from its starting position (toggle in Inspector).
///   • Movement stops when destroyed and resumes on Reset().
/// </summary>
public class Target : MonoBehaviour, IDamageable
{
    private const float MaxHealth = 100f;

    // ─── Movement (configure per-target in the Inspector) ─────────────────────

    [Header("Movement")]
    [Tooltip("Tick to make this target move. Leave unticked for a static target.")]
    [SerializeField] private bool isMoving = false;

    [Tooltip("How many full left right cycles per second.")]
    [SerializeField] private float moveSpeed = 1.5f;

    [Tooltip("How far the target travels left and right from its start position.")]
    [SerializeField] private float moveRange = 3f;

    // ─── Runtime state ────────────────────────────────────────────────────────

    private float _currentHealth;
    private bool _isDestroyed;
    private Vector3 _startPosition;   // World position when the scene starts
    private UIManager _ui;
    private Renderer _renderer;

    // ─── IDamageable ──────────────────────────────────────────────────────────
    public bool IsDestroyed => _isDestroyed;

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _ui = Object.FindFirstObjectByType<UIManager>();
        _renderer = GetComponent<Renderer>();
        _startPosition = transform.position;   // Remember where we were placed in the scene
        Reset();
    }

    private void Update()
    {
        // Only move while alive and movement is enabled
        if (!isMoving || _isDestroyed) return;

        // Mathf.Sin produces a smooth -1 → +1 wave over time.
        // Multiplying by moveRange scales the wave to the desired distance.
        // Multiplying Time.time by moveSpeed controls how fast the wave cycles.
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = _startPosition + transform.right * offset;
    }

    // ─────────────────────────────────────────────────────────────────────────

    public void TakeDamage(float amount)
    {
        if (_isDestroyed) return;

        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0f);

        if (_currentHealth <= 0f)
        {
            _isDestroyed = true;

            if (_renderer != null)
                _renderer.material.color = Color.red;

            string msg = "HEDEF KULLANAMAZ DURUMA GELDI, [T] ile yenileyin";
            Debug.Log(msg);
            _ui?.ShowEvent(msg);
            _ui?.UpdateTargetHealth(0f, destroyed: true);
        }
        else
        {
            Debug.Log($"Hedef sağlık: {_currentHealth}/{MaxHealth}");
            _ui?.UpdateTargetHealth(_currentHealth, destroyed: false);
        }
    }

    public void Reset()
    {
        _currentHealth = MaxHealth;
        _isDestroyed = false;

        // Snap back to the original placed position before movement resumes
        transform.position = _startPosition;

        if (_renderer != null)
            _renderer.material.color = Color.white;

        string msg = $"Hedef yenilendi. Sağlık: {_currentHealth}/{MaxHealth}";
        Debug.Log(msg);
        _ui?.ShowEvent(msg);
        _ui?.UpdateTargetHealth(_currentHealth, destroyed: false);
    }
}
