using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Choose : MonoBehaviour
{
    [SerializeField] private List<AttackerCharacter_SO> list1 = new List<AttackerCharacter_SO>();
    //[SerializeField] private List<DefensiveCharacter_SO> list2 = new List<DefensiveCharacter_SO> ();
    [SerializeField] private string listName = "Attacker";
    [SerializeField] private int currentList1;
    //[SerializeField] private int currentList2;
    public static int childID;
    public Image image;
    public Text describe;


    private void Update()
    {
        SetInformation();
    }

    public void SetInformation()
    {
        if (listName == "Attacker")
        {
            image.sprite = list1[currentList1].sprite;
            describe.text = list1[currentList1]._description;
        }
        else
        {
            Debug.Log("listName出问题了");
        }
    }

    public void ChangeInformation(string LR)
    {

        if (LR == "Left")
        {
            currentList1 = (list1.Count + currentList1 - 1) % list1.Count;
        }
        else if (LR == "Right")
        {
            currentList1 = (currentList1 + 1) % list1.Count;
            Debug.Log("+1");
        }
        else
        {
            Debug.Log("按钮有问题");
        }


    }
    public void ContinueButtonClick()
    {
        childID = list1[currentList1].childID;
    }



}
