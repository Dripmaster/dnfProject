using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEvent : MonoBehaviour
{
    public Sprite hoverImage;
    Sprite originImage;
    GameObject mark;

    private IEnumerator markCoroutine;
    Vector3 originMarkPos;

    // Start is called before the first frame update
    void Awake()
    {
        originImage = gameObject.GetComponent<SpriteRenderer>().sprite;
        mark = transform.Find("mark") ? transform.Find("mark").gameObject : null;
        if (mark != null)
            originMarkPos = mark.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseEnter()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = hoverImage;
        if (mark != null)
        {
            markCoroutine = RunMarkWave(0.25f, 0.1f);
            StartCoroutine(markCoroutine);
        }
    }
    private void OnMouseExit()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = originImage;
        if (mark != null)
        {
            mark.transform.position = originMarkPos;
            StopCoroutine(markCoroutine);
        }
    }
    private void OnMouseDown()
    {
        if (gameObject.tag == "selectTown")
        {
            GameObject.Find("townUi").gameObject.GetComponent<TownUiManager>().OpenTownUi();
        }
        if (gameObject.tag == "selectDungeon")
        {
            mapType mapType = mapType.grass;
            switch (gameObject.name)
            {
                case "grass":
                    mapType = mapType.grass;
                    break;
                case "grass_1":
                    mapType = mapType.miniGrass;
                    break;
                case "fire":
                    mapType = mapType.fire;
                    break;
                case "fire_1":
                    mapType = mapType.miniFire;
                    break;
                case "water":
                    mapType = mapType.water;
                    break;
                case "water_1":
                    mapType = mapType.miniWater;
                    break;
                case "dark":
                    mapType = mapType.dark;
                    break;
                case "glory":
                    mapType = mapType.glow;
                    break;
            }
            GameObject.Find("dungeonUi").gameObject.GetComponent<DungeonUiManager>().OpenDungeonUi(mapType);
        }
    }

    IEnumerator RunMarkWave(float speed, float distance)
    {
        Transform markTransform = mark.transform;
        float frame = 30;
        float upTarget = markTransform.position.y + distance;
        float originPosY = markTransform.position.y;
        bool isUp = true;

        while (true)
        {
            if (isUp)
                markTransform.position = new Vector2(markTransform.position.x, markTransform.position.y + (distance / (speed * frame)));
            else
                markTransform.position = new Vector2(markTransform.position.x, markTransform.position.y - (distance / (speed * frame)));

            if (isUp && markTransform.position.y >= upTarget)
            {
                isUp = false;
            }
            if (!isUp && markTransform.position.y <= originPosY)
                isUp = true;

            yield return new WaitForSeconds(speed / (speed * frame));
        }
    }
}
