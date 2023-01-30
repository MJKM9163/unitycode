using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public void GameSceneCtrl() {
        Debug.Log("클릭 확인");
        SceneManager.LoadScene("PlayScene");
        Debug.Log("씬 확인");
    }
}
