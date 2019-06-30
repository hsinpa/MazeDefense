using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel 
{
    public int capital {
        get {
            return Mathf.FloorToInt(total_capital);
        }
    }
    private float total_capital;
}