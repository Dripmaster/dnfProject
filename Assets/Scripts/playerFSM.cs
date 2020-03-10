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

    // Use this for initialization
    void Awake()
    {
        base.Awake();
        speedRate = 100;
        hp = maxHP;
        atkNum = 0;
        setState(State.idle);
    }

    // Update is called once per frame
    void Update()
    {
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
        if (objectState == State.attack)
            speedRate = 10;
        else
            speedRate = 100;

        if (moveDir != Vector2.zero)
        {
            GetComponent<Rigidbody2D>().velocity = moveDir * moveSpeed * speedRate / 100;

            int degree = Mathf.RoundToInt((Mathf.Atan2(moveDir.y,moveDir.x)/Mathf.PI*180f -180 )* -1)/45;
            _anim.setDir(degree.ToString());

            return true;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = moveDir;
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

    IEnumerator move()
    {
        do
        {
            yield return null;
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
            bool end =_anim.isEnd();
            if (movePlayer())
            {
                if (end) {
                    setState(State.move);
                }
            }
            else if (end)
            {
                setState(State.idle);
            }
        } while (!newState);
    }
}
