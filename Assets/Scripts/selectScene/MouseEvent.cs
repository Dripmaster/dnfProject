using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEvent : MonoBehaviour
{
    public Sprite hoverImage;
    Sprite originImage;

    // Start is called before the first frame update
    void Awake()
    {
        originImage = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseEnter()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = hoverImage;
    }
    private void OnMouseExit()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = originImage;
    }
}
