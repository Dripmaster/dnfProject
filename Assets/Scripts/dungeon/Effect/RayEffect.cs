using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayEffect : MonoBehaviour
{
    public GameObject rayStart;
    public GameObject rayScale;
    public GameObject rayResult;
    public Transform rayDir;
    public Vector2 rayDirpos;
    float raySpeed = 10;
    bool isDone = false;
    bool isPlay = true;
    void Update()
    {
        if (!isPlay) {

            return;
        }
        if(rayDir!=null)
        rayDirpos = Vector2.Lerp(rayDirpos, rayDir.position, Time.deltaTime *1f);
        
        //레이캐스팅 결과정보를 hit라는 이름으로 정한다.
        RaycastHit2D hit;

        //레이캐스트 쏘는 위치, 방향, 결과값, 최대인식거리
        hit = Physics2D.Raycast(transform.position, rayDirpos);
        float Xscale = Mathf.SmoothStep(rayScale.transform.localScale.x, hit.distance, raySpeed * Time.deltaTime);
        if (hit.distance < Xscale) {
            Xscale = hit.distance;
        }
        if (hit.distance - Xscale <= 0.1f)
        {
            Xscale = hit.distance;
            Vector2 toPos = hit.point;
            rayResult.transform.position = toPos;
            if (rayResult.GetComponent<RayEffect>() != null)
            {
                rayResult.GetComponent<RayEffect>().rayDirpos = Vector3.Reflect(rayDirpos.normalized, hit.normal);
            }
            //레이캐스트가 닿은 곳에 오브젝트를 옮긴다.
            if (isDone == false)
            {
                isDone = true;
                if (rayResult.GetComponent<RayEffect>() != null)
                {
                    rayResult.GetComponent<RayEffect>().setState(true);
                }
                else {
                    rayResult.GetComponent<SpriteRenderer>().enabled = true;
                }
            }

        }
        else {
            if (isDone) {
                isDone = false;

                if (rayResult.GetComponent<RayEffect>() != null)
                    rayResult.GetComponent<RayEffect>().setState(false);
                else { 
                    rayResult.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
        //거리에 따른 레이저 스케일 변화
        rayScale.transform.localScale = new Vector3(Xscale, 1, 1);
        
        
        float rotAngle = Vector2.Angle(Vector2.right, rayDirpos);

        if (rayDirpos.y < 0) {
            rotAngle *= -1;
        }
        Quaternion toRot = Quaternion.AngleAxis(rotAngle, Vector3.forward);
        rayScale.transform.rotation = toRot;

    }
    public void setState(bool state) {
        isPlay = state;

        if (!isPlay)
        {

            GetComponent<SpriteRenderer>().enabled = false;

            rayResult.GetComponent<SpriteRenderer>().enabled = false;
            rayScale.transform.localScale = Vector2.zero;
            if (rayResult.GetComponent<RayEffect>() != null)
                rayResult.GetComponent<RayEffect>().setState(false);
        }
        else {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
