using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldmapPortal : MonoBehaviour
{
    [SerializeField] private GameObject pressE;
    private void OnTriggerStay2D(Collider2D other){
        if(other.transform.root.name.Equals("Player")){
            pressE.SetActive(true);
            if(Input.GetKey(KeyCode.E)){
                Debug.Log("World Map");
                MapUI.MapAppear = true;
            }
        }
    }
        private void OnTriggerEnter2D(Collider2D other){
        if(other.transform.root.name.Equals("Player")){
            pressE.transform.position = gameObject.transform.position + new Vector3(0f, 1f, 0f);
        }
    }


    private void OnTriggerExit2D(Collider2D other){
        if(other.transform.root.name.Equals("Player")){
            pressE.SetActive(false);
        }
    }
}
