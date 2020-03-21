using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTwinkle : MonoBehaviour
{
    public float twinkleTime;
    public int twinkleCount;
    public float showTime;
    public float delay;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        StartCoroutine("RunTwinkle");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator RunTwinkle()
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        while (true)
        {
            int count = 0;
            while (count < twinkleCount)
            {
                yield return new WaitForSeconds(twinkleTime);
                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.enabled = true;
                count += 1;
            }

            yield return new WaitForSeconds(showTime);
        }
    }
}
