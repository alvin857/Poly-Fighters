using UnityEngine;
using UnityEngine.UI;

public enum Team
{
    TeamA,
    TeamB
}

public class UnitAI : MonoBehaviour
{
    [Header("Team")]
    public Team team;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float rotationSpeed = 8f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;

    [Header("Animation")]
    public Animator animator;

    [Header("Blocking")]
    public bool canBlock = false;
    [Range(0f, 1f)]
    public float blockChance = 0.3f;
    public GameObject blockSparkPrefab;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 4f, 0);
    public float healthBarSmoothSpeed = 5f;

    private float currentHealth;
    private float displayedHealth;
    private float lastAttackTime;
    private UnitAI target;

    private Image healthBarFill;
    private Transform healthBarTransform;

    void Start()
    {
        // Safety: auto-find animator if not assigned
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        currentHealth = maxHealth;
        displayedHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            GameObject bar = Instantiate(
                healthBarPrefab,
                transform.position + healthBarOffset,
                Quaternion.identity
            );

            healthBarTransform = bar.transform;

            Transform fill = bar.transform.Find("Fill");
            if (fill != null)
            {
                healthBarFill = fill.GetComponent<Image>();
                healthBarFill.type = Image.Type.Filled;
                healthBarFill.fillMethod = Image.FillMethod.Horizontal;
                healthBarFill.fillAmount = 1f;
            }
            else
            {
                Debug.LogError("HealthBarPrefab missing Fill child.");
            }
        }
    }

    void Update()
    {
        UpdateHealthBar();

        AcquireTarget();
        if (target == null)
        {
            SetIdle();
            return;
        }

        RotateTowardsTarget();

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > attackRange)
        {
            MoveForward();
            SetWalking();
        }
        else
        {
            SetIdle();
            TryAttack();
        }
    }

    // ---------------- HEALTH BAR ----------------
    void UpdateHealthBar()
    {
        if (healthBarFill == null || healthBarTransform == null) return;

        displayedHealth = Mathf.Lerp(
            displayedHealth,
            currentHealth,
            Time.deltaTime * healthBarSmoothSpeed
        );

        float percent = displayedHealth / maxHealth;
        healthBarFill.fillAmount = percent;
        healthBarFill.color = Color.Lerp(Color.red, Color.green, percent);

        healthBarTransform.position = transform.position + healthBarOffset;

        if (Camera.main != null)
        {
            healthBarTransform.LookAt(Camera.main.transform);
        }
    }

    // ---------------- TARGETING ----------------
    void AcquireTarget()
    {
        UnitAI[] units = FindObjectsOfType<UnitAI>();
        float closest = Mathf.Infinity;
        UnitAI closestEnemy = null;

        foreach (UnitAI unit in units)
        {
            if (unit == this) continue;
            if (unit.team == team) continue;

            float d = Vector3.Distance(transform.position, unit.transform.position);
            if (d < closest)
            {
                closest = d;
                closestEnemy = unit;
            }
        }

        target = closestEnemy;
    }

    // ---------------- ROTATION ----------------
    void RotateTowardsTarget()
    {
        Vector3 dir = target.transform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            rot,
            rotationSpeed * 360f * Time.deltaTime
        );
    }

    // ---------------- MOVEMENT ----------------
    void MoveForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    // ---------------- COMBAT ----------------
    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;
        animator.SetTrigger("isAttacking");
        target.TakeDamage(attackDamage);
    }

    // ---------------- ANIMATION HELPERS ----------------
    void SetWalking()
    {
        animator.SetBool("isWalking", true);
    }

    void SetIdle()
    {
        animator.SetBool("isWalking", false);
    }

    // ---------------- DAMAGE & BLOCKING ----------------
    public void TakeDamage(float damage)
    {
        bool blocked = TryBlock();

        if (blocked)
        {
            damage *= 0.5f; // half damage on block
            SpawnBlockSpark();
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            if (healthBarTransform != null)
                Destroy(healthBarTransform.gameObject);

            Destroy(gameObject);
        }
    }

    bool TryBlock()
    {
        if (!canBlock) return false;

        return Random.value <= blockChance;
    }

    void SpawnBlockSpark()
{
    if (blockSparkPrefab == null) return;

    Vector3 sparkPos = transform.position + Vector3.up * 1.5f;
    GameObject spark = Instantiate(blockSparkPrefab, sparkPos, Quaternion.identity);

    Destroy(spark, 1f);
}
}
