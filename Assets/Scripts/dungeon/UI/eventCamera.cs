using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
