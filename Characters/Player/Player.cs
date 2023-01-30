using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gm;

    public float maxSpeed;
    public float jumpPower;
    float jumpZ;
    public Vector2 inputVec; // 상하 위치 값
    Vector2 nextVec; // 움직이려는 위치 값
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
        jumpPower = 1;
        jumpZ = 0;
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

    IEnumerator JumpCo() {
        Debug.Log("점프!");
        float YSave = rigid.position.y;
        bool jumpCheck = true;
        anim.SetBool("jump", true);
        while (true) {
            // Debug.Log(jumpZ);
            // if (attackAnimCheck) {
            //     inputVec.y -= jumpZ;
            //     rigid.MovePosition(rigid.position + inputVec);
            //     anim.SetBool("jump", false);
            //     anim.SetBool("down", false);
            //     jumpCheck = true;
            //     jumpZ = 0;
            //     break;
            // }

            if (jumpPower > jumpZ && jumpCheck) {
                jumpZ += Time.fixedDeltaTime * 3.5f;

            } else if (YSave + jumpPower <= rigid.position.y && jumpCheck) {
                anim.SetBool("jump", false);
                anim.SetBool("down", true);
                jumpCheck = false;
            }
            
            if (!jumpCheck) {
                jumpZ -= Time.fixedDeltaTime * 3.5f;
                if (YSave >= rigid.position.y - 0.01f) {
                    jumpCheck = true;
                    jumpZ = 0;
                    anim.SetBool("down", false);
                    break;
                }
            }

            inputVec.y += jumpZ;
            yield return null;
        }
    }


    // 컴퓨터 성능에 따라 프레임이 다르다.
    // 단순한 로직 업데이트에 사용
    void Update()
    {

        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
        // 좌 or 우 입력 버튼에서 땠을 때 확인
        // 속도를 0으로 설정
        // 런 애니메이션 비활성화
        if (Input.GetButtonUp("Horizontal") && inputVec.y == 0) {
            anim.SetInteger("move", 0);
        } else if (Input.GetButtonUp("Vertical") && inputVec.x == 0) {
            anim.SetInteger("move", 0);
        }

        if (Input.GetMouseButtonDown(0) && atacando &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("jump") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("down")
        ) {
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
            anim.SetInteger("move", 0);
        }

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackHitPos.position, boxSize);
    }

    // 고정된 프레임으로 업데이트!
    // 물리 연산 로직에 주로 사용
    void FixedUpdate() {

        nextVec = inputVec.normalized * maxSpeed * Time.fixedDeltaTime;

        // 현재 공격중이 아닐 때
        // 앞, 뒤로 움직일 수 있다.
        if (!attackAnimCheck) {
            rigid.MovePosition(rigid.position + nextVec);
            if (nextVec.x != 0 || nextVec.y != 0) {
                anim.SetInteger("move", 1);
            } else {
                anim.SetInteger("move", 0);
            }

            // 현재 실행중인 애니메이션이 점프가 아닐 경우
            // 점프(스페이스)를 눌렀을 경우, 캐릭터 y축 속도가 0일 경우
            // 캐릭터의 +y축으로 힘을 가한다.
            // 점프 애니메이션 실행을 위해 jump를 활성화한다.
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("jump") &&
                !anim.GetCurrentAnimatorStateInfo(0).IsName("down") &&
                Input.GetButton("Jump")) {
                    Debug.Log("점프!");
                    //StopCoroutine("JumpCo");
                    StartCoroutine("JumpCo");
            }
        }
    }

    // 모든 Update 로직이 끝나면 실행된다.
    // 카메라 설정이나 후처리 로직을 주로 작성한다.
    void LateUpdate() {
        if (inputVec.x < 0 && !attackAnimCheck && !spriter.flipX) {
            spriter.flipX = true;
            Vector3 minusOne = attackHitPos.position;
            minusOne.x -= 1.74f;
            attackHitPos.position = minusOne;

        } else if (inputVec.x > 0 && !attackAnimCheck && spriter.flipX) {
            spriter.flipX = false;
            Vector3 minusOne = attackHitPos.position;
            minusOne.x += 1.74f;
            attackHitPos.position = minusOne;
        }
    }
}
