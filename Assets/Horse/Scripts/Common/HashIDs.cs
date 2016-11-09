using UnityEngine;
using System.Collections;

public class HashIDs : MonoBehaviour {

    [HideInInspector] public int jumpHash;
    [HideInInspector] public int speedHash;
    [HideInInspector] public int horizontalHash;
    [HideInInspector] public int shiftHash;
    [HideInInspector] public int galopingHash;
    [HideInInspector] public int trottingHash;
    [HideInInspector] public int deathHash;
    [HideInInspector] public int SwimHash;
    [HideInInspector] public int standHash;
    [HideInInspector] public int fowardHash;
    [HideInInspector] public int mountHash;
    [HideInInspector] public int mountSide;
    [HideInInspector] public int inclinationHash;
    [HideInInspector] public int fallingHash;
    [HideInInspector] public int fallingBackHash;
    [HideInInspector] public int sleepHash;
    [HideInInspector] public int attackHash;
    [HideInInspector] public int IntHash;
    [HideInInspector] public int FloatHash;

    void Awake()
    {
        jumpHash = Animator.StringToHash("Jumping");
        speedHash = Animator.StringToHash("Speed");
        horizontalHash = Animator.StringToHash("Horizontal");
        shiftHash = Animator.StringToHash("Shift");
        galopingHash = Animator.StringToHash("Galloping");
        trottingHash = Animator.StringToHash("Trotting");
        deathHash = Animator.StringToHash("Death");
        standHash = Animator.StringToHash("Stand");
        fowardHash = Animator.StringToHash("FowardPressed");
        mountHash = Animator.StringToHash("Mount");
        mountSide = Animator.StringToHash("MountSide");
        SwimHash = Animator.StringToHash("Swimming");
        inclinationHash = Animator.StringToHash("Inclination");
        fallingHash = Animator.StringToHash("Falling");
        fallingBackHash = Animator.StringToHash("FallingBack");
        sleepHash = Animator.StringToHash("Sleep");
        attackHash = Animator.StringToHash("HorseAttack");
        IntHash = Animator.StringToHash("HorseInt");
        FloatHash = Animator.StringToHash("HorseFloat");
    } 
	
}
