using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossData
{
    public static BossData instance { get; private set; }

    public Dictionary<string, int> BossSkillCount = new Dictionary<string, int> {
        {"Thor", 4},
        {"Surtur", 4}
    };

    public List<string> PassiveList = new List<string> {

    };
    public List<string> AsiaBossList = new List<string> {

    };
    public List<string> EuropeBossList = new List<string> {
        "Thor",
        "Surtur"
    };
    public List<string> NorthAmericaBossList = new List<string> {

    };
    public List<string> SouthAmericaBossList = new List<string> {

    };
    public List<string> AfricaBossList = new List<string> {

    };
    public List<string> OceaniaBossList = new List<string> {

    };
}
