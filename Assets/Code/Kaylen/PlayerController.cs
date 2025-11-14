using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float holdMoveSpeed = 2.5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;
    public float gravity = 20f;

    [Header("References")]
    public Transform playerModel;

    [Header("Tag Settings")]
    public GameObject tagHitboxPrefab;
    public float tagCooldown = 1f;
    public float tagOffset = 1f;

    [Header("Health Settings")]
    public List<Image> heartsUI;
    public Sprite fullHeartSprite;
    public Sprite lostHeartSprite;
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.1f;

    [Header("Damage Settings")]
    public float invincibilityTime = 3f;
    private bool isInvincible = false;

    [Header("Animation")]
    public Animator animator;

    [Header("Star Tracking")]
    public bool pickedUpKey = false;
    public bool tookDamage = false;


    private Rigidbody rb;
    private bool canTag = true;
    private GameObject activeHitbox;
    private Coroutine tagCoroutine;
    private bool isGrounded;

    private Runner heldRunner = null;
    public Transform holdPoint;
    public bool isHoldingRunner { get; private set; } = false;
    private Coroutine holdReleaseCoroutine;

    private void Awake()
    {
        currentHealth = maxHealth;
        for (int i = 0; i < heartsUI.Count; i++)
        {
            if (i < maxHealth) heartsUI[i].sprite = fullHeartSprite;
            else heartsUI[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (playerModel == null)
            Debug.LogWarning("PlayerMovement: playerModel not assigned.");

        if (holdPoint == null)
        {
            holdPoint = new GameObject("HoldPoint").transform;
            holdPoint.SetParent(playerModel, true);
        }
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, GetComponent<CapsuleCollider>().height / 2 + groundCheckDistance);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Vector3 v = rb.linearVelocity;
            v.y = jumpForce;
            rb.linearVelocity = v;
        }

        // Tag input (left click)
        if (Input.GetMouseButtonDown(0))
        {
            // Trigger catch animation
            if (animator != null)
                animator.SetTrigger("catching");

            if (isHoldingRunner)
            {
                // Drop if already holding
                DropRunner();
            }
            else if (canTag)
            {
                // Start tag attempt
                if (tagCoroutine != null) StopCoroutine(tagCoroutine);
                tagCoroutine = StartCoroutine(DoTag());
            }
        }

        // Movement input
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        bool isWalking = new Vector3(x, 0f, z).sqrMagnitude > 0.01f;

        // Apply Animator parameters
        if (animator != null)
        {
            animator.SetBool("walking", isWalking);
            animator.SetBool("isHolding", isHoldingRunner);
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();

        if (!isGrounded)
            rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        if (isHoldingRunner && heldRunner != null)
        {
            heldRunner.transform.position = holdPoint.position;
            heldRunner.transform.rotation = holdPoint.rotation;
        }
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(x, 0f, z).normalized;
        float speed = isHoldingRunner ? holdMoveSpeed : moveSpeed;
        Vector3 move = inputDir * speed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        if (inputDir.sqrMagnitude > 0.01f && playerModel != null)
        {
            Quaternion targetRot = Quaternion.LookRotation(inputDir);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator DoTag()
    {
        if (!canTag) yield break;
        canTag = false;
        FindAnyObjectByType<GrabUI>()?.StartCooldownUI(tagCooldown);

        if (activeHitbox != null) Destroy(activeHitbox);

        if (playerModel == null)
        {
            Debug.LogError("DoTag: playerModel is null, cannot spawn hitbox.");
            canTag = true;
            yield break;
        }

        Vector3 spawnPos = playerModel.position + playerModel.forward * tagOffset;
        Quaternion spawnRot = playerModel.rotation;

        activeHitbox = Instantiate(tagHitboxPrefab, spawnPos, spawnRot);
        TagHitbox th = activeHitbox.GetComponent<TagHitbox>();
        if (th != null) th.owner = this;

        activeHitbox.transform.SetParent(playerModel, true);

        yield return new WaitForSeconds(tagCooldown);

        if (activeHitbox != null) Destroy(activeHitbox);

        canTag = true;
        tagCoroutine = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Tagger")) TakeDamage();
    }

    public void PickUpRunner(Runner runner)
    {
        if (runner == null || isHoldingRunner) return;

        holdPoint.rotation = playerModel.rotation;

        Collider runCol = runner.GetComponent<Collider>();
        Rigidbody runRb = runner.GetComponent<Rigidbody>();
        Runner runAgent = runner.GetComponent<Runner>();

        if (runRb != null) runRb.isKinematic = true;
        if (runCol != null) runCol.enabled = false;
        if (runAgent != null) runAgent.enabled = false;

        heldRunner = runner;
        isHoldingRunner = true;

        runner.transform.SetParent(holdPoint, true);
        runner.transform.localPosition = Vector3.zero;
        runner.transform.localRotation = Quaternion.identity;

        if (holdReleaseCoroutine != null) StopCoroutine(holdReleaseCoroutine);
        holdReleaseCoroutine = StartCoroutine(AutoReleaseRunner());
    }

    private IEnumerator AutoReleaseRunner()
    {
        float duration = Random.Range(5f, 10f);
        yield return new WaitForSeconds(duration);

        if (isHoldingRunner) DropRunner();
    }

    public void DropRunner()
    {
        if (!isHoldingRunner || heldRunner == null) return;

        Collider runCol = heldRunner.GetComponent<Collider>();
        Rigidbody runRb = heldRunner.GetComponent<Rigidbody>();
        Runner runAgent = heldRunner.GetComponent<Runner>();

        // Re-enable collider and runner script (but keep physics disabled until we set position)
        if (runCol != null) runCol.enabled = true;
        if (runAgent != null) runAgent.enabled = false; // disable briefly while we reposition

        // Detach from player (no world-position preserving surprises)
        heldRunner.transform.SetParent(null);

        // Desired drop position in world-space (in front of player)
        Vector3 dropPosition = transform.position + transform.forward * 1.5f + Vector3.up * 0.1f;
        Quaternion dropRotation = Quaternion.LookRotation(transform.forward, Vector3.up);

        // If there's a Rigidbody, set its position and zero velocities properly:
        if (runRb != null)
        {
            // Make sure it's kinematic while we set the position cleanly
            runRb.isKinematic = true;

            // Force position directly on the Rigidbody (world-space)
            runRb.position = dropPosition;
            runRb.rotation = dropRotation;

            // MovePosition to ensure interpolation/internal state is correct
            runRb.MovePosition(dropPosition);
            runRb.linearVelocity = Vector3.zero;
            runRb.angularVelocity = Vector3.zero;

            // Now re-enable physics and agent
            runRb.isKinematic = false;
        }
        else
        {
            // No rigidbody — set transform directly
            heldRunner.transform.position = dropPosition;
            heldRunner.transform.rotation = dropRotation;
        }

        // Re-enable Runner script AFTER we've placed it
        if (runAgent != null)
        {
            runAgent.enabled = true;
            runAgent.OnDropped(); // tells Runner to resume from current position (does NOT teleport)
        }

        // cleanup
        heldRunner = null;
        isHoldingRunner = false;

        if (holdReleaseCoroutine != null)
        {
            StopCoroutine(holdReleaseCoroutine);
            holdReleaseCoroutine = null;
        }

        Debug.Log("Dropped runner at: " + dropPosition);
    }


    public Runner GetHeldRunner() => heldRunner != null ? heldRunner.GetComponent<Runner>() : null;

    private void TakeDamage()
    {
        if (currentHealth <= 0 || isInvincible) return;

        currentHealth--;
        tookDamage = true; // Player has taken damage

        if (currentHealth < heartsUI.Count)
            heartsUI[currentHealth].sprite = lostHeartSprite;

        StartCoroutine(InvincibilityCoroutine());

        if (currentHealth <= 0) Die();
    }


    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("Player died!");
        enabled = false;
    }

    public void DisableTagging()
    {
        canTag = false;
        if (tagCoroutine != null) StopCoroutine(tagCoroutine);
        if (activeHitbox != null) Destroy(activeHitbox);

        if (animator != null)
        {
            animator.SetLayerWeight(1, 0f);
        }
    }


    public bool HasPerfectRun()
    {
        return pickedUpKey && !tookDamage;
    }

}
