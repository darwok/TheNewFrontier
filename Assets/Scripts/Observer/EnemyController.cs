using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Build")]
    [SerializeField] private Animator animator;
    [SerializeField] private float hp = 100f;
    [SerializeField] private Transform player;
    [SerializeField] int points;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private Collider attackCollider;
    [SerializeField] private ParticleSystem hitP;
    [SerializeField] private float arrowDamage = 50f;
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] int currentPatrolPoint = 0;
    [SerializeField] private float toPatrol = 20f; // Distance to start patrolling
    private NavMeshAgent agent;
    private Vector3 lastPosition;
    private bool isAttacking = false;
    private bool isDead = false;

    public void Init(Transform playerTransform)
    {
        player = playerTransform;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
        currentPatrolPoint = 0;

        if (player == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
                player = go.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        float distanceToPlayer = GetFlatDistanceToPlayer();
        if (isDead) return;
        bool isMoving = (transform.position != lastPosition);
        animator.SetBool("Walking", isMoving);

        if (distanceToPlayer <= attackRange)
        {
            if (!isAttacking)
            {
                StartAttack();
            }
        }
        else if (distanceToPlayer <= toPatrol)
        {
            StopAttack();
            agent.SetDestination(player.position);
        }
        else
        {
            StopAttack();
            Patrol();
        }
        if (!isAttacking) DisableAttackCollider();
        lastPosition = transform.position;
        if (currentPatrolPoint == patrolPoints.Count) currentPatrolPoint = 0;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Arrow"))
        {
            GetHit(arrowDamage);
        }
    }

    public void GetHit(float damage)
    {
        if (isDead) return;
        hitP.Play();
        hp -= damage;
        if (hp <= 0)
        {
            StartCoroutine(Die());
            var stats = Object.FindFirstObjectByType<PlayerStatsSubject>();
            if (stats != null) stats.AddScore(points);

        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        agent.isStopped = true;
        GetComponent<Collider>().enabled = false;
        StopAttack();
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    private void StartAttack()
    {
        isAttacking = true;
        animator.SetBool("Attacking", true);
        if (agent != null)
        {
            agent.isStopped = true;      // stop moving
            agent.ResetPath();           // clear current destination so it doesn't "slide", cause it was moonwalking towards the player while attacking.
        }
    }

    private void StopAttack()
    {
        isAttacking = false;
        animator.SetBool("Attacking", false);
        if (agent != null && !isDead)
        {
            agent.isStopped = false;     // allow movement again only if still alive
        }
    }

    void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
            return;

        if (currentPatrolPoint >= patrolPoints.Count)
            currentPatrolPoint = 0;

        Transform target = patrolPoints[currentPatrolPoint];
        agent.SetDestination(target.position);

        if (Vector3.Distance(transform.position, target.position) < 3f)
            currentPatrolPoint++;
    }

    private float GetFlatDistanceToPlayer()
    {
        Vector3 enemyPos = transform.position;
        Vector3 playerPos = player.position;
        enemyPos.y = 0f;
        playerPos.y = 0f;
        return Vector3.Distance(enemyPos, playerPos);
    }
}
