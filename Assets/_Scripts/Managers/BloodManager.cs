using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodManager : MonoBehaviour
{
    private int blood;
    public int Blood => blood;

    private string bloodFilePath;
    private string bloodData;

    private void Awake(){
        bloodFilePath = Path.Combine(Application.dataPath, "Datas", "Save", "Blood Data.txt");
        bloodData = File.ReadAllText(bloodFilePath);
        int.TryParse(bloodData, out blood);
    }

    public void AddBlood(int amount){
        blood += amount;
    }
    public void UseBlood(int amount){
        blood -= amount;
    }

    public void UpdateBloodSaveFile(){
        bloodFilePath = Path.Combine(Application.dataPath, "Datas", "Save", "Blood Data.txt");
        bloodData = blood.ToString();
        File.WriteAllText(bloodFilePath, bloodData);
    }
}
