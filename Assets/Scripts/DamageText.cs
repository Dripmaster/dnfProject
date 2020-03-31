﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    Vector2 moveDir;
    float deleteTime = 0.4f;
    float tempTime;
    Vector2 scale;
    // Start is called before the first frame update
    void Start()
    {
        
        moveDir = new Vector2(0, 1f);
    }
    private void OnEnable()
    {
        tempTime = deleteTime;
    }

    // Update is called once per frame
    void Update()
    {
        tempTime -= Time.deltaTime;
        if (tempTime <= 0)
        {
            gameObject.SetActive(false);
        }
        transform.Translate(moveDir*Time.deltaTime);
        
    }
}