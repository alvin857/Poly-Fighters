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

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 4f, 0);
    public float healthBarSmoothSpeed = 5f;

    private float currentHealth;
    private float displayedHealth;
    private float lastAttackTime;
    private UnitAI target;

    private Image healthBarFill;
    private Transform healthBarTransform;  // Store the root transform of the health bar

    void Start()
    {
        currentHealth = maxHealth;
        displayedHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            GameObject bar = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            
            // Don't parent it to the unit - keep it independent
            healthBarTransform = bar.transform;

            // Find the "Fill" child specifically
            Transform fillTransform = bar.transform.Find("Fill");
            if (fillTransform != null)
            {
                healthBarFill = fillTransform.GetComponent<Image>();
                if (healthBarFill == null)
                {
                    Debug.LogError("Fill object missing Image component.");
                }
                else
                {
                    // Make sure Fill is set to Filled type
                    healthBarFill.type = Image.Type.Filled;
                    healthBarFill.fillMethod = Image.FillMethod.Horizontal;
                    healthBarFill.fillAmount = 1f;
                }
            }
            else
            {
                Debug.LogError("HealthBarPrefab missing 'Fill' child object.");
            }
        }
    }

    void Update()
    {
        // Update health bar
        if (healthBarFill != null && healthBarTransform != null)
        {
            // Smoothly animate the fill amount
            displayedHealth = Mathf.Lerp(displayedHealth, currentHealth, Time.deltaTime * healthBarSmoothSpeed);
            float healthPercent = Mathf.Clamp01(displayedHealth / maxHealth);
            healthBarFill.fillAmount = healthPercent;

            // Color transition: Green (full health) -> Yellow -> Red (low health)
            healthBarFill.color = Color.Lerp(Color.red, Color.green, healthPercent);

            // Update position to follow unit
            healthBarTransform.position = transform.position + healthBarOffset;

            // Face the camera (billboard effect)
            if (Camera.main != null)
            {
                // Make it face the camera by looking at camera and then rotating 180 degrees
                Vector3 directionToCamera = Camera.main.transform.position - healthBarTransform.position;
                healthBarTransform.rotation = Quaternion.LookRotation(directionToCamera);
            }
        }

        // Combat / AI
        AcquireTarget();
        if (target == null) return;

        RotateTowardsTarget();

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > attackRange)
        {
            MoveForward();
        }
        else
        {
            TryAttack();
        }
    }

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

    void RotateTowardsTarget()
    {
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * 360f * Time.deltaTime
        );
    }

    void MoveForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;
        target.TakeDamage(attackDamage);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            // Destroy the health bar before destroying the unit
            if (healthBarTransform != null)
            {
                Destroy(healthBarTransform.gameObject);
            }
            Destroy(gameObject);
        }
    }
}