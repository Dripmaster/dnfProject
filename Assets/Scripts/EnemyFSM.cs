using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyFSM : FSMbase
{
    
    float speedRate;
    int degree;
    Rigidbody2D RBD;
    Transform player;
    Vector2 moveDir;
    BoxCollider2D _Colider;
    GameObject bulletPrefab;
    float tempDelay;
    bool attackAllow;

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
        tempDelay = 0f;
        attackAllow = true;
    }

    void Update()
    {
        
    }

    void delayCount() {
        tempDelay += Time.deltaTime;
        if (tempDelay >= attackDelay) {
            attackAllow = true;
        }
    }

    void knockBack() {
        moveDir = Vector2.zero;
        moveDir = (player.position - transform.position).normalized;
        RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * speedRate / 100 * -1*Time.deltaTime);
    }

    void lookPlayer()
    {
        moveDir = Vector2.zero;
        moveDir = (player.position - transform.position).normalized;

        degree = Mathf.RoundToInt((Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f - 180) * -1) / 45;
        _anim.setDir(degree);
    }

    void moveEnemy() {
        if (Vector2.Distance(player.position, transform.position) <= attackRange)
            return;

        moveDir = Vector2.zero;
        moveDir = (player.position - transform.position).normalized;

        degree = Mathf.RoundToInt((Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f - 180) * -1) / 45;
        _anim.setDir(degree);

        RBD.MovePosition((Vector2)transform.position+moveDir * moveSpeed * speedRate / 100*Time.deltaTime);
    }

    bool detectPlayer() {
        if (!attackAllow)
            return false;
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
            delayCount();
            if (detectPlayer()) {
                setState(State.attack);
            }
        } while (!newState);
    }

    void spawnBullet() {
        if (myType == type.Long)
        {
            bulletEffect b = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<bulletEffect>();
            b.transform.Rotate(new Vector3(0, 0, (Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f)));
            b.setAnim(name, attackPoint, myType == type.boss);
        }
        else {
            for (int i = -2; i < 3; i++) {
                bulletEffect b = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<bulletEffect>();
                b.transform.Rotate(new Vector3(0, 0, 25*i+(Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f)));
                b.setAnim(name, attackPoint, myType == type.boss);
            }
        }
        
    }

    IEnumerator attack()
    {
        bool _animEnd = false;
        RBD.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        bool doneAttack = false;
        do
        {
            yield return null;
            lookPlayer();
            if (_anim.isEnd(1) && !doneAttack)
            {
                if (myType != type.Short)
                    spawnBullet();
                else if(detectPlayer() && objectState == State.attack)
                    DamageReceiver.playerHit(attackPoint);
                _animEnd = true;
                doneAttack = true;
            }
            if (_animEnd)
            {
                setState(State.move);
                attackAllow = false;
                tempDelay = 0f;
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
    private void OnDrawGizmos()
    {
        Handles.color = new Color(0, 0, 255, 0.2f);
        Handles.DrawSolidArc(transform.position, new Vector3(0, 0, 1), moveDir, 90 / 2, attackRange);
        Handles.DrawSolidArc(transform.position, new Vector3(0, 0, 1), moveDir, -90 / 2, attackRange);
    }
}
