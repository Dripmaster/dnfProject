using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class bulletEffect : MonoBehaviour
{

    myAnimator _anim;
    Vector2 moveDir;
    float attackPoint;
    GameObject prefab;
    string effPath;
    // Start is called before the first frame update
    void Awake()
    {
        prefab = Resources.Load<GameObject>("prefabs/Effect");
        _anim = GetComponent<myAnimator>();
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
            
            EffectScript es = Instantiate(prefab, col.ClosestPoint(transform.position), Quaternion.identity).GetComponent<EffectScript>();
            es.initAni(effPath);
        }
        else if (col.tag == "wall")
        {
            gameObject.SetActive(false);

            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, transform.forward);

            EffectScript es = Instantiate(prefab, col.ClosestPoint(transform.position), Quaternion.identity).GetComponent<EffectScript>();

            es.initAni(effPath);
        }
    }
}
