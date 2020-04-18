using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
[System.Serializable]
public class tileSet
{

    public int type; // bigSword, sowrd, Hammer, long short boss
    public string name = "dark"; // bigSword, sowrd, Hammer, dark, fire, glow, grass, water
    public Vector2 pos;
    public int id;
}
public class mapMaker : MonoBehaviour
{
    public GameObject tilePrefab;

    
    [System.Serializable]
    public class TileList
    {
        public int MapNum = 1;
        public int floorNum = 1;
        public string prefabName="";
        public List<tileSet> map = new List<tileSet>();

        public void  addTile(tileSet t) {
            map.Add(t);
        }
    }

    [System.Serializable]
    public class SerializedLIST
    {
        public List<TileList> maps = new List<TileList>();
    }

    TileList tileList;
    SerializedLIST MapList;
    int idex = 0;
    public int mapNum = 1;
    public int floorNum = 1;
    GameObject currentMap;
    tileSet currentTile;
    // Start is called before the first frame update
    void Awake()
    {
        MapList = new SerializedLIST();
        tileList = new TileList();
        currentTile = new tileSet();
        print("--------------------------저장:space---------------------------------------");
        loadTileList();
        
    }
    void Update()
    {
        setTileToObject();
        saveKeyInput();
        /*
        keyInput();
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = CastRay();
            if (target != null)
            {
                if (target.tag == "customTile")
                {
                    currentTile = tileList.map[int.Parse(target.name)];
                    print("몬스터 속성 변경: [d]ark, [f]ire, [g]low, gras[s], [w]ater");
                    print("몬스터 종류 변경: [l]ong, shor[t], [b]oss");
                    print("방향키로 해당 몬스터 이동");
                    print("해당 몬스터 지우기 : [x]");
                }
                else if (target.tag == "wall")
                {
                    print("여긴 벽입니다.");
                }
                else if (target.tag == "floor")
                {
                    currentTile = new tileSet();
                    target = Instantiate(tilePrefab, pos, Quaternion.identity);
                    target.name = tileList.map.Count.ToString();
                    target.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("image/enemy/water/long/move/6/0");
                    print("몬스터 속성을 지정해 주세요: [d]ark, [f]ire, [g]low, gras[s], [w]ater");
                    print("몬스터 종류를 지정해 주세요: [l]ong, shor[t], [b]oss");
                    print("Enter 키 입력 시 지정된 몬스터가 추가됩니다.");
                    print("방향키로 해당 몬스터 이동");
                    print("해당 몬스터 지우기 : [x]");
                }
            }
            else {
                print("다시 선택 바랍니다.");
            }
            
        }
        */
    }
    void saveKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            saveTileList();
            loadTileList();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            loadTileList();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            saveTileList();
            mapNum--;
            loadTileList();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            saveTileList();
            mapNum++;
            loadTileList();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            saveTileList();
            floorNum--;
            loadTileList();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            saveTileList();
            floorNum++;
            loadTileList();
        }
    }
    /*
    void keyInput() {
        if (Input.GetKeyDown(KeyCode.D)) {
            currentTile.name = "dark";
            setTileToObject();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            currentTile.name = "fire";
            setTileToObject();

        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            currentTile.name = "glow";
            setTileToObject();

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentTile.name = "grass";
            setTileToObject();

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentTile.name = "water";
            setTileToObject();

        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            currentTile.type = 4;
            setTileToObject();

        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentTile.type = 5;
            setTileToObject();

        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            currentTile.type =6;
            setTileToObject();

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Destroy(target);
            target = null;
            if(currentTile.id < tileList.map.Count)
            tileList.map.RemoveAt(currentTile.id);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            currentTile.id = tileList.map.Count;
            currentTile.pos = target.transform.position;
            if(tileList.map.Count<=currentTile.id)
                tileList.map.Add(currentTile);
        }
    }

        
    Vector2  CastRay()
    {
        target = null;
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider != null)
        {
            target = hit.collider.gameObject;
        }
        return pos;
    }
    */
    void setTileToObject() {
        foreach (var item in GameObject.FindGameObjectsWithTag("customTile"))
        {
            string[] nametype = item.name.ToLower().Split(',');
            try
            {
                item.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("image/enemy/" + nametype[0] + "/" + nametype[1] + "/move/6/0");
            }
            catch
            {
                print("image/enemy/" + nametype[0] + "/" + nametype[1] + "/move/6/0");
            }
        }
        
    }
    void initMap() {
        int i = 0;

        foreach (GameObject t in GameObject.FindGameObjectsWithTag("customTile"))
        {
            Destroy(t);
        }
        foreach (tileSet t in tileList.map) {
            GameObject g = Instantiate(tilePrefab, t.pos, Quaternion.identity);
            g.transform.SetParent(GameObject.Find("LoadedObject").transform);
            t.id = i++;
            g.name = t.name+","+Enum.GetName(typeof(type),t.type);
        }
    }

    void saveTileList() {
        TileList tileList = new TileList();
        int count = 0;
        foreach (var item in GameObject.FindGameObjectsWithTag("customTile"))
        {
            string[] nametype = item.name.Split(',');
            tileSet t = new tileSet();
            t.name = nametype[0];
            t.type = (int)Enum.Parse(typeof(type),nametype[1]);
            t.pos = item.transform.position;
            t.id = count++;
            tileList.addTile(t);
        }
        if (tileList.MapNum != mapNum || tileList.floorNum != floorNum)
        {
            bool chk = false;
            for (int i = 0; i < MapList.maps.Count; i++)
            {
                if (MapList.maps[i].floorNum == floorNum && MapList.maps[i].MapNum == mapNum)
                {
                    idex = i;
                    chk = true;
                    break;
                }
            }
            if (!chk) {
                idex = MapList.maps.Count;
            }
            tileList.MapNum = mapNum;
            tileList.floorNum = floorNum;
        }
        if (currentMap.activeInHierarchy == false) {
            currentMap = GameObject.FindGameObjectWithTag("map");
        }
        tileList.prefabName = currentMap.name;
        if (MapList.maps.Count <=idex)
        {
            MapList.maps.Add(tileList);
        }
        else {
            MapList.maps[idex] = tileList;
        }
        string str = JsonUtility.ToJson(MapList);
        print(str);
        print("save 완료");
        
        File.WriteAllText(Application.dataPath + "/MapData/data.json", str);
    }

    void loadTileList() {
        string jsonString = File.ReadAllText(Application.dataPath + "/MapData/data.json");
        MapList = JsonUtility.FromJson<SerializedLIST>(jsonString);
        tileList = null;
        currentMap = GameObject.FindGameObjectWithTag("map");
        if (MapList != null)
        {
            for (int i = 0; i < MapList.maps.Count; i++)
            {
                if (MapList.maps[i].floorNum == floorNum && MapList.maps[i].MapNum == mapNum)
                {
                    idex = i;
                    tileList = MapList.maps[i];
                    print("load success : " + mapNum + "번째 맵 " + floorNum + "층");
                    if (currentMap.name != tileList.prefabName) {
                        GameObject m = null;
                        try
                        {
                            m = Instantiate(Resources.Load<GameObject>("prefabs/mapMaker/maps/" + tileList.prefabName), Vector2.zero, Quaternion.identity);
                            m.name = tileList.prefabName;
                            currentMap.SetActive(false);
                        }
                        catch
                        {
                            m = GameObject.FindGameObjectWithTag("map");
                            print("prefabLoadError :"+tileList.prefabName);
                        }
                        finally
                        {
                            currentMap = m;

                        }
                    }
                    break;
                }
            }
        }
        else {
            MapList = new SerializedLIST();
            print("new map : " + mapNum + "번째 맵 " + floorNum + "층");
        }
        if (tileList == null) {
            idex = MapList.maps.Count;
            tileList = new TileList();
            print("new map : " + mapNum + "번째 맵 " + floorNum + "층");

        }
        initMap();
    }
}
