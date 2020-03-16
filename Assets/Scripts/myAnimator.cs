using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myAnimator : MonoBehaviour
{
    string animPath;
    string state;
    int direction;
    SpriteRenderer sr;
    int animNum;
    public bool hasDir = false;
    WaitForSeconds WFS;
    public float speed = 0.5f;
    public int sprLength = 0;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animNum = 0;//anim 번호 처음으로 초기화
        WFS = new WaitForSeconds(0);
        StartCoroutine(sprUpdater());
    }
    void LateUpdate()
    {
        if (sr.sprite == null)
            print("image load error : "+string.Format(SLM.animPathFormat, animPath, state, direction, animNum));
    }
    void sprUptate_None()
    {//state, 방향 필요 없음
        sr.sprite = SLM.getSpr(string.Format(SLM.animPathFormat_NONE, animPath, animNum));
        animNum++;
        if (animNum >= sprLength)
            animNum = 0;
    }
    void sprUptate() {
       
        sr.sprite = SLM.getSpr(string.Format(SLM.animPathFormat, animPath, state, direction, animNum));

        animNum++;
        if (animNum >= sprLength)
            animNum = 0;
    }
    public void setPath(string aPath) {

        animPath = aPath;
    }
    public void setState(string stat) {
        state = stat;
        animNum = 0;
        if(hasDir)
            sprLength = SLM.countSprite(string.Format(SLM.animPathInitFormat, animPath, stat, direction));
        else
            sprLength = SLM.countSprite(string.Format(SLM.animPathFormat_NONE, animPath));
        WFS  = new WaitForSeconds(speed/sprLength);
    }
    public void setDir(int dir) {
        direction = dir;
    }
    public void initAnims(string[] stats) {
        string[] paths = new string[stats.Length*8];
        for (int i = 0; i < stats.Length; i++) {
            for (int j = 0; j < 8; j++) {
                if(hasDir)
                paths[i*8+j] = string.Format(SLM.animPathInitFormat, animPath,stats[i], j);
                else
                    paths[i * 8 + j] = string.Format(SLM.animPathInitFormat_NONE, animPath, stats[i], j);

            }
        }
        SLM.Load(paths);
    }
    public void initAnims(string stat)
    {
        string[] paths = new string[8];
        for (int j = 0; j < 8; j++) { 
            {
                if(hasDir)
                    paths[j] = string.Format(SLM.animPathInitFormat, animPath, stat, j);
                else
                    paths[j] = string.Format(SLM.animPathInitFormat_NONE, animPath, stat, j);

            }
        }
        SLM.Load(paths);
    }
    public bool isEnd(int about = 0) {
        return (animNum >= sprLength - about-1); 
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
