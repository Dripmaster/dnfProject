using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using System;

public class playerFSM : FSMbase
{
    public static playerFSM instance;
    
    public Vector2 attackfan;//에디터전용
    float speedRate;
    int atkNum;
    int degree;
    Rigidbody2D RBD;
    bool animEnd;
    float checkDashTime = 0.3f;//대쉬 가능 시간
    public float dashRate = 500;
    sceneEffect sEffect;
    //int canDash = 0;//dash페이즈 0=기본, 1=처음 눌림, 2=뗌, 3= 다시 눌림(대쉬시작)
    bool dashState = false;
    KeyCode downKey;
    Vector2 dashDir;
    public GameObject[] dashEffects;
    int movecount = 0;
    BoxCollider2D _Colider;
    public GameObject myAlert;
    public myParticle myparticle;
    GameObject darkScreen;
    SpriteRenderer confuseAni;
    Image hpBar;
    Image myHeart;
    bool isFreeze = false;
    bool canDash;
    bool isHitted = false;

    bool isKnockBack = false;
    bool confuseKey;
    bool isEyeDebuff = false;

    // Use this for initialization
    new void Awake()
    {
        base.Awake();
        instance = this;
        foreach (GameObject g in dashEffects)
            g.SetActive(false);

        hpBar = GameObject.Find("hpBar_fg").GetComponent<Image>();
        myHeart = GameObject.Find("Heart").GetComponent<Image>();
        dashDir = new Vector2(0, 0);
        speedRate = 100;
        atkNum = 0;
        RBD = GetComponent<Rigidbody2D>();
        _Colider = GetComponent<BoxCollider2D>();
        attackfan = new Vector2(0, -1);
        DamageReceiver.addPlayer(this);
        if(myAlert ==null)
            myAlert = GameObject.Find("noHp");
        if (darkScreen == null)
            darkScreen = GameObject.Find("darkScreen");
        if (confuseAni == null)
            confuseAni = GameObject.Find("confuseAni").GetComponent<SpriteRenderer>(); ;
        setAlert(false);
        sEffect = Camera.main.GetComponent<sceneEffect>();
    }
    new private void OnEnable()
    {
        base.OnEnable();
        canDash = true;
        isHitted = false;
        confuseKey = false;
        isKnockBack = false;
        isFreeze = false;
        isEyeDebuff = false;
        if(darkScreen != null)
        darkScreen.SetActive(isEyeDebuff);
        myparticle.Stop();
        myparticle.setSr(GetComponent<SpriteRenderer>());
        init_Stat();
        for (int i = 1; i < 4; i++)
        {
            _anim.initAnims("attack/" + i);
        }
        setState(State.idle);
        if(hpBar!=null)
        StartCoroutine(lerpHPbar());
        if(myHeart!=null)
        StartCoroutine(transHeart());
        if (confuseAni != null)
            confuseAni.enabled = (false);
    }
    IEnumerator transHeart()
    {
        Color hColor = Color.white;
        float alpha = 1;
        float alphaDir = 1;
        do
        {
            float amount = hp / maxHp;
            alpha -= Time.deltaTime * alphaDir;
            if ((alphaDir==1)&&(amount - 0.1f > alpha)) {
                alphaDir *= -1;
            }
            if ((alphaDir == -1) && (amount < alpha))
            {
                alphaDir *= -1;
            }
            hColor.a = alpha;
            myHeart.color = hColor;
            yield return null;
        } while (hp > 0);
        myHeart.color= hColor;
    }
    IEnumerator lerpHPbar()
    {
        do
        {
            if (hp != maxHp)
            {
                do
                {
                    hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, hp / maxHp, Time.deltaTime * 5);
                    yield return null;
                } while (hpBar.fillAmount - (hp / maxHp) >= 0.01f);
                hpBar.fillAmount = hp / maxHp;
            }
            else {
                yield return null;
            }
        } while (hp>0);
                hpBar.fillAmount = 0;
        
    }
    void dashEffect(int i) {
        dashEffects[i].SetActive(true);
        
        dashEffects[i].transform.position = new Vector2(transform.position.x,transform.position.y-0.457f);
    }
    void dashCount()
    {
        
    }
    public void playerFreeze(bool condition = true) {
        if (condition)
        {
            isFreeze = true;
        }
        else {
            isFreeze = false;
        }
    
    }
    void setAlert(bool value) {
        if (myAlert!= null)
            myAlert.SetActive(value);
    }
    bool dashPlayer() {//dash페이즈 체크
        if (dashState)
            return false;
        if (canDash == false)
            return false;
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            dashDir.x = 0;
            dashDir.y = 0;
            movecount = 0;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movecount++;
                dashDir.x += -1;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                dashDir.x += 1;
                movecount++;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                dashDir.y += 1;
                movecount++;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                dashDir.y += -1;
                movecount++;
            }
            return true;
        }
        return false;
    }
    public void playerKnockBack(Vector2 moveDir) {
        setState(State.idle);
        isKnockBack = true;
        StartCoroutine(knockBack(moveDir));
    }
    IEnumerator knockBack(Vector2 moveDir) {

        Vector2 tempPos = transform.position;
        float tempTime = 0f;

        Physics2D.IgnoreLayerCollision(8, 9);
        RBD.constraints = RigidbodyConstraints2D.FreezeRotation;
        do {
            RBD.MovePosition(Vector2.Lerp(transform.position,moveDir*5+(Vector2)tempPos,Time.deltaTime*5));
            tempTime += Time.deltaTime;
            yield return null;
        } while ((Vector2.Distance(tempPos,transform.position)<=5f )&& tempTime<=1f);
        RBD.constraints = RigidbodyConstraints2D.FreezeAll;
        isKnockBack = false;
        Physics2D.IgnoreLayerCollision(8, 9,false);
    }
    bool movePlayer()
    {
        if (isFreeze)
            return false;
        Vector2 moveDir = new Vector2(0, 0);
        if (!dashState)
            movecount = 0;
        int t = movecount;
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir.x += -1;
            movecount++;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movecount++;
            moveDir.x += 1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movecount++;
            moveDir.y += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movecount++;
            moveDir.y += -1;
        }

        if ((moveDir != Vector2.zero || dashState))
        {
            RBD.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (dashState)
                moveDir = dashDir;

            if (confuseKey)
                moveDir *= -1;
            degree = Mathf.RoundToInt((Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f - 180) * -1) / 45;
            _anim.setDir(degree);
            attackfan = moveDir;

            if (objectState == State.attack) {
                moveDir *= 0.1f;
            }
            if (!dashState)
            {
                // RBD.velocity = moveDir * moveSpeed * speedRate / 100;
                //transform.Translate(moveDir * moveSpeed* speedRate / 100 * Time.deltaTime);
                if (movecount >= 2)
                {
                    RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * speedRate / 100 / Mathf.Sqrt(2) * Time.deltaTime);
                }
                else
                    RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * speedRate / 100 * Time.deltaTime);
            }
            else
            {
                movecount = t;
                if (t >= 2)
                {
                    RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * dashRate / 100 / Mathf.Sqrt(2) * Time.deltaTime);
                }
                else
                    RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * dashRate / 100 * Time.deltaTime);
            }
            //RBD.velocity = dashDir * moveSpeed * dashRate / 100;
            return true;
        }
        else
        {
            if (!isKnockBack)
            {
                RBD.velocity = Vector2.zero;
                RBD.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
        return false;
    }
    bool attackInput()
    {
        if (dashState || isFreeze)
            return false;
        if (Input.GetKey(KeyCode.Z))
        {
            RBD.velocity = Vector2.zero;
            return true;
        }
        if (Input.GetKey(KeyCode.Tab))
        {
            RBD.velocity = Vector2.zero;
            DamageReceiver.playerAttack(500000,true);
            return true;
        }
        atkNum = 1;
        return false;
    }
    public void hitted(float damage) {
        if (isHitted) {
            return;
        }
        if (hp <= 0)
            return;
        if (!__hpFix)
            hp -= damage;
        setAlert(true);
        if (hp <= 0)
        {
            setState(State.dead);
        }
        else {
            StartCoroutine(hit());
        }
    }
    IEnumerator idle()
    {
        _anim.setSpeed(1);
        do
        {
            yield return null;
            if (dashPlayer())
            {
                StartCoroutine(dashTimer());
                continue;
            }
            if (attackInput())
            {
                _anim.setSpeed(attackSpeed);
                setState(State.attack,atkNum);
            }
            else
            {
                if (movePlayer())
                    setState(State.move);
            }

        } while (!newState);
    }
    IEnumerator canDashTimer() {
        yield return new WaitForSeconds(checkDashTime);
        canDash = true;
    }
    IEnumerator dashTimer() {
        dashState = true;
        canDash = false;
        myparticle.Play();
        Physics2D.IgnoreLayerCollision(8,9);
        Physics2D.IgnoreLayerCollision(8,10);
        Vector2 temppos = transform.position;
        for (int i= 0; i < 5; i++) { 
                dashEffect(i);
            if (Vector2.Distance(temppos, transform.position) >= 3.5f)
                break;
            yield return new WaitForSeconds(0.02f);
        }
        dashDir = Vector2.zero;
        myparticle.Stop();
        Physics2D.IgnoreLayerCollision(8, 9,false);
        Physics2D.IgnoreLayerCollision(8, 10,false);
        dashState = false;
        StartCoroutine(canDashTimer());
    }
    IEnumerator dead()
    {

        RBD.constraints = RigidbodyConstraints2D.FreezeAll;
        do
        {
            yield return null;
            if (_anim.isEnd())
            {
                _anim.Pause();
                _anim.setSpeed(1f);
                break;
            }
        } while (!newState);
    }
    IEnumerator hit()
    {
        isHitted = true;
        _anim.setColor(new Color(0.5f, 0.5f, 0.5f, 1));
        yield return new WaitForSeconds(0.1f);
        _anim.setColor(new Color(1, 1, 1, 1));
        isHitted = false;
    }
    public void confuseStart() {
        confuseKey = true;
        StartCoroutine(confuse());
    }
    IEnumerator confuse() {
        confuseAni.enabled = true;
        yield return new WaitForSeconds(3f);
        confuseAni.enabled = false;
        confuseKey = false;
    }
    IEnumerator move()
    {
        _anim.setSpeed(0.5f);
        do
        {
            yield return null;
            if (dashPlayer())
            {
                StartCoroutine(dashTimer());
                continue;
            }
            if (!dashState&&attackInput())
            {
                _anim.setSpeed(attackSpeed);
                setState(State.attack,atkNum);
            }
            else
            {
                if (!movePlayer())
                {
                    _anim.setSpeed(1f);
                    setState(State.idle);
                }
            }
        } while (!newState);
    }
    void shakeCamera() {
        if (sEffect != null) {
            sEffect.shakeStart();
        }
    }
    public void startDarkSide()
    {
        StartCoroutine(darkSide());
    }
    public void cutEye() { 
        
        StartCoroutine(eyeSizeDown());
    }
    IEnumerator eyeSizeDown() {
        darkScreen.SetActive(true);
        isEyeDebuff = true;
        yield return new WaitForSeconds(3);
        isEyeDebuff = false;
        darkScreen.SetActive (false);
    }

    IEnumerator darkSide()
    {
        Color c = sr.color;
        do
        {
            c.a -= Time.deltaTime;
            sr.color = c;
            yield return null;
        } while (c.a >= 0.3f);
        yield return new WaitForSeconds(1f);
        do
        {
            c.a += Time.deltaTime;
            sr.color = c;
            yield return null;
        } while (c.a <= 0.95f);
        c.a = 1;
        sr.color = c;
    }
    IEnumerator attack()
    {
        
        bool secondAtk = false;//대검 3번째 공격 두번휘둘
        bool doneAttack = false;//한번만 공격
        ////////////이펙트 생성 방향 (안맞는거는 수정예정)
        float rot;
        if (name != "bigsword")
            rot = -90;
        else
            rot = 0;

        if (atkNum == 3 && name == "bigsword")
        {
            if (DamageReceiver.playerAttack(attackPoint))
            {
                shakeCamera();
            }
            EffectScript es = EffectManager.instance.getEffect(transform.position);
            es.transform.rotation = Quaternion.Euler(0, 0, -degree * 45+rot);
            es.initAni("effect/playerAttack/" + name + "/1",attackSpeed/2);
            es.gameObject.SetActive(true);

            secondAtk = true;
        }
        else
        {
            EffectScript es = EffectManager.instance.getEffect(transform.position);
            es.transform.rotation = Quaternion.Euler(0, 0, -degree * 45+rot);
            es.initAni("effect/playerAttack/" + name + "/" + atkNum , attackSpeed);
            es.gameObject.SetActive(true);
        }
        ///////////////

        do
        {
            yield return null;
            animEnd =_anim.isEnd();
            
            if (dashPlayer())
            {//공격하다 대쉬하면 공격 캔슬
                StartCoroutine(dashTimer());
                setState(State.move);
                break;
            }
            movePlayer(); //공격중방향전환
            if (animEnd)
            {
                if (attackInput())
                    setState(State.attack, atkNum);
                else
                {
                    _anim.setSpeed(1);
                    setState(State.idle);
                }
            }
            if (!doneAttack && _anim.isEnd(_anim.sprLength - 3)) {
                //공격
                doneAttack = true;
                if (DamageReceiver.playerAttack(attackPoint))
                {
                    shakeCamera();
                }
                atkNum++;
                if (atkNum > 3)
                    atkNum = 1;
            }
            if (doneAttack && _anim.isEnd(_anim.sprLength - 3))
            {
                //대검 두번째공격
                if (secondAtk)
                {
                    
                    EffectScript es = EffectManager.instance.getEffect(transform.position);
                    es.transform.rotation = Quaternion.Euler(0, 0, -degree * 45+rot);
                    es.initAni("effect/playerAttack/" + name + "/2", attackSpeed/2);
                    es.gameObject.SetActive(true);

                    secondAtk = false;
                }
            }
        } while (!newState);
    }
    private void OnDrawGizmos()
    {
        Handles.color = new Color(0,0,255,0.2f);
        Handles.DrawSolidArc(transform.position, new Vector3(0,0,1), attackfan, attackAngle / 2, attackRange);
        Handles.DrawSolidArc(transform.position, new Vector3(0,0,1), attackfan, -attackAngle / 2, attackRange);
    }
    
}
