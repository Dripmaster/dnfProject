using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : MonoBehaviour
{
    protected myAnimator _anim;
    float deleteTime = 0.5f;
    bool StartDelete;
    float tempTime;
    SpriteRenderer sr;
    Color c;
    bool init = false;
    bool isitemGain = false;
    bool isSetAlpha = true;
    // Start is called before the first frame update
    public void Awake()
    {
        StartDelete = false;
        _anim = GetComponent<myAnimator>();
        sr = GetComponent<SpriteRenderer>();
        c = sr.color;
    }
    private void OnEnable()
    {
        if(!isSetAlpha)
        c.a = 1;
        sr.color = c;
        if (isitemGain)
            deleteTime = 1f;
        else
            deleteTime = 0.5f;
        _anim.animNum = 0;
    }
    public void setImage(Sprite s,bool value = false) {
        _anim.enabled = false;
        sr.sprite = s;
        StartDelete = true;
        if (value) {
            isitemGain = true;
        }
        gameObject.SetActive(true);
        init = true;
    }
    public void initAni(string path ,float speed=0.5f) {
        _anim.enabled = true;
        _anim.setPath(path);
        _anim.speed = speed;
        StartDelete = false;
        init = true;
        _anim.initAnims();
        if (_anim.sprLength == 0) {
            print(path);
        }
    }
    public void setAlpha(float a) {
        c.a = a;
        sr.color = c;
        isSetAlpha = true;
    }
    public void setOffset(float time) {
        _anim.setOffset(time);
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            gameObject.SetActive(false);
            return;
        }
        if (!StartDelete && _anim.isEnd())
        {
            _anim.Pause();
            StartDelete = true;
        }
        if (StartDelete) {
            tempTime += Time.deltaTime;
            if (tempTime >= deleteTime)
            {
                if (isitemGain) {
                    EffectManager.instance.popitemGain(this);
                }
                gameObject.SetActive(false);
                isSetAlpha = false;
                StartDelete = false;
                init = false;
                tempTime = 0;
            }
            c.a -= Time.deltaTime / deleteTime;
            sr.color = c;
        }
    }

}
