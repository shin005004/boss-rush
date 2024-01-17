using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private string destination;

    private void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.name.Equals("Player")){
            if(Input.GetKeyDown(KeyCode.E)){
                GameManager.Instance.BookManager.SetBookRoomType(destination);
            }
        }
    }
}
