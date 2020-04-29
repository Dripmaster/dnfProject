using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntangleScript : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer sr;
    bool atkEnemyORplayer;
    void Awake(){
        sr = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        atkEnemyORplayer = false;
        sr.color = new Color(1, 1, 1, 0);
    }
    public void doEffect(bool type) {
        atkEnemyORplayer = type;
        StartCoroutine(entangle());
        foreach (var item in GetComponentsInChildren<AttackShadow>())
        {
            item.doEffect();
        }
    }
    IEnumerator entangle() {    
        yield return new WaitForSeconds(1);
        if (atkEnemyORplayer) { //true:atkEnemy
            foreach (var item in DamageReceiver.GetEnemyFSMs(transform.position))
            {
                item.getEntangled();
            }
        }
        else
        {
            if (Vector2.Distance(playerFSM.instance.transform.position,transform.position)<=0.5f) {
                playerFSM.instance.getEntangled();
            }
        }
    }
}
