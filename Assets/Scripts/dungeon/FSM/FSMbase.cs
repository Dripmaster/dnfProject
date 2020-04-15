using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FSMbase : MonoBehaviour
{
    
    public State objectState;
    public myAnimator _anim;
    public bool newState;
    string myPath;
    protected float maxHp;
    protected float hp;
    protected float attackPoint;
    protected new string name;
    public type myType;
    public float attackRange;
    protected float moveSpeed = 5;
    protected float attackDelay = 2.0f;
    protected float attackSpeed = 0.5f;
    public float attackAngle = 50f;
    public bool __hpFix = false;
    public void Awake()
    {
        objectState = State.idle;
        if(_anim == null)
        _anim = GetComponent<myAnimator>();
    }
    public void init_Stat() {
        maxHp = DataSetManager.instance.loadFSMData(1,(int)myType);
        attackPoint = DataSetManager.instance.loadFSMData(2,(int)myType);
        attackRange = DataSetManager.instance.loadFSMData(3,(int)myType);
        attackDelay = DataSetManager.instance.loadFSMData(4,(int)myType);
        attackSpeed = DataSetManager.instance.loadFSMData(5,(int)myType);
        moveSpeed = DataSetManager.instance.loadFSMData(6,(int)myType);
        attackAngle = DataSetManager.instance.loadFSMData(7, (int)myType);
        hp = maxHp;
    }
    public void setTypeName(int t, string n) {
        myType = (type)t;
        name = n;
        init_Stat();
    }
    public virtual void initAnim() {
        if ((int)myType <= 3)
        {
            name = Enum.GetName(typeof(type), myType).ToLower();
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
