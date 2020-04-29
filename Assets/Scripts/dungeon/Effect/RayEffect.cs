using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayEffect : MonoBehaviour
{
    public GameObject rayScale;
    public GameObject rayRed;
    Transform rayDir;
    EnemyFSM[] Targets;
    public Vector2 rayDirpos;
    int current_target;
    float raySpeed = 10;
    int count = 0;
    bool init;
    private void OnEnable()
    {if(init)
        StartCoroutine(enableCounter());
    }
    IEnumerator enableCounter() {
        yield return new WaitForSeconds(5);

        init = false;
        gameObject.SetActive(false);
    }
    void Update()
    {
        if (Targets == null || Targets.Length == 0)
        {
            Targets = DamageReceiver.getBlazeTarget(transform.position);
            if (Targets == null || Targets.Length == 0)
            {
                rayScale.transform.localScale = new Vector3(0, 1, 1);
                return;
            }
            current_target = 0;
        }
        rayDir = Targets[current_target].transform;
        //rayDirpos = Vector2.Lerp(rayDirpos, rayDir.position-transform.position, Time.deltaTime *2f);
        rayDirpos = (rayDir.position - transform.position).normalized;
        float dis = Vector2.Distance(transform.position, rayDir.position);
        float Xscale = (rayScale.transform.localScale.x + raySpeed * Time.deltaTime * 5);
        if (dis < Xscale) {
            Xscale = dis;
        }

        //레이저 빨간색
        rayRed.transform.localPosition = new Vector2(Time.deltaTime * 5 + rayRed.transform.localPosition.x, 0);

        //로테이션

        float rotAngle = Vector2.Angle(Vector2.right, rayDirpos);
        if (rayDirpos.y < 0)
        {
            rotAngle *= -1;
        }
        Quaternion toRot = Quaternion.AngleAxis(rotAngle, Vector3.forward);
        rayScale.transform.rotation = toRot;

        if (dis<=Xscale)
        {

            //레이캐스트가 닿은 곳에 오브젝트를 옮긴다.
            Xscale = dis;
            Vector2 toPos = rayDir.position;
            if (rayRed.transform.localPosition.x >= 0.95f)
            {
                rayRed.transform.localPosition = new Vector2(0.05f, 0);
                //거리에 따른 레이저 스케일 변화
                rayScale.transform.localScale = new Vector3(0, 1, 1);
                count++;
                try
                {
                    Targets = DamageReceiver.getBlaze(Targets[current_target], transform.position);
                }
                catch
                {
                    print(current_target);
                    throw;
                }
                if (count >= 10)
                {
                    gameObject.SetActive(false);
                    init = false;
                    return;
                }
                if (Targets == null || Targets.Length == 0)
                {
                     Targets = DamageReceiver.getBlazeTarget(transform.position);

                    return;
                }
                current_target = (current_target + 1) % Targets.Length;
            }
        }
        else {
            if (rayRed.transform.localPosition.x >= 0.95f)
            {
                rayRed.transform.localPosition = new Vector2(0.05f, 0);
            }
        }
        //거리에 따른 레이저 스케일 변화
        rayScale.transform.localScale = new Vector3(Xscale, 1, 1);
        
    }
    public void setTargets(EnemyFSM[] t) {
        current_target = 0;

        count = 0;
        transform.position =  playerFSM.instance.transform.position + new Vector3(0,0.5f);
        Targets = t;
        current_target = 0;
        rayRed.transform.localPosition = new Vector2(0.05f, 0);
        init = true;
    }
}
