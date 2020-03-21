using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletEffect : MonoBehaviour
{

    myAnimator _anim;
    
    // Start is called before the first frame update
    void Awake()
    {
        _anim = GetComponent<myAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setAnim(string name, bool isBoss = false) {
        if (!isBoss)
            _anim.setPath("bullet/" + name);
        else
            _anim.setPath("bullet/boss/" +name);

        _anim.initAnims();
        
    }
}
