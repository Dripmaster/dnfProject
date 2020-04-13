using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : MonoBehaviour
{
    protected myAnimator _anim;
    float deleteTime = 1f;
    bool StartDelete;
    float tempTime;
    SpriteRenderer sr;
    Color c;
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
        c.a = 1;
        sr.color = c;
    }
    public void setImage(Sprite s) {
        _anim.enabled = false;
        sr.sprite = s;
        StartDelete = true;
        gameObject.SetActive(true);
    }
    public void initAni(string path ,float speed=0.5f) {
        _anim.enabled = true;
        _anim.setPath(path);
        _anim.speed = speed;
        _anim.initAnims();
    }
    public void setOffset(float time) {
        _anim.setOffset(time);
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartDelete&&_anim.isEnd())
        {
            _anim.Pause();
            StartDelete = true;
        }
        if (StartDelete) {
            tempTime += Time.deltaTime;
            if (tempTime >= deleteTime)
            {
                gameObject.SetActive(false);
                StartDelete = false;
                tempTime = 0;
            }
            c.a -= Time.deltaTime / deleteTime;
            sr.color = c;
        }
    }

}
