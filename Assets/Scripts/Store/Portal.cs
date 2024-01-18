using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private string destination;

    private void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.name.Equals("Player")){
            Debug.Log("Player");
            if(Input.GetKey(KeyCode.E)){
                Debug.Log("E");
                GameManager.Instance.BookManager.SetBookRoomType(destination);
                SceneLoader.Instance.LoadBookShelfScene();
            }
        }
    }
}
