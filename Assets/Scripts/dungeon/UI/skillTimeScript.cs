using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skillTimeScript : MonoBehaviour
{
    public Image fg;
    public Text txt;
    float maxTime;
    float currentTime;
    bool currentCool;
    int intTime;
    // Start is called before the first frame update
    void Awake()
    {
        currentCool = true;
    }
    public void startCool(float max) {
        currentCool = false;
        maxTime = max;
        intTime = (int)maxTime;
        currentTime = maxTime;

        txt.text = intTime.ToString();
        fg.fillAmount = 1;
        StartCoroutine(coolCount());
    }
    IEnumerator coolCount() {
        do
        {
            currentTime -= Time.deltaTime;
            fg.fillAmount = currentTime / maxTime;
            if (intTime != (int)currentTime) {
                intTime = (int)currentTime;
                txt.text = intTime.ToString();
            }
            yield return null;
        } while (currentTime>=0);
        currentCool = true;
        txt.text = "";
    }
    public bool getCool() {
        return currentCool;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
