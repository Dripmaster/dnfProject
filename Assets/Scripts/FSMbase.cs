using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMbase : MonoBehaviour
{
    public enum State
    {
        idle = 0,
        move,
        attack,
        dead,
        skill,
        hited
    };
    public State objectState;
    public myAnimator _anim;
    public bool newState;
    public string myPath;
    //
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
    }

    // Update is called once per frame
    public virtual void Update()
    {

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
