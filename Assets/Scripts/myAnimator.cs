using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myAnimator : MonoBehaviour
{

    const string animPathFormat = "image/{0}/{1}/{2}/{3}";//0 :경로 1:state 2:방향 3:anim번호
    const string animPathFormat_NONE = "image/{0}/{1}";//0 :경로 1:anim번호
    string animPath;
    string sprPath;
    string state;
    string direction;
    SpriteRenderer sr;
    int animNum;
    Sprite s;
    public bool hasDir = false;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        //test
        direction = "5";
        animNum = 0;
    }
    void Update()
    {
        if (hasDir)
            sprUptate();
        else
            sprUptate_None();
    }
    void sprUptate_None()
    {
        sprPath = string.Format(animPathFormat_NONE, animPath, animNum);
        s = Resources.Load<Sprite>(sprPath);
        if (s == null)
        {
            animNum = 0;
            sprPath = string.Format(animPathFormat_NONE, animPath, animNum);
            s = Resources.Load<Sprite>(sprPath);
        }
        sr.sprite = s;

        animNum++;
    }
    void sprUptate() {
        sprPath = string.Format(animPathFormat, animPath,state,direction,animNum);
        s = Resources.Load<Sprite>(sprPath);
        if (s == null)
        {
            animNum = 0;
            sprPath = string.Format(animPathFormat, animPath, state, direction, animNum);
            s = Resources.Load<Sprite>(sprPath);
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
    }
    public void setDir(string dir) {
        direction = dir;
    }
    public bool isEnd() {
        if(hasDir)
        sprPath = string.Format(animPathFormat, animPath, state, direction, animNum+1);
        else
        sprPath = string.Format(animPathFormat_NONE, animPath, animNum+1);
        s = Resources.Load<Sprite>(sprPath);
        if (s == null)
            return true;

        return false;
    }
}
