using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public float speed;
    public float delay;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine("RunFadeIn");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator RunFadeIn()
    {
        float frame = 10;
        Color color = spriteRenderer.color;
        float diffColorA = 1 / (speed * frame);
        float waitSecond = speed / (speed * frame);

        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        while (color.a < 1f)
        {
            color.a += diffColorA;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(waitSecond);
        }
    }
}
