using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletEffect : MonoBehaviour
{

    myAnimator _anim;
    Vector2 moveDir;
    float attackPoint;
    // Start is called before the first frame update
    void Awake()
    {
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
            _anim.setPath("bullet/" + name);
        else
            _anim.setPath("bullet/boss/" +name);

        _anim.initAnims();
        attackPoint = atk;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.name == "player") {
            DamageReceiver.playerHit(attackPoint);
            gameObject.SetActive(false);
        }

    }
}
