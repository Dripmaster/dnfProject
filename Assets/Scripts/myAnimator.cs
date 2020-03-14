using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myAnimator : MonoBehaviour
{
    string animPath;
    string sprPath;
    string state;
    int direction;
    SpriteRenderer sr;
    int animNum;
    Sprite s;
    public bool hasDir = false;
    WaitForSeconds WFS;
    public float speed = 0.5f;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animNum = 0;//anim 번호 처음으로 초기화
        WFS = new WaitForSeconds(0);
        StartCoroutine(sprUpdater());
    }
    void Update()
    {
        
    }
    void sprUptate_None()
    {//state, 방향 필요 없음
        sprPath = string.Format(SLM.animPathFormat_NONE, animPath, animNum);
        s = SLM.getSpr(sprPath);
        if (s == null)
        {
            animNum = 0;
            sprPath = string.Format(SLM.animPathFormat_NONE, animPath, animNum);
            s = SLM.getSpr(sprPath);
        }
        sr.sprite = s;

        animNum++;
    }
    void sprUptate() {
        sprPath = string.Format(SLM.animPathFormat, animPath,state,direction,animNum);
        s = SLM.getSpr(sprPath);
        if (s == null)
        {
            animNum = 0;
            sprPath = string.Format(SLM.animPathFormat, animPath, state, direction, animNum);
            s = SLM.getSpr(sprPath);
        }
        sr.sprite = s;
        
        animNum++;
    }
    public void setPath(string aPath) {

        animPath = aPath;
    }
    public void setState(string stat) {
        state = stat;
        animNum = 0;
        WFS  = new WaitForSeconds(speed/SLM.countSprite(string.Format(SLM.animPathInitFormat, animPath, stat,direction)));
    }
    public void setDir(int dir) {
        direction = dir;
    }
    public void initAnims(string[] stats) {
        string[] paths = new string[stats.Length*8];
        for (int i = 0; i < stats.Length; i++) {
            for (int j = 0; j < 8; j++) {
                paths[i*8+j] = string.Format(SLM.animPathInitFormat, animPath,stats[i], j);
            }
        }
        SLM.Load(paths);
    }
    public void initAnims(string stat)
    {
        string[] paths = new string[8];
        for (int j = 0; j < 8; j++) { 
            {
                paths[j] = string.Format(SLM.animPathInitFormat, animPath, stat, j);
            }
        }
        SLM.Load(paths);
    }
    public bool isEnd() {
        if(hasDir)
        sprPath = string.Format(SLM.animPathFormat, animPath, state, direction, animNum+1);
        else
        sprPath = string.Format(SLM.animPathFormat_NONE, animPath, animNum+1);
        return !SLM.isSpr(sprPath); ;
    }

    IEnumerator sprUpdater()
    {
        
        do
        {
            if (hasDir)
                sprUptate();
            else
                sprUptate_None();
            yield return WFS;


        } while (gameObject.activeInHierarchy);


    }

}
