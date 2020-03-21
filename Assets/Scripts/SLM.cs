using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SLM//Sprite Load Manager
{
    static public string animPathFormat = "image/{0}/{1}/{2}/{3}";//0:경로 1:state 2:방향 3:anim번호
    static public string animPathInitFormat = "image/{0}/{1}/{2}";//0:경로 1:state 2:방향
    static public string animPathFormat_NONE = "image/{0}/{1}";//0:경로 1:anim번호
    static public string animPathInitFormat_NONE = "image/{0}";//0:경로

    static private Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();
    
    static public void Load(string[] path)
    {
        for (int i = 0; i < path.Length; i++) {
            Sprite[] objs = Resources.LoadAll<Sprite>(path[i]);
            if (objs.Length == 0)
                continue;
            for (int j = 0; j < objs.Length; j++) {
                _cache[path[i] + "/"+objs[j].name] = objs[j];
                
            }
        }
    }
    static public void Load(string path)
    {
        Sprite[] objs = Resources.LoadAll<Sprite>(path);
        if (objs.Length == 0)
            return;
        for (int j = 0; j < objs.Length; j++)
        {
            _cache[path + "/" + objs[j].name] = objs[j];

        }
    }
    static public Sprite getSpr(string path) {
        if (isSpr(path)) {
            if(_cache[path] == null)
            {
                _cache[path] = Resources.Load<Sprite>(path);
                Debug.Log("Reload");
            }
            return _cache[path];
        }

        return null;
    }
    static public void clearDic() {
        
    }
    static public bool isSpr(string path) {
        return _cache.ContainsKey(path);
    }
    static public int countSprite(string path)
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
