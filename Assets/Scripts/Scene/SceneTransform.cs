using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransform : MonoBehaviour
{
    public void SceneTransform_(string scene_)
    {
        Debug.Log("��ʼ����ת����" + scene_);
        SceneManager.LoadSceneAsync(scene_);
        
    }
    
}
