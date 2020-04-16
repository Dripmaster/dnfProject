using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myParticle : MonoBehaviour
{
    bool isPlaying = false;
    public GameObject particlePrefab;
    SpriteRenderer sr;
    SpriteRenderer playersr;
    List<GameObject> particles;
    // Start is called before the first frame update
    void Awake()
    {
        particles = new List<GameObject>();
    }
    private void OnEnable()
    {
        StartCoroutine(Emitter());
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject p in particles) {
            if (p.activeInHierarchy)
            {
                sr = p.GetComponent<SpriteRenderer>();
                Color c = sr.color;
                c.a -= Time.deltaTime / 0.4f;
                sr.color = c;
                if (c.a <= 0) {
                    p.SetActive(false);
                }
            }
        }
    }
    void initobj(GameObject g) {
        sr = g.GetComponent<SpriteRenderer>();
        sr.sprite = playersr.sprite;
        sr.color = new Color(255, 255, 255, 1);
        g.SetActive(true);
        g.transform.position = playersr.transform.position;
    }
    public void setSr(SpriteRenderer playerSr) {
        playersr = playerSr;
    }
    public void Play()
    {
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;   
    }
    IEnumerator Emitter() {

        do
        {
            if (!isPlaying)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(0.02f);

            GameObject obj=null;
            foreach (GameObject p in particles)
            {
                if (!p.activeInHierarchy)
                {
                    obj = p;
                    break;
                }
            }
            if (obj == null)
            {
                obj = Instantiate(particlePrefab, playersr.transform.position, Quaternion.identity);
                particles.Add(obj);
            }
            initobj(obj);
        } while (true);
    }
}
