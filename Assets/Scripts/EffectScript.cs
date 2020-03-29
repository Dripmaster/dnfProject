using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : MonoBehaviour
{
    protected myAnimator _anim;
    // Start is called before the first frame update
    public void Awake()
    {
        _anim = GetComponent<myAnimator>();
    }

    public void initAni(string path ,float speed=0.5f) {
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
        if (_anim.isEnd(-1))
            gameObject.SetActive(false);
    }

}
