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
    bool animEnd;
    Transform player;
    Vector2 moveDir;
    void Awake()
    {
        base.Awake();
        speedRate = 100;
        setState(State.move);
        player = GameObject.Find("player").transform;
        RBD = GetComponent<Rigidbody2D>();
        
    }
    void Update()
    {
        
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

        RBD.velocity = moveDir * moveSpeed * speedRate / 100;
    }
    bool detectPlayer() {
        if (Vector2.Distance(player.position, transform.position) <= attackRange)
            return true;

        return false;
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

    IEnumerator attack()
    {
        RBD.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        do
        {
            yield return null;
            lookPlayer();
            if (!detectPlayer())
            {
                setState(State.move);
            }

        } while (!newState);
        RBD.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
