using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dashEffect : MonoBehaviour
{
    Vector2 moveDir;
    public float speed = 1;
    public float deleteTime = 0.2f;
    float tempTime = 0;
    float firstX;
    SpriteRenderer sr;
    Color c;
    // Start is called before the first frame update
    void Awake()
    {
        moveDir = new Vector2(1,0);
        firstX = transform.localPosition.x;
        sr = GetComponent<SpriteRenderer>();
        c = sr.color;
    }
    void OnEnable() {
        tempTime = 0;
        transform.localPosition = new Vector2(firstX, 0);
        c.a = 1;
    }
    

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDir*Time.deltaTime*speed);
        c.a -= Time.deltaTime*5;
        sr.color = c;
        tempTime += Time.deltaTime;
        if (tempTime > deleteTime) {
            transform.parent.gameObject.SetActive(false);
            
        }
    }
}
