using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager: MonoBehaviour
{
    public static EffectManager instance;
    List<EffectScript> EffectList;
    List<bulletEffect> bulletList;
    List<GameObject> DagmageList;
    List<EffectScript> itemGainList;
    GameObject effectPrefab;
    GameObject bulletPrefab;
    GameObject damagePrefab;
    Transform effParent;
    Transform CamParent;

    Vector3 initPos;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
        CamParent = Camera.main.transform;
        effParent = GameObject.Find("EffectParent").transform;
        initPos = new Vector3(4.858f,-3.332f,10);
        if (itemGainList == null)
            itemGainList = new List<EffectScript>();
    }

    public void AddEffect(EffectScript e) {
        if(EffectList == null)
            EffectList = new List<EffectScript>();
        e.transform.parent = effParent;
        EffectList.Add(e);
    }

    public EffectScript getEffect(Transform t, bool isitemGain = false) {

        return getEffect(t.position,isitemGain);
    }

    public EffectScript getEffect(Vector2 v,bool isitemGain=false)
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
                e.transform.rotation = Quaternion.identity;
                break;
            }
        }
        if (effect == null)
        {
            if (effectPrefab == null)
                effectPrefab = Resources.Load<GameObject>("prefabs/Effect/Effect");
            effect = GameObject.Instantiate(effectPrefab, v, Quaternion.identity).GetComponent<EffectScript>();
            effect.gameObject.SetActive(false);
            AddEffect(effect);
        }
        if (isitemGain) {
            effect.transform.SetParent(CamParent,false) ;
            itemGainList.Add(effect);
            setListPosition();
        }
        return effect;
    }
    public void setListPosition() {
        Vector3 tempPos = initPos;
        foreach (var item in itemGainList)
        {
            item.transform.localPosition = tempPos;
            tempPos.y += 0.5f;
        }
    }
    public void popitemGain(EffectScript e) {
        e.transform.parent = effParent;
        itemGainList.Remove(e);
    }

    public void AddBullet(bulletEffect e)
    {
        if (bulletList == null)
            bulletList = new List<bulletEffect>();
        bulletList.Add(e);
        e.transform.parent = effParent;
    }


    public bulletEffect getbullet(Transform t)
    {

        return getbullet(t.position);
    }

    public bulletEffect getbullet(Vector2 v)
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
                e.transform.rotation = Quaternion.identity;
                break;
            }
        }
        if (effect == null)
        {
            if (bulletPrefab == null)
                bulletPrefab = Resources.Load<GameObject>("prefabs/dungeon/bullet");
            effect = GameObject.Instantiate(bulletPrefab, v, Quaternion.identity).GetComponent<bulletEffect>();
            effect.gameObject.SetActive(false);
            AddBullet(effect);
        }

        return effect;
    }

    public void AddDamage(float atkPoint, Vector2 pos, RectTransform damageTextGen)
    {
        if (DagmageList == null)
            DagmageList = new List<GameObject>();
        GameObject g = null;
        foreach (GameObject e in DagmageList)
        {
            if (e.gameObject.activeInHierarchy == false)
            {
                g = e;
                break;
            }
        }
        if (g == null)
        {
            if (damagePrefab == null)
                damagePrefab = Resources.Load<GameObject>("prefabs/ui/DamageText");
            g = GameObject.Instantiate(damagePrefab, pos, Quaternion.identity);
        }
        g.transform.SetParent(damageTextGen.parent, false);
        g.GetComponent<Text>().text = atkPoint.ToString("0");
        g.transform.localPosition = damageTextGen.localPosition;
        g.gameObject.SetActive(true);
        DagmageList.Add(g);
    }

}
