using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;


[RequireComponent(typeof(HashIDs))]
public class HorseController : MonoBehaviour
{
    public enum HorseMeshType
    {
        Horse1 = 0, Horse2 = 1, BigHorse = 2, PolyArt = 3, MineCraft = 4, Custom = 5
    }
    //float normalizedtime;
    #region Variables 

    #region Connection with Rider Variables

    [Tooltip("Enable this if you want to control the horse alone, without requiring mounting")]
    public bool PlayerControlled;

    [Tooltip("This activate the animator layer that contains the horse's bones offset, in order to use the same animation. Custom is for use your own Horse Model")]
    public HorseMeshType horseType;

    [HideInInspector]
    public bool Mounted;
    [HideInInspector]
    public bool Mountedside;
    [HideInInspector]
    public Animator RiderAnimator; // Rider's Animatorto control both Sync animators from here
    #endregion

    #region Horse Components 
    private Animator anim;
    private Rigidbody horseRigidBody;
    private BoxCollider horseCollider;
    private HashIDs hashs;  //Hash ID For the animator parameters
    private float scaleFactor;
    #endregion

    #region Animator Parameters Variables
    private float speed;
    private float direction;
    private float MaxSpeed = 1f; //1 Walking, 2 Trotting, 3 Cantering, 4 Galloping, 5 Sprinting


    //States Variables
    private bool
        galoping,
        trotting,
        swimming,
        flying, // Coming Soon Pegassus
        falling,
        fallingback,
        sleep,
        jump,
        Shift, //Cantering ,Sprint, turn 180
        attack,
        IsInWaterHip,
        IsInWater,
        stand,
        fowardPressed = false;


    private int sleepCount, horseInt;
    private float horizontal, vertical, horsefloat = 0, MaxHeight;
    private float jumpingCurve;
    #endregion


    [Header("Custom Options")]

    [Tooltip("Add more rotation to the turn animations")]
    [Range(0, 100)]
    public float TurnSpeed = 5;

    [Tooltip("Amount of idle cycles before go to sleep state")]
    [Range(1, 100)]
    public int GoToSleep = 10;

    [Tooltip("Max Distance for the horse to recover from a big fall, Greater value: The horse will still running after falling from a great height")]
    [Range(1, 100)]
    public float recoverByFallDist;

    [Tooltip("Smoothness for snapping to ground, Higher value Harder Snap")]
    [Range(1, 100)]
    public float SnapToGround = 20f;


    [Header("Swim Options")]
    [Range(1, 10)]
    public float swimSpeed = 1f;
    [Range(1, 100)]
    public float SwimTurnSpeed = 1f;



    #region FixTransform_Variables
    RaycastHit hit_Hip, hit_Chest, WaterHitCenter;
    Vector3 Horse_Hip, Horse_Chest;
    Vector3 ErrorTresh = Vector3.up * 0.5f;
    float distanceHip;
    const float fallingMultiplier = 1.6f;
    float heightHorse;
    float WaterDistanceHip, distanceChest;
    float angleTerrain, inclination, fallheight;
    Pivots[] pivots;
    private Vector3 ColliderCenter;
    private Vector3 rigidVelocity;
    #endregion

    [Header("Keys for Horse Speed")]

    public KeyCode Walk = KeyCode.Alpha1;
    public KeyCode Trot = KeyCode.Alpha2;
    public KeyCode Gallop = KeyCode.Alpha3;

    [Header("Reference For Rider IKs Points")]
    #region Transform feet and mount on the Horse
    public Transform RidersLink;   // Reference for the RidersLink Bone
    public Transform LeftIK;       // Reference for the LeftFoot correct position on the mount
    public Transform RightIK;      // Reference for the RightFoot correct position on the mount
    public Transform LeftKnee;     // Reference for the LeftKnee correct position on the mount
    public Transform RightKnee;    // Reference for the RightKnee correct position on the mount
    #endregion

    #endregion

