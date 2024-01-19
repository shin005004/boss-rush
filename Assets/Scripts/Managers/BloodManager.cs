using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodManager : MonoBehaviour
{
    public int Blood;

    private string bloodFilePath;
    private string bloodData;

    private void Awake(){
        bloodFilePath = Path.Combine(Application.dataPath, "Datas", "Blood Data.txt");
        bloodData = File.ReadAllText(bloodFilePath);
        int.TryParse(bloodData, out Blood);
    }

    public void AddBlood(int amount){
        Blood += amount;
    }
    public void UseBlood(int amount){
        Blood -= amount;
    }
}
