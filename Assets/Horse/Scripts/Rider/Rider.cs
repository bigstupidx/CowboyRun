using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(HashIDs))]
public class Rider : MonoBehaviour
{
    #region Variables

    [HideInInspector]
    public bool Mounted;
    [HideInInspector]
    public bool Can_Mount;
    [HideInInspector]
    public bool IsInHorse = false;
    [HideInInspector]
    public bool Mountedside;
    [HideInInspector]
    public HorseController HorseCntler;
    [HideInInspector]
    public Transform RiderLink;


    #region RiderComponents
    protected Animator anim;
    protected AnimatorStateInfo LayercurrentState;
    protected Rigidbody RigidRider;
    protected Collider colliderRider;
    protected HashIDs hashs;
    #endregion
    [Tooltip("Enable this if you are want to start mounted on a horse")]
    public bool StartMounted;
    [Tooltip("Reference the horse you want to start mounted on")]
    public HorseController HorseToMount;
    public KeyCode MountKey = KeyCode.F;


    #region Public Adjust Position and Rotation Variables when Riders mount the horse
    public Vector3 PositionOffset;
    public Vector3 RotationOffset;
    #endregion

    [Space]

    [Tooltip("Enable this if you are using Opsive or Invector 3rd Person Controller")]
    public bool CustomTPC;

    [Tooltip("Keep Referenced Scripts enable while is mounted on the horse")]
    public MonoBehaviour[] KeepActive;
   
    [HideInInspector]
    public bool stand;
    #endregion

    //--------------------------Setting the animator on the Root Game Object-------------------------------------------------------------------------
    protected void SetAnimator()
    {
        anim = GetComponent<Animator>();
        if (!anim.avatar)
        {
            foreach (Animator childAnimator in GetComponentsInChildren<Animator>())
            {
                if (childAnimator != anim)
                {
                    anim.avatar = childAnimator.avatar;
                    Destroy(childAnimator);
                    break;
                }
            }
        }
    }



    //--------------------------Mount Logic-----------------------------------------------------------------------------------------------------------    
    public virtual void EnableMounting()
    {

        //Send to the Horse controller that mounted is active
        Mounted = true;
        HorseCntler.Mounted = Mounted;
        HorseCntler.Mountedside = Mountedside;

        //Deactivate stuffs for the Rider's Rigid Body
        RigidRider.useGravity = false;
        RigidRider.isKinematic = true;
        //  RigidRider.constraints = RigidbodyConstraints.FreezeAll;
        colliderRider.enabled = false;
        IsInHorse = true;

        //Linking the Rider to the horse
        transform.parent = RiderLink;
        transform.position = RiderLink.position;
        transform.localPosition = transform.localPosition + PositionOffset;
    }

    //------------------------- UnMount Logic---------------------------------------------------------------------------------------------------------    
    public virtual void DisableMounting(Vector3 Laspos)
    {
        Mounted = false;

        //Send to the horse he is no longer mounted
        HorseCntler.Mounted = Mounted;
        HorseCntler.Mountedside = Mountedside;

        //Unlinking the rider to the horse
        transform.parent = null;


        //Reactivate stuffs for the Rider's Rigid Body
        RigidRider.useGravity = true;
        RigidRider.isKinematic = false;
        colliderRider.enabled = true;
        IsInHorse = false;

    }

    //-----------------------------------IN HERE (DE)ACTIVATE THE 3RD PERSON COMPONENTS---------------------------------------------------------------
    protected void ActivateComponents(bool enabled)
    {
        MonoBehaviour[] AllComponents = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour item in AllComponents)
        {
            //if you want to add a Script to not be deactivate while Mounting Added in here
            if (!(item is Rider) && !(item is HashIDs))
            {
                bool keepactive = false;

                //Keep active custom scripts
                foreach (MonoBehaviour keep in KeepActive)
                {
                    if (item == keep)
                    {
                        keepactive = true;
                        break;
                    }
                }
                if (!keepactive)
                {
                    item.enabled = enabled;
                }
            }
        }
    }

    // ----------------------------------------------Find a horse when triggers the mount colliders on a near horse---------------------------------------
    public virtual void findHorse(HorseController horse)
    {
        HorseCntler = horse;
        if (HorseCntler == null)
        {
            RiderLink = null;
        }
        else
        {
            RiderLink = HorseCntler.RidersLink;
        }
    }

}


