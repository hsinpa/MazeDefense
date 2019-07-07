using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel 
{
    public string _id;

    public System.Action<int> OnCapitalChange;

    public int capital
    {
        get
        {
            return (int)(total_capital);
        }
    }
    private float total_capital = 150;

    public void EarnPrize(float prize) {
        total_capital += prize;

        if (OnCapitalChange != null)
            OnCapitalChange(capital);
    }

    public static PlayerModel CreatePlayer() {
        PlayerModel nPlayer = new PlayerModel();
        nPlayer._id = System.Guid.NewGuid().ToString();

        return nPlayer;
    }
}