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

	public ArrayList horseVect = new ArrayList ();

	public ArrayList GetHorses(){
		return horseVect;
	}

	public void AddHorse(GameObject horse){
		if (horse != null) {
			horseVect.Add (horse);
		}

	}

	public void RemoveHorse(GameObject horse){
		if (horse != null) {
			horseVect.Remove (horse);
		}
	}
}
