using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class bulletEffect : EffectScript
{

    Vector2 moveDir;
    float attackPoint;
    string effPath;
    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
        moveDir = new Vector2(8, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDir*Time.deltaTime);
    }

    public void setAnim(string name, float atk,bool isBoss = false) {
        if (!isBoss)
        {
            _anim.setPath("bullet/" + name);
            effPath = ("effect/enemyLong/" + name);
            GetComponent<BoxCollider2D>().size = new Vector2(0.88f, 0.38f);
        }
        else
        {
            _anim.setPath("bullet/boss/" + name);
            effPath = ("effect/enemyBoss/"+name);

        }

        _anim.initAnims();
        attackPoint = atk;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.name == "player") {
            DamageReceiver.playerHit(attackPoint);
            gameObject.SetActive(false);

            EffectScript es = EffectManager.getEffect(col.ClosestPoint(transform.position));
            es.initAni(effPath);
            es.gameObject.SetActive(true);
        }
        else if (col.tag == "wall")
        {
            gameObject.SetActive(false);

            EffectScript es = EffectManager.getEffect(col.ClosestPoint(transform.position));
            es.initAni(effPath);
            es.gameObject.SetActive(true);

        }
    }
}
