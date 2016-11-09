using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(HashIDs))]
public class RiderControllerMounted : MonoBehaviour
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

    [Tooltip("Leave Empty if you have a Custom Camera Rig")]
    public UnityStandardAssets.Cameras.FreeLookCam CharacterCamera; 

    #region RiderComponents
    private Animator anim;
    private AnimatorStateInfo LayercurrentState;
    private Rigidbody RigidRider;
    private CapsuleCollider Col;
    private HashIDs hashs;
    #endregion

    int MountLayer;
    Vector3 Lastpos;
    int count = 10;
    public KeyCode MountKey;
    #region Public Adjust Position and Rotation Variables when Riders mount the horse
    public Vector3 PositionOffset;
    public Vector3 RotationOffset;
    #endregion


    #region IK VARIABLES    
    Transform leftIKFoot = null;
    Transform rightIKFoot = null;
    Transform leftIKKnee = null;
    Transform rightIKKnee = null;

    float leftIKFootWeight = 0f;
    float rightIKFootWeight = 0f;
    #endregion


    private bool stand = true;
    #endregion

    //----------------------------Use this for initialization---------------------------------------------------------------------------------------
    void Start()
    {
        SetAnimator();
        hashs = GetComponent<HashIDs>();
        Col = GetComponent<CapsuleCollider>();
        RigidRider = GetComponent<Rigidbody>();
        MountLayer = anim.GetLayerIndex("Mounted");

        if (CharacterCamera)
        {
            CharacterCamera.SetTarget(transform);
        }
    }

    //--------------------------Setting the animator on the Root Game Object-------------------------------------------------------------------------
    void SetAnimator()
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
    public void EnableMounting()
    {
        //Deactivate stuffs for the Rider's Rigid Body

        RigidRider.useGravity = false;
        RigidRider.isKinematic = true;
        RigidRider.constraints = RigidbodyConstraints.FreezeAll;
        Col.enabled = false;
     

        IsInHorse = true;

        //Linking the Rider to the horse
        transform.parent = RiderLink;
        transform.position = RiderLink.position + PositionOffset;

        //Getting the correct Rotation for the rider
        transform.rotation = RiderLink.rotation;
        transform.Rotate(new Vector3(0, -90, -90));
        transform.Rotate(RotationOffset);

        //disable ThirdPersonControllers
        ThirdPerson(false);

        //Camera Change to horse 
       if (CharacterCamera) CharacterCamera.SetTarget(HorseCntler.transform);
    }

    //------------------------- UnMount Logic---------------------------------------------------------------------------------------------------------    
    public void DisableMounting()
    {
       
        Mounted = false;

        //Send to the horse he is no longer mounted
        HorseCntler.Mounted = Mounted;
        HorseCntler.Mountedside = Mountedside;

        //unlinking the rider to the horse
        transform.parent = null;


        //LastPosition on the ground when finish unmounting
        Ray LowerPoint = new Ray(Lastpos + Vector3.up, -Vector3.up);
        RaycastHit hitray;

        if (Physics.Raycast(LowerPoint, out hitray, 2f))
        {
            transform.position = new Vector3(Lastpos.x, hitray.point.y, Lastpos.z);
        }
        else transform.position = Lastpos;
       
        //Rotation looking the horse
        Vector3 link = HorseCntler.RidersLink.position;
        Vector3 direction = new Vector3(link.x, transform.position.y, link.z);
        transform.LookAt(direction, Vector3.up);


        //Camera Change to Player
        if (CharacterCamera) CharacterCamera.SetTarget(transform);

        //Reactivate stuffs for the Rider's Rigid Body
        RigidRider.useGravity = true;
        RigidRider.isKinematic = false;
        RigidRider.constraints = RigidbodyConstraints.None;
        RigidRider.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        Col.enabled = true;
        RigidRider.Sleep();

        //Enable ThirdPersonControllers

        ThirdPerson(true);


        count = 0;
        IsInHorse = false;
        anim.SetLayerWeight(MountLayer, 0);
    }

    //-----------------------------------IN HERE (DE)ACTIVATE THE 3RD PERSON CONTROLLER---------------------------------------------------------------
    void ThirdPerson(bool enabled)
    {
        MonoBehaviour[] AllComponents = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour item in AllComponents)
        {
            //if you want to add a Script to not be deactivate while Mounting Added in here
            if (!(item is RiderControllerMounted) && !(item is HashIDs))
            {
                item.enabled = enabled;
            }
        }
    }

    //----------"Unmount Left" and "Unmount Right" Clips Send A message with the last lower positions of the feet--------------------------------------
    public void Down()
    {
        Lastpos = anim.pivotPosition;
    }

    //--------------------------------Checking Rays Visually--------------------------------------------------------------------------------------------
    public void DrawRays()
    {
        Debug.DrawRay(anim.pivotPosition, Vector3.down, Color.blue);
    }

    // -------------Get the rigid body on sleep for remove unwanted flicker rigid positions, up to 3 frames
    void stybilizeUnmountPosition()
    {
        if (count < 2)
        {
            RigidRider.Sleep();
            transform.position = new Vector3(Lastpos.x, transform.position.y, Lastpos.z);
            count++;
        }
    }

    //--------------------------------------------------Update------------------------------------------------------------------------------------------
    void Update()
    {
        DrawRays();  //for debuging

      
        stybilizeUnmountPosition();

        stand = anim.GetBool(hashs.standHash);
        //set the side where I was Mounted

        if (HorseCntler != null)
        {
            if (Can_Mount && !Mounted)
            { 
                #if !UNITY_ANDROID && !UNITY_IOS
                if (Input.GetKeyDown(MountKey))
                {
                    //Send to the Horse controller that mounted is active
                    Mounted = true;
                    HorseCntler.Mounted = Mounted;
                    HorseCntler.Mountedside = Mountedside;
                    anim.SetLayerWeight(MountLayer, 1);
                }
                #endif

                #if UNITY_ANDROID || UNITY_IOS
                if (CrossPlatformInputManager.GetButtonDown("Mount")) //Needed to add on Edit/ProjectSettings/Input   "Mount"  
                {
                    Mounted = true;
                    HorseCntler.Mounted = true;
                    HorseCntler.Mountedside = Mountedside;
                }
                #endif
            }
            else
            {
                if (Mounted)
                {
                    #if !UNITY_ANDROID || !UNITY_IOS
                    if (Input.GetKeyDown(MountKey) && stand)
                    {
                        Mounted = false;
                        HorseCntler.Mounted = false;
                    }
                    #endif


                    #if UNITY_ANDROID || UNITY_IOS
                    if (CrossPlatformInputManager.GetButtonDown("Mount") && stand) //Needed to add on Edit/ProjectSettings/Input   "Mount"  
                    {
                        Mounted = false;
                        HorseCntler.Mounted = false;
                    }
                    #endif
                }
            }
            anim.SetBool("Mount", Mounted);
            anim.SetBool("MountSide", Mountedside);
            
            
        }
    }

    // ----------------------------------------------Find a horse when triggers the mount colliders on a near horse---------------------------------------
    public void findHorse(HorseController horse)
    {
        HorseCntler = horse;
        if (HorseCntler == null)
        {
            RiderLink = null;
            leftIKFoot = null;
            rightIKFoot = null;
            leftIKKnee = null;
            rightIKKnee = null;
        }
        else
        {
            RiderLink = HorseCntler.RidersLink;
            leftIKFoot = HorseCntler.LeftIK;
            rightIKFoot = HorseCntler.RightIK;
            leftIKKnee = HorseCntler.LeftKnee;
            rightIKKnee = HorseCntler.RightKnee;
        }
    }

    //----------------------IK Feet Adjusment-------------------------------------------------------------------------------------------------------------
    void OnAnimatorIK()
    {
        if (HorseCntler != null)
        {
            //linking the weights to the animator
            if (IsInHorse)
            {
                leftIKFootWeight = 1f;
                rightIKFootWeight = 1f;

                if (anim.GetCurrentAnimatorStateInfo(MountLayer).IsTag("Mounting") || anim.GetCurrentAnimatorStateInfo(MountLayer).IsTag("Unmounting"))
                {
                    leftIKFootWeight = anim.GetFloat("IKLeftFoot");
                    rightIKFootWeight = anim.GetFloat("IKRightFoot");
                }
                if (anim.GetNextAnimatorStateInfo(MountLayer).IsName("Idle Blend"))
                {
                    leftIKFootWeight = 1f;
                    rightIKFootWeight = 1f;
                }

                //setting the weight
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftIKFootWeight);
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightIKFootWeight);

                anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftIKFootWeight);
                anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightIKFootWeight);


                //Knees
                anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftIKFootWeight);
                anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightIKFootWeight);

                //setting the IK Positions
                anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftIKFoot.position);
                anim.SetIKPosition(AvatarIKGoal.RightFoot, rightIKFoot.position);

                //Knees
                anim.SetIKHintPosition(AvatarIKHint.LeftKnee, leftIKKnee.position);
                anim.SetIKHintPosition(AvatarIKHint.RightKnee, rightIKKnee.position);




                //setting the IK Rotations
                anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftIKFoot.rotation);
                anim.SetIKRotation(AvatarIKGoal.RightFoot, rightIKFoot.rotation);


            }
        }
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);

            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
        }
    }

}


