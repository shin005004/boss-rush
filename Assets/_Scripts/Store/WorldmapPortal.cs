using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldmapPortal : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.name.Equals("Player")){
            if(Input.GetKey(KeyCode.E)){
                Debug.Log("World Map");
                MapUI.MapAppear = true;
            }
        }
    }
}
