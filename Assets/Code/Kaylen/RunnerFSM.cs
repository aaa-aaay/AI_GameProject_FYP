using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Runner : MonoBehaviour
{
    public enum State { Idle, Run }
    public State currentState = State.Idle;

    [Header("Movement Settings")]
    public float nodeReachThreshold = 0.5f;
    public float dangerRadius = 5f;
    public float jumpHeight = 1.5f;
    public float jumpDuration = 0.5f;
    public float jumpDistance = 1.5f;

    [Header("References")]
    public List<PathNode> allNodes;
    [Header("Animation Settings")]
    public Animator animator;

    private Rigidbody rb;
    private bool isJumping = false;
    private bool isBeingHeld = false;

    private Vector3 jumpStartPos;
    private Vector3 jumpTargetPos;
    private float jumpTimer = 0f;

    public List<PathNode> path = new List<PathNode>();
    public int pathIndex = 0;
    private Transform closestDanger;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = true;

        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Runner Animator not assigned!");

        PickNewIdleTarget();
    }

    void Update()
    {
        // Stop AI if currently being held
        if (isBeingHeld)
            return;

        DetectDanger();

        switch (currentState)
        {
            case State.Idle:
                IdleBehaviour();
                break;
            case State.Run:
                RunBehaviour(closestDanger);
                break;
        }

        HandleJump();
    }

    // ---------------------------
    //  PICKUP / DROP HANDLERS
    // ---------------------------

    public void OnPickedUp()
    {
        // Called externally by Player script when picked up
        isBeingHeld = true;
        rb.isKinematic = true; // So physics won't move it
        path.Clear(); // Stop path movement temporarily
    }

    // Add this public method to Runner (inside class)
    public void OnDropped()
    {
        // Ensure we aren't considered "being held" anymore
        isBeingHeld = false;

        // Do not change transform here. Use current world position as the start point.

        // Recompute path relative to current location WITHOUT teleporting the runner.
        // Choose a sensible next goal: if currently in "Run" state keep escaping behavior, else pick idle route.
        if (currentState == State.Run && closestDanger != null)
        {
            // Rebuild escape path from current position
            PickFurthestNodeFrom(closestDanger);
        }
        else
        {
            // Continue wandering — pick a new idle target that begins from the closest node
            PathNode closest = FindClosestNode();
            if (closest != null)
            {
                // Build path from closest node to some random goal but do not set transform
                PathNode goal = allNodes[Random.Range(0, allNodes.Count)];
                path = AStar.FindPath(closest, goal);
                pathIndex = 0;
            }
        }

        // Reset motion state so the next Update/MoveAlongPath uses current transform as start.
        isJumping = false;
        jumpTimer = 0f;
    }


    // ---------------------------
    //  MAIN BEHAVIOURS
    // ---------------------------

    private void DetectDanger()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, dangerRadius);
        bool dangerDetected = false;
        Transform closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if ((hit.CompareTag("Player") || hit.CompareTag("Tagger") || hit.CompareTag("Runner")) && hit.gameObject != gameObject)
            {
                float d = Vector3.Distance(transform.position, hit.transform.position);
                if (d < closestDist)
                {
                    closestDist = d;
                    closest = hit.transform;
                    dangerDetected = true;
                }
            }
        }

        closestDanger = closest;

        if (dangerDetected)
        {
            if (currentState != State.Run)
            {
                currentState = State.Run;
                PickFurthestNodeFrom(closest);
            }
        }
        else if (currentState == State.Run && !dangerDetected)
        {
            currentState = State.Idle;
            PickNewIdleTarget();
        }
    }

    void IdleBehaviour()
    {
        MoveAlongPath();
        if (path.Count == 0 || pathIndex >= path.Count)
            PickNewIdleTarget();
    }

    void RunBehaviour(Transform dangerSource)
    {
        MoveAlongPath();
        if (path.Count == 0 || pathIndex >= path.Count)
        {
            if (dangerSource != null)
                PickFurthestNodeFrom(dangerSource);
            else
                PickNewIdleTarget();
        }
    }

    // ---------------------------
    //  PATHFINDING / MOVEMENT
    // ---------------------------

    void PickNewIdleTarget()
    {
        PathNode start = FindClosestNode();
        PathNode goal = allNodes[Random.Range(0, allNodes.Count)];
        path = AStar.FindPath(start, goal);
        pathIndex = 0;
    }

    void PickFurthestNodeFrom(Transform target)
    {
        PathNode start = FindClosestNode();
        if (target == null) return;

        Vector3 toTarget = (target.position - transform.position).normalized;
        PathNode bestNode = null;
        float bestScore = -Mathf.Infinity;

        foreach (var node in allNodes)
        {
            Vector3 toNode = (node.transform.position - transform.position).normalized;
            float alignment = Vector3.Dot(toNode, toTarget);

            if (alignment < 0f)
            {
                float dist = Vector3.Distance(node.transform.position, target.position);
                float score = dist * (1f - alignment);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestNode = node;
                }
            }
        }

        if (bestNode == null)
        {
            float maxDist = -Mathf.Infinity;
            foreach (var node in allNodes)
            {
                float d = Vector3.Distance(node.transform.position, target.position);
                if (d > maxDist)
                {
                    maxDist = d;
                    bestNode = node;
                }
            }
        }

        if (bestNode != null)
        {
            path = AStar.FindPath(start, bestNode);
            pathIndex = 0;
        }
    }

    void MoveAlongPath()
    {
        if (isJumping || path == null || pathIndex >= path.Count) return;

        Vector3 nodeTarget = path[pathIndex].transform.position;
        Vector3 dirToNode = nodeTarget - transform.position;
        float distToNode = dirToNode.magnitude;

        if (distToNode > nodeReachThreshold)
        {
            Vector3 stepDir = dirToNode.normalized;
            Vector3 smallTarget = transform.position + stepDir * Mathf.Min(jumpDistance, distToNode);
            StartJump(smallTarget);
        }
        else
        {
            pathIndex++;
        }
    }

    void StartJump(Vector3 targetPos)
    {
        isJumping = true;
        jumpStartPos = transform.position;
        jumpTargetPos = targetPos;
        jumpTimer = 0f;

        if (animator != null)
            animator.Play("Jump");
    }

    void HandleJump()
    {
        if (!isJumping) return;

        jumpTimer += Time.deltaTime;
        float t = jumpTimer / jumpDuration;

        float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
        Vector3 pos = Vector3.Lerp(jumpStartPos, jumpTargetPos, t);
        pos.y += height;

        rb.MovePosition(pos);

        Vector3 lookDir = (jumpTargetPos - jumpStartPos).normalized;
        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(new Vector3(lookDir.x, 0, lookDir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }

        if (t >= 1f)
        {
            isJumping = false;
        }
    }

    PathNode FindClosestNode()
    {
        PathNode closest = null;
        float minDist = Mathf.Infinity;

        foreach (var n in allNodes)
        {
            float d = Vector3.Distance(transform.position, n.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = n;
            }
        }

        return closest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerRadius);

        if (closestDanger != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, closestDanger.position);
        }
    }
}
