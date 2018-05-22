using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;


public class FifthParallelControl : MonoBehaviour {

    public GameObject fifthParallel;
    public GameObject blackRoom;
    public GameObject pointCloud;
    
    void Start () {
		MidiMaster.knobDelegate += RoomButtonHandler;
    }
	


    void RoomButtonHandler(MidiChannel channel, int button, float value)
    {
        switch (button)
        {
            case 110:
                //turns the "Fifth Parallel Gallery" space on and off on keyboard button press
                {
                    if (value == 1.0)
                    {
                        blackRoom.SetActive(false);
                        fifthParallel.SetActive(true);
                    }
                    else
                    {
                        blackRoom.SetActive(true);
                        fifthParallel.SetActive(false);
                    }
                }
                break;
            case 109:
                //activates and deactivates the kinect point cloud on keyboard button press
                {
                    if (value == 1.0)
                    {
                        pointCloud.SetActive(true);
                    }
                    else
                    {
                        pointCloud.SetActive(false);
                    }
                }
                break;
        }
    }
}
