using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dupEffect : MonoBehaviour
{
    ParticleSystem ps;
    // Start is called before the first frame update
    public bool isDo = false;
    Vector2 moveDir;
    EffectScript myEffect;
    Rigidbody2D RBD;

    List<Collider2D> temp;
    void Awake()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        ps.Stop();
        moveDir = new Vector2(0, -1);
        RBD = GetComponent<Rigidbody2D>();
        myEffect = GetComponent<EffectScript>();
    }
    private void OnEnable()
    {
        temp = new List<Collider2D>();

    }
    IEnumerator shoot() {
        ps.Play();
        Vector2 temppos = transform.position;
        do
        {
            RBD.MovePosition(transform.position + (Vector3)(moveDir * Time.deltaTime * 25));
            if (ps.isPlaying&&(Vector2.Distance(temppos, transform.position) >= 4.5f))
                ps.Stop();
            if (Vector2.Distance(temppos, transform.position) >= 5f)
                break;
            yield return null;
        } while (isDo);
        isDo = false;
        if (ps.isPlaying)
            ps.Stop();
    }
    public void startCharge()
    {
        if (gameObject.activeInHierarchy)
            return;
        isDo = true;
        myEffect.initAni("player/sword/skill/"+playerFSM.instance.degree%8);
        moveDir = playerFSM.instance.attackfan;
        ps.transform.rotation =Quaternion.Euler(-playerFSM.instance.degree * 45, 270, 90);
        myEffect.setAlpha(0.5f);
        gameObject.SetActive(true);
        transform.position = playerFSM.instance.transform.position + (Vector3)playerFSM.instance.attackfan/2;

        StartCoroutine(shoot());
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDo&&col.tag == "Enemy" && col is BoxCollider2D)
        {
            if (!temp.Contains(col))
            {
                temp.Add(col);
                DamageReceiver.playerSkill(playerFSM.instance.getAtkP(), col.GetComponent<EnemyFSM>(), 5f);
            }
        }
        else if (col.tag == "wall")
        {
           // isDo = false;
        }
    }
}
