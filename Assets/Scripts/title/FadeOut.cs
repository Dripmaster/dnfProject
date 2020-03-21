using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public float speed;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator RunFadeOut()
    {
        float frame = 10;

        Color color = spriteRenderer.color;
        float diffColorA = color.a / (speed * frame);
        float waitSecond = speed / (speed * frame);


        while (color.a > 0.0f)
        {
            color.a -= diffColorA;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(waitSecond);
        }
    }
}
