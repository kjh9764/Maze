using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointChk : MonoBehaviour
{
   
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Á¾·á");
            gameManager.GameEnd();
        }
    }

}
