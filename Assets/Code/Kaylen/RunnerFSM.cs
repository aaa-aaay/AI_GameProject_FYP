using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Runner : MonoBehaviour
{
    public enum State { Idle, Run }
    public State currentState = State.Idle;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float nodeReachThreshold = 0.5f;
    public float dangerRadius = 5f;

    [Header("References")]
    public List<PathNode> allNodes;     // Assign all PathNodes in the arena

    private Rigidbody rb;
    public List<PathNode> path = new List<PathNode>();
    public int pathIndex = 0;

    // Track the closest danger for Gizmo line
    private Transform closestDanger;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        PickNewIdleTarget(); // start by idling
    }

    void Update()
    {
        // Detect any object with tag "Runner" or "Tagger" in danger radius
        Collider[] hits = Physics.OverlapSphere(transform.position, dangerRadius);
        bool dangerDetected = false;
        Transform closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if ((hit.CompareTag("Runner") || hit.CompareTag("Tagger")) && hit.gameObject != gameObject)
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

        closestDanger = closest; // store for Gizmo

        // Switch state based on detection
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

        // Execute behavior
        switch (currentState)
        {
            case State.Idle: IdleBehaviour(); break;
            case State.Run: RunBehaviour(closest); break;
        }
    }

    // -------------------------------
    void IdleBehaviour()
    {
        MoveAlongPath();

        if (path.Count == 0 || pathIndex >= path.Count)
            PickNewIdleTarget();
    }

    void PickNewIdleTarget()
    {
        PathNode start = FindClosestNode();
        PathNode goal = allNodes[Random.Range(0, allNodes.Count)];
        path = AStar.FindPath(start, goal);
        pathIndex = 0;
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

            // Prefer nodes opposite direction from target
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

        // Fallback: just the furthest node
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
        if (path == null || path.Count == 0) return;
        if (pathIndex >= path.Count) return;

        Vector3 targetPos = path[pathIndex].transform.position;
        Vector3 dir = targetPos - transform.position;
        float dist = dir.magnitude;

        if (dist < nodeReachThreshold)
        {
            pathIndex++;
            return;
        }

        dir.Normalize();
        rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
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
        // Draw danger radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerRadius);

        // Draw line to closest danger
        if (closestDanger != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, closestDanger.position);
        }
    }
}
