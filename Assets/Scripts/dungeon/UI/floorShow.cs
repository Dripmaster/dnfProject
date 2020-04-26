using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class floorShow : MonoBehaviour
{
    public Sprite[] sprites;
    Image image;
    int floorNum = 0; 
    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
    }
    public void setFloor() {
        image.sprite = sprites[++floorNum];
    }
}