    #region Properies       
    public bool Stand
    {
        get { return this.stand; }
    }
    public float MaxFallHeight
    {
        set { fallheight = value; }
        get { return this.fallheight; }
    }
    public bool Sleep

    {
        set { sleep = value; }
    }
    public int SleepCount
    {
        set { sleepCount = value; }
        get { return this.sleepCount; }
    }
    public bool isFalling
    {
        // set { falling = value; }
        get { return this.falling; }
    }
    public int HorseInt
    {
        set { horseInt = value; }
        get { return this.horseInt; }
    }

    public float HorseFloat
    {
        set { horsefloat = value; }
        get { return this.horsefloat; }
    }
    #endregion


    // Use this for initialization
    void Start()
    {
       // Cursor.visible = false;
        anim = GetComponent<Animator>();
        hashs = GetComponent<HashIDs>();
        
        horseCollider = GetComponent<BoxCollider>();
        horseRigidBody = GetComponent<Rigidbody>();

        ColliderCenter = horseCollider.center;

        pivots = GetComponentsInChildren<Pivots>(); //Pivots are Strategically Transform objects use to cast rays used by the horse

        scaleFactor = transform.localScale.y;  //TOTALLY SCALABE HORSE


        heightHorse = pivots[0].transform.localPosition.y * scaleFactor; // Height from Chest to ground

        anim.SetInteger("HorseType", (int)horseType); //Adjust the layer for the curret horse Type

        horseRigidBody.drag = 0;
        horseRigidBody.angularDrag = 25f;
        sleepCount = 0;

        //
    }
    //----------------------Link the buttons pressed with correspond variables-------------------------------------------------------------------------------
    void getButtons()
    {
        fowardPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow); //If foward is pressed

        #if UNITY_ANDROID || UNITY_IOS

                if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
                    fowardPressed = true;

                else fowardPressed = false;

        #endif

        attack = CrossPlatformInputManager.GetButtonDown("Fire1");            //Get the Attack button

        if (attack)
        {
            horseInt = Random.Range(0, 3);                                         //Random Attack ID: ID identify the type of attack
            sleepCount = 0;
        }


        horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        vertical = CrossPlatformInputManager.GetAxis("Vertical");

        Shift = CrossPlatformInputManager.GetButton("Fire3");
       // Shift = Input.GetKey(KeyCode.LeftShift);
        
        #if UNITY_ANDROID || UNITY_IOS
         
        #endif

        //Get the Sprint/Cantering/Turn180 button
        jump = CrossPlatformInputManager.GetButtonDown("Jump");             //Get the Jump button

