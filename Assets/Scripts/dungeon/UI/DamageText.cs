using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    Vector2 moveDir;
    float deleteTime = 0.5f;
    float tempTime;
    Vector2 scale;
    // Start is called before the first frame update
    void Awake()
    {
        moveDir = new Vector2(Random.Range(-1f,1f), 1f);

        scale = transform.localScale;
    }
    private void OnEnable()
    {
        moveDir.x = Random.Range(-1f, 1f);
        tempTime = deleteTime;
        transform.localScale = scale;
    }


    // Update is called once per frame
    void Update()
    {
        tempTime -= Time.deltaTime;
        if (tempTime <= 0)
        {
            gameObject.SetActive(false);
        }
        
        if (tempTime >= deleteTime * 0.6)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, scale * 2, Time.deltaTime);
            transform.Translate(moveDir * Time.deltaTime);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector2.zero, Time.deltaTime / deleteTime * 2);
            transform.Translate(moveDir * Time.deltaTime/5);
        }

        //rect.localScale = Vector3.Lerp(rect.localScale,scale,Time.deltaTime*10f);

    }
}
