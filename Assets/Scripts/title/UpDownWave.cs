using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownWave : MonoBehaviour
{
    public float speed;
    public float distance;

    bool isUp = true;

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator RunWave()
    {
        float frame = 30;
        float upTarget = transform.position.y + distance;
        float originPosY = transform.position.y;

        while (true)
        {
            if (isUp)
                transform.position = new Vector2(transform.position.x, transform.position.y + (distance / (speed * frame)));
            else
                transform.position = new Vector2(transform.position.x, transform.position.y - (distance / (speed * frame)));

            if (isUp && transform.position.y >= upTarget)
            {
                isUp = false;
            }
            if (!isUp && transform.position.y <= originPosY)
                isUp = true;


            yield return new WaitForSeconds(speed / (speed * frame));
        }
    }
}
