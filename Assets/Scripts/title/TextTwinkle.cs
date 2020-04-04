using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTwinkle : MonoBehaviour
{
    public float twinkleTime;
    public int twinkleCount;
    public float showTime;
    public float delay;

    bool canClick = false;

    private SpriteRenderer spriteRenderer;

    GameObject sceneFade;

    // Start is called before the first frame update
    void Awake()
    {
        sceneFade = GameObject.Find("fade");

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        StartCoroutine("RunTwinkle");
    }

    // Update is called once per frame
    void Update()
    {
        if (canClick && Input.anyKeyDown)
        {
            canClick = false;
            SceneChangeManager sceneChangeManager = GameObject.Find("SceneManager").GetComponent<SceneChangeManager>();
            sceneChangeManager.ChangeScene("tutorial", 1, 0.7f);
        }
    }

    IEnumerator RunTwinkle()
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        canClick = true;
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
