using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SLM//Sprite Load Manager
{
    static private Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();
    
    static public void Load(string[] path)
    {
        for (int i = 0; i < path.Length; i++) {
            Sprite[] objs = Resources.LoadAll<Sprite>(path[i]);
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
    static public bool isSpr(string path) {
        return _cache.ContainsKey(path);
    }
}
