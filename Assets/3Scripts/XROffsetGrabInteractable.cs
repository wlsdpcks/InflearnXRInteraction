using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROffsetGrabInteractable : XRGrabInteractable
{
    private class Pose
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    private Dictionary<IXRSelectInteractor, Pose> savedPose = new Dictionary<IXRSelectInteractor, Pose>();

    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject;
        var interactorAttachTransform = interactor.GetAttachTransform(this);
        var interactableAttachTransform = attachTransform;

        var pose = new Pose();
        pose.position = interactorAttachTransform.localPosition;
        pose.rotation = interactorAttachTransform.localRotation;
        savedPose[interactor] = pose;

        var haveAttach = interactableAttachTransform != null;
        interactorAttachTransform.position = haveAttach ? interactableAttachTransform.position : rb.worldCenterOfMass;
        interactorAttachTransform.rotation = haveAttach ? interactableAttachTransform.rotation : rb.rotation;

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        var interactor = args.interactorObject;
        if (savedPose.TryGetValue(interactor, out Pose pose))
        {
            var interactorAttachTransform = interactor.GetAttachTransform(this);
            interactorAttachTransform.localPosition = pose.position;
            interactorAttachTransform.localRotation = pose.rotation;

            savedPose.Remove(interactor);
        }

        base.OnSelectExited(args);
    }
}