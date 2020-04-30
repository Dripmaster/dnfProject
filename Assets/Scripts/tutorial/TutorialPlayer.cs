using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayer : MonoBehaviour
{

    GameObject mark;
    GameObject moveTip;
    GameObject player;

    bool canMove = false;

    // Start is called before the first frame update
    void Awake()
    {
        mark = transform.Find("mark").gameObject;
        moveTip = GameObject.Find("move_tutorial").gameObject;
        //player = GameObject.Find("player").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && Input.anyKeyDown)
            gameObject.SetActive(false);
    }

    IEnumerator RunMarkUp()
    {
        SpriteRenderer spriteRenderer = mark.GetComponent<SpriteRenderer>();
        float frame = 30;

        float speed = 0.3f;
        Color color = spriteRenderer.color;
        float diffColorA = 1 / (speed * frame);
        float waitSecond = speed / (speed * frame);

        Vector3 pos = mark.transform.position;
        float diffPosY = 0.2f / (speed * frame);

        while (color.a < 1f)
        {
            pos.y = pos.y + diffPosY;
            mark.transform.position = pos;
            color.a += diffColorA;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(waitSecond);
        }
        yield return new WaitForSeconds(1.0f);


        speed = 0.2f;
        diffColorA = 1 / (speed * frame);
        waitSecond = speed / (speed * frame);
        while (color.a > 0f)
        {
            color.a -= diffColorA;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(waitSecond);
        }

        StartCoroutine("RunShowMoveTip");
    }

    IEnumerator RunShowMoveTip()
    {
        SpriteRenderer spriteRenderer = moveTip.GetComponent<SpriteRenderer>();
        float frame = 30;

        float speed = 0.2f;
        Color color = spriteRenderer.color;
        float diffColorA = 1 / (speed * frame);
        float waitSecond = speed / (speed * frame);

        Vector3 pos = moveTip.transform.position;
        float diffPosY = 0.1f / (speed * frame);



        while (color.a < 1f)
        {
            pos.y = pos.y + diffPosY;
            moveTip.transform.position = pos;
            color.a += diffColorA;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(waitSecond);
        }
        //player.GetComponent<playerFSM>().enabled = true;
        canMove = true;
    }
}
