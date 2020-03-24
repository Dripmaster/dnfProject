using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : MonoBehaviour
{
    myAnimator _anim;
    // Start is called before the first frame update
    void Awake()
    {
        _anim = GetComponent<myAnimator>();
        
    }

    public void initAni(string path ,float speed=1) {
        _anim.setPath(path);
        _anim.speed = speed;
        _anim.initAnims();
    }

    // Update is called once per frame
    void Update()
    {
        if (_anim.isEnd(-1))
            gameObject.SetActive(false);
    }

}
