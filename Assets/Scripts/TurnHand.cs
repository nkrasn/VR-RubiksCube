using UnityEngine;
using System.Collections;

public class TurnHand : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    private RubiksCube rubiksCube;
    private HoldHand holdHand;

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

        rubiksCube = GameObject.FindGameObjectWithTag("RubiksCube").GetComponent<RubiksCube>();
        holdHand = GetComponent<HoldHand>();
    }

	void Update()
	{
        // Stop everything if input wasn't received
        if(!getInput())
            return;

        if(touchpadPressed || gripPressed)
        {
            // Find the closest center piece
            GameObject closestCenter = rubiksCube.getCenters()[0].getGameObject();
            float closestDist = (closestCenter.transform.position - device.transform.pos).magnitude;

            foreach(Cubie cubie in rubiksCube.getCenters())
            {
                float dist = (cubie.getGameObject().transform.position - transform.position).magnitude;
                Debug.Log("current cubie: " + cubie.getGameObject().name);
                Debug.Log("current dist: " + dist);
                if(dist < closestDist)
                {
                    closestDist = dist;
                    closestCenter = cubie.getGameObject();
                }
            }

            // Turn the side
            if(!rubiksCube.isTurning())
            {
                rubiksCube.parentSidePieces(closestCenter.transform.localPosition);
                if(touchpadPressed)
                    rubiksCube.beginTurn(true);
                else if(gripPressed)
                    rubiksCube.beginTurn(false);
            }

            // Say which piece is the closest
            Debug.Log(closestCenter.name);
        }
    }

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