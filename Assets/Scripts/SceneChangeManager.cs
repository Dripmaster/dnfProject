using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public GameObject loadMask;
    public GameObject backMask;
    public void ChangeScene(int scene, int type, float speed)
    {
        StartCoroutine(RunSceneFadeOut(scene, type, speed));
    }

    public void StartScene(int type, float speed, float delay = 0f, System.Action callback = null)
    {
        StartCoroutine(RunSceneFadeIn(type, speed, delay, callback));
    }

    public void HideScene(int type = 1)
    {
        GameObject bg;
        if (type == 0)
        {
            bg = transform.Find("white").gameObject;
        }
        else
            bg = transform.Find("black").gameObject;

        bg.SetActive(true);
        SpriteRenderer spriteRenderer = bg.GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = 1;
        spriteRenderer.color = color;
    }

    IEnumerator RunSceneFadeOut(int scene, int type, float speed)
    {
        AsyncOperation asyncOperation =
        SceneManager.LoadSceneAsync(scene);
        asyncOperation.allowSceneActivation = false;
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

        asyncOperation.allowSceneActivation = true;
        if (loadMask != null)
        {
            loadMask.SetActive(true);
            backMask.SetActive(true);
            do
            {
                loadMask.transform.localScale = new Vector2(asyncOperation.progress * 11f, 1);
                yield return null;
            } while (!asyncOperation.isDone);
        }
        else {
        }
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