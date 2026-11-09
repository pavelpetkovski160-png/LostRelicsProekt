using UnityEngine;

// Finite state machine for enemy ships:
//   Patrol, Chase, Attack, Collect (go for artifacts) and Flee (low health).
// A coloured dot on the ship shows the current state.
[RequireComponent(typeof(Ship))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack, Collect, Flee }

    [Header("Ranges")]
    public float detectRange = 10f;
    public float attackRange = 6f;
    public float aimTolerance = 15f;   // degrees
    public float patrolRadius = 8f;
    public float collectRange = 20f;   // how far away it notices artifacts

    [Header("Keep distance (no ramming)")]
    public float keepDistance = 5f;       // preferred distance while attacking
    public float tooCloseDistance = 3.5f; // closer than this: back away

    [Header("Flee")]
    [Range(0f, 1f)] public float fleeHealthFraction = 0.4f; // flee at 40% health or less
    public float fleeBuffer = 4f;      // extra distance before it calms down again

    [Header("State indicator colours")]
    public Color patrolColor = Color.green;
    public Color chaseColor = Color.yellow;
    public Color attackColor = Color.red;
    public Color collectColor = new Color(0.75f, 0.45f, 1f);
    public Color fleeColor = new Color(0.3f, 0.7f, 1f);

    public State CurrentState => state;

    Ship ship;
    Health health;
    ArtifactCarrier carrier;
    Transform player;
    Artifact targetArtifact;
    State state = State.Patrol;
    Vector2 patrolTarget;
    float throttle, turn;
    SpriteRenderer indicator;

    void Awake()
    {
        ship = GetComponent<Ship>();
        health = GetComponent<Health>();
        carrier = GetComponent<ArtifactCarrier>();
        CreateIndicator();
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
        PickPatrolPoint();
        ApplyIndicatorColor();
    }

    void Update()
    {
        float dist = player ? Vector2.Distance(transform.position, player.position) : float.MaxValue;
        targetArtifact = carrier != null ? FindNearestArtifact() : null;

        // 1) Decide which state we should be in (the transitions)
        SetState(DecideState(dist));

        // 2) Do what the current state says
        switch (state)
        {
            case State.Patrol:
                if (Vector2.Distance(transform.position, patrolTarget) < 1f) PickPatrolPoint();
                SteerTo(patrolTarget, 0.5f);
                break;

            case State.Chase:
                SteerTo(player.position, 1f);
                break;

            case State.Attack:
                float angle = SteerTo(player.position, 0f);   // turn to face the player
                // Hold a safe distance instead of ramming: back away if too close,
                // creep closer if too far, otherwise stay put and shoot.
                if (dist < tooCloseDistance) throttle = -0.5f;
                else if (dist > keepDistance) throttle = 0.3f * Mathf.Clamp01(1f - Mathf.Abs(angle) / 120f);
                else throttle = 0f;
                if (Mathf.Abs(angle) < aimTolerance) ship.Fire();
                break;

            case State.Collect:
                SteerTo(targetArtifact.transform.position, 0.8f);
                break;

            case State.Flee:
                Vector2 away = (Vector2)transform.position +
                               ((Vector2)transform.position - (Vector2)player.position).normalized * 10f;
                SteerTo(away, 1f);
                break;
        }
    }

    void FixedUpdate() => ship.Drive(throttle, turn);

    // The transition rules, in priority order: Flee > Attack > Chase > Collect > Patrol.
    State DecideState(float dist)
    {
        bool lowHealth = health != null && health.Current <= health.maxHealth * fleeHealthFraction;
        float fleeRange = state == State.Flee ? detectRange + fleeBuffer : detectRange;

        if (lowHealth && dist <= fleeRange) return State.Flee;
        if (dist <= attackRange) return State.Attack;
        if (dist <= detectRange) return State.Chase;
        if (targetArtifact != null) return State.Collect;
        return State.Patrol;
    }

    Artifact FindNearestArtifact()
    {
        Artifact best = null;
        float bestDist = collectRange;
        foreach (var a in Artifact.All)
        {
            if (a == null) continue;
            float d = Vector2.Distance(transform.position, a.transform.position);
            if (d < bestDist) { bestDist = d; best = a; }
        }
        return best;
    }

    void SetState(State next)
    {
        if (next == state) return;
        state = next;
        ApplyIndicatorColor();
    }

    // Returns signed angle to target (degrees); sets turn/throttle.
    float SteerTo(Vector2 target, float speedFactor)
    {
        Vector2 dir = target - (Vector2)transform.position;
        float angle = Vector2.SignedAngle(transform.up, dir);
        turn = Mathf.Clamp(angle / 30f, -1f, 1f);
        throttle = speedFactor * Mathf.Clamp01(1f - Mathf.Abs(angle) / 120f);
        return angle;
    }

    void PickPatrolPoint() =>
        patrolTarget = (Vector2)transform.position + Random.insideUnitCircle * patrolRadius;

    // ---- State indicator (a small coloured dot on the deck) ----

    void CreateIndicator()
    {
        var go = new GameObject("StateIndicator");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        go.transform.localScale = Vector3.one * 0.5f;
        indicator = go.AddComponent<SpriteRenderer>();
        indicator.sprite = DotSprite.Get();
        indicator.sortingOrder = 10;
    }

    void ApplyIndicatorColor()
    {
        if (indicator == null) return;
        switch (state)
        {
            case State.Patrol:  indicator.color = patrolColor;  break;
            case State.Chase:   indicator.color = chaseColor;   break;
            case State.Attack:  indicator.color = attackColor;  break;
            case State.Collect: indicator.color = collectColor; break;
            case State.Flee:    indicator.color = fleeColor;    break;
        }
    }
}
