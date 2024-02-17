using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelfPortal : MonoBehaviour
{
    [SerializeField] private GameObject pressE;
    private GameObject bookShelf;
    private string destination;
    private bool onPortal;
    public bool OnPortal => onPortal;

    private void Start(){
        bookShelf = gameObject.transform.parent.gameObject;
        destination = bookShelf.GetComponent<MiniBookShelf>().destination;
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.transform.root.name.Equals("Player")){
            pressE.transform.position = gameObject.transform.position + new Vector3(0f, 1f, 0f);
        }
    }

    private void OnTriggerStay2D(Collider2D other){
        if(other.transform.root.name.Equals("Player")){
            pressE.SetActive(true);
            if(Input.GetKey(KeyCode.E)){
                GameManager.Instance.BookManager.SetBookRoomType(destination);
                SceneLoader.Instance.LoadBookShelfScene();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.transform.root.name.Equals("Player")){
            pressE.SetActive(false);
        }
    }
}
