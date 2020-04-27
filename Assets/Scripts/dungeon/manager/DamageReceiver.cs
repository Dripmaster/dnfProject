using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageReceiver
{
    static playerFSM player;
    static List<EnemyFSM> enemys;
    static bool enemyRemain = true;
    public static void init(GameObject enemyPrefab) {
        if (enemys == null)
            enemys = new List<EnemyFSM>();
        for (int i = 0; i < 100; i++)
        {
            GameObject g = GameObject.Instantiate(enemyPrefab, Vector2.zero, Quaternion.identity);
            g.SetActive(false);
        }
    }
    public static void addEnemy(EnemyFSM e) {

        e.transform.parent = GameObject.Find("EnemyParent").transform;
        enemys.Add(e);
    }
    public static bool isEnemyRemain()
    {
        int count = 0;
            foreach (EnemyFSM e in enemys) {
                if (e.gameObject.activeInHierarchy == true) {
                    count++;
                    break;
                }
            }
            if (count == 0)
            {
                enemyRemain = false;
            }
            else {
                enemyRemain = true;
            }

        return enemyRemain;
    }
    public static void setEnemy(mapMaker.TileList tileList, GameObject enemyPrefab) {
       
        foreach (tileSet t in tileList.map)
        {
            EnemyFSM enemy=null;
            foreach (EnemyFSM e in enemys)
            {
                if (e.gameObject.activeInHierarchy == false)
                {
                    enemy = e;
                    enemy.transform.position = t.pos;
                    break;
                }
            }
            if(enemy==null)
            enemy = GameObject.Instantiate(enemyPrefab, t.pos, Quaternion.identity).GetComponent<EnemyFSM>();
            enemy.setTypeName(t.type, t.name);
            enemy.gameObject.SetActive(true);
        }
    }
    public static void addPlayer(playerFSM p) {
        player = p;
    }
    public static void playerHit(float attackPoint) {
        player.hitted(attackPoint);
    }

    public static bool playerAttack(float attackPoint,bool cheet = false) {
        bool attackOk = false;
        int atkPoint;
        int successCount = 0;
        for (int i = 0; i < enemys.Count; i++)
        {
            if (!enemys[i].isDead() && (isColMonster(enemys[i].getCol().ClosestPoint(playerFSM.instance.transform.position))||cheet))
            {
                atkPoint = (int)(attackPoint * Random.Range(0.9f, 1.2f));
                   attackOk = true;
                successCount++;
                enemys[i].hitted(atkPoint);
                showHitEffect(enemys[i].getCol());
                EffectManager.instance.AddDamage(atkPoint, enemys[i].transform.position, enemys[i].getDamageTextGen());

                //playerFSM.instance.addCombo(1);
            }
        }
        if(successCount>0)
            playerFSM.instance.addCombo(successCount);
        return attackOk;
    }
    public static bool playerSkill(float attackPoint,float knockBackDegre=1,bool allDir = false,Vector2 pos = new Vector2())
    {
        bool attackOk = false;
        int atkPoint;
        int successCount = 0;
        for (int i = 0; i < enemys.Count; i++)
        {
            if (!enemys[i].isDead() && isColMonster(enemys[i].getCol().ClosestPoint(playerFSM.instance.transform.position),allDir,pos))
            {
                atkPoint = (int)(attackPoint * Random.Range(0.9f, 1.2f));
                attackOk = true;
                successCount++;
                enemys[i].hitted(atkPoint,knockBackDegre);
                showHitEffect(enemys[i].getCol());
                EffectManager.instance.AddDamage(atkPoint, enemys[i].transform.position, enemys[i].getDamageTextGen());

                //playerFSM.instance.addCombo(1);
            }
        }
        if (successCount > 0)
            playerFSM.instance.addCombo(successCount);
        return attackOk;
    }
    public static void playerSkill(float attackPoint, EnemyFSM enemy,float knockBackDegre = 1)
    {
        int atkPoint;
        int successCount = 0;

        atkPoint = (int)(attackPoint * Random.Range(0.9f, 1.2f));
        successCount++;
        enemy.hitted(atkPoint, knockBackDegre);
        showHitEffect(enemy.getCol());
        EffectManager.instance.AddDamage(atkPoint, enemy.transform.position, enemy.getDamageTextGen());

        if (successCount > 0)
            playerFSM.instance.addCombo(successCount);
        return;
    }
    static bool isColMonster(Vector2 ePos,bool allDir = false,Vector2 pos = new Vector2()) {
        if (allDir) {
            if (Vector2.Distance(ePos, pos) <= player.attackRange)
                return true;
        }
        if (Vector2.Distance(ePos, player.transform.position) > player.attackRange)
            return false;
        var dotValue = Mathf.Cos(Mathf.Deg2Rad * (player.attackAngle / 2));
        Vector2 direction = ePos - (Vector2)player.transform.position;
        if (Vector2.Dot(direction.normalized, player.attackfan) > dotValue) {
            
            return true;
        }
        return false;
    }

    static void showHitEffect(BoxCollider2D mColider)
    {
        //Vector2 RandomDir = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
        //EffectScript es = EffectManager.instance.getEffect(mColider.ClosestPoint((Vector2)player.transform.position+RandomDir));
        EffectScript es = EffectManager.instance.getEffect(mColider.ClosestPoint((Vector2)player.transform.position));
        //es.setOffset(Random.Range(0,10)/100f);
        es.initAni("effect/playerHit/" + player.myType.ToString());
        es.gameObject.SetActive(true);
    }
}
