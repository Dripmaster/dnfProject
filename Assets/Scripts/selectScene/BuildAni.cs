using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildAni : MonoBehaviour
{
    Animator ani;
    ParticleSystem buildParticle;

    private ParticleSystem.MinMaxGradient originColor;
    private ParticleSystem.MinMaxCurve originSize;
    private ParticleSystem.MinMaxCurve originVeloX;
    private ParticleSystem.MinMaxCurve originVeloY;
    private Vector3 originScale;
    private float originGravity;
    private float originLife;
    private float originArc;



    int count = 0;
    void Start()
    {
        ani = gameObject.GetComponent<Animator>();
        buildParticle = gameObject.transform.Find("buildParticle").GetComponent<ParticleSystem>();

        originColor = buildParticle.main.startColor;
        originSize = buildParticle.main.startSize;
        originVeloX = buildParticle.velocityOverLifetime.x;
        originVeloY = buildParticle.velocityOverLifetime.y;
        originScale = buildParticle.shape.scale;
        originGravity = buildParticle.main.gravityModifier.constant;
        originLife = buildParticle.main.startLifetime.constant;
        originArc = buildParticle.shape.arc;
    }


    // Update is called once per frame
    void Update()
    {
    }

    void FinishBuild()
    {
        count = 0;
        ParticleSystem.MainModule pMain = buildParticle.main;
        ParticleSystem.VelocityOverLifetimeModule pVelo = buildParticle.velocityOverLifetime;
        ParticleSystem.ShapeModule pShape = buildParticle.shape;
        pMain.startColor = originColor;
        pMain.gravityModifier = originGravity;
        pMain.startSize = originSize;
        pMain.startLifetime = originLife;
        pVelo.x = originVeloX;
        pVelo.y = originVeloY;
        pShape.arc = originArc;
        pShape.scale = originScale;

        transform.Find("alert").gameObject.SetActive(false);
        transform.Find("success").gameObject.SetActive(false);
        transform.Find("failure").gameObject.SetActive(false);

    }
    public IEnumerator RerunAni()
    {
        if (count <= 4)
        {
            if (count == 4)
                yield return new WaitForSeconds(0.7f);
            ani.Play("buildAni", -1, 0f);
        }
        if (count > 4)
        {
            yield return new WaitForSeconds(2f);
            FinishBuild();
            GameObject.Find("townUi").GetComponent<TownUiManager>().ClearSelect();
            GameObject.Find("townUi").transform.Find("canvas").GetComponent<GraphicRaycaster>().blockingObjects = GraphicRaycaster.BlockingObjects.None;
            GameObject.Find("invenUi").transform.Find("canvas").GetComponent<GraphicRaycaster>().blockingObjects = GraphicRaycaster.BlockingObjects.None;
            GameObject.Find("buildAni").transform.Find("build").gameObject.SetActive(false);
        }

    }
    public IEnumerator CreateParticle()
    {
        buildParticle.Play();
        ParticleSystem.MainModule pMain = buildParticle.main;
        ParticleSystem.VelocityOverLifetimeModule pVelo = buildParticle.velocityOverLifetime;
        ParticleSystem.ShapeModule pShape = buildParticle.shape;
        if (count == 4)
        {
            if (CheckSuccess())
            {
                pMain.startColor = new ParticleSystem.MinMaxGradient(new Color(0.58f, 0.74f, 0.26f, 1));
                pMain.gravityModifier = 0;
                pMain.startSize = new ParticleSystem.MinMaxCurve(2.4f, 2.9f);
                pMain.startLifetime = 1f;
                pVelo.x = new ParticleSystem.MinMaxCurve(pVelo.x.constantMin - 1f, pVelo.x.constantMax + 1f);
                pVelo.y = new ParticleSystem.MinMaxCurve(-5, 5);
                pShape.arc = 360;
                transform.Find("success").gameObject.SetActive(true);
                soundMgr.instance.Play("success");
            }
            else
            {
                pMain.startColor = new ParticleSystem.MinMaxGradient(new Color(0.42f, 0.07f, 0.07f, 1));
                pMain.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);
                pMain.startLifetime = 1f;
                pVelo.x = new ParticleSystem.MinMaxCurve(-4f, 4f);
                pVelo.y = new ParticleSystem.MinMaxCurve(7, 9);
                pShape.scale = new Vector3(5, 1, 1);
                transform.Find("failure").gameObject.SetActive(true);
                soundMgr.instance.Play("fail");

            }

            count++;
        }
        else
        {
            soundMgr.instance.PlayOneShot("make");
            pMain.startSize = new ParticleSystem.MinMaxCurve(pMain.startSize.constantMin + 0.2f, pMain.startSize.constantMax + 0.2f);
            pVelo.x = new ParticleSystem.MinMaxCurve(pVelo.x.constantMin - 1f, pVelo.x.constantMax + 1f);
            pVelo.y = new ParticleSystem.MinMaxCurve(pVelo.y.constantMin + 0.5f, pVelo.y.constantMax + 0.5f);
            count++;
        }
        yield return 0;
    }

    public void StartAni()
    {
        if (!GameObject.Find("townUi").GetComponent<TownUiManager>().isSelectWeapon ||
            !GameObject.Find("townUi").GetComponent<TownUiManager>().isSelectMaterial)
        {
            return;
        }

        GameObject.Find("townUi").transform.Find("canvas").GetComponent<GraphicRaycaster>().blockingObjects = GraphicRaycaster.BlockingObjects.TwoD;
        GameObject.Find("invenUi").transform.Find("canvas").GetComponent<GraphicRaycaster>().blockingObjects = GraphicRaycaster.BlockingObjects.TwoD;
        GameObject.Find("buildAni").transform.Find("build").gameObject.SetActive(true);
    }

    private bool CheckSuccess()
    {
        //확률 기본 50%
        //3강 이상 30% 
        //3강 이상 실패시 10%확률로 파괴
        //스킬 획득확률 3강 이상 성공했을시 20% 
        item selectWeapon = GameObject.Find("townUi").GetComponent<TownUiManager>().selectWeapon;
        item selectmaterial = GameObject.Find("townUi").GetComponent<TownUiManager>().selectMaterial;

        playerDataManager.instance.popItem((itemType)selectmaterial.type, 1, false);

        int rand;
        if (selectWeapon.upgradeList[(int)selectmaterial.type - 1] > 3)
        {
            rand = Random.Range(0, 100) + 1;
            if (rand <= 30)
            {
                rand = Random.Range(0, 100) + 1;
                if (!selectWeapon.hasSkillList[(int)selectmaterial.type - 1] && rand <= 25)
                {
                    print(rand);
                    selectWeapon.hasSkillList[(int)selectmaterial.type - 1] = true;
                    transform.Find("alert").gameObject.SetActive(true);
                }
                selectWeapon.upgradeList[(int)selectmaterial.type - 1] += 1;
                return true;
            }
            else
            {
                rand = Random.Range(0, 100) + 1;
                if (rand <= 10)
                {
                    // 여기 삭제 이벤트 추가
                }
                return false;
            }
        }
        else
        {
            rand = Random.Range(0, 100) + 1;
            if (rand >= 50)
            {

                selectWeapon.upgradeList[(int)selectmaterial.type - 1] += 1;
                return true;
            }
            else
                return false;
        }


    }
}
