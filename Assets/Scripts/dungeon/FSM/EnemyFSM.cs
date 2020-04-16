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
    Vector2 moveDir;
    BoxCollider2D _Colider;
    float tempDelay;
    bool attackAllow;
    public RectTransform damageTextGen;
    public Image hpFrame;
    public Image hpBar;
    public Image attackFrame;
    public Image attackBar;
    public float attackStopTime = 0.2f;
    public GameObject doEffect1;
    public GameObject doEffect2;
    int FSMCol = 0;

    new void Awake()
    {
        base.Awake();
        RBD = GetComponent<Rigidbody2D>();
        DamageReceiver.addEnemy(this);
        _Colider = GetComponent<BoxCollider2D>(); 
        gameObject.SetActive(false);
        attackBar.gameObject.SetActive(false);
        attackFrame.gameObject.SetActive(false);
    }
    void Update()
    {
        RBD.velocity = Vector2.zero;   
    }
    new private void OnEnable()
    {
        base.OnEnable();
        speedRate = 100;
        setState(State.move);
        tempDelay = 0f;
        attackAllow = true;
        FSMCol = 0;
        setHpBar();
        setColider();
    }
    void setColider() {
        switch (myType)
        {
            case type.boss:
                _Colider.size = new Vector2(1.3f, 1.3f);
                _Colider.offset = new Vector2(0, 0);
                break;
            case type.Long:
            case type.Short:
                _Colider.size = new Vector2(0.34f, 0.31f);
                _Colider.offset = new Vector2(0, -0.13f);
                break;
            default: break;
        }
    }
    void setHpBar() {
        switch (myType)
        {
            case type.boss:
                hpFrame.sprite = SLM.instance.getSpr("image/enemy/bossFrame");
                hpBar.sprite = SLM.instance.getSpr("image/enemy/bossHp");
                damageTextGen.anchoredPosition = new Vector2(0,14);
                hpFrame.rectTransform.anchoredPosition = new Vector2(0,13.89f);
                hpBar.rectTransform.anchoredPosition = new Vector2(0,13.89f);
                attackBar.rectTransform.anchoredPosition = new Vector2(0,11.53f);
                attackFrame.rectTransform.anchoredPosition = new Vector2(0,11.53f);
                break;
            case type.Long:
            case type.Short:
                hpFrame.sprite = SLM.instance.getSpr("image/enemy/frame");
                hpBar.sprite = SLM.instance.getSpr("image/enemy/hp");
                damageTextGen.anchoredPosition = new Vector2(0, 4.5f);
                hpFrame.rectTransform.anchoredPosition = new Vector2(0, 4.46f);
                hpBar.rectTransform.anchoredPosition = new Vector2(0, 4.46f);
                attackBar.rectTransform.anchoredPosition = new Vector2(0, 3.66f);
                attackFrame.rectTransform.anchoredPosition = new Vector2(0, 3.66f);
                break;
            default: break;
        }
        hpFrame.SetNativeSize();
        hpBar.SetNativeSize();
        hpBar.fillAmount = 1;
        hpFrame.gameObject.SetActive(false);
        hpBar.gameObject.SetActive(false);
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
        moveDir = (playerFSM.instance.transform.position - transform.position).normalized;
        RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * speedRate / 100 * -1*Time.deltaTime/2);
        

    }

    void lookPlayer()
    {
        moveDir = Vector2.zero;
        moveDir = (playerFSM.instance.transform.position - transform.position).normalized;

        degree = (Mathf.RoundToInt((Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f - 180) * -1) / 45);
        _anim.setDir(degree);
    }

    void moveEnemy()
    {
        if (FSMCol != 0)
        {
            RBD.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }
        RBD.constraints = RigidbodyConstraints2D.FreezeRotation;
        moveDir = Vector2.zero;
        moveDir = (playerFSM.instance.transform.position - transform.position).normalized;

        degree = Mathf.RoundToInt((Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f - 180) * -1) / 45;
        _anim.setDir(degree);

        RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * speedRate / 100 * Time.deltaTime);
    }

    bool detectPlayer() {
        if (!attackAllow)
            return false;
        if (Vector2.Distance(playerFSM.instance.transform.position, transform.position) <= attackRange)
            return true;
        
        return false;
    }
    public void hitted(float damage) {
        RBD.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (hp == maxHp) {
            hpFrame.gameObject.SetActive(true);
            hpBar.gameObject.SetActive(true);

        }
        if (hp <= 0)
        {
            return;
        }
        if(!__hpFix)
        hp -= damage;
        hpBar.fillAmount = hp / maxHp;
        if (hp <= 0)
        {
            hp = 0;
            _anim.speed = 0.5f;
            setState(State.dead);
            itemGen();
        }
        else if(myType!=type.boss){
            _anim.speed = 0.1f;
            setState(State.hited);
        }
    }
    void itemGen() {
            itemManager.instance.itemGenerate(transform.position,itemType.gold);
        itemType t = (itemType)System.Enum.Parse(typeof(itemType), name + "Mat");
        //TODO: 랜덤으로 나오게

        if (myType == type.boss) { 
            
            itemManager.instance.itemGenerate(damageTextGen.position,t);
        }
        itemManager.instance.itemGenerate(transform.position,t);

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
        
        bool doneAttack = false;
        do
        {
            _anim.setSpeed(1);
            lookPlayer();
            
            if (_anim.animNum==6 && !doneAttack)
            {
                _anim.Pause(false);
                attackBar.gameObject.SetActive(true);
                attackFrame.gameObject.SetActive(true);
                attackBar.fillAmount = 0;
                doEffect1.SendMessage("doEffect", SendMessageOptions.DontRequireReceiver);
                doEffect2.SendMessage("doEffect", SendMessageOptions.DontRequireReceiver);
                float tempTime = 0f;
                do {
                    attackBar.fillAmount = tempTime/ attackStopTime;
                    yield return new WaitForSeconds(attackStopTime/10);
                    tempTime += attackStopTime / 10f;
                    if (tempTime >= attackStopTime)
                        break;
                } while(true);
                _anim.reOn();
                if (myType != type.Short)
                    spawnBullet();
                else if(detectPlayer() && objectState == State.attack)
                    DamageReceiver.playerHit(attackPoint);
                doneAttack = true;
            }
            if (_anim.isEnd(1))
            {
                setState(State.move);
                attackAllow = false;
                tempDelay = 0f;
            }
            yield return null;
        } while (!newState);
        attackFrame.gameObject.SetActive(false);
        attackBar.gameObject.SetActive(false);
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
        RBD.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    IEnumerator dead()
    {
        
        do
        {
            if (_anim.isEnd())
            {
                gameObject.SetActive(false);
                _anim.speed = 1f;

                setState(State.move);
                LevelManager.instance.checkEnemy();
                break;
            }
            yield return null;
        } while (!newState);
        
    }
    private void OnDrawGizmos()
    {
        Handles.color = new Color(255, 0, 0, 0.2f);
        Handles.DrawSolidArc(transform.position, new Vector3(0, 0, 1), moveDir, 90 / 2, attackRange);
        Handles.DrawSolidArc(transform.position, new Vector3(0, 0, 1), moveDir, -90 / 2, attackRange);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8) {
            FSMCol++;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8) {
            FSMCol--;
            if (FSMCol < 0)
                FSMCol = 0;
        }
    }
    public BoxCollider2D getCol() {
        return _Colider;
    }
}
