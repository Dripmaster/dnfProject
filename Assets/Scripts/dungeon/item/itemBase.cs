using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemBase : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator _anim;
    bool targetOn = false;
    float velocity = 8f;
    float velo;
    float tempY;
    float veloX;
    bool downs = true;
    itemManager.itemType type;
    private void OnEnable()
    {
        targetOn = false;
        downs = true;
        veloX = Random.Range(-1f, 1f);
        velo = velocity;
        tempY = transform.position.y;
    }
    void inPlayer()
    {
        _anim.SetTrigger("reset");
        gameObject.SetActive(false);
        
    }
    private void Update()
    {

        if (!targetOn&&!downs&&Vector2.Distance(playerFSM.instance.transform.position, transform.position) < 2)
        {
            targetOn = true;
        }
        if (targetOn)
        {
            moveToPlayer();
        }
        if (downs)
        {
            velo -= 0.5f;
            transform.Translate(new Vector2(veloX * Time.deltaTime, velo * Time.deltaTime));
            if (velo < 0 && transform.position.y - tempY <= 0.1f)
            {
                downs = false;
                if (type != itemManager.itemType.gold) {
                    targetOn = true;
                }
            }
        }
    }
    void moveToPlayer()
    {
        transform.position = Vector2.Lerp(transform.position, playerFSM.instance.transform.position, Time.deltaTime * 8f);
        if (Vector2.Distance(playerFSM.instance.transform.position, transform.position) <= 0.5f)
        {
            inPlayer();
            itemEvent();
        }
    }
    public void itemEvent() {
        itemManager.instance.itemEvent(type);
    }
    public void setAnim(itemManager.itemType itemType) {
        type = itemType;
        switch (itemType) {
            case itemManager.itemType.gold:_anim.SetTrigger("goldAnim");break;
            case itemManager.itemType.darkMat:_anim.SetTrigger("darkMat");break;
            case itemManager.itemType.fireMat:_anim.SetTrigger("fireMat");break;
            case itemManager.itemType.glowMat:_anim.SetTrigger("glowMat");break;
            case itemManager.itemType.grassMat: _anim.SetTrigger("grassMat");break;
            case itemManager.itemType.waterMat: _anim.SetTrigger("waterMat");break;
            default:break;
        }
    
    }
}
