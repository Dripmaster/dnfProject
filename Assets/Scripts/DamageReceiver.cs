﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageReceiver
{
    static playerFSM player;
    static List<EnemyFSM> enemys;

    public static void addEnemy(EnemyFSM e) {
        if (enemys == null)
            enemys = new List<EnemyFSM>();
        enemys.Add(e);
    }

    public static void addPlayer(playerFSM p) {
        player = p;
    }
    public static void playerHit(float attackPoint) {
        
        player.hitted(attackPoint);
    }

    public static void playerAttack(float attackPoint) {
        if (enemys == null)
            enemys = new List<EnemyFSM>();
        for (int i = 0; i < enemys.Count; i++)
        {
            if (!enemys[i].isDead()&& isColMonster(enemys[i].transform.position))
            {
                enemys[i].hitted(attackPoint);
                showHitEffect(enemys[i].getCol());
                EffectManager.AddDamage(attackPoint, enemys[i].transform.position,enemys[i].getDamageTextGen());
            }
            
        }
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
        //EffectScript es = EffectManager.getEffect(mColider.ClosestPoint((Vector2)player.transform.position+RandomDir));
        EffectScript es = EffectManager.getEffect(mColider.ClosestPoint((Vector2)player.transform.position));
        //es.setOffset(Random.Range(0,10)/100f);
        es.initAni("effect/playerHit/" + player.name);
        es.gameObject.SetActive(true);
    }
}