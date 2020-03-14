using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FSMbase : MonoBehaviour
{
    public enum State
    {
        idle = 0,
        move,
        attack,
        dead,
        skill,
        hited,
        NONE
    };
    public State objectState;
    public myAnimator _anim;
    public bool newState;
    public string myPath;
    
    public void Awake()
    {
        objectState = State.idle;
        _anim = GetComponent<myAnimator>();
        setAnim();
    }
    public void OnEnable()
    {
        StartCoroutine("FSMmain");
    }

    public virtual void setAnim() {
        _anim.setPath(myPath);
        _anim.setDir(6);//아래방향으로 초기화
        _anim.initAnims(Enum.GetNames(typeof(State)));
    }

    public virtual void setState(State s)
    {
        newState = true;
        objectState = s;
        _anim.setState(objectState.ToString());
        
    }
    public virtual void setState(State s,int atkNum)
    {
        newState = true;
        objectState = s;
        _anim.setState(objectState.ToString()+"/"+atkNum);
    }

    IEnumerator FSMmain()
    {
        while (true)
        {
            newState = false;
            yield return StartCoroutine(objectState.ToString());
        }
    }
    IEnumerator idle()
    {
        do
        {
            yield return null;

        } while (!newState);
    }

    IEnumerator move()
    {
        do
        {
            yield return null;
        } while (!newState);
    }

}
