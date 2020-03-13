using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerFSM : FSMbase
{
    public float attackRange = 1.1f;
    public float moveSpeed = 5;
    float speedRate;
    float maxHP = 100;
    float hp;
    int atkNum;
    int degree;
    Rigidbody2D RBD;
    bool animEnd;
    public float checkDashTime = 0.75f;//대쉬 가능 시간
    float checkDashTimeTemp;
    int canDash = 0;//dash페이즈 0=기본, 1=처음 눌림, 2=뗌, 3= 다시 눌림(대쉬시작)
    bool dashState = false;

    // Use this for initialization
    void Awake()
    {
        base.Awake();
        speedRate = 100;
        hp = maxHP;
        atkNum = 0;
        checkDashTimeTemp = checkDashTime;
        setState(State.idle);
        RBD = GetComponent<Rigidbody2D>();
        for (int i = 1; i < 4; i++) {
            _anim.initAnims("attack/" + i);
        }
    }
    void Update() {
        dashCount();
    }
    void dashCount()
    {
        if (canDash<3)
        {
            checkDashTimeTemp -= Time.deltaTime;
            if (checkDashTimeTemp < 0) {
                checkDashTimeTemp = checkDashTime;
                canDash = 0;
            }
        }
    }
    bool dashPlayer() {//dash페이즈 체크
        if (dashState)
            return false;
        if (canDash == 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                canDash = 1;
                checkDashTimeTemp = checkDashTime;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                canDash = 1;
                checkDashTimeTemp = checkDashTime;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                canDash = 1;
                checkDashTimeTemp = checkDashTime;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                canDash = 1;
                checkDashTimeTemp = checkDashTime;
            }
        }
        else if (canDash == 1) {
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                canDash = 2;
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                canDash = 2;

            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                canDash = 2;

            }
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                canDash = 2;

            }
        }
        else if (canDash == 2)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                canDash = 3;
                return true;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                canDash = 3;
                return true;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                canDash = 3;
                return true;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                canDash = 3;
                return true;
            }
        }
        return false;
    }
    bool movePlayer()
    {
        Vector2 moveDir = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir.x += -1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDir.x += 1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDir.y += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDir.y += -1;
        }
        if (dashState)
            speedRate = 1000;
        else if (objectState == State.attack)
            speedRate = 20;
        else
            speedRate = 100;
        
        if (moveDir != Vector2.zero)
        {
            RBD.velocity = moveDir * moveSpeed * speedRate / 100;

            degree = Mathf.RoundToInt((Mathf.Atan2(moveDir.y,moveDir.x)/Mathf.PI*180f -180 )* -1)/45;
            _anim.setDir(degree);

            return true;
        }
        else
        {
            RBD.velocity = moveDir;
        }
        return false;
    }
    bool attackInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            atkNum++;
            if (atkNum > 3)
                atkNum = 1;
            return true;
        }
        return false;
    }
    IEnumerator idle()
    {
        do
        {
            yield return null;
            if (dashPlayer())
            {
                StartCoroutine(dashTimer());
                continue;
            }
            if (attackInput())
            {
                setState(State.attack,atkNum);
            }
            else
            {
                if (movePlayer())
                    setState(State.move);
            }

        } while (!newState);
    }
    IEnumerator dashTimer() {
        dashState = true;
        yield return new WaitForSeconds(0.1f);
        dashState = false;
        canDash = 0;
    }
    IEnumerator move()
    {
        do
        {
            yield return null;
            if (dashPlayer())
            {
                StartCoroutine(dashTimer());
                continue;
            }
            if (attackInput())
            {
                setState(State.attack,atkNum);
            }
            else
            {
                if (!movePlayer())
                    setState(State.idle);
            }
        } while (!newState);
    }
    IEnumerator attack()
    {
        do
        {
            yield return null;
            animEnd =_anim.isEnd();
            if (dashPlayer())
            {
                StartCoroutine(dashTimer());
                setState(State.move);
                continue;
            }
            if (movePlayer())
            {
                if (animEnd) {
                    setState(State.move);
                }
            }
            else if (animEnd)
            {
                setState(State.idle);
            }
        } while (!newState);
    }
}
