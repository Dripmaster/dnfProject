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
    public int degree;
    Rigidbody2D RBD;
    bool animEnd;
    float checkDashTime = 0.3f;//대쉬 가능 시간
    public float dashRate = 500;
    sceneEffect sEffect;
    //int canDash = 0;//dash페이즈 0=기본, 1=처음 눌림, 2=뗌, 3= 다시 눌림(대쉬시작)
    bool dashState = false;
    Vector2 dashDir;
    public GameObject[] dashEffects;
    int movecount = 0;
    BoxCollider2D _Colider;
    public GameObject myAlert;
    public myParticle myparticle;
    dupEffect myCharge;
    ParticleSystem hammerParticle;
    ParticleSystem bigSwordParticle;
    ParticleSystem[] itemParticle;
    GameObject darkScreen;
    public bool isTuto;
    RayEffect blaze;
    SpriteRenderer confuseAni;
    Image hpBar;
    Image myHeart;
    comboText[] cText;
    public skillTimeScript[] cools;
    int nowC;
    bool isFreeze = false;
    bool canDash;
    bool isHitted = false;
    int myComboCount;
    float comboTimer = 0f;
    float attackSpeedChange;

    bool isDark = false;
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

        speedRate = 100;
        atkNum = 0;

            hpBar = GameObject.Find("hpBar_fg").GetComponent<Image>();
            myHeart = GameObject.Find("Heart").GetComponent<Image>();
            dashDir = new Vector2(0, 0);
            RBD = GetComponent<Rigidbody2D>();
            _Colider = GetComponent<BoxCollider2D>();
            attackfan = new Vector2(0, -1);
            DamageReceiver.addPlayer(this);
            if (myAlert == null)
                myAlert = GameObject.Find("noHp");
            if (darkScreen == null)
                darkScreen = GameObject.Find("darkScreen");
            if (confuseAni == null)
                confuseAni = GameObject.Find("confuseAni").GetComponent<SpriteRenderer>(); ;
            setAlert(false);
            myCharge = GameObject.Find("chargeDup").GetComponent<dupEffect>();
            sEffect = Camera.main.GetComponent<sceneEffect>();
            cText = GameObject.Find("comboCanvas").GetComponentsInChildren<comboText>();
            itemParticle = GameObject.Find("effectRoot").GetComponentsInChildren<ParticleSystem>();
            hammerParticle = GameObject.Find("hammerParticle").GetComponent<ParticleSystem>();
            bigSwordParticle = GameObject.Find("slashParticle").GetComponent<ParticleSystem>();
            cools = GameObject.Find("skillTimeCanvas").GetComponentsInChildren<skillTimeScript>();
            blaze = GameObject.Find("Blaze").GetComponent<RayEffect>();
            Entangles = GameObject.Find("Entangles");
            if (playerDataManager.instance.getEquip() != null)
                myType = (type)(playerDataManager.instance.getEquip().type - 7);
            isDark = false;
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
        if (darkScreen != null)
            darkScreen.SetActive(isEyeDebuff);
        myparticle.Stop();
        myparticle.setSr(GetComponent<SpriteRenderer>());
        init_Stat();
        attackPoint += playerDataManager.instance.showAtkPoint(playerDataManager.instance.getEquip());
        for (int i = 1; i < 4; i++)
        {
            _anim.initAnims("attack/" + i);
        }
        setState(State.idle);
        if (hpBar != null)
            StartCoroutine(lerpHPbar());
        if (myHeart != null)
            StartCoroutine(transHeart());
        if (confuseAni != null)
            confuseAni.enabled = (false);
        myComboCount = 0;
        nowC = 0;
        comboTimer = 0;
        foreach (var item in itemParticle)
        {
            item.Stop();
        }
        StartCoroutine(comboTime());
        StartCoroutine(inputManage());
        initSkill();
        attackSpeedChange = attackSpeed;
        degree = 6;
        blaze.gameObject.SetActive(false);
        foreach (var item in cools[0].GetComponentsInChildren<Image>())
        {
            item.sprite = SLM.instance.getSpr("image/UI/skill/"+myType);
        }
        FSMstart();
    }
    public float getAtkP() {
        return attackPoint;
    }
    void initSkill() {
        switch (myType) {
            case type.BigSword:
                mySkillStrategy = new bigSwordSkill(); break;
            case type.Sword:
                mySkillStrategy = new swordSkill(); break;
            case type.Hammer:
                mySkillStrategy = new hammerSkill(); break;
        }
        if (playerDataManager.instance.getEquip() != null) {
            for (int i = 0; i < playerDataManager.instance.getEquip().hasSkillList.Count; i++)
            {
                if (playerDataManager.instance.getEquip().hasSkillList[i])
                {
                    if(i == 0 || i==2 || i == 3)
                    cools[i+1].GetComponent<CanvasGroup>().alpha = 1;
                }
                else
                {
                    if((i+1)<cools.Length)
                    cools[i+1].GetComponent<CanvasGroup>().alpha = 0;
                }
            }
        }
        else
        {
            cools[2].GetComponent<CanvasGroup>().alpha = 0;
        }
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
            if ((alphaDir == 1) && (amount - 0.1f > alpha)) {
                alphaDir *= -1;
            }
            if ((alphaDir == -1) && (amount+0.1f < alpha))
            {
                alphaDir *= -1;
            }
            hColor.a = alpha;
            myHeart.color = hColor;
            yield return null;
        } while (hp > 0);
        myHeart.color = hColor;
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
                } while ((hpBar.fillAmount - (hp / maxHp) >= 0.01f) || (hpBar.fillAmount - (hp / maxHp)<=-0.01f));
                hpBar.fillAmount = hp / maxHp;
            }
            else {
                yield return null;
            }
        } while (hp > 0);
        hpBar.fillAmount = 0;

    }
    void dashEffect(int i) {
        dashEffects[i].SetActive(true);

        dashEffects[i].transform.position = new Vector2(transform.position.x, transform.position.y - 0.457f);
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
        if (myAlert != null)
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
            RBD.MovePosition(Vector2.Lerp(transform.position, moveDir * 5 + (Vector2)tempPos, Time.deltaTime * 5));
            tempTime += Time.deltaTime;
            yield return null;
        } while (isKnockBack&&((Vector2.Distance(tempPos, transform.position) <= 5f) && tempTime <= 1f));
        RBD.constraints = RigidbodyConstraints2D.FreezeAll;
        isKnockBack = false;
        Physics2D.IgnoreLayerCollision(8, 9, false);
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
            if(!isEntangled)
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
            if (!dashState&&!isEntangled)
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
            else if(!isEntangled)
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
            myComboCount = 10000;
            DamageReceiver.playerAttack(500000, true);
            return true;
        }
        atkNum = 1;
        return false;
    }
    public void addCombo(int addValue) {
        StartCoroutine(comboCounter(addValue));
    }
    IEnumerator comboTime()
    {
        do
        {
            if (myComboCount > 0)
            {
                do
                {
                    comboTimer += Time.deltaTime;
                    yield return null;
                } while (comboTimer <= 2);
                myComboCount = 0;
                attackSpeedChange = attackSpeed;
            }
            else
            {
                yield return null;
            }
        }
        while (gameObject.activeInHierarchy);
    }
    IEnumerator comboCounter(int addValue) {
        while (addValue>0) {

            myComboCount += 1;
            cText[nowC].setComboValue(myComboCount);
            nowC++;
            comboTimer = 0;
            nowC %= cText.Length;
            attackSpeedChange = attackSpeed-myComboCount*0.001f;
            if (attackSpeedChange <= 0.1f) {
                attackSpeedChange = 0.1f;
            }
            addValue--;
            yield return new WaitForSeconds(0.1f);
        }
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
        soundMgr.instance.PlayOneShot("hit");
        if (hp <= 0)
        {
            setState(State.dead);
        }
        else {
            StartCoroutine(hit());
        }
    }
    IEnumerator inputManage() {
        int result = 0;
        bool used = false;
        do
        {
            result = skillInput();
            if (result>0)
            {
                if (result == 1 &&(cools[0].getCool()))
                {
                    _anim.setSpeed(attackSpeedChange);
                    initSkill();
                    setState(State.skill);
                }
                if (result == 2 && (cools[1].getCool()))
                {
                    if(playerDataManager.instance.getEquip() != null)
                    if (
                    playerDataManager.instance.getEquip().hasSkillList[0]) {
                        setSkill(skillType.DarkSide);
                        _anim.setSpeed(attackSpeedChange);
                        setState(State.skill);
                    }
                }
                if (result == 3 && (cools[3].getCool()))
                {
                    if (playerDataManager.instance.getEquip() != null)
                        if (
                     playerDataManager.instance.getEquip().hasSkillList[2])
                        {
                            setSkill(skillType.Blaze);
                            _anim.setSpeed(attackSpeedChange);
                            setState(State.skill);
                        }
                }
                if (result == 4 && (cools[4].getCool()))
                {
                    if (playerDataManager.instance.getEquip() != null)
                        if (
                     playerDataManager.instance.getEquip().hasSkillList[3])
                        {
                            setSkill(skillType.Entangle);
                            _anim.setSpeed(attackSpeedChange);
                            setState(State.skill);
                        }
                }
            }
            result = itemInput();
            if (result>0)
            {
                if(result==1)
                    used = playerDataManager.instance.popItem(itemType.healPotion,1,false);

                if (result == 2)
                    used = playerDataManager.instance.popItem(itemType.clearPotion, 1, false);
                if (used) {
                    itemUse(result);
                }
            }
            yield return null;
        } while (gameObject.activeInHierarchy &&hp>0);
    }
    void CleanPlayer() {
         isKnockBack = false;
         confuseKey = false;
        confuseAni.enabled = false;
         isEyeDebuff = false;
        isEntangled = false;
        if(EntangleES!=null)
        EntangleES.stop();
        darkScreen.SetActive(false);
    }
    IEnumerator healingPlayer() {
        int count = 0;
        do
        {
            hp += maxHp * 0.05f;
            if (hp >= maxHp)
                hp = maxHp;
            soundMgr.instance.PlayOneShot("heal");
            yield return new WaitForSeconds(1);
        } while (++count<5 && hp>0);
    }
    void itemUse(int n) {
        itemParticle[n-1].Play();
        if (n == 1)
        {
            StartCoroutine(healingPlayer());
        }
        else {
            CleanPlayer();
            soundMgr.instance.PlayOneShot("usePotion");
        }
    }
    int itemInput() {
        int result = 0;
        if (Input.GetKeyDown(KeyCode.Q)) {
            result = 1;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            result = 2;
        }
        return result;
    }
    int skillInput() {
        int result = 0;
        if (objectState == State.skill || objectState == State.dead) {
            return 0;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            result = 1;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            result = 2;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            result = 3;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            result = 4;
        }
        return result;
    }
    public void setSkill(skillType sType, bool value = true)
    {
        if (!value)
        {
            mySkillStrategy = null;
            return;
        }
        switch (sType)
        {
            case skillType.DarkSide: mySkillStrategy = new daskSideSkill(gameObject); break;
            case skillType.Entangle: mySkillStrategy = new entagngleSkill(Entangles,true); break;
            case skillType.Blaze: mySkillStrategy = new blazeSkill(); break;
            default: break;
        }
        mySkillStrategy.setTrans(transform, attackRange);
    }
    IEnumerator idle()
    {
        _anim.setSpeed(1);
        if (soundMgr.instance != null)
        soundMgr.instance.LoopStop();
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
                _anim.setSpeed(attackSpeedChange);
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
        soundMgr.instance.PlayLoop("dash");
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
        if (objectState == State.move)
        {
            soundMgr.instance.LoopStop();
        }
        else {
            soundMgr.instance.LoopStop();
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
                LevelManager.instance.deadPlayer();
                break;
            }
        } while (!newState);
    }
    IEnumerator hit()
    {
        isHitted = true;
        _anim.setColor(new Color(0.5f, 0.5f, 0.5f, sr.color.a));
        yield return new WaitForSeconds(0.05f);
        _anim.setColor(new Color(1, 1, 1, sr.color.a));
        yield return new WaitForSeconds(0.05f);
        _anim.setColor(new Color(0.5f, 0.5f, 0.5f, sr.color.a));
        yield return new WaitForSeconds(0.05f);
        _anim.setColor(new Color(1, 1, 1, sr.color.a));
        yield return new WaitForSeconds(0.05f);
        _anim.setColor(new Color(0.5f, 0.5f, 0.5f, sr.color.a));
        yield return new WaitForSeconds(0.05f);
        _anim.setColor(new Color(1, 1, 1, sr.color.a));
        yield return new WaitForSeconds(0.05f);
        isHitted = false;
    }
    public bool IsDark() {
        return isDark;
    }
    public void confuseStart() {
        if (__hpFix)
            return;
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
                _anim.setSpeed(attackSpeedChange);
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
        if (isDark)
            return;
        cools[1].startCool(10);
        StartCoroutine(darkSide());
    }
    public void cutEye() {
        if (hp <= 0)
            return;
        StartCoroutine(eyeSizeDown());
    }
    IEnumerator eyeSizeDown() {
        darkScreen.SetActive(true);
        isEyeDebuff = true;
        yield return new WaitForSeconds(3);
        isEyeDebuff = false;
        darkScreen.SetActive (false);
    }
    protected class blazeSkill : skillStrategy
    {
        override
        public void doSkill()
        {
            ///1.블레이즈 생성
            if (!instance.blaze.gameObject.activeInHierarchy) {
                instance.blaze.setTargets(DamageReceiver.getBlazeTarget());
                instance.blaze.gameObject.SetActive(true);

                ///2.쿨타임적용
                instance.cools[3].startCool(10f);
            }
        }
    }
    class bigSwordSkill : skillStrategy
    {
        override
        public void doSkill()
        {
            ///1.스킬 이펙트 출력
            instance.bigSwordParticle.transform.position = instance.transform.position;
            instance.bigSwordParticle.Play();

            instance.StartCoroutine(instance.spin());
            ///2.쿨타임
            instance.cools[0].startCool(5f);
            ///3.사운드
            soundMgr.instance.Play("longSkill");

        }
    }
    IEnumerator spin() {
        int count = 0;
        do
        {
            if (DamageReceiver.playerSkill(instance.attackPoint, 1, true, bigSwordParticle.transform.position)) {
                soundMgr.instance.PlayOneShot("attack1");
            }
            yield return new WaitForSeconds(0.2f);
        } while (++count<3);
    }
    class swordSkill : skillStrategy
    {
        override
        public void doSkill()
        {

            ///1.스킬 이펙트 출력
            instance.myCharge.startCharge();
            instance.cools[0].startCool(5f);
            ///2.사운드 출력
            soundMgr.instance.Play("swordSkill");
        }
    }
    class hammerSkill : skillStrategy
    {
        override
        public void doSkill()
        {
            ///1.현재 지점으로 부터 원 반경에 있는 몬스터 전체 데미지 및 넉백
            instance.attackRange *= 2.5f;
            DamageReceiver.playerSkill(instance.attackPoint*1.5f,10);
            instance.attackRange /= 2.5f;

            ///2.스킬 이펙트 출력
            EffectScript e = EffectManager.instance.getEffect((Vector2)instance.transform.position + instance.attackfan);
            if (instance.attackfan.y >= 0)
                e.transform.position -= new Vector3(0, 0.5f);
            else { 
                e.transform.position += new Vector3(0, 0.2f);
            }
            e.initAni("effect/playerAttack/hammer/skill",0.1f);

            soundMgr.instance.PlayOneShot("hammerSkill");
            e.setScaleValue(4);
            e.gameObject.SetActive(true);
            instance.cools[0].startCool(5f);
        }
    }
    IEnumerator skill() {
        RBD.constraints = RigidbodyConstraints2D.FreezeAll;
        bool isDo = false;
        if (mySkillStrategy is swordSkill)
        {
            doSkill();
        }
        else
        if (mySkillStrategy is hammerSkill)
        {
            hammerParticle.transform.position = transform.position;
            hammerParticle.Play();
            soundMgr.instance.PlayOneShot("hammerJump");
        }
        else
        if (mySkillStrategy is bigSwordSkill)
        {
            doSkill();
        }
        else {
            doSkill();
            setState(State.move);
        }

        do
        {
            yield return null;
            if (!isDo && _anim.isEnd(8))
            {
                isDo = true;
                if (mySkillStrategy is hammerSkill)
                {

                    doSkill();
                }
            }
            if (_anim.isEnd())
            {

                setState(State.idle);
                break;
            }
            if (dashPlayer())
            {//공격하다 대쉬하면 공격 캔슬
                StartCoroutine(dashTimer());
                setState(State.move);
                break;
            }
        } while (!newState);
    }
    IEnumerator darkSide()
    {
        Physics2D.IgnoreLayerCollision(8, 9);
        Physics2D.IgnoreLayerCollision(8, 10);
        isDark = true;
        Color c = sr.color;
        do
        {
            c.a -= Time.deltaTime;
            sr.color = c;
            yield return null;
        } while (c.a >= 0.3f);
        yield return new WaitForSeconds(2.5f);
        do
        {
            c.a += Time.deltaTime;
            sr.color = c;
            yield return null;
        } while (c.a <= 0.95f);
        c.a = 1;
        sr.color = c;
        Physics2D.IgnoreLayerCollision(8, 10,false);
        Physics2D.IgnoreLayerCollision(8, 9,false);
        isDark = false;
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
                soundMgr.instance.PlayOneShot("attack" + atkNum);
            }
            else { 
                soundMgr.instance.PlayOneShot("longSwing");
            }
            EffectScript es = EffectManager.instance.getEffect(transform.position);
            es.transform.rotation = Quaternion.Euler(0, 0, -degree * 45+rot);
            es.initAni("effect/playerAttack/" + name + "/1", attackSpeedChange / 2);
            es.gameObject.SetActive(true);

            secondAtk = true;
        }
        else
        {
            EffectScript es = EffectManager.instance.getEffect(transform.position);
            es.transform.rotation = Quaternion.Euler(0, 0, -degree * 45+rot);
            es.initAni("effect/playerAttack/" + name + "/" + atkNum , attackSpeedChange);
            es.gameObject.SetActive(true);
        }
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
                {
                _anim.setSpeed(attackSpeedChange);
                    setState(State.attack, atkNum);
                }
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

                    soundMgr.instance.Play("attack" + atkNum);
                }
                else
                {
                    soundMgr.instance.PlayOneShot("swordSwing");
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
                    es.initAni("effect/playerAttack/" + name + "/2", attackSpeedChange / 2);
                    es.gameObject.SetActive(true);
                    secondAtk = false;
                }
            }
        } while (!newState);
    }
    public BoxCollider2D getCol()
    {
        return _Colider;
    }
    /*
    private void OnDrawGizmos()
    {
        //Handles.color = new Color(0,0,255,0.2f);
        //Handles.DrawSolidArc(transform.position, new Vector3(0,0,1), attackfan, attackAngle / 2, attackRange);
        //Handles.DrawSolidArc(transform.position, new Vector3(0,0,1), attackfan, -attackAngle / 2, attackRange);
    }*/
    
}
