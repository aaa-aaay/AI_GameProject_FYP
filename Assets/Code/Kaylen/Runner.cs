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
    public Transform tagger;            // Tagger AI
    public List<PathNode> allNodes;     // Assign all PathNodes in the arena

    private Rigidbody rb;
    public List<PathNode> path = new List<PathNode>();
    public int pathIndex = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        PickNewIdleTarget(); // start by idling
    }

    void Update()
    {
        float distToTagger = Vector3.Distance(transform.position, tagger.position);

        // --- State switching ---
        if (distToTagger <= dangerRadius)
        {
            currentState = State.Run;
        }
        else if (currentState == State.Run && distToTagger > dangerRadius * 1.5f)
        {
            // Safe: go back to idle wandering
            currentState = State.Idle;
            PickNewIdleTarget();
        }

        // --- Execute state behaviour ---
        switch (currentState)
        {
            case State.Idle:
                IdleBehaviour();
                break;
            case State.Run:
                RunBehaviour();
                break;
        }
    }

    // -------------------------------
    // IDLE: move to a random node
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

    // -------------------------------
    // RUN: chain furthest nodes until safe
    // -------------------------------
    void RunBehaviour()
    {
        MoveAlongPath();

        // Reached end of current path
        if (path.Count == 0 || pathIndex >= path.Count)
        {
            float distToTagger = Vector3.Distance(transform.position, tagger.position);

            if (distToTagger <= dangerRadius)
            {
                // Still in danger → keep running to the next furthest node
                PickFurthestNodeFromTagger();
            }
            else
            {
                // Safe → back to idle
                currentState = State.Idle;
                PickNewIdleTarget();
            }
        }
    }

    void PickFurthestNodeFromTagger()
    {
        PathNode start = FindClosestNode();

        if (tagger == null) return;

        Vector3 toTagger = (tagger.position - transform.position).normalized;

        PathNode bestNode = null;
        float bestScore = -Mathf.Infinity;

        foreach (var node in allNodes)
        {
            Vector3 toNode = (node.transform.position - transform.position).normalized;

            // Dot product tells relative direction (-1 = opposite, +1 = same direction)
            float alignment = Vector3.Dot(toNode, toTagger);

            // Prefer nodes *behind* relative to tagger
            if (alignment < 0f)
            {
                // Score = distance to tagger (further = better) + weight to "behind-ness"
                float dist = Vector3.Distance(node.transform.position, tagger.position);
                float score = dist * (1f - alignment); // weighting: behind nodes get boosted

                if (score > bestScore)
                {
                    bestScore = score;
                    bestNode = node;
                }
            }
        }

        // If no "behind" nodes found, fallback to just the furthest
        if (bestNode == null)
        {
            float maxDist = -Mathf.Infinity;
            foreach (var node in allNodes)
            {
                float d = Vector3.Distance(node.transform.position, tagger.position);
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

        if (pathIndex >= path.Count)
        {
            // Reached end of path, pick new target depending on state
            switch (currentState)
            {
                case State.Idle:
                    PickNewIdleTarget();
                    break;
                case State.Run:
                    PickFurthestNodeFromTagger();
                    break;
            }
            return;
        }

        Vector3 targetPos = path[pathIndex].transform.position;
        Vector3 dir = (targetPos - transform.position);
        float dist = dir.magnitude;

        if (dist < nodeReachThreshold)
        {
            pathIndex++;
            return; // wait until next frame to move to next node
        }

        dir.Normalize();
        rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);

        // Rotate toward movement direction
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerRadius);
    }
}
