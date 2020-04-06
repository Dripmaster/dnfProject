using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        SceneChangeManager sceneChangeManager = GameObject.Find("SceneManager").GetComponent<SceneChangeManager>();
        sceneChangeManager.StartScene(1, 0.7f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
