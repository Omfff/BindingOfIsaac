using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EnemyState
{
    Idle,

    Wander,

    Follow,

    Die,

    Attack
};

public enum EnemyType
{
    Melee,

    Dash,

    Ranged,

    Shooting,

    Laser
};

public class Enemy : MonoBehaviour
{
    protected GameObject player;

    public EnemyState currState = EnemyState.Idle;

    public EnemyType enemyType;

    public float health;

    public float range;

    public float speed;

    public float attackRange;

    public float coolDown;

    private bool chooseDir = false;

    private bool dead = false;

    protected bool coolDownAttack = false;

    private Material matDefault;

    private Material matWhite;

    SpriteRenderer spriteRender;

    public bool notInRoom = false;

    private Vector3 randomDir;

    public GameObject deathEffect;

    public GameObject damageTextPf;

    public PathFinding pathFinding;

    private List<Vector3> path;

    private Vector3 playerPos;

    private Vector3 egoPos;

    private Vector3 wanderDir;

    private int currentPathIndex;

    private int[] dirX = { 0, 0, -1, 1 };
    private int[] dirY = { 1, -1, 0, 0 };

    //public GameObject impactEffect;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Physics2D.queriesStartInColliders = false;
        //switch (enemyType)
        //{
        //    case (EnemyType.Melee):
        //        attackRange = 1;
        //        break;
        //    case (EnemyType.Ranged):
        //        attackRange = 6;
        //        range = 10;
        //        break;
        //    case (EnemyType.Dash):
        //        attackRange = 4;
        //        range = 8;
        //        coolDown = 2;
        //        break;
        //    case EnemyType.Laser:
            
        //        break;
        //}
        spriteRender = GetComponent<SpriteRenderer>();
        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material;
        matDefault = spriteRender.material;
        SetPathFinder();
    }

    void SetPathFinder()
    {
        pathFinding = MapGridController.instance.GetPathFindingUtil();
        //GameObject.FindGameObjectWithTag("MapController").GetComponent<MapGridController>()
        path = null;
        currentPathIndex = 1;
        egoPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case (EnemyState.Idle):
                break;
            case (EnemyState.Wander):
                Wander();
                break;
            case (EnemyState.Follow):
                Follow();
                break;
            case (EnemyState.Die):
                break;
            case (EnemyState.Attack):
                Attack();
                break;
        }

        if (!notInRoom)
        {
            if (pathFinding == null)
            {
                SetPathFinder();
            }
            updatePosInGrid();

            if (IsPlayerInRange(range) && currState != EnemyState.Die)
            {
                currState = EnemyState.Follow;
            }
            else if (!IsPlayerInRange(range) && currState != EnemyState.Die)
            {
                currState = EnemyState.Wander;
            }

            if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
            {
                currState = EnemyState.Attack;
            }

            if (currState != EnemyState.Follow)
            {
                currentPathIndex = 1;
                path = null;
            }
        }
        else
        {
            currState = EnemyState.Idle;
        }
    }

    private void Wander()
    {
        if (!chooseDir)
        {
            StartCoroutine(ChooseDirection());
        }

        transform.position += -wanderDir * speed * Time.deltaTime;
        // add walk animation x speed :Mathf.Abs(wanderDir.x* speed), y speed: Mathf.Abs(wanderDir.y* speed)

        if (IsPlayerInRange(range))
        {
            currState = EnemyState.Follow;
        }
    }

    private void Follow()
    {
        if (!pathFinding.isEqualInGridPos(playerPos, player.transform.position))
        {
            path = pathFinding.FindPath(transform.position, player.transform.position);
            playerPos = player.transform.position;
            currentPathIndex = 1;
        }

        if (path != null && path.Count > 0)
        {
            Debug.DrawLine(new Vector3(path[0].x, path[0].y) * 1f, new Vector3(path[0 + 1].x, path[0 + 1].y) * 1f, Color.red, .2f);
            for (int i = 1; i < path.Count - 1; i++)
            {
                Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 1f, new Vector3(path[i + 1].x, path[i + 1].y) * 1f, Color.green, .2f);
            }

            Vector3 targetPosition = path[currentPathIndex];
            if (currentPathIndex >= path.Count || currentPathIndex <= 0)
                Debug.Log(currentPathIndex.ToString() + "," + path.Count.ToString());
            if (Vector3.Distance(transform.position, targetPosition) > 1f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= path.Count)
                {
                    path = null;
                }
            }
        }

    }

    protected virtual void Death()
    {
        if (pathFinding.GetGrid().GetValue(egoPos) != null)
        {
            pathFinding.GetGrid().GetValue(egoPos).SetIsWalkable(true);
        }

        Instantiate(deathEffect, transform.position, Quaternion.identity);

        GameObject itemSpawner = GameObject.FindGameObjectWithTag("ItemSpawner");
        itemSpawner.GetComponent<ItemSpawner>().dropItemAftherEnemyDeath(transform.position);

        Destroy(gameObject);
        RoomController.instance.StartCoroutine(RoomController.instance.RoomCoroutine());
    }

    protected virtual void Attack()
    {
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }
    private IEnumerator ChooseDirection()
    {
        chooseDir = true;
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        int randomIndex = Random.Range(0, 3);
        wanderDir = new Vector3(dirX[randomIndex], dirY[randomIndex], 0);
        chooseDir = false;
    }

    private void ChangeDirection()
    {
        int randomIndex = Random.Range(0, 3);
        wanderDir = new Vector3(dirX[randomIndex], dirY[randomIndex], 0);
    }

    private void updatePosInGrid()
    {
        PathNode pN = pathFinding.GetGrid().GetValue(egoPos);
        if (pN != null)
        {
            pN.SetIsWalkable(true);
        }
        egoPos = transform.position;
        pN = pathFinding.GetGrid().GetValue(egoPos);
        if (pN != null)
        {
            pN.SetIsWalkable(false);
        }
    }

    protected IEnumerator CoolDown()
    {
        coolDownAttack = true;
        yield return new WaitForSeconds(coolDown);
        coolDownAttack = false;
    }

    public void getHurt(float damage)
    {
        health -= damage;

        GameObject textPopUp = Instantiate(damageTextPf, transform.position, Quaternion.identity);
        DamageTextPopUp damageText = textPopUp.GetComponent<DamageTextPopUp>();
        damageText.Setup((int)damage);

        spriteRender.material = matWhite;
        if (health > 0)
        {
            Invoke("ResetMaterial", 0.1f);
        }
        else
        {
            Death();
        }
    }

    private void ResetMaterial()
    {
        spriteRender.material = matDefault;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (currState == EnemyState.Wander)
        {
            ChangeDirection();
        }
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (enemyType == EnemyType.Dash && isDashing && !isHurtPlayerInDashing)
    //    {
    //        GameController.DamagePlayer(1);
    //        isHurtPlayerInDashing = true;
    //    }
    //}

}
