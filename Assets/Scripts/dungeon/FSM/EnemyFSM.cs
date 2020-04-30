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
    CircleCollider2D _secondColider;
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
    float attackAmount;
    List<skillType> skillCycle; 
    int FSMCol = 0;
    float knockBackDegree = 1;

    new void Awake()
    {
        base.Awake();
        RBD = GetComponent<Rigidbody2D>();
        DamageReceiver.addEnemy(this);
        _Colider = GetComponent<BoxCollider2D>();
        _secondColider = GetComponent<CircleCollider2D>();
        gameObject.SetActive(false);
        attackBar.gameObject.SetActive(false);
        attackFrame.gameObject.SetActive(false);
    }
    void Update()
    {
        //RBD.velocity = Vector2.zero;   
    }
    new private void OnEnable()
    {
        base.OnEnable();
        speedRate = 100;
        setState(State.move);
        tempDelay = 0f;
        attackAllow = false;
        FSMCol = 0;
        setHpBar();
        setColider();
        initSkill();
    }
    void initSkill() {
        if (myType == type.boss)
        {
            Entangles = GameObject.Find("Entangles0");
            skillCycle = new List<skillType>();
            switch (name)
            {
                case "dark":
                    skillCycle.Add(skillType.DarkSide);
                    skillCycle.Add(skillType.DarkScreen);
                    skillCycle.Add(skillType.DarkSide);
                    skillCycle.Add(skillType.BulletWave);
                    skillCycle.Add(skillType.Confuse);
                    skillCycle.Add(skillType.DarkScreen);
                    skillCycle.Add(skillType.KnockBack);
                    skillCycle.Add(skillType.Entangle);
                    break;
                case "glow":
                    skillCycle.Add(skillType.KnockBack);
                    skillCycle.Add(skillType.BulletWave);
                    skillCycle.Add(skillType.KnockBack);
                    skillCycle.Add(skillType.DarkSide);
                    skillCycle.Add(skillType.DarkScreen);
                    break;
                case "grass":
                    skillCycle.Add(skillType.KnockBack);
                    skillCycle.Add(skillType.Entangle);
                    break;
                case "water":
                    skillCycle.Add(skillType.BulletWave);
                    skillCycle.Add(skillType.KnockBack);
                    skillCycle.Add(skillType.KnockBack);
                    break;
                case "fires":
                    skillCycle.Add(skillType.BulletWave);
                    skillCycle.Add(skillType.KnockBack);
                    skillCycle.Add(skillType.KnockBack);
                    break;
            }
            setSkill(skillCycle[Random.Range(0, skillCycle.Count)]);
        }
        else
            setSkill(0, false);
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
        StartCoroutine(lerpAttackbar());
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
        RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * knockBackDegree * -1*Time.deltaTime/2);
    }

    void lookPlayer()
    {
        moveDir = Vector2.zero;
        moveDir = (playerFSM.instance.transform.position - transform.position).normalized;

        degree = (Mathf.RoundToInt((Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f - 180) * -1) / 45);
        _anim.setDir(degree);
    }

    void moveEnemy(bool value)
    {

        if (FSMCol != 0)
        {
            RBD.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }
        else if(value)
        {

            RBD.constraints = RigidbodyConstraints2D.FreezeRotation;
            moveDir = Vector2.zero;
            moveDir = (playerFSM.instance.transform.position - transform.position).normalized;

            degree = Mathf.RoundToInt((Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f - 180) * -1) / 45;
            _anim.setDir(degree);

            RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * speedRate / 100 * Time.deltaTime);
        }
    }

    bool detectPlayer() {
        if (playerFSM.instance.IsDark())
            return false;
        if (Vector2.Distance(playerFSM.instance.transform.position, transform.position) <= attackRange)
            return true;
        
        return false;
    }
    public void hitted(float damage, float knockBackDegree=1) {
        if (sr.color.a>=0.5f) {
            hpFrame.gameObject.SetActive(true);
            hpBar.gameObject.SetActive(true);
        }
        if (hp <= 0)
        {
            return;
        }
        if (myType == type.boss && hp >= maxHp * 0.6f && (hp - damage) < maxHp * 0.6f)
        {
            skillCycle.Add(skillType.BulletWave);
            skillCycle.Add(skillType.Confuse);
            skillCycle.Add(skillType.Entangle);
        }
        if (myType == type.boss && hp >= maxHp * 0.3f && (hp - damage) < maxHp * 0.3f)
        {

            skillCycle.Add(skillType.Entangle);
            skillCycle.Remove(skillType.DarkSide);
            skillCycle.Add(skillType.BulletWave);
            skillCycle.Add(skillType.Confuse);
            attackDelay = 0.5f;
            
        }
        if (!__hpFix)
        hp -= damage;

        if (gameObject.activeInHierarchy)
            StartCoroutine(lerpHPbar());
        if (hp <= 0)
        {
            hp = 0;
            _anim.speed = 0.5f;
            setState(State.dead);
            itemGen();
        }
        else if(myType!=type.boss){
            RBD.constraints = RigidbodyConstraints2D.FreezeRotation;
            _anim.speed = 0.1f;
            this.knockBackDegree = knockBackDegree;
            setState(State.hited);
        }
    }
    IEnumerator lerpHPbar() {

        do {

            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, hp / maxHp, Time.deltaTime*5);
            yield return null;
        } while (hpBar.fillAmount -(hp/maxHp)>=0.01f);
        hpBar.fillAmount = hp / maxHp;
    }
    IEnumerator lerpAttackbar()
    {

        do
        {
            if (attackBar.fillAmount - attackAmount <0.01f )
            {
                attackBar.fillAmount = Mathf.Lerp(attackBar.fillAmount, attackAmount, Time.deltaTime * 5);
            }
            else {
                attackBar.fillAmount = attackAmount;

            }
            yield return null;
        } while (gameObject.activeInHierarchy);
    }
    void itemGen() {
            itemManager.instance.itemGenerate(transform.position,itemType.gold);
        itemType t = (itemType)System.Enum.Parse(typeof(itemType), name + "Mat");
        //TODO: 랜덤으로 나오게

        itemManager.instance.itemGenerate(transform.position, itemType.gold);
        itemManager.instance.itemGenerate(transform.position, itemType.gold);

        itemManager.instance.itemGenerate(transform.position, t);
        if (myType == type.Long) {

            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(transform.position, t);
            itemManager.instance.itemGenerate(transform.position, t);
        }
        if (myType == type.boss)
        {
            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(transform.position, itemType.gold);
            itemManager.instance.itemGenerate(damageTextGen.position,t);
            itemManager.instance.itemGenerate(damageTextGen.position,t);
            itemManager.instance.itemGenerate(damageTextGen.position,t);
            itemManager.instance.itemGenerate(damageTextGen.position,t);
            itemManager.instance.itemGenerate(damageTextGen.position,t);
            itemManager.instance.itemGenerate(damageTextGen.position,t);
            itemManager.instance.itemGenerate(damageTextGen.position,t);
            itemManager.instance.itemGenerate(damageTextGen.position,t);
            itemManager.instance.itemGenerate(damageTextGen.position,t);
            itemManager.instance.itemGenerate(damageTextGen.position,t);
        }

    }
    IEnumerator move()
    {
        do
        {
            _anim.setSpeed(1);

            if (playerFSM.instance.IsDark()||isEntangled)
            {
                    _anim.setDir(Random.Range(0, 8));
                    RBD.constraints = RigidbodyConstraints2D.FreezeAll;
                    yield return new WaitForSecondsRealtime(0.5f);
                    continue;
            }
            else
            {
                    moveEnemy(attackAllow);
            }
            delayCount();
            if (detectPlayer()) {
                if (attackAllow)
                    setState(State.attack);
            }

            yield return null;
        } while (!newState);
    }

    void spawnBullet() {
        if (myType == type.Long)
        {
            bulletEffect b = EffectManager.instance.getbullet(transform.position);
            b.transform.Rotate(new Vector3(0, 0, (Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f)));
            b.setAnim(name, attackPoint, myType == type.boss);
            b.gameObject.SetActive(true);
        }
        else {
            bulletEffect b;
            for (int i = -2; i < 3; i++)
            {
                b = EffectManager.instance.getbullet(transform.position);
                b.transform.Rotate(new Vector3(0, 0, 25 * i + (Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f)));
                b.setAnim(name, attackPoint, myType == type.boss);
                b.gameObject.SetActive(true);
            }
        }
    }

    IEnumerator attack()
    {
        RBD.constraints = RigidbodyConstraints2D.FreezeAll;
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
                attackAmount = 0;
                float tempTime = 0f;
                
                if (mySkillStrategy != null && Random.Range(0f,1f)>=0.5f)
                {
                    yield return StartCoroutine(skill());

                    setSkill(skillCycle[Random.Range(0,skillCycle.Count)]);
                    _anim.reOn();
                }
                else
                {
                    doEffect1.SendMessage("doEffect", SendMessageOptions.DontRequireReceiver);
                    doEffect2.SendMessage("doEffect", SendMessageOptions.DontRequireReceiver);
                    do
                    {
                        attackAmount = tempTime / attackStopTime;
                        yield return new WaitForSeconds(attackStopTime / 10);
                        tempTime += attackStopTime / 10f;
                        if (tempTime >= attackStopTime)
                            break;
                    } while (true);
                    _anim.reOn();
                    if (hp <= 0)
                    {
                        setState(State.dead);
                        break;
                    }
                    if (myType != type.Short)
                        spawnBullet();
                    else if (detectPlayer() && objectState == State.attack)
                        DamageReceiver.playerHit(attackPoint);
                }
                soundMgr.instance.Play("enemyAttack");

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
        RBD.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    IEnumerator hited()
    {

        _Colider.enabled = false;
        _secondColider.enabled = true;
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
        _Colider.enabled = true;
        _secondColider.enabled = false;
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
    IEnumerator skill() {
        //!TODO EffectManager.instance.gogo rererererrerererererere 1.enemy 생성때 보스면 파티클추가 및 색 조정, 그 후 플레이만 눌러서 하는걸로
        GameObject g = GameObject.Instantiate(Resources.Load<GameObject>("prefabs/Effect/particle/glowParticle"), transform.position, Quaternion.identity);

        ParticleSystem[] p = g.GetComponentsInChildren<ParticleSystem>();
        Color parColor = Color.white;
        switch (name) {
            case "fire":parColor = Color.red; break;
            case "glow":parColor = Color.yellow; break;
            case "water":parColor = Color.blue; break;
            case "dark":parColor = Color.gray; break;
            case "grass":parColor = Color.green; break;
        }
        foreach (var item in p)
        {

            ParticleSystem.MainModule mainModule = item.main;

            mainModule.startColor = parColor;
            if(hp<maxHp*0.3f)
            mainModule.startLifetime = 2;
        }
        g.GetComponent<ParticleSystem>().Play();
        if (hp < maxHp * 0.3f)
        {
            GameObject.Destroy(g, 0.75f);
        }
        else {
            GameObject.Destroy(g, 1.5f);
        }
        float tempTime = 0;
        do
        {

            attackAmount = tempTime / 1.5f;
            if (hp < maxHp * 0.3f)
            {
                tempTime += 1.5f / 10f;
                yield return new WaitForSeconds(0.75f / 10);
            }
            else {

                tempTime += 1.5f / 10f;
                yield return new WaitForSeconds(1.5f / 10);
            }
            if (tempTime >= 1.5f)
                break;
        } while (true);
        if(hp>0)
        doSkill();
    }
    
    /*private void OnDrawGizmos()
    {
        
        //Handles.color = new Color(255, 0, 0, 0.2f);
        //Handles.DrawSolidArc(transform.position, new Vector3(0, 0, 1), moveDir, 90/2, attackRange);
        //Handles.DrawSolidArc(transform.position, new Vector3(0, 0, 1), moveDir, -90 / 2, attackRange);

    }*/
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
    public void glowHeal() {
        if (name == "glow"&&myType != type.boss)
        {
            hp += maxHp / 10f;
            if (hp >= maxHp)
                hp = maxHp;
            StartCoroutine(lerpHPbar());
        }
    }
    public void setSkill(skillType sType,bool value = true) {
        if (!value)
        {
            mySkillStrategy = null;
            return;
        }
        switch (sType) {
            case skillType.Confuse: mySkillStrategy = new confuseKeySkill();break;
            case skillType.KnockBack: mySkillStrategy = new knockBackSkill();break;
            case skillType.DarkSide: mySkillStrategy = new daskSideSkill(gameObject);break;
            case skillType.DarkScreen: mySkillStrategy = new eyeCutSkill();break;
            case skillType.Entangle: mySkillStrategy = new entagngleSkill(Entangles,false);break;
            case skillType.BulletWave: mySkillStrategy = new bulletWaveSkill(this);break;
            default: break;
        }
        mySkillStrategy.setTrans(transform,attackRange);
    }
    class confuseKeySkill : skillStrategy
    {

        override
        public void doSkill()
        {
            playerFSM.instance.confuseStart();
        }
    }
    class eyeCutSkill : skillStrategy
    {

        override
        public void doSkill()
        {
            playerFSM.instance.cutEye();
        }
    }
    class bulletWaveSkill : skillStrategy
    {
        EnemyFSM me;
        public bulletWaveSkill(EnemyFSM e) {
            me = e;
        }
        override
        public void doSkill()
        {
            me.StartCoroutine(me.BulletSpawn());

        }
    }
    void randomBullet() {
        bulletEffect b;
        int dir = Random.Range(0, 4);
        if (dir == 0)
            for (int i = -3; i < 4; i++)
            {
                b = EffectManager.instance.getbullet(new Vector2(8, i * 2f));
                b.transform.Rotate(new Vector3(0, 0, 180));
                b.setAnim(name, attackPoint, true);
                b.gameObject.SetActive(true);
            }
        if (dir == 1)
            for (int i = -3; i < 4; i++)
            {
                b = EffectManager.instance.getbullet(new Vector2(-7.8f, i * 2f));
                b.transform.Rotate(new Vector3(0, 0, 0));
                b.setAnim(name, attackPoint, true);
                b.gameObject.SetActive(true);
            }
        if (dir == 2)
            for (int i = -4; i < 5; i++)
            {
                b = EffectManager.instance.getbullet(new Vector2(i * 1.8f, 6.5f));
                b.transform.Rotate(new Vector3(0, 0, -90));
                b.setAnim(name, attackPoint, true);
                b.gameObject.SetActive(true);
            }
        if (dir == 3)
            for (int i = -4; i < 5; i++)
            {
                b = EffectManager.instance.getbullet(new Vector2(i * 1.8f, -7));
                b.transform.Rotate(new Vector3(0, 0, 90));
                b.setAnim(name, attackPoint, true);
                b.gameObject.SetActive(true);
            }
    }
    public IEnumerator BulletSpawn() {
        int count = 0;
        do
        {
            randomBullet();
            yield return new WaitForSeconds(1f);
        } while (++count<5);
    }

    class knockBackSkill : skillStrategy
    {

        override
            public void doSkill()
        {
            //!TODO EffectManager.instance.gogo ererererererer
            GameObject g = GameObject.Instantiate(Resources.Load<GameObject>("prefabs/Effect/particle/shockWaveParticle"), myTrans.position, Quaternion.identity);
                g.GetComponent<ParticleSystem>().Play();
            GameObject.Destroy(g,2f);

            if (Vector2.Distance(playerFSM.instance.transform.position,myTrans.position)<=checkRange)
            playerFSM.instance.playerKnockBack((playerFSM.instance.transform.position- myTrans.position).normalized);
        }
    }
    public void startDarkSide()
    {
        StartCoroutine(darkSide());
    }
    IEnumerator darkSide()
    {
        Color c = sr.color;
        do
        {
            c.a -= Time.deltaTime;
            sr.color = c;
            hpBar.gameObject.SetActive(false);
            hpFrame.gameObject.SetActive(false);
            yield return null;
        } while (c.a >= 0.01f);
        yield return new WaitForSeconds(3f);
        do
        {
            c.a += Time.deltaTime;
            sr.color = c;
            yield return null;
        } while (c.a <= 0.95f);
        c.a = 1;
        sr.color = c;
    }

}
