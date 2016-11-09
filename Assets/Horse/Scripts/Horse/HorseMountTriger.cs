using UnityEngine;
using System.Collections;

public class HorseMountTriger : MonoBehaviour {

    public bool LeftSide;
    HorseController HorseC;
    RiderControllerMounted RCM;
    Rider RFPS;


    // Use this for initialization
    void Start() {
        HorseC = GetComponentInParent<HorseController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!HorseC.Mounted)
        {
            if (other.GetComponent<RiderControllerMounted>())  //Old Script
            {
                RCM = other.GetComponent<RiderControllerMounted>(); 

                RCM.Can_Mount = true;
                RCM.findHorse(HorseC);

                if (LeftSide) RCM.Mountedside = true;
                else RCM.Mountedside = false;
            }

            if (other.GetComponent<Rider>())
            {
                RFPS = other.GetComponent<Rider>();
                RFPS.Can_Mount = true;
                RFPS.findHorse(HorseC);

                if (LeftSide) RFPS.Mountedside = true;
                else RFPS.Mountedside = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (RCM != null)
        {
            if (RCM == other.GetComponent<RiderControllerMounted>())
            {
                RCM = other.GetComponent<RiderControllerMounted>();

                if (RCM.Can_Mount)
                {
                    RCM.Can_Mount = false;
                }
                RCM.findHorse(null);

                RCM = null;
            }
        }

        if (RFPS != null)
        {
            if (RFPS == other.GetComponent<Rider>())
            {
                RFPS = other.GetComponent<Rider>();

                if (RFPS.Can_Mount)
                {
                    RFPS.Can_Mount = false;
                }
                RFPS.findHorse(null);

                RFPS = null;
            }
        }
    }
}
