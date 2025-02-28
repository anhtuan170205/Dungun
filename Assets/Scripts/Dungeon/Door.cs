using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private BoxCollider2D doorCollider;
    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool isPreviouslyOpen = false; 
    private Animator animator;

    private void Awake()
    {
        doorCollider.enabled = false;

        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon) { OpenDoor(); }
    }
    private void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            isPreviouslyOpen = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            animator.SetBool(Settings.open, true);
        }
    }
    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        animator.SetBool(Settings.open, false);
    }
    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (isPreviouslyOpen)
        {
            isOpen = false;
            OpenDoor();
        }
    }

    private void OnEnable() 
    {
        animator.SetBool(Settings.open, isOpen);   
    }

    #region VALIDATION
    #if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
    #endif
    #endregion
}
