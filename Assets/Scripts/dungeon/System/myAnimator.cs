﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myAnimator : MonoBehaviour
{
    string animPath;
    string state;
    int direction;
    SpriteRenderer sr;
    public int animNum;
    public bool hasDir = false;
    WaitForSeconds WFS;
    public float speed = 0.5f;
    public int sprLength = 0;
    bool aniPause = false;
    bool isEnded = false;
    float offsetTime = 0f;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animNum = 0;//anim 번호 처음으로 초기화
        WFS = null;
    }
    void OnEnable() {
        aniPause = false;
        isEnded = false;
        StartCoroutine(sprUpdater());
    }
    public void setOffset(float time)
    {
        Pause();
        offsetTime = time;
    }
    void sprUptate_None()
    {//state, 방향 필요 없음
        
        sr.sprite = SLM.instance.getSpr(string.Format(SLM.instance.animPathFormat_NONE, animPath, animNum));
    }
    void sprUptate() {
        
        sr.sprite = SLM.instance.getSpr(string.Format(SLM.instance.animPathFormat, animPath, state, direction, animNum));
        
    }
    public void setPath(string aPath) {

        animPath = aPath;
    }
    public void setState(string stat) {
        state = stat;
        animNum = 0;
        sprLength = SLM.instance.countSprite(string.Format(SLM.instance.animPathInitFormat, animPath, stat, direction));

        if (sprLength != 0)
            WFS  = new WaitForSeconds(speed/sprLength);
    }
    public void setDir(int dir) {
        direction = dir%8;
    }
    public void initAnims(string[] stats) {
        
        string[] paths = new string[stats.Length*8];
        for (int i = 0; i < stats.Length; i++) {
            for (int j = 0; j < 8; j++) {
                paths[i*8+j] = string.Format(SLM.instance.animPathInitFormat, animPath,stats[i], j);
            }
        }
        try
        {
            sr.sprite = SLM.instance.Load(paths);
        }
        catch
        {
            print(SLM.instance);
        }
    }
    public void initAnims(string stat)
    {
        string[] paths = new string[8];
        for (int j = 0; j < 8; j++) { 
            {
               paths[j] = string.Format(SLM.instance.animPathInitFormat, animPath, stat, j);
            }
        }
        SLM.instance.Load(paths);
    }
    public void initAnims() {
        animNum = 0;
        SLM.instance.Load(string.Format(SLM.instance.animPathInitFormat_NONE,animPath));
        sprLength = SLM.instance.countSprite((string.Format(SLM.instance.animPathInitFormat_NONE, animPath)));
        if (sprLength != 0)
            WFS = new WaitForSeconds(speed / sprLength);
    }
    public Sprite currentSpr() {

        return sr.sprite;
    }
    
    public bool isEnd(int about = 0) {
        //if(sprLength == 1&&isEnded)
        //print(animNum);

            //return isEnded;
            return (animNum >= (sprLength- about)) || isEnded;
        //return (animNum >= sprLength - about-1); 
    }
    public void Pause(bool init = true) {
        aniPause = true;
        if (init)
        {
            animNum = sprLength - 1;
            if (hasDir)
                sprUptate();
            else
                sprUptate_None();
        }
    }
    public void reOn()
    {
        aniPause = false;
    }
    public void setColor(Color c) {
        sr.color = c;
    }
    public void setSpeed(float spd) {
        speed = spd;
        WFS = new WaitForSeconds(speed / sprLength);
    }

    IEnumerator sprUpdater()
    {
        do
        {
            if (aniPause) {
                if (offsetTime != 0)
                {
                    sr.sprite = null;
                    yield return new WaitForSeconds(offsetTime);

                    aniPause = false;
                }
                else
                {
                    yield return null;
                }
                continue;
            }
            isEnded = false;
            if (animNum >= sprLength)
            {
                animNum = 0;
                isEnded = true;
            }
            if (hasDir)
                sprUptate();
            else
                sprUptate_None();
            if (WFS == null)
            {
                if (sprLength != 0)
                    WFS = new WaitForSeconds(speed / (float)sprLength);
            }
            animNum++;
            if (isEnded == true)
                yield return null;
            else
            yield return WFS;

        } while (gameObject.activeInHierarchy);

        
    }

}
