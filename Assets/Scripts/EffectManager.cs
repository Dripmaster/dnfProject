using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectManager
{
    static List<EffectScript> EffectList;
    static List<bulletEffect> bulletList;
    static GameObject effectPrefab;
    static GameObject bulletPrefab;


    public static void AddEffect(EffectScript e) {
        if(EffectList == null)
            EffectList = new List<EffectScript>();
        EffectList.Add(e);
    }


    public static EffectScript getEffect(Transform t) {

        return getEffect(t.position);
    }

    public static EffectScript getEffect(Vector2 v)
    {
        if (EffectList == null)
            EffectList = new List<EffectScript>();
        EffectScript effect = null;

        foreach (EffectScript e in EffectList)
        {
            if (e.gameObject.activeInHierarchy == false)
            {
                
                effect = e;
                e.transform.position = v;
                break;
            }
        }
        if (effect == null)
        {
            if (effectPrefab == null)
                effectPrefab = Resources.Load<GameObject>("prefabs/Effect");
            effect = GameObject.Instantiate(effectPrefab, v, Quaternion.identity).GetComponent<EffectScript>();
            effect.gameObject.SetActive(false);
            AddEffect(effect);
        }
        
        return effect;
    }


    public static void AddBullet(bulletEffect e)
    {
        if (bulletList == null)
            bulletList = new List<bulletEffect>();
        bulletList.Add(e);
    }


    public static bulletEffect getbullet(Transform t)
    {

        return getbullet(t.position);
    }

    public static bulletEffect getbullet(Vector2 v)
    {
        if (bulletList == null)
            bulletList = new List<bulletEffect>();
        bulletEffect effect = null;

        foreach (bulletEffect e in bulletList)
        {
            if (e.gameObject.activeInHierarchy == false)
            {

                effect = e;
                e.transform.position = v;
                break;
            }
        }
        if (effect == null)
        {
            if (bulletPrefab == null)
                bulletPrefab = Resources.Load<GameObject>("prefabs/bullet");
            effect = GameObject.Instantiate(bulletPrefab, v, Quaternion.identity).GetComponent<bulletEffect>();
            effect.gameObject.SetActive(false);
            AddEffect(effect);
        }

        return effect;
    }
}
