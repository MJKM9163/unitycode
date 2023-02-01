using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOption : MonoBehaviour
{
    // 게임 구성에 필요한 함수들 작성하는 공간
    // 자주 사용하는 함수 등등..

    public void indexAdjustment(SpriteRenderer sprite, Rigidbody2D obj) {
        sprite.sortingOrder = (int)obj.position.y * -1;
    }
}
