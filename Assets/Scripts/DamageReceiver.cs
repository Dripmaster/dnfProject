using System.Collections;
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

        for (int i = 0; i < enemys.Count; i++)
        {
            if (isColMonster(enemys[i].transform.position))
                enemys[i].hitted(attackPoint);
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
}
