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
    protected float maxHp;
    protected float hp;
    protected float attackPoint;
    public string name;
    public type myType;
    public float attackRange;
    protected float moveSpeed = 5;
    protected float attackDelay = 2.0f;

    public void Awake()
    {
        objectState = State.idle;
        _anim = GetComponent<myAnimator>();
    }
    public void init_Stat() {
        maxHp = 1000; //나중엔 데이터로부터 받아오기
        hp = maxHp;
        attackPoint = 50;//나중엔 데이터로부터 받아오기
        attackDelay = 2f; //나중엔 데이터로부터 받아오기
        switch (myType)
        {
            case type.Long: attackRange = 3; break;
            case type.Short: attackRange = 1.1f; break;
            case type.boss: attackRange = 4f; break;
            case type.player: attackRange = 2f; break;
        }
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
        initAnim();
        setAnim();
        init_Stat();
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
