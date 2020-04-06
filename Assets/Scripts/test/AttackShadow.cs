using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackShadow : MonoBehaviour
{
    SpriteRenderer sr;
    SpriteRenderer parent;


    public float delay;
    public float speed;
    public Vector2 startScale;

    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.enabled = false;
        parent = transform.parent.GetComponent<SpriteRenderer>();
        print(parent.sprite);
    }

    // Update is called once per frame
    void Update()
    {
        if (sr.enabled == false && Input.GetKeyDown(KeyCode.X))
        {
            sr.sprite = parent.sprite;
            sr.color = new Color(255, 255, 255, 0.3f);
            sr.transform.localScale = new Vector3(startScale.x, startScale.y, 1); // custom 가능하게
            sr.enabled = true;
            StartCoroutine("RunEffect");
        }
    }

    IEnumerator RunEffect()
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        float frame = 30;
        //float speed = 0.05f;

        Vector3 scale = sr.transform.localScale;
        Color color = sr.color;
        float diffScale = (startScale.x - 1.0f) / (speed * frame);
        float waitSecond = speed / (speed * frame);


        while (scale.x > 1f)
        {
            scale.x -= diffScale;
            scale.y -= diffScale;
            sr.transform.localScale = scale;
            yield return new WaitForSeconds(waitSecond);
        }

        sr.enabled = false;
    }
}
