using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundLayer, playerLayer;

    public Vector3 patrolPoint;
    bool patrolPointSet;
    public float patrolRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public bool isDie = false;
    public float reSpawnTime = 15f;
    public float DieTime;
    public bool needReSpawn = false;

    public Animator animator;
    public float maxHP = 100f;
    public float currentHP = 100f;
    private float reduceHPSpeed = 2f;
    public Image healthBarImage;
    private float _targetHP = 1;
    private Camera _cam;
    public Transform infoUI;
    public float attackValue;
    public float defenceValue;
    public int expValue;
    public string EnemyName = "";
    public int EnemyLevel = 1;
    public int EnemyID = 0;
    public ItemDrop itemDrop;
    public int coinValue = 0;
    public bool isBoss = false;

    public GameObject ValueChange3DDisplayObj;


    private ThirdPersonController playerController;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _cam = Camera.main;
        currentHP = maxHP;
        playerController = player.GetComponent<ThirdPersonController>();
        itemDrop = GetComponent<ItemDrop>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();

        if (isDie)
        {

            if (needReSpawn)
            {
                isDie = false;
                transform.gameObject.SetActive(true);
                agent.enabled = true;
                animator.SetBool("isDie", false);
                currentHP = maxHP;
                infoUI.gameObject.SetActive(true);
            }
            return;
        }

        infoUI.rotation = Quaternion.LookRotation(infoUI.position - _cam.transform.position);
        healthBarImage.fillAmount = Mathf.MoveTowards(healthBarImage.fillAmount, _targetHP, reduceHPSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            animator.SetFloat("Speed", 0);
        }
        else
        {
            animator.SetFloat("Speed", Mathf.Abs(agent.speed));
        }
        

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange && playerController.isDie) Patrol();
        if(playerInSightRange && !playerInAttackRange && !playerController.isDie)
        {
            ChasePlayer();
        }
        if (playerInAttackRange && !playerController.isDie) AttackPlayer();
    }

    public void UpdateHealthBar()
    {
        _targetHP = currentHP / maxHP;
    }

    private void AttackPlayer()
    {
        if (MapsDataSingleton.Instance.LocationAreaName == "Space Ship")
            return;

        agent.SetDestination(transform.position);
        //animator.SetBool("isAttack", true);
        animator.SetFloat("Speed", 0);

        if (!alreadyAttacked)
        {
            animator.Play("Attack");
            alreadyAttacked = true;
            if(player)
            {
                playerController.ReceieveDamage(gameObject,attackValue);
            }
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        //Debug.Log($"[{System.DateTime.Now}] {gameObject.name} is attacking {player.name}");
        //animator.SetBool("isAttack", false);

        alreadyAttacked = false;
    }

    public void ReceieveAttack(float value)
    {
        currentHP = currentHP - value;

        playerController.ShowLocationChangeUIAnim(((int)(-1 * value)).ToString(), Color.white, Camera.main.WorldToScreenPoint(transform.position));
        //if (ValueChange3DDisplayObj != null)
        //{
        //    Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText((int)(-1 * value), false, Color.red);
        //}

        if(currentHP <= 0)
        {
            if (isBoss) PlayerData.Instance.isGameWin = true;
            itemDrop.DropItem();
            PlayerData.Instance.totalCoin += coinValue;
            isDie = true;
            animator.SetBool("isDie", true);
            needReSpawn = false;
            playerController._attackTarget = null;
            DieTime = 0; infoUI.gameObject.SetActive(false);
            agent.enabled = false;
            Invoke(nameof(DieAction), 2f);
            playerController.ReceieveExp(expValue);
        }
    }

    private void DieAction()
    {
        transform.gameObject.SetActive(false);
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void Patrol()
    {
        if (!patrolPointSet) SearchPatrolPoint();
        if (patrolPointSet)
        {
            agent.SetDestination(patrolPoint);
        }

        Vector3 distanceToPatrolPoint = transform.position - patrolPoint;

        if (distanceToPatrolPoint.magnitude < 0.08f) patrolPointSet = false;

    }

    private void SearchPatrolPoint()
    {
        float randomZ = Random.Range(-patrolRange, patrolRange);
        float randomX = Random.Range(-patrolRange, patrolRange);

        patrolPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(patrolPoint, -transform.up, 2f, groundLayer))
            patrolPointSet = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
