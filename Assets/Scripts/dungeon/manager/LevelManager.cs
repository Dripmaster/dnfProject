using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    mapMaker.SerializedLIST MapList;
    mapMaker.TileList tileList;
    public int mapNum;
    public int floorNum;
    public GameObject enemyPrefab;
    bool isError = false;
    List<GameObject> mapObject;
    public GameObject playerPrefab;
    int currentMap = 0;
    public floorShow floorShow;
    void Awake()
    {
        instance = this;
        mapObject = new List<GameObject>();
        loadTileList();
        initmap();
        loadSprite();
        DamageReceiver.init(enemyPrefab);
        setEnemy();

    }
    void loadSprite() {
        foreach (var typeItem in Enum.GetNames(typeof(type)))
        {
            foreach (var stateItem in Enum.GetNames(typeof(State)))
            {
                for (int i = 0; i < 8; i++)
                {
                    SLM.instance.Load(string.Format(SLM.instance.animPathInitFormat, "enemy/" + typeItem, stateItem, i));
                }
            }
        }
       
    }
    private void OnEnable()
    {
        Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
    }
    public void checkEnemy() {
        if (DamageReceiver.isEnemyRemain() == false && mapChangeFrame())
        {
            floorNum++;
            loadMap();
            if (!isError)
            {
                StartCoroutine(pause());
            }
        }
    }
    IEnumerator pause() {
        
        yield return new WaitForSeconds(2.0f);
        playerFSM.instance.playerFreeze();
        yield return new WaitForSeconds(0.5f);
        currentMap++;
        Physics2D.IgnoreLayerCollision(8, 11);
        do {
            yield return null;
        } while (!mapChangeFrame());
        floorShow.setFloor();
        Physics2D.IgnoreLayerCollision(8, 11, false);
        playerFSM.instance.playerFreeze(false);
        setEnemy();
        mapObject[currentMap - 1].SetActive(false);
    }
    public GameObject getCurrentMap() {
        return mapObject[currentMap];
    }
    bool mapChangeFrame() {
        if (Vector3.zero == mapObject[currentMap].transform.position)
            return true;
        Vector3 moveDir = Vector3.Lerp(mapObject[currentMap].transform.position,Vector2.zero,Time.deltaTime*2.5f);

        for (int i = 0; i < mapObject.Count; i++)
        {
            if (i != currentMap) { 
        mapObject[i].transform.position += moveDir - mapObject[currentMap].transform.position;

            }
        }
        mapObject[currentMap].transform.position = moveDir;

        if (Vector2.Distance(Vector2.zero, mapObject[currentMap].transform.position) <= 0.1f)
        {
            mapObject[currentMap].transform.position = Vector2.zero;
            return true;
        }
        return false;

    }
    void setEnemy()
    {
        if (isError)
            return;

        DamageReceiver.setEnemy(tileList,enemyPrefab);
    }
    void initmap()
    {
        if (MapList != null)
        {
            for (int i = 0; i < MapList.maps.Count; i++)
            {
                if (MapList.maps[i].MapNum == mapNum)
                {
                    int mapX,mapY;
                    if (((MapList.maps[i].floorNum-1) / 3) % 2 == 0)
                    {
                        mapX = ((MapList.maps[i].floorNum - 1) % 3) * 20;
                    }
                    else { 
                        mapX = 40 + ((MapList.maps[i].floorNum - 1)%3) * -20;
                    }
                    mapY = ((MapList.maps[i].floorNum - 1) / 3)*20;
                    try
                    {
                        mapObject.Add(Instantiate(Resources.Load<GameObject>("prefabs/mapMaker/maps/" + MapList.maps[i].prefabName), new Vector2(mapX, mapY), Quaternion.identity));
                    }
                    catch
                    {
                        print("prefabs/mapMaker/maps/" + MapList.maps[i].prefabName);
                        print(MapList.maps[i].floorNum + "F :" + mapX + "," + mapY);
                    }
                }
            }
        }
        foreach (var item in mapObject)
        {
            item.transform.parent = GameObject.Find("MapParent").transform;
        }
        loadMap();
    }
    void loadMap() {
        tileList = null;
        foreach (var item in MapList.maps)
        {
            if (item.floorNum == floorNum && item.MapNum == mapNum)
                tileList = item;
        }
        if (tileList == null)
        {
            print("load Error : " + mapNum + "번째 맵 " + floorNum + "층");
            isError = true;
        }
        foreach (var item in GameObject.FindGameObjectsWithTag("wall"))
        {
            item.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void loadTileList()
    {
        string jsonString = File.ReadAllText(Application.dataPath + "/MapData/data.json");
        MapList = JsonUtility.FromJson<mapMaker.SerializedLIST>(jsonString);
        tileList = null;
    }
}
