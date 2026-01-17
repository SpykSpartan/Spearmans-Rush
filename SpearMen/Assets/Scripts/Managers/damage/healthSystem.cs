using System.Collections;
using UnityEngine;

public class healthSystem : MonoBehaviour, IDamageable
{
    [Header("Health + Death Settings")]
    public int baseMaxHealth = 100;
    [SerializeField] private int currentHealth;
    public int maxHealth { get; private set; }

    public bool IsDead => isDead;
    private bool isDead = false;

    public delegate void HealthChanged(int current, int max);
    public event HealthChanged OnHealthChanged;

    public delegate void DeathEvent();
    public event DeathEvent OnDeath;

    private PlayerStat statSystem;

    public bool isInvulnerable = false;

    private Animator animator;
    private DamageAudioManager damageSFX;

    [Header("Regeneration Settings")]
    public bool enableRegen = true;
    public float regenDelay = 3f;
    public float regenInterval = 1f;
    public int regenAmount = 3;

    private Coroutine regenCoroutine;

    [Header("Hit VFX Settings")]
    public GameObject hitVFXObject;
    public GameObject healVFX;
    public float hitVFXDuration = 0.25f;
    public float healVFXDuration = 0.25f;
    public Transform vfxSpawnPoint;

    private PlayerMovement playerMovement;
    private UnityEngine.AI.NavMeshAgent aiAgent;

    private void Start()
    {
        statSystem = GetComponent<PlayerStat>();
        animator = GetComponent<Animator>();
        damageSFX = GetComponent<DamageAudioManager>();

        playerMovement = GetComponent<PlayerMovement>();
        aiAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        UpdateMaxHealth();

        if (currentHealth <= 0)
            currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (!CompareTag("Player")) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            TryStartRegen();
        }
    }

    public void UpdateMaxHealth()
    {
        if (statSystem != null)
        {
            maxHealth = Mathf.RoundToInt(baseMaxHealth * statSystem.healthMultiplier);
        }
        else
        {
            maxHealth = baseMaxHealth;
        }

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || isInvulnerable) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        if (damageSFX != null)
            damageSFX.PlayDamageSFX();

        SpawnHitVFX();

        Debug.Log($"{gameObject.name} took {amount} damage. Health now: {currentHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (regenCoroutine != null)
                StopRegen("Took damage, regen cancelled.");
        }
    }

    private void SpawnHitVFX()
    {
        if (hitVFXObject == null) return;

        ParticleSystem ps = hitVFXObject.GetComponent<ParticleSystem>();

        hitVFXObject.SetActive(true);

        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }

        StartCoroutine(DisableVFX(hitVFXDuration));
    }

    private IEnumerator DisableVFX(float delay)
    {
        yield return new WaitForSeconds(delay);
        hitVFXObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        SpawnHealVFX();
    }

    private void SpawnHealVFX()
    {
        if (healVFX == null) return;
        
        ParticleSystem ps = healVFX.GetComponent<ParticleSystem>();

        healVFX.SetActive(true);

        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }

        StartCoroutine(DisableHealVFX(healVFXDuration));
    }


    private IEnumerator DisableHealVFX(float delay)
    {
        yield return new WaitForSeconds(delay);
        healVFX.SetActive(false);
    }

    public void ForceSetHealth(int amount)
    {
        currentHealth = Mathf.Clamp(amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TryStartRegen()
    {
        if (!enableRegen || IsDead || currentHealth >= maxHealth) return;

        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegenerateHealth());
        }
    }

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(regenDelay);

        while (!IsDead && currentHealth < maxHealth)
        {
            Heal(regenAmount);
            yield return new WaitForSeconds(regenInterval);
        }

        regenCoroutine = null;
    }

    private void StopRegen(string reason = "")
    {
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
            Debug.Log($"Regen stopped. Reason: {reason}");
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} has died.");
        OnDeath?.Invoke();

        StopRegen("Entity died.");

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (aiAgent != null)
            aiAgent.isStopped = true;

        if (animator != null)
            animator.SetTrigger("Death");

        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence()
    {
        float deathAnimLength = 2f;

        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float timer = 0f;

            while (!stateInfo.IsName("Death") && timer < 3f)
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                timer += Time.deltaTime;
            }

            deathAnimLength = (stateInfo.IsName("Death")) ? stateInfo.length : 2f;
        }

        yield return new WaitForSeconds(deathAnimLength);

        Destroy(gameObject);
    }
}