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
            }
        }
        return attackOk;
    }
    static bool isColMonster(Vector2 ePos) {
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
        es.initAni("effect/playerHit/" + player.name);
        es.gameObject.SetActive(true);
    }
}
