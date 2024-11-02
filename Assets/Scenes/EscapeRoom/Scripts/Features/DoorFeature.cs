using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorFeature : BaseFeature
{
    [Header("Door Configuration")]
    [SerializeField]
    private Transform doorPivot;

    [SerializeField]
    private float maxAngle = 90.0f;

    [SerializeField]
    private bool reverseAngleDirection = false;

    [SerializeField]
    private float speed = 25f;

    [SerializeField]
    private bool open = false;

    [SerializeField]
    private bool makeItKinematicOnceOpened = false;

    [Header("Interaction Configuration")]
    [SerializeField]
    private XRSocketInteractor socketInteractor;

    [SerializeField]
    private XRSimpleInteractable simpleInteractable;
    private IEnumerator currCoroutine;
    private void Start()
    {
        // doors with sockets
        socketInteractor?.selectEntered.AddListener((s) =>
        {
            OpenDoor();
            PlayOnStarted();
        });
        socketInteractor?.selectExited.AddListener((s) =>
        {
            PlayOnEnded();
            socketInteractor.socketActive = featureUsage == FeatureUsage.Once ? true : false;
        });

        // doors with simple interactables for instance a locker
        simpleInteractable?.selectEntered.AddListener((s) =>
        {
            OpenDoor();
            PlayOnStarted();
        });
        simpleInteractable?.selectExited.AddListener((s) =>
        {
            PlayOnEnded();
        });

    }

    public void OpenDoor()
    {
        if (!open)
        {
            PlayOnStarted();
            open = true;
            currCoroutine = ProcessMotion();
            StartCoroutine(currCoroutine);
            StopInteractions();
        }
    }
    private IEnumerator ProcessMotion()
    {
        while (open)
        {
            var angle = doorPivot.localEulerAngles.y < 180 ? doorPivot.localEulerAngles.y : doorPivot.localEulerAngles.y - 360;

            angle = reverseAngleDirection ? Mathf.Abs(angle) : angle;

            if (angle <= maxAngle)
            {
                doorPivot.Rotate(Vector3.up, speed * Time.deltaTime * (reverseAngleDirection ? -1 : 1));
            }
            else
            {
                open = false;
                var featureRigidBody = GetComponent<Rigidbody>();
                if (featureRigidBody != null && makeItKinematicOnceOpened)
                    featureRigidBody.isKinematic = true;

            }
            yield return null;
        }


    }
    private void StopInteractions()
    {
        //disable interactable when opened
        if (simpleInteractable != null)
            simpleInteractable.enabled = false;
    }

}
