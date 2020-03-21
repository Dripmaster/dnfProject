using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class playerFSM : FSMbase
{
    public float attackRange = 1.1f;
    public float attackAngle = 50f;
    public float moveSpeed = 5;
    Vector2 attackfan;//에디터전용
    float speedRate;
    int atkNum;
    int degree;
    Rigidbody2D RBD;
    bool animEnd;
    public float checkDashTime = 0.75f;//대쉬 가능 시간
    public float dashRate = 500;
    float checkDashTimeTemp;
    int canDash = 0;//dash페이즈 0=기본, 1=처음 눌림, 2=뗌, 3= 다시 눌림(대쉬시작)
    bool dashState = false;
    KeyCode downKey;
    Vector2 dashDir;
    GameObject[] dashEffects;
    int movecount = 0;
    // Use this for initialization
    void Awake()
    {
        base.Awake();
        dashEffects = GameObject.FindGameObjectsWithTag("dashPaticles");
        foreach (GameObject g in dashEffects)
            g.SetActive(false);
        dashDir = new Vector2(0, 0);
        speedRate = 100;
        atkNum = 0;
        checkDashTimeTemp = checkDashTime;
        setState(State.idle);
        RBD = GetComponent<Rigidbody2D>();
        for (int i = 1; i < 4; i++) {
            _anim.initAnims("attack/" + i);
        }
        attackfan = new Vector2(0,-1);
    }
    void Update() {
        dashCount();
    }
    void dashEffect(int i) {
        dashEffects[i].SetActive(true);
        
        dashEffects[i].transform.position = new Vector2(transform.position.x,transform.position.y-0.457f);
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
                downKey = KeyCode.LeftArrow;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                canDash = 1;
                checkDashTimeTemp = checkDashTime;
                downKey = KeyCode.RightArrow;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                canDash = 1;
                checkDashTimeTemp = checkDashTime;
                downKey = KeyCode.UpArrow;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                canDash = 1;
                checkDashTimeTemp = checkDashTime;
                downKey = KeyCode.DownArrow;
            }
        }
        else if (canDash == 1) {
            if (Input.GetKeyUp(downKey))
            {
                canDash = 2;
            }
        }
        else if (canDash == 2)
        {
            if (Input.GetKeyDown(downKey))
            {
                canDash = 3;
                dashDir.x = 0;
                dashDir.y = 0;
                movecount = 0;
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    movecount++;
                    dashDir.x += -1;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    dashDir.x += 1;
                    movecount++;
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    dashDir.y += 1;
                    movecount++;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    dashDir.y += -1;
                    movecount++;
                }
                return true;
            }
        }
        return false;
    }
    bool movePlayer()
    {
        Vector2 moveDir = new Vector2(0, 0);
        if (!dashState)
            movecount = 0;
        int t = movecount;
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir.x += -1;
            movecount++;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movecount++;
            moveDir.x += 1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movecount++;
            moveDir.y += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movecount++;
            moveDir.y += -1;
        }
        
        

        if ((moveDir != Vector2.zero || dashState) && objectState!=State.attack)
        {
            if (dashState)
                moveDir = dashDir;

            degree = Mathf.RoundToInt((Mathf.Atan2(moveDir.y, moveDir.x) / Mathf.PI * 180f - 180) * -1) / 45;
            _anim.setDir(degree);
            attackfan = moveDir;


            if (!dashState)
            {
                // RBD.velocity = moveDir * moveSpeed * speedRate / 100;
                //transform.Translate(moveDir * moveSpeed* speedRate / 100 * Time.deltaTime);
                if (movecount >= 2)
                {
                    RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * speedRate / 100 / Mathf.Sqrt(2) * Time.deltaTime);
                }
                else
                    RBD.MovePosition((Vector2)transform.position + moveDir * moveSpeed * speedRate / 100 * Time.deltaTime);
            }
            else
            {
                movecount = t;
                if (t >= 2)
                {
                    RBD.MovePosition((Vector2)transform.position + dashDir * moveSpeed * dashRate / 100 / Mathf.Sqrt(2) * Time.deltaTime);
                }
                else
                    RBD.MovePosition((Vector2)transform.position + dashDir * moveSpeed * dashRate / 100 * Time.deltaTime);
            }
            //RBD.velocity = dashDir * moveSpeed * dashRate / 100;
            return true;
        }
        else
        {
            RBD.velocity = Vector2.zero;
        }
        return false;
    }
    bool attackInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            atkNum++;
            if (atkNum > 3)
                atkNum = 1;
            RBD.velocity = Vector2.zero;
            return true;
        }
        atkNum = 1;
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
        Vector2 tempPos = transform.position;
        
        for(int i= 0; i < 5; i++) { 
                dashEffect(i);
            yield return new WaitForSeconds(0.02f);
            
        }
        dashDir = Vector2.zero;
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
            if (!dashState&&attackInput())
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
        RBD.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        _anim.speed = 0.5f;
        do
        {
            yield return null;
            animEnd =_anim.isEnd();
            
            if (dashPlayer())
            {
                StartCoroutine(dashTimer());
                setState(State.move);
                break;
            }
            //movePlayer(); 공격중방향전환
            if (animEnd)
            {
                if (attackInput())
                    setState(State.attack, atkNum);
                else
                    setState(State.idle);
            }
            if (_anim.isEnd(_anim.sprLength / 2)) {
                //공격

            }
        } while (!newState);
        _anim.speed = 1;
        RBD.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    private void OnDrawGizmos()
    {
        Handles.color = new Color(0,0,255,0.2f);
        Handles.DrawSolidArc(transform.position, new Vector3(0,0,1), attackfan, attackAngle / 2, attackRange);
        Handles.DrawSolidArc(transform.position, new Vector3(0,0,1), attackfan, -attackAngle / 2, attackRange);

    }
}
