using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2_CameraMovement : MonoBehaviour
{
    public bool moving = false; // camera is moving or standing still

    private Vector3 position_origin; // no need to set it, it will be the camera's current pos
    public Vector3 position_destination;

    private float lerp_duration = 10f; // time it takes to go from posO to posD (and viceversa) [DON'T SET IT TO 0]
    // I ended up making the lerp smoother with a "pseudo-bellcurve" (hardcoded), so the "duration" is not precise anymore
    private float progress = 0f; // internal clock to handle lerp progress towards a position [0..1]
    private bool moving_forward = true; // true or false, depending on which pos we're moving towards

    private 


    // Start is called before the first frame update
    void Start()
    {
        position_origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            // Movement
            if (moving_forward)
            {
                transform.position = Vector3.Lerp(position_origin, position_destination, progress);
            }
            else // moving back to posO
            {
                transform.position = Vector3.Lerp(position_destination, position_origin, progress);
            }

            // Check if finished
            if (progress == 1f)
            {
                moving_forward = !moving_forward; // reverse direction
                progress = 0f; // reset internal clock
            }

            float progress_unclamped = progress + (Time.deltaTime / lerp_duration) * 5*(progress * (1-progress) + 0.005f);
            // This last term is higher in the middle and lower on both sides. (pseudo-bellcurve)

            progress = Mathf.Min(progress_unclamped, 1f); // clamped to not overshoot 1
        }
    }

    public void on_CamMovement_toggle_changed(bool value)
    {
        moving = value;
    }
}
