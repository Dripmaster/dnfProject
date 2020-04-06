using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    GameObject blackBg;
    GameObject whiteBg;



    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeScene(string scene, int type, float speed)
    {
        StartCoroutine(RunSceneFadeOut(scene, type, speed));
    }

    public void StartScene(int type, float speed, float delay = 0f, System.Action callback = null)
    {
        StartCoroutine(RunSceneFadeIn(type, speed, delay, callback));
    }

    IEnumerator RunSceneFadeOut(string scene, int type, float speed)
    {
        GameObject bg;
        if (type >= 1)
            bg = transform.Find("black").gameObject;
        else
            bg = transform.Find("white").gameObject;
        bg.SetActive(true);

        SpriteRenderer spriteRenderer = bg.GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = 0;
        spriteRenderer.color = color;

        float frame = 20;
        float diffColorA = 1 / (speed * frame);
        float waitSecond = speed / (speed * frame);

        while (color.a < 1f)
        {
            color.a += diffColorA;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(waitSecond);
        }

        SceneManager.LoadScene(scene);
    }
    IEnumerator RunSceneFadeIn(int type, float speed, float delay, System.Action callback)
    {
        GameObject bg;
        if (type >= 1)
            bg = transform.Find("black").gameObject;
        else
            bg = transform.Find("white").gameObject;

        bg.SetActive(true);
        SpriteRenderer spriteRenderer = bg.GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = 1;
        spriteRenderer.color = color;

        if (delay > 0f)
            yield return new WaitForSeconds(delay);


        float frame = 20;
        float diffColorA = color.a / (speed * frame);
        float waitSecond = speed / (speed * frame);


        while (color.a > 0.0f)
        {
            color.a -= diffColorA;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(waitSecond);
        }

        bg.SetActive(false);
        if (callback != null)
            callback.Invoke();
    }
}