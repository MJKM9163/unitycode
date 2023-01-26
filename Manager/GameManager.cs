using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text textBox;
    //public GameObject test;
    string test;

    public void Action(string testObj) {
        test = testObj;
        Debug.Log(test);
        textBox.text = test;
    }
}
