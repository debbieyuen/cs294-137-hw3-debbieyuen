using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
// and overlays a cube on each of the tracked image
// Note that this code assumes that all tracked images are unique and named differently

[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageInfoManager : MonoBehaviour
{


    ARTrackedImageManager m_TrackedImageManager;
    Dictionary<string, GameObject> gameobjectDict = new Dictionary<string, GameObject>();
    private TrackingState prevTrackingState = TrackingState.None; //For newer versions of ARFoundation
    void Awake()
    {
        //This gets a reference to the AR Tracked Image Manager attached to the 'AR session Origin' gameobject
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }


    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {

        //eventArgs.added contains all the newly trackedImages that were found this frame
        foreach (var trackedImage in eventArgs.added)
        {

            //Write code here to deploy stuff whenever a new image is found in the tracking
            //e.g. Create a new virtual object and/or attach it to the tracked image
            //trackedImage.referenceImage.name -> Name of the tracked image
            //trackedImage.transform.position -> Position of the tracked image in the real world 
            //trackedImage.transform.rotation -> Rotation of the tracked image in the real world 

            //For example, here when we find a new tracked Image, we create a cube, and place it at the location of the tracked image. We add the gameobject to a dictionary, using the name of the image as a key.
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            cube.transform.position = trackedImage.transform.position;
            cube.transform.rotation = trackedImage.transform.rotation;
            gameobjectDict.Add(trackedImage.referenceImage.name, cube);

            prevTrackingState = trackedImage.trackingState; //For newer versions of ARFoundation
        }

        //eventArgs.removed contains all the trackedImages that were not found by the AR camera, either because it was removed from the camera's views or becuase the camera could not detect it.
        foreach (var trackedImage in eventArgs.removed)
        {
            //If we loose tracking of the image, we destroy the conrresponding cube and remove the image name's entry from the dictionary
            Destroy(gameobjectDict[trackedImage.referenceImage.name]);
            gameobjectDict.Remove(trackedImage.referenceImage.name);

            prevTrackingState = trackedImage.trackingState; //For newer versions of ARFoundation
        }

        //eventArgs.updated contains all the trackedImages which are currently being tracked, but its position and/or rotation changed
        foreach (var trackedImage in eventArgs.updated)
        {
            //trackedImage.transform.position -> Updated Position of the tracked image in the real world 
            //trackedImage.transform.rotation -> Updated Rotation of the tracked image in the real world 

            //if tracked image moves, we move the corresponding gameobject to match it's position.
            gameobjectDict[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
            gameobjectDict[trackedImage.referenceImage.name].transform.rotation = trackedImage.transform.rotation;

            //Add the Below if else code  block For newer versions of ARFoundation
            if (prevTrackingState == TrackingState.Tracking && trackedImage.trackingState != prevTrackingState)
            {
                //We lost and then regained tracking so we add cube again

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                cube.transform.position = trackedImage.transform.position;
                cube.transform.rotation = trackedImage.transform.rotation;
                gameobjectDict.Add(trackedImage.referenceImage.name, cube);

            }
            else if (prevTrackingState != TrackingState.Tracking && trackedImage.trackingState != prevTrackingState)
            {
                //We lost tracking so we remove the cube
                Destroy(gameobjectDict[trackedImage.referenceImage.name]);
                gameobjectDict.Remove(trackedImage.referenceImage.name);
            }

            prevTrackingState = trackedImage.trackingState; //For newer versions of ARFoundation

        }
    }
}

