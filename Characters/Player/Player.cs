using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gm;
    public float maxSpeed;
    public float jumpPower;
    float jumpZ;
    CapsuleCollider2D coll;
    bool hLock;
    bool vLock;
    public Vector2 inputVec; // 상하 위치 값
    Vector2 nextVec; // 움직이려는 위치 값
    bool atacando; // 공격이 가능한지 확인 (true = 공격 가능함)
    bool attackAnimCheck; // 공격 중인지 확인 (true = 공격 중)
    int attackCombo; // 공격 콤보 (1부터 시작)
    public Vector2 boxSize;
    public GameObject weapon;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;


    WaitForSeconds delay003 = new WaitForSeconds(0.03f);

    // 초기 설정
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        maxSpeed = 40;
        jumpPower = 4;
        jumpZ = 0;
        coll = GetComponent<CapsuleCollider2D>();
        hLock = false;
        vLock = false;
        atacando = true;
        attackAnimCheck = false;
        attackCombo = 0;
    }

    void move() {
        float x = Mathf.Clamp(rigid.position.x, -109f, 109f);
        float y = Mathf.Clamp(rigid.position.y, -35f, 35f + (PlayerStatus.isAir ? 20f : 0f));
        if (PlayerStatus.isJump) {
            rigid.velocity = new Vector2(nextVec.x * maxSpeed, rigid.velocity.y);
        } else {
            rigid.velocity = nextVec * maxSpeed;
        }

        if (nextVec.x != 0 || nextVec.y != 0) {
            anim.SetInteger("move", 1);
        } else {
            anim.SetInteger("move", 0);
        }
    }

    // 공격 애니메이션에 사용
    // 일정 프레임이 지난 후 공격 활성 및 다음 공격 모션 설정
    void attack() {
        atacando = true;
        if (attackCombo < 3) {
            attackCombo++;
        }
    }

    // 공격 애니메이션에 사용
    // 공격 애니메이션이 종료되면 공격 설정 초기화
    void attackEnd() {
        attackAnimCheck = false;
        atacando = true;
        attackCombo = 0;
        anim.SetInteger("combo", attackCombo);
        PlayerStatus.isAttack = false;
        weapon.SetActive(false);
    }

    // 공격 에니메이션에 사용
    // 히트 박스 온 함수
    void hitBoxOn() {
        weapon.SetActive(true);
    }

    void dtest() {
        anim.SetBool("damaged", false);
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("공격 감지");
        Debug.Log(other.gameObject.tag);

        if (!other.CompareTag("EnemyHitBox")) {
            return;
        }

        anim.SetBool("damaged", true);
        Utile.onDamaged(other.transform.position, transform.position, rigid);
        Debug.Log("Enemy에게 타격당함!");

        Invoke("dtest", 0.5f);

    }

    // void Start()
    // {

    // }

    IEnumerator Skill1() {
        PlayerStatus.isSkill = true; // 임시 스킬 사용중 표시(방향, 움직임 등 제한 기능)
        float speed = spriter.flipX ? 1f : -1f;
        rigid.AddForce(transform.right * (spriter.flipX ? -91f : 91f), ForceMode2D.Impulse);
        while(spriter.flipX ? speed <= 13f : speed >= -13) {
            rigid.AddForce(transform.right * speed, ForceMode2D.Impulse);
            if (spriter.flipX) {
                speed += 1;
            } else {
                speed -= 1;
            }
            yield return delay003;

        }
        PlayerStatus.isSkill = false;
    }

    IEnumerator JumpCo() {
        PlayerStatus.isJump = true;
        PlayerStatus.isAir = true;
        Vector3 minus = rigid.position;
        float YSave = rigid.position.y;
        float speed = -1f;
        vLock = true;
        rigid.AddForce(transform.up * 55f, ForceMode2D.Impulse);
        anim.SetBool("jump", true);
        while(speed >= -20) {
            if (speed == -10) {
                anim.SetBool("jump", false);
                anim.SetBool("down", true);
            }
            rigid.AddForce(transform.up * speed, ForceMode2D.Impulse);
            speed -= 1;
            if (rigid.position.y < YSave) {
                break;
            }
            yield return delay003;
        }
        rigid.velocity = Vector3.zero;
        minus.x = rigid.position.x;
        rigid.position = minus;
        vLock = false;
        anim.SetBool("down", false);
        anim.SetInteger("move", 0);
        PlayerStatus.isJump = false;
        PlayerStatus.isAir = false;
    }

    // 컴퓨터 성능에 따라 프레임이 다르다.
    // 단순한 로직 업데이트에 사용
    void Update()
    {

        // 락이 걸려있지 않으면 움직임 입력을 받음
        inputVec.x = hLock ? 0 : Input.GetAxisRaw("Horizontal");
        if (!vLock || !PlayerStatus.isJump) {
            inputVec.y = Input.GetAxisRaw("Vertical");
        }
        //inputVec.y = (vLock || PlayerStatus.isJump) ? 0 : Input.GetAxisRaw("Vertical");

        // 움직임 입력 버튼에서 손 땠음을 때 확인
        // 점프 중이 아닐 때 확인
        // 속도를 0으로 설정
        if ((Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical")) &&
            !PlayerStatus.isJump) {
            rigid.velocity = Vector2.zero;
        }

        // 캐릭터가 멈춰있다면 런 애니메이션 비활성화
        if (inputVec.y == 0 && inputVec.x == 0) {
            anim.SetInteger("move", 0);
        }

        // 공격
        if (Input.GetMouseButtonDown(0) && atacando &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("jump") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("down")
        ) {
            PlayerStatus.isAttack = true;
            attackAnimCheck = true;
            atacando = false;
            rigid.velocity = Vector2.zero;
            anim.SetTrigger("attack");
            anim.SetInteger("combo", attackCombo == 0 ? ++attackCombo : attackCombo);

            Utile.hitBox(weapon.transform.position, boxSize, "Enemy");

            anim.SetInteger("move", 0);
        }

        if (Input.GetMouseButtonDown(1)) {
            Debug.Log("마우스 우클릭");
            StartCoroutine("Skill1");
        }

        // 현재 실행중인 애니메이션이 점프가 아닐 경우
        // 점프(스페이스)를 눌렀을 경우, 캐릭터 y축 속도가 0일 경우
        // 캐릭터의 +y축으로 힘을 가한다.
        // 점프 애니메이션 실행을 위해 jump를 활성화한다.
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("jump") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("down") &&
            Input.GetButtonDown("Jump")) {
            rigid.velocity = Vector2.zero;
            StartCoroutine("JumpCo");
        }

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(weapon.transform.position, boxSize);
    }
    
    // 고정된 프레임으로 업데이트!
    // 물리 연산 로직에 주로 사용
    void FixedUpdate() {
        nextVec = inputVec.normalized * maxSpeed * Time.fixedDeltaTime;

        // 현재 공격중이 아닐 때
        // 앞, 뒤로 움직일 수 있다.
        if (!attackAnimCheck) {
            if ((inputVec.x != 0 || inputVec.y != 0)) {
                Utile.indexAdjustment(spriter, rigid);
                move();
            }
        }
    }

    // 모든 Update 로직이 끝나면 실행된다.
    // 카메라 설정이나 후처리 로직을 주로 작성한다.
    void LateUpdate() {

        // ✔반복되는 코드 리팩토링하기
        // 캐릭터 방향에 따라 히트박스 위치 옮기기
        if (inputVec.x < 0 && !attackAnimCheck && !spriter.flipX) {
            spriter.flipX = true;
            Vector3 minus = weapon.transform.position;
            minus.x -= 17.4f;
            weapon.transform.position = minus;

        } else if (inputVec.x > 0 && !attackAnimCheck && spriter.flipX) {
            spriter.flipX = false;
            Vector3 minusOne = weapon.transform.position;
            minusOne.x += 17.4f;
            weapon.transform.position = minusOne;
        }
    }
}
