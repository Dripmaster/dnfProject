using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitSelectScene : MonoBehaviour
{
    public GameObject grass;
    public GameObject fire;
    public GameObject water;
    public GameObject glory;
    public GameObject dark;

    // Start is called before the first frame update
    void Start()
    {
        SceneChangeManager sceneChangeManager = GameObject.Find("SceneManager").GetComponent<SceneChangeManager>();
        sceneChangeManager.HideScene(1);

        //playerDataManager.instance.setMapProgress(mapType.miniWater, 10); //치트키 

        int miniGrassFloor = playerDataManager.instance.getMapProgress(mapType.miniGrass);
        int miniFireFloor = playerDataManager.instance.getMapProgress(mapType.miniFire);
        int miniWaterFloor = playerDataManager.instance.getMapProgress(mapType.miniWater);
        int grassFloor = playerDataManager.instance.getMapProgress(mapType.grass);
        int fireFloor = playerDataManager.instance.getMapProgress(mapType.fire);
        int waterFloor = playerDataManager.instance.getMapProgress(mapType.water);


        if (miniGrassFloor >= 10) // grass 던전 언락 조건 
        {
            grass.transform.Find("grass").gameObject.SetActive(true);
            grass.transform.Find("lockGrass").gameObject.SetActive(false);
        }

        if (miniFireFloor >= 10) // fire 던전 언락 조건 
        {
            fire.transform.Find("fire").gameObject.SetActive(true);
            fire.transform.Find("lockFire").gameObject.SetActive(false);
        }

        if (miniWaterFloor >= 10) // water 던전 언락 조건 
        {
            water.transform.Find("water").gameObject.SetActive(true);
            water.transform.Find("lockWater").gameObject.SetActive(false);
        }

        if (grassFloor >= 10 && fireFloor >= 10 && waterFloor >= 10) // dark 던전 언락 조건 
        {
            dark.transform.Find("dark").gameObject.SetActive(true);
            dark.transform.Find("lockDark").gameObject.SetActive(false);
        }

        if (grassFloor >= 10 && fireFloor >= 10 && waterFloor >= 10) // glory 던전 언락 조건 
        {
            glory.transform.Find("glory").gameObject.SetActive(true);
            glory.transform.Find("lockGlory").gameObject.SetActive(false);
        }
        sceneChangeManager.StartScene(1, 0.7f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
