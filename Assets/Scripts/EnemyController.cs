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
    [SerializeField] int maxHP;
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
    private PlayerController playerController;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
        currentPatrolPoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        bool isMoving = (transform.position != lastPosition);
        animator.SetBool("Walking", isMoving);
        
        //float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 enemyPos = transform.position;
        Vector3 playerPos = player.position;
        enemyPos.y = 0f;
        playerPos.y = 0f;
        float distanceToPlayer = Vector3.Distance(enemyPos, playerPos);

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
        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        agent.isStopped = true;
        GetComponent<Collider>().enabled = false;
        StopAttack();
        animator.SetTrigger("Death");
        scoreManager.instance.AddScore(points);
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    private void StartAttack()
    {
        isAttacking = true;
        animator.SetBool("Attacking", true);
    }

    private void StopAttack()
    {
        isAttacking = false;
        animator.SetBool("Attacking", false);
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
        agent.SetDestination(patrolPoints[currentPatrolPoint].position);
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 3) currentPatrolPoint++;
    }
}
