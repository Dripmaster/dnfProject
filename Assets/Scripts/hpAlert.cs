using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hpAlert : MonoBehaviour
{
    SpriteRenderer sr;
    float tempTime;
    float AlertTime = 0.5f;
    Color c;
    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        tempTime = AlertTime;
        c = sr.color;
    }
    void OnEnable()
    {
        tempTime = AlertTime;
        c.a = 0;
    }

    // Update is called once per frame
    void Update()
    {
        tempTime -= Time.deltaTime;
        if (tempTime >= 0)
        {
            c.a = 1 - Mathf.Abs((AlertTime/2 - tempTime) / (AlertTime/2));
            sr.color = c;
        }
        else if (tempTime < 0) {
            gameObject.SetActive(false);
        }
    }
}
