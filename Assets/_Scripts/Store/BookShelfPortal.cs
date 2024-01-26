using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelfPortal : MonoBehaviour
{
    private GameObject bookShelf;
    private string destination;

    private void Start(){
        bookShelf = gameObject.transform.parent.gameObject;
        destination = bookShelf.GetComponent<MiniBookShelf>().destination;
    }

    private void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.name.Equals("Player")){
            if(Input.GetKey(KeyCode.E)){
                GameManager.Instance.BookManager.SetBookRoomType(destination);
                SceneLoader.Instance.LoadBookShelfScene();
            }
        }
    }
}
