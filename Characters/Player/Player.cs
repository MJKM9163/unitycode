using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gm;

    public float maxSpeed;
    public float jumpPower;
    float h;
    bool atacando; // 공격이 가능한지 확인 (true = 공격 가능함)
    bool attackAnimCheck; // 공격 중인지 확인 (true = 공격 중)
    int attackCombo; // 공격 콤보 (1부터 시작)
    public Transform attackHitPos;
    public Vector2 boxSize;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    // 초기 설정
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        maxSpeed = 4;
        jumpPower = 5;
        atacando = true;
        attackAnimCheck = false;
        attackCombo = 0;
    }

    // void Start()
    // {

    // }

    // 애니메이션에 사용
    // 일정 프레임이 지난 후 공격 활성 및 다음 공격 모션 설정
    void attack() {
        atacando = true;
        if (attackCombo < 3) {
            attackCombo++;
        }
    }

    // 애니메이션에 사용
    // 공격 애니메이션이 종료되면 공격 설정 초기화
    void attackEnd() {
        attackAnimCheck = false;
        atacando = true;
        attackCombo = 0;
        anim.SetInteger("combo", attackCombo);
        gm.Action("");
    }


    // 컴퓨터 성능에 따라 프레임이 다르다.
    // 단순한 로직 업데이트에 사용
    void Update()
    {
        // 좌 or 우 입력 버튼에서 땠을 때 확인
        // 속도를 0으로 설정
        // 런 애니메이션 비활성화
        if (Input.GetButtonUp("Horizontal")) {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x*0.5f, rigid.velocity.y);
            anim.SetFloat("speed", 0);
        }

        if (Input.GetMouseButtonDown(0) && atacando) {
            attackAnimCheck = true;
            atacando = false;
            anim.SetTrigger("attack");
            anim.SetInteger("combo", attackCombo == 0 ? ++attackCombo : attackCombo);

            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackHitPos.position, boxSize, 0);
            foreach(Collider2D collider in collider2Ds) {
                if (collider.tag == "Enemy") {
                    // collider.GetComponent<Enemy>().TakeDamage(1);
                    Debug.Log("적 감지");
                    gm.Action(collider.name + "를 타격!");
                }
            }

            rigid.velocity = new Vector2(rigid.velocity.normalized.x*0.5f, rigid.velocity.y);
            anim.SetFloat("speed", 0);
        }

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackHitPos.position, boxSize);
    }

    // 고정된 프레임으로 업데이트!
    // 물리 연산 로직에 주로 사용
    void FixedUpdate() {

        h = Input.GetAxisRaw("Horizontal");

        // 현재 공격중이 아닐 때
        // 앞, 뒤로 움직일 수 있다.
        if (!attackAnimCheck) {
            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

            // 현재 실행중인 애니메이션이 점프가 아닐 경우
            // 점프(스페이스)를 눌렀을 경우, 캐릭터 y축 속도가 0일 경우
            // 캐릭터의 +y축으로 힘을 가한다.
            // 점프 애니메이션 실행을 위해 jump를 활성화한다.
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("jump") &&
                Input.GetButton("Jump") && rigid.velocity.y == 0) {
                    rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                    anim.SetBool("jump", true);
            }

            // 오른쪽 속도가 maxSpeed를 넘었다면
            // velocity를 maxSpeed 값으로 고정한다.
            if (rigid.velocity.x > maxSpeed) {
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
                anim.SetFloat("speed", rigid.velocity.x);

            // 왼쪽 속도가 maxSpeed를 넘었다면
            // velocity를 maxSpeed 값으로 고정한다.
            } else if (rigid.velocity.x < maxSpeed*(-1)) {
                rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
                anim.SetFloat("speed", maxSpeed);
            }
        }

        // 레이캐스트를 사용했기에 FixedUpdate에 로직을 작성
        // 현재 속도가 마이너스일 때 로직 실행 (마이너스는 밑으로 떨어짐을 의미)
        // 점프를 비활성하고 떨어짐을 활성한다.
        // 레이캐스트를 사용해 땅을 감시한다.
        // 감지된 땅이 있다면 떨어짐을 비활성한다.
        if (rigid.velocity.y < 0) {
            anim.SetBool("down", true);
            anim.SetBool("jump", false);
            // Debug.DrawRay(rigid.position, new Vector3(0, -1.5f, 0), new Color(0,1,0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, new Vector3(0, -1.5f, 0), 1.5f, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null) {
                anim.SetBool("down", false);
            }
        }
    }

    // 모든 Update 로직이 끝나면 실행된다.
    // 카메라 설정이나 후처리 로직을 주로 작성한다.
    void LateUpdate() {
        if (h < 0 && !attackAnimCheck) {
            spriter.flipX = true;
        } else if (h > 0 && !attackAnimCheck) {
            spriter.flipX = false;
        }
    }
}
