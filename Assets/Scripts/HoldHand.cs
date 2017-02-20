using UnityEngine;
using System.Collections;

public class HoldHand : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    
    private GameObject rubiksCube;
    private bool holding;

    private FixedJoint joint;

    private bool itemSelected;

    // Input
    private bool triggerPressed, touchpadPressed;
    private bool triggerReleased, touchpadReleased;
    private bool triggerHeld, touchpadHeld;
    private bool gripPressed;
    private bool menuPressed;
    private int touchpadXDirection;

    void Start()
	{
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        
        rubiksCube = null;
        holding = false;

        joint = trackedObj.gameObject.AddComponent<FixedJoint>();
    }
	
	void Update()
	{
        // Stop everything if input wasn't received
        if(!getInput())
            return;

        // Grabbing
        if(triggerPressed && itemSelected)
        {
            if(rubiksCube != null)
                grab();
        }
        // Releasing
        if(triggerReleased && holding)
        {
            if(rubiksCube != null)
                release();
        }
    }



    // Grabbing logic
    public void grab()
    {
        holding = true;
        joint.connectedBody = rubiksCube.GetComponent<Rigidbody>();
    }

    // Releasing logic
    public void release()
    {
        holding = false;
        joint.connectedBody = null;

        // Make it thrown
        rubiksCube.GetComponent<Rigidbody>().velocity = device.velocity;
        rubiksCube.GetComponent<Rigidbody>().angularVelocity = device.angularVelocity;
    }



    // If an object is touched, record the object
    void OnTriggerEnter(Collider col)
    {
        if(!holding)
        {
            rubiksCube = col.gameObject;
            itemSelected = true;
        }
    }

    // If not touching an object anymore, say so
    void OnTriggerExit(Collider col)
    {
        itemSelected = false;
    }



    public bool isHolding() { return holding; }



    // Records input, returns true if input was able to be gathered
    private bool getInput()
    {
        // No device found, try finding it again
        if(device == null)
            device = SteamVR_Controller.Input((int)trackedObj.index);
        // If device still not found, try again next time
        if(device == null)
            return false;

        triggerPressed = device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
        triggerReleased = device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger);

        touchpadPressed = device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad);
        touchpadPressed = device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad);

        triggerHeld = device.GetPress(SteamVR_Controller.ButtonMask.Trigger);
        touchpadHeld = device.GetPress(SteamVR_Controller.ButtonMask.Touchpad);

        gripPressed = device.GetPressDown(SteamVR_Controller.ButtonMask.Grip);

        menuPressed = device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu);

        touchpadXDirection = (int)Mathf.Sign(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x);

        return true;
    }
}