        if (Input.GetKeyDown(KeyCode.K))                                    //Get the Death button
        {
            horseInt = Random.Range(0, 2);
        }
    }
    //----------------------linking  the Parameters Horse && Rider------------------------------------------------------------------------

    void LinkingAnimator(Animator anim_)
    {
        jumpingCurve = anim.GetFloat("JumpCurve");
        anim_.SetFloat(hashs.speedHash, speed*vertical);
        anim_.SetFloat(hashs.horizontalHash, direction);
        anim_.SetBool(hashs.shiftHash, Shift);
        anim_.SetBool(hashs.galopingHash, galoping);
        anim_.SetBool(hashs.trottingHash, trotting);
        anim_.SetBool(hashs.standHash, stand);
        anim_.SetBool(hashs.jumpHash, jump);
        anim_.SetBool(hashs.fowardHash, fowardPressed);
        anim_.SetBool(hashs.SwimHash, swimming);
        anim_.SetFloat(hashs.inclinationHash, inclination);
        anim_.SetBool(hashs.fallingHash, falling);
        anim_.SetBool(hashs.fallingBackHash, fallingback);
        anim_.SetBool(hashs.sleepHash, sleep);
        anim_.SetBool(hashs.attackHash, attack);
        anim_.SetInteger(hashs.IntHash, horseInt);
        

        if (falling || isJumping(1,true))
        {
            anim_.SetFloat(hashs.FloatHash, horsefloat);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            anim_.SetTrigger(hashs.deathHash);
        }



    }

    //--Add more Rotations to the current turn animations  using the public turnSpeed float--------------------------------------------
    void TurnAmount()
    {
        //For going Foward and Backward
        if (vertical >= 0)
        {
           
            if (!swimming)
                transform.Rotate(transform.up, TurnSpeed * 3 * horizontal * Time.deltaTime);
            else
                transform.Rotate(transform.up, SwimTurnSpeed * 3 * horizontal * Time.deltaTime);

            //horseRigidBody.AddTorque((transform.up * horizontal) * TurnSpeed / 2 / Time.deltaTime);
        }
        else
        {
            if (!swimming)
            {
                transform.Rotate(transform.up, TurnSpeed * 3 * -horizontal * Time.deltaTime);
            }
            else
            {
                transform.Rotate(transform.up, SwimTurnSpeed * 3 * -horizontal * Time.deltaTime);
            }
            // horseRigidBody.AddTorque((transform.up * -horizontal) * TurnSpeed / 2 / Time.deltaTime);
        }

        //Add Speed to current animation foward swim
        if (swimming && vertical>0)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * swimSpeed * vertical/2, Time.deltaTime);
        }
    }



    //--------The Collider is not touching the floor to avoid collision with small objects, Here the Height is fixed , Terrain Compatible----------
    void FixPosition()
    {
        //Position to cast a ray downward from the  Horse Chest and Horse Hip.
        Horse_Hip = pivots[0].transform.position;
        Horse_Chest = pivots[1].transform.position;

        inclination = ((Horse_Chest.y - Horse_Hip.y) / 2) / scaleFactor;  //Calculate the inclination between front and rear legs\

        Vector3 JumpingCurve = new Vector3(0, jumpingCurve, 0);

        //Ray From Horse Hip to the ground
        if (Physics.Raycast(Horse_Hip + JumpingCurve, -transform.up, out hit_Hip, (heightHorse + jumpingCurve) * fallingMultiplier * scaleFactor, LayerMask.GetMask("Default")))
        {
            Debug.DrawRay(Horse_Hip + JumpingCurve, -transform.up * heightHorse * scaleFactor * fallingMultiplier, Color.green);
            distanceHip = hit_Hip.distance - jumpingCurve;
        }
        else
        {
            fallingback = true;
        }


        //Ray From Horse Chest to the ground
        if (Physics.Raycast(Horse_Chest + JumpingCurve, -transform.up, out hit_Chest, (heightHorse + jumpingCurve) * fallingMultiplier * scaleFactor, LayerMask.GetMask("Default")))
        {
            distanceChest = hit_Chest.distance - jumpingCurve;
            Debug.DrawRay(Horse_Chest + JumpingCurve, -transform.up * heightHorse * scaleFactor * fallingMultiplier, Color.green);

        }
        else
        {
           falling = true;
           
        }




        //Front RayWater Cast
        if (Physics.Raycast(horseCollider.bounds.center + ErrorTresh+ JumpingCurve, -transform.up, out WaterHitCenter, heightHorse * scaleFactor*2, LayerMask.GetMask("Water")))
        {
            Debug.DrawRay(horseCollider.bounds.center + ErrorTresh + JumpingCurve, -transform.up * heightHorse * scaleFactor * 2, Color.blue);
           
            //if there nothing on top of the water
            if (hit_Chest.distance>WaterHitCenter.distance)
            {
                IsInWater = true;
            }
        }
        else
        {
            IsInWater = false;
        }




        //If the horse has  water near the Neck then start swimming
        if (WaterHitCenter.transform) //if we hit water
        {
            if (Horse_Chest.y < WaterHitCenter.transform.position.y && IsInWater)
            {
                swimming = true;
                falling = false;
            }
            if (IsInWater && distanceChest < 1f * scaleFactor)  //else stop swimming when he is coming out of the water
            {
               swimming = false;
               MaxSpeed = 1f;
               galoping = false;
               trotting = false;
            }
        }



        //-----------------------------water Adjusment-------------------------------------------
        if (swimming)
        {
            falling = false;
            fallingback = false;
            horseRigidBody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;

            float angleWater = Vector3.Angle(transform.up, Vector3.up);

            Quaternion finalRot = Quaternion.FromToRotation(transform.up, Vector3.up) * horseRigidBody.rotation;

            //Smoothy rotate until is Aling with the Water
            if (angleWater > 0.1f)
                transform.rotation = Quaternion.Lerp(transform.rotation, finalRot, Time.deltaTime * 10);
            else
                transform.rotation = finalRot;

            //Smoothy Move until is Aling with the Water
            if (WaterHitCenter.transform)
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, WaterHitCenter.transform.position.y - heightHorse, transform.position.z), Time.deltaTime * 5f);
        }
        else
        {
            //------------------------------------------------Terrain Adjusment-----------------------------------------

            //----------Calculate the Align vector of the terrain------------------
            Vector3 direction = (hit_Chest.point - hit_Hip.point).normalized;
            Vector3 Side = Vector3.Cross(Vector3.up, direction).normalized;
            Vector3 SurfaceNormal = Vector3.Cross(direction, Side).normalized;
            angleTerrain = Vector3.Angle(transform.up, SurfaceNormal);

            // ------------------------------------------Orient To Terrain-----------------------------------------  
            Quaternion finalRot = Quaternion.FromToRotation(transform.up, SurfaceNormal) * horseRigidBody.rotation;

            // If the horse is falling smoothly aling with the horizontal
            if ((isJumping(0.55f, true)) || fallingback)
            {
                finalRot = Quaternion.FromToRotation(transform.up, Vector3.up) * horseRigidBody.rotation;
                transform.rotation = Quaternion.Lerp(transform.rotation, finalRot, Time.deltaTime * 5f);
            }
            else
            {
                // if the terrain changes hard smoothly adjust to the terrain 
                if (angleTerrain > 2 && !stand)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, finalRot, Time.deltaTime * 10f);
                }
                else
                {
                    transform.rotation = finalRot;
                }
            }

            float distance = distanceHip;
            float realsnap = SnapToGround; // changue in the inspector the  adjusting speed for the terrain

            if (isJumping(0.5f, false)) //Calculate from the front legss
            {
                //calculate the adjust distance from the front legs if is finishing jumping. Solution for jumping High places at the same distance
                 distance = distanceChest;
                realsnap = SnapToGround / 4;
            }

            //-----------------------------------------Snap To Terrain-------------------------------------------
            if (distance != heightHorse)
            {
                float diference = heightHorse - distance;
                transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, diference, 0), Time.deltaTime * realsnap);
            }
        }
    }

    public void Falling()
    {
        //if the horse stay stucked while falling move foward  ... basic solution
        if (falling && horseRigidBody.velocity.magnitude<0.1 && !swimming)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward*2, Time.deltaTime);
        }


        RaycastHit offGround; //Front Falling Ray

        Vector3 FallingVectorFront = pivots[2].transform.position; // get the Transform Falling from vector within the horse hierarchy
        Debug.DrawRay(FallingVectorFront, -transform.up * heightHorse * fallingMultiplier, Color.blue);

        //Ignore Layer Horse 20
        var layermask = 1 << 20;
        layermask = ~layermask;

        if (Physics.Raycast(FallingVectorFront, -transform.up, out offGround, heightHorse * fallingMultiplier,layermask))
        {
            //---------------------Finding the water while falling------------------------

            if (offGround.transform.gameObject.layer == 4) //4 is Water layer
            {
                if (offGround.transform) //if we hit water
                {
                    IsInWater = true;
                    if (((FallingVectorFront.y - offGround.point.y) < heightHorse*scaleFactor) && IsInWater && falling)
                    {
                        swimming = true;
                        falling = false;
                    }
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("JumpingDown") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime>0.5f)
                    {
                        swimming = true;
                       falling = false;
                    }
                    
                }
            }
            else
            {
                IsInWater = false;
            }

            if ((offGround.distance <= heightHorse * fallingMultiplier) && !IsInWater)
            {
                if (falling)
                {
                    falling = false;

                    

                    if (MaxFallHeight - transform.position.y < recoverByFallDist * scaleFactor) horseInt = -1;  // If the fall distance is too big changue to recoverfall animation
                    else horseInt = 0;
                }

                    horseRigidBody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;
            }
        }
        else
        {
            if (!IsInWater)
            {
                falling = true;

                //--------------------Fall Blend for Stretch Front Legs while falling-----------------
                if (falling)
                {
                    RaycastHit HeightDistance;

                    if (Physics.Raycast(FallingVectorFront, -transform.up, out HeightDistance, 100f))
                    {
                        if (MaxHeight < HeightDistance.distance)
                        {
                            MaxHeight = HeightDistance.distance; //Calculate the MaxDistance  from the horse.
                        }

                        //Fall Blend between fall animations
                        horsefloat = Mathf.Lerp(horsefloat, HeightDistance.distance / (MaxHeight - heightHorse), Time.deltaTime * 10f);
                    }
                }
               
                //------------------------------------------------------------------------------------
                if (!isJumping(0.50f, true)) //deactivate when the horse is jumping on his High jump altitude
                {
                    horseRigidBody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
                }
            }
        }

     

        //recover the original rotation in air
        if (falling || anim.GetCurrentAnimatorStateInfo(0).IsName("JumpingDown"))
        {
            Quaternion finalRot = Quaternion.FromToRotation(transform.up, Vector3.up) * horseRigidBody.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, finalRot, Time.fixedDeltaTime * 5);
        }
    }


    //--------------------Adjusting the Capsule Collider when the horse Jump----------------------------------------
    void JumpingCollider()
    {
        horseCollider.center = ColliderCenter + new Vector3(0, anim.GetFloat("JumpCurve"), 0);
    }

    //--------------------------Check if the horse is in the Jumping State------------------------------------------
    bool isJumping( float normalizedtime, bool half)
    {
        if (half)  //if is jumping the first half
        {

            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Jump"))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < normalizedtime)
                    return true;
            }

            if (anim.GetNextAnimatorStateInfo(0).IsTag("Jump"))
            {
                if (anim.GetNextAnimatorStateInfo(0).normalizedTime < normalizedtime)
                    return true;
            }
        }
        else //if is jumping the second half
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Jump"))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > normalizedtime)
                    return true;
            }

            if (anim.GetNextAnimatorStateInfo(0).IsTag("Jump"))
            {
                if (anim.GetNextAnimatorStateInfo(0).normalizedTime > normalizedtime)
                    return true;
            }
        }


        return false;

    }

    //--------------------------Prevent From falling while going backwards from a cliff------------------------------------------
    public void FallingBackPrevent()
    {
        RaycastHit BackRay;
        Vector3 FallingVectorBack = pivots[3].transform.position; 

        if (Physics.Raycast(FallingVectorBack, -Vector3.up, out BackRay, heightHorse * scaleFactor * 1.7f, LayerMask.GetMask("Default")))
        {
            fallingback = false;
        }
        else
        {
            if (!swimming)
            {
                fallingback = true;
                if (speed * vertical < 0)
                    speed = -0.10f;
            }
        }

        //prevent to go uphill
        if (inclination > 0.3 && speed * vertical > 0)
        {
            speed = Mathf.Lerp(speed, 0, Time.fixedDeltaTime * 1f); ;
        }

        if (inclination > 0.2 && speed * vertical > 0)
        {
            speed = Mathf.Lerp(speed, 1, Time.fixedDeltaTime * 1f); ;
        }



    }

    //--------------------------------------------------------------------------------------------------------------------------
    void FixedUpdate()
    {
        JumpingCollider();

        Falling();

        if (!falling)
        {
            FixPosition();
            if (Mounted || PlayerControlled)
            {
                TurnAmount();
            }
        }

        FallingBackPrevent();
    }



    void MovementSystem()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        if (Input.GetKeyDown(KeyCode.Alpha1))       //Walking
        {
            MaxSpeed = 1f;
            galoping = false;
            trotting = false;
        }

        if ((horizontal != 0) || (vertical != 0))
            stand = false;
        else stand = true;

        if (Input.GetKeyDown(KeyCode.Alpha2))       //Trotting
        {
            MaxSpeed = 2f;
            galoping = false;
            trotting = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))       //Galloping
        {
            MaxSpeed = 3f;
            galoping = true;
            trotting = false;
        }

        if (Shift)
        {
            if (trotting) MaxSpeed = 2.5f;   //Cantering
            if (galoping) MaxSpeed = 4f;     //Sprinting 
        }
        else
        {
            if (trotting) MaxSpeed = 2f;     //Stop Cantering
            if (galoping) MaxSpeed = 3f;     //Stop Sprinting      
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (CrossPlatformInputManager.GetButtonDown("One")) //Needed to add on Edit/ProjectSettings/Input   "One"  
        {
            MaxSpeed = 1f;
            galoping = false;
            trotting = false;
        }

        if ((horizontal != 0) || (vertical != 0))
            stand = false;
        else stand = true;

        if (CrossPlatformInputManager.GetButtonDown("Two")) //Needed to add on Edit/ProjectSettings/Input   "Two"  
        {
            MaxSpeed = 2f;
            galoping = false;
            trotting = true;
        }

        if (CrossPlatformInputManager.GetButtonDown("Three")) //Needed to add on Edit/ProjectSettings/Input   "Three"  
        {
            MaxSpeed = 3f;
            galoping = true;
            trotting = false;
        }

        if (Shift)
        {
            if (trotting) MaxSpeed = 2.5f;   //Cantering
            if (galoping) MaxSpeed = 4f;     //Sprinting 
        }
        else
        {
            if (trotting) MaxSpeed = 2f;     //Stop Cantering
            if (galoping) MaxSpeed = 3f;     //Stop Sprinting      
        }
#endif

        speed = Mathf.Lerp(speed, MaxSpeed, Time.deltaTime * 2);            //smoothly transitions bettwen velocities
        direction = Mathf.Lerp(direction, horizontal, Time.deltaTime * 8);  //smoothly transitions bettwen directions


        if (!stand)
        {
            sleepCount = 0; // Reset the sleep conter
        }



        LinkingAnimator(anim);
        //Control the Rider Animator from here...  Syncs animations
        if (RiderAnimator)
        {
            LinkingAnimator(RiderAnimator);
        }
    }

    void Update()
    {
        

        //Wake the Horse Up
        if (sleep && fowardPressed)
        {
            sleep = false;
        }

        getButtons(); //GET the Input Buttons
        if (PlayerControlled)
        {
            MovementSystem();
        }
        else if (Mounted || PlayerControlled)
        {
            if (RiderAnimator == null)
            {
                if (GetComponentInChildren<RiderControllerMounted>())
                    RiderAnimator = GetComponentInChildren<RiderControllerMounted>().transform.GetComponent<Animator>();
                //---------------------------------------------------------------------------------New Script
                if (GetComponentInChildren<Rider>())
                    RiderAnimator = GetComponentInChildren<Rider>().transform.GetComponent<Animator>();
            }
            else
            {   // if the riders finish mounting
                if (!RiderAnimator.GetCurrentAnimatorStateInfo(RiderAnimator.GetLayerIndex("Mounted")).IsTag("Mounting"))
                {
                    MovementSystem();
                }
            }
        }
        else if (RiderAnimator != null)
        {
            if (!RiderAnimator.GetCurrentAnimatorStateInfo(RiderAnimator.GetLayerIndex("Mounted")).IsTag("Unmounting"))
            {
                RiderAnimator = null;
            }
        }
    }



}
