using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // Start is called before the first frame update
     
    [SerializeField]private CardCreate currentCard;
    private Vector3 mousePos;

   
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        if (Input.GetMouseButtonDown(1)&&currentCard!=null)
        {
            currentCard.CardCreateCharacter(mousePos);
            Debug.Log("Éú³É½ÇÉ«");
        }
    }

    
}
