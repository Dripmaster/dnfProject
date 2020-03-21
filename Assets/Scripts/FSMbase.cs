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
    };
    public enum type
    {
        player = 0,
        Long,
        Short,
        boss,
        
    };
    public State objectState;
    public myAnimator _anim;
    public bool newState;
    string myPath;
    public float maxHp;
    protected float hp;
    protected float attackPoint;
    public string name;
    public type myType;
    
    public void Awake()
    {
        objectState = State.idle;
        _anim = GetComponent<myAnimator>();
        initAnim();
        setAnim();
        init_Stat();
        
    }
    public void init_Stat() {
        hp = maxHp;
        attackPoint = 50;//나중엔 데이터로부터 받아오기
    }
    public virtual void initAnim() {
        if (myType == 0)
        {
            myPath = "player/" + name;
        }
        else {
            myPath = "enemy/" + name + "/"+Enum.GetName(typeof(type), myType).ToLower();
        }

    }//name으로부터 해당 애니메이션 주소 구해서 myPath로 넣기
    public void OnEnable()
    {
        hp = maxHp;
        StartCoroutine("FSMmain");
    }

    public void setAnim() {
        _anim.setPath(myPath);
        _anim.setDir(6);//아래방향으로 초기화
        _anim.initAnims(Enum.GetNames(typeof(State)));
    }

    public void setState(State s)
    {
        newState = true;
        objectState = s;
        _anim.setState(objectState.ToString());
        
    }
    public void setState(State s,int atkNum)
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
