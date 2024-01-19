using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodManager : MonoBehaviour
{
    public int Blood;

    private void Awake(){
        Blood = 0;
    }

    public void AddBlood(int amount){
        Blood += amount;
    }
    public void UseBlood(int amount){
        Blood -= amount;
    }
}
