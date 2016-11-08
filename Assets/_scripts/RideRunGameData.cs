using UnityEngine;
using System.Collections;

public class RideRunGameData : Singleton<RideRunGameData>
{
    private bool isgameover;
    public bool IsGameOver
    {
        get
        {
            return isgameover;
        }
        set
        {
            isgameover = value;
        }
    }
    
}
