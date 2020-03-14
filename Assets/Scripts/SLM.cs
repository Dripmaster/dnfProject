using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SLM//Sprite Load Manager
{
    static public string animPathFormat = "image/{0}/{1}/{2}/{3}";//0:경로 1:state 2:방향 3:anim번호
    static public string animPathInitFormat = "image/{0}/{1}/{2}";//0:경로 1:state 2:방향
    static public string animPathFormat_NONE = "image/{0}/{1}";//0:경로 1:anim번호

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
    static public Sprite getSpr(string path) {
        Sprite s = null;
        if(isSpr(path))
        s = _cache[path];
        return s;
    }
    static public void clearDic() {
        
    }
    static public bool isSpr(string path) {
        return _cache.ContainsKey(path);
    }
    static public float countSprite(string path)
    {
        float i = 0;
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
