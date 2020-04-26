using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class potionInfo : MonoBehaviour
{
    public bool isHeal;
    public Image myImage;
    Text myText;
    int myCount;
    // Start is called before the first frame update
    void Awake()
    {
        myText = GetComponent<Text>();
        if (isHeal)
        {
            playerDataManager.instance.setPotionInfo(this, 0);
        }
        else { 
            playerDataManager.instance.setPotionInfo(this, 1);
        }
    }
    public void setItemCount(int v) {
        myCount = v;
        myText.text = myCount+"";
        if (myCount <= 0)
        {
            myImage.color = new Color(0.6f, 0.6f, 0.6f, 1);
        }
        else {
            myImage.color = new Color(1, 1, 1, 1);
        }
    }
}
