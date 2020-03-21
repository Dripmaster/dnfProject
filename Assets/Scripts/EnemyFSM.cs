using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : FSMbase
{
    public float attackRange = 1.1f;
    public float moveSpeed = 5;
    float speedRate;
    int degree;
    Rigidbody2D RBD;
    Transform player;
    Vector2 moveDir;
    BoxCollider2D _Colider;
    GameObject bulletPrefab;

    void Awake()
    {
        base.Awake();
        speedRate = 100;
        setState(State.move);
        player = GameObject.Find("player").transform;
        bulletPrefab = Resources.Load<GameObject>("prefabs/bullet");
        RBD = GetComponent<Rigidbody2D>();
        DamageReceiver.addEnemy(this);
        _Colider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        
    }
    void knockBack() {
        moveDir = Vector2.zero;
        moveDir = (player.position - transform.position).normalized;
        RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * speedRate / 100 * -1*Time.deltaTime);
    }
    void lookPlayer() {
        moveDir = Vector2.zero;
        moveDir = (player.position - transform.position).normalized;

        degree = Mathf.RoundToInt((Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f - 180) * -1) / 45;
        _anim.setDir(degree);
    }
    void moveEnemy() {
        moveDir = Vector2.zero;
        moveDir = (player.position - transform.position).normalized;

        degree = Mathf.RoundToInt((Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f - 180) * -1) / 45;
        _anim.setDir(degree);

        RBD.MovePosition((Vector2)transform.position+moveDir * moveSpeed * speedRate / 100*Time.deltaTime);
    }
    bool detectPlayer() {
        if (Vector2.Distance(player.position, transform.position) <= attackRange)
            return true;

        return false;
    }
    public void hitted(float damage) {
        if (hp <= 0)
        {
            return;
        }
        
        hp -= damage;
        if (hp <= 0)
        {
            _anim.speed = 0.5f;
            setState(State.dead);
            
        }
        else if(myType!=type.boss){
            _anim.speed = 0.1f;
            setState(State.hited);
        }

    }
    IEnumerator move()
    {
        do
        {
            yield return null;
            moveEnemy();
            if (detectPlayer()) {
                setState(State.attack);
            }
        } while (!newState);
    }
    void spawnBullet() {
        //bulletEffect b = Instantiate(bulletPrefab, transform);

    }

    IEnumerator attack()
    {
        bool _animEnd = false;
        RBD.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        bool doneAttack = false;
        lookPlayer();
        do
        {
            yield return null;
            if (_anim.isEnd(1) && !doneAttack)
            {
                if(detectPlayer() && objectState == State.attack)
                DamageReceiver.playerHit(attackPoint);
                _animEnd = true;
                doneAttack = true;
            }
            if (_animEnd)
                if (!detectPlayer())
                {
                    setState(State.move);
                }
                else {
                    lookPlayer();
                    _animEnd = false;
                    doneAttack = false;
                }

        } while (!newState);
        RBD.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    IEnumerator hited()
    {
        _Colider.isTrigger = true;
        
        do
        {
            yield return null;
            knockBack();
            
            if (_anim.isEnd(-1))
            {
                _anim.speed = 1;
                setState(State.move);
            }            
        } while (!newState);
        _Colider.isTrigger = false;

    }
    IEnumerator dead()
    {
        _Colider.isTrigger = true;

        do
        {
            yield return null;
            if (_anim.isEnd())
            {
                gameObject.SetActive(false);
                _anim.speed = 1f;
                break;
            }
        } while (!newState);
        _Colider.isTrigger = false;
    }
}
