using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
public class EnemyFSM : FSMbase
{
    float speedRate;
    int degree;
    Rigidbody2D RBD;
    Transform player;
    Vector2 moveDir;
    BoxCollider2D _Colider;
    float tempDelay;
    bool attackAllow;
    RectTransform damageTextGen;
    Image hpBar;

    void Awake()
    {
        base.Awake();
        player = GameObject.Find("player").transform;
        RBD = GetComponent<Rigidbody2D>();
        DamageReceiver.addEnemy(this);
        _Colider = GetComponent<BoxCollider2D>();
        damageTextGen = transform.Find("enemyCanvas/TextGen").GetComponent<RectTransform>();
    }
    void Update()
    {
        RBD.velocity = Vector2.zero;   
    }
    private void OnEnable()
    {
        base.OnEnable();
        speedRate = 100;
        setState(State.move);
        tempDelay = 0f;
        attackAllow = true;
        setHpBar();
    }
    void setHpBar() {
        switch (myType)
        {
            case type.boss: transform.Find("enemyCanvas/hpFrame").GetComponent<Image>().sprite = SLM.getSpr("image/enemy/bossFrame");
                hpBar = transform.Find("enemyCanvas/hpValue").GetComponent<Image>();
                hpBar.sprite = SLM.getSpr("image/enemy/bossHp");
                break;
            case type.Long:
            case type.Short:
                transform.Find("enemyCanvas/hpFrame").GetComponent<Image>().sprite = SLM.getSpr("image/enemy/frame");
                hpBar = transform.Find("enemyCanvas/hpValue").GetComponent<Image>();
                hpBar.sprite = SLM.getSpr("image/enemy/hp");
                break;
            default: break;
        }
        transform.Find("enemyCanvas/hpFrame").GetComponent<Image>().SetNativeSize();
        hpBar.SetNativeSize();
        hpBar.fillAmount = 1;
    }
    public bool isDead() {
        if (hp <= 0)
            return true;
        return false;
    }
    void delayCount() {
        tempDelay += Time.deltaTime;
        if (tempDelay >= attackDelay) {
            attackAllow = true;
        }
    }
    public RectTransform getDamageTextGen() {
        return damageTextGen;
    }

    void knockBack() {
        moveDir = Vector2.zero;
        moveDir = (player.position - transform.position).normalized;
        RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * speedRate / 100 * -1*Time.deltaTime/2);
    }

    void lookPlayer()
    {
        moveDir = Vector2.zero;
        moveDir = (player.position - transform.position).normalized;

        degree = (Mathf.RoundToInt((Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f - 180) * -1) / 45 +1);
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
        hpBar.fillAmount = hp / maxHp;
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
            _anim.setSpeed(1);
            moveEnemy();
            delayCount();
            if (detectPlayer()) {
                setState(State.attack);
            }
            yield return null;
        } while (!newState);
    }

    void spawnBullet() {
        if (myType == type.Long)
        {
            bulletEffect b = EffectManager.getbullet(transform.position);
            b.transform.Rotate(new Vector3(0, 0, (Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f)));
            b.setAnim(name, attackPoint, myType == type.boss);
            b.gameObject.SetActive(true);
        }
        else {
            bulletEffect b;
            for (int i = -2; i < 3; i++)
            {
                b = EffectManager.getbullet(transform.position);
                b.transform.Rotate(new Vector3(0, 0, 25 * i + (Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f)));
                b.setAnim(name, attackPoint, myType == type.boss);
                b.gameObject.SetActive(true);
            }
        }
        
    }

    IEnumerator attack()
    {
        bool _animEnd = false;
        
        bool doneAttack = false;
        do
        {
            _anim.setSpeed(1);
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
            yield return null;
        } while (!newState);

    }
    IEnumerator hited()
    {
        do
        {
            _anim.setSpeed(0.1f);
            knockBack();
            if (_anim.isEnd(-1))
            {
                setState(State.move);
            }
            yield return null;
        } while (!newState);
    }
    IEnumerator dead()
    {
        
        do
        {
            if (_anim.isEnd())
            {
                gameObject.SetActive(false);
                _anim.speed = 1f;
                break;
            }
            yield return null;
        } while (!newState);
        
    }
    private void OnDrawGizmos()
    {
        Handles.color = new Color(0, 0, 255, 0.2f);
        Handles.DrawSolidArc(transform.position, new Vector3(0, 0, 1), moveDir, 90 / 2, attackRange);
        Handles.DrawSolidArc(transform.position, new Vector3(0, 0, 1), moveDir, -90 / 2, attackRange);
    }
    public BoxCollider2D getCol() {
        return _Colider;
    }
}
