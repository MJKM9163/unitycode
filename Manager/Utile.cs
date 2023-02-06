using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utile : MonoBehaviour
{
    // 게임 구성에 필요한 함수들 작성하는 공간
    // 자주 사용하는 함수 등등..

    // y축 위치에 따라 레이어 인덱스 변경
    public static void indexAdjustment(SpriteRenderer sprite, Rigidbody2D obj) {
        sprite.sortingOrder = (int)obj.position.y * -1;
    }

    // 히트박스 생성 후 타격 감지
    // pos -> 히트박스 위치
    // size -> 히트박스 크기
    // target -> 대상
    public static void hitBox(Vector3 pos, Vector2 size, string target) {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos, size, 0);
            foreach(Collider2D collider in collider2Ds) {
                if (collider.tag == target) {
                    // collider.GetComponent<Enemy>().TakeDamage(1);
                    Debug.Log(target+" 감지");
                }
            }
    }

    // 피격 이벤트
    // targetPos -> 피격한 대상 position
    // myPos -> 피격당한 대상 position
    // myRigid -> 피격당한 rigid
    public static void onDamaged(Vector2 targetPos, Vector2 myPos, Rigidbody2D myRigid) {
        int dirx = myPos.x - targetPos.x > 0 ? 1 : -1;
        myRigid.AddForce(new Vector2(dirx, 1) * 5, ForceMode2D.Impulse);
    }
}
