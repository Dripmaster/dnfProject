﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelManager : MonoBehaviour
{

    mapMaker.SerializedLIST MapList;
    mapMaker.TileList tileList;
    public int mapNum;
    public int floorNum;
    GameObject enemyPrefab;
    bool isError = false;
    GameObject[] mapObject;
    int currentMap = 0;
    // Start is called before the first frame update
    bool isPause;
    void Awake()
    {
        mapObject = GameObject.FindGameObjectsWithTag("map");
        enemyPrefab = Resources.Load<GameObject>("prefabs/enemy");
        loadTileList();
        loadMap();
        setEnemy();
        mapObject[1].transform.position = new Vector2(0,-20);
        isPause = false;
        //mapObject[1].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isError||isPause)
            return;
        if (DamageReceiver.isEnemyRemain()==false&& mapChangeFrame()) {
            floorNum++;
            loadMap();
            isPause = true;
            StartCoroutine(pause());
        }
    }
    IEnumerator pause() {
        yield return new WaitForSeconds(1.5f);
        currentMap = ++currentMap % 2;
        isPause = false;
    }
    bool mapChangeFrame() {
        if (Vector3.zero == mapObject[currentMap].transform.position)
            return true;
        Physics2D.IgnoreLayerCollision(8, 11);
        Vector3 moveDir = Vector3.Lerp(mapObject[currentMap].transform.position,Vector2.zero,Time.deltaTime*2.5f);

        mapObject[(currentMap+1)%2].transform.position += moveDir - mapObject[currentMap].transform.position;
        mapObject[currentMap].transform.position = moveDir;

        if (Vector2.Distance(Vector2.zero, mapObject[currentMap].transform.position) <= 0.1f)
        {
            mapObject[currentMap].transform.position = Vector2.zero;
            mapObject[(currentMap + 1) % 2].transform.position = new Vector2(0, -20);
            Physics2D.IgnoreLayerCollision(8, 11, false);
            setEnemy();
            return true;
        }
        return false;

    }
    void setEnemy()
    {
        if (isError)
            return;

        foreach (mapMaker.tileSet t in tileList.map)
        {
            EnemyFSM enemy = Instantiate(enemyPrefab, t.pos, Quaternion.identity).GetComponent<EnemyFSM>();
            enemy.setTypeName(t.type, t.name);
            enemy.gameObject.SetActive(true);
        }
    }
    void loadMap() {
        tileList = null;
        if (MapList != null)
        {
            for (int i = 0; i < MapList.maps.Count; i++)
            {
                if (MapList.maps[i].floorNum == floorNum && MapList.maps[i].MapNum == mapNum)
                {
                    tileList = MapList.maps[i];
                    Debug.Log("load success : " + mapNum + "번째 맵 " + floorNum + "층");

                    break;
                }
            }
        }
        if (tileList == null)
        {
            Debug.Log("load Error : " + mapNum + "번째 맵 " + floorNum + "층");
            isError = true;
        }
    }

    void loadTileList()
    {
        string jsonString = File.ReadAllText(Application.dataPath + "/MapData/data.json");
        MapList = JsonUtility.FromJson<mapMaker.SerializedLIST>(jsonString);
        tileList = null;
    }
}
