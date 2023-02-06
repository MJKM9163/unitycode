using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCode : MonoBehaviour
{
    public GameObject hitBox;
    public Vector2 hitBoxSize;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;


    void attackTest() {
        anim.SetTrigger("attack");
        hitBox.SetActive(true);
        Utile.hitBox(hitBox.transform.position, hitBoxSize, "Player");
    }

    void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log(other.gameObject.tag);

        if (!other.CompareTag("PlayerHitBox")) {
            return;
        }

        Utile.onDamaged(other.transform.position, transform.position, rigid);
        Debug.Log("플레이어에게 타격당함!");

        Invoke("attackTest", 1.5f);
    }

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
