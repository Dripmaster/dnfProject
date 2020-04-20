using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEvent : MonoBehaviour
{
    public Sprite hoverImage;
    public
    Sprite originImage;
    GameObject mark;

    private IEnumerator markCoroutine;
    Vector3 originMarkPos;

    // Start is called before the first frame update
    void Awake()
    {
        originImage = gameObject.GetComponent<SpriteRenderer>().sprite;
        mark = transform.Find("mark") ? transform.Find("mark").gameObject : null;
        if (mark != null)
            originMarkPos = mark.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseEnter()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = hoverImage;
        if (mark != null)
        {
            markCoroutine = RunMarkWave(0.25f, 0.1f);
            StartCoroutine(markCoroutine);
        }
    }
    private void OnMouseExit()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = originImage;
        if (mark != null)
        {
            mark.transform.position = originMarkPos;
            StopCoroutine(markCoroutine);
        }
    }

    IEnumerator RunMarkWave(float speed, float distance)
    {
        Transform markTransform = mark.transform;
        float frame = 30;
        float upTarget = markTransform.position.y + distance;
        float originPosY = markTransform.position.y;
        bool isUp = true;

        while (true)
        {
            if (isUp)
                markTransform.position = new Vector2(markTransform.position.x, markTransform.position.y + (distance / (speed * frame)));
            else
                markTransform.position = new Vector2(markTransform.position.x, markTransform.position.y - (distance / (speed * frame)));

            if (isUp && markTransform.position.y >= upTarget)
            {
                isUp = false;
            }
            if (!isUp && markTransform.position.y <= originPosY)
                isUp = true;

            yield return new WaitForSeconds(speed / (speed * frame));
        }
    }
}
