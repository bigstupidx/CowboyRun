using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RideRunGameData : Singleton<RideRunGameData>
{
    private bool isUpdated;
    public bool IsUpdated
    {
        get
        {
			return isUpdated;
        }
        set
        {
			isUpdated = value;
        }
    }
}
