using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class title : MonoBehaviour
{
    public Animator cube;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cube.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
            soundMgr.instance.Play("btnc");

            this.enabled = false;
        }
    }
}
