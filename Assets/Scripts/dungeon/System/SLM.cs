using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLM: MonoBehaviour//Sprite Load Manager
{
    public static SLM instance;
    public string animPathFormat = "image/{0}/{1}/{2}/{3}";//0:경로 1:state 2:방향 3:anim번호
    public string animPathInitFormat = "image/{0}/{1}/{2}";//0:경로 1:state 2:방향
    public string animPathFormat_NONE = "image/{0}/{1}";//0:경로 1:anim번호
    public string animPathInitFormat_NONE = "image/{0}";//0:경로

    private Dictionary<string, Sprite> _cache;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        _cache = new Dictionary<string, Sprite>();
    }
    public Sprite Load(string[] path)
    {
        Sprite s = null;
        for (int i = 0; i < path.Length; i++) {
            if (_cache.ContainsKey(path[i]+"/0")) {
                s = _cache[path[i] + "/0"];
                continue;
            }
            Sprite[] objs = Resources.LoadAll<Sprite>(path[i]);
            if (objs.Length == 0)
                continue;
            for (int j = 0; j < objs.Length; j++) {
                _cache[path[i] + "/"+objs[j].name] = objs[j];
            }
            if(s==null)
            s = objs[0];
        }
        return s;
    }
    public void Load(string path)
    {
        if (_cache.ContainsKey(path+ "/0"))
        {
            return;
        }
        Sprite[] objs = Resources.LoadAll<Sprite>(path);
        if (objs.Length == 0)
            return;
        for (int j = 0; j < objs.Length; j++)
        {
            _cache[path + "/" + objs[j].name] = objs[j];

        }
    }
    public Sprite getSpr(string path) {
        if (!isSpr(path))
        {
            _cache[path] = Resources.Load<Sprite>(path);
        }

        return _cache[path];
    }
    public void clearDic() {
        
    }

    public bool isSpr(string path) {
        return _cache.ContainsKey(path);
    }
    public int countSprite(string path)
    {
        int i = 0;
        while (true) {
            if (isSpr(path + "/" + i))
            {
                i++;
            }
            else
            {
                break;
            }
        }
        return i;
    }
}
