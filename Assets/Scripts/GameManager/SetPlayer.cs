using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPlayer : MonoBehaviour
{
    

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
    }

   

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            GameObject players = GameObject.FindGameObjectWithTag("Players");
            if (players != null)
            {
                players.transform.GetChild(Choose.childID).gameObject.SetActive(true);
                Debug.Log("�����ɫ: " + Choose.childID);
            }
            else
            {
                Debug.LogError("Players������û�ҵ���");
            }
        }
    }
}
