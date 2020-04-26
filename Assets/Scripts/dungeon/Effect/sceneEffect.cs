using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneEffect : MonoBehaviour
{
    public void shakeStart() {
        StartCoroutine(shakeCamera());
    }

    IEnumerator shakeCamera() {
        float deltaX=0.2f,deltaY=0.2f;
        int count = 0;
        do {
            count++;
            transform.position += new Vector3(Random.Range(-deltaX,deltaX), Random.Range(-deltaY, deltaY));
            yield return null;
        } while (count<3);
    }
}
