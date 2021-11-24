using System;
using System.Collections;
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
    public GameObject session_origin;
    public GameObject cube;
    private bool isCompeleted;
    private List<string> sequence;

    private int global_seq_idx; 

    ARTrackedImageManager m_TrackedImageManager;
    Dictionary<string, GameObject> gameobjectDict = new Dictionary<string, GameObject>();
    private TrackingState prevTrackingState = TrackingState.None; //For newer versions of ARFoundation
    void Awake()
    {;
        sequence = cube.GetComponent<ColorChange>().sequence;
        isCompeleted = cube.GetComponent<ColorChange>().isCompleted;

        global_seq_idx = session_origin.GetComponent<ARButtonManager>().seq_idx;

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

        sequence = cube.GetComponent<ColorChange>().sequence;
        isCompeleted = cube.GetComponent<ColorChange>().isCompleted;
        global_seq_idx = this.GetComponent<ARButtonManager>().seq_idx;

        if (!isCompeleted)
        {
            // Don't track images if cube isn't done changing colors
            foreach (var key in gameobjectDict.Keys) {
                Destroy(gameobjectDict[key]);
                gameobjectDict.Remove(key);
            }

            print("is completed");
             return; 
        }

        //eventArgs.added contains all the newly trackedImages that were found this frame
        foreach (var trackedImage in eventArgs.added)
        {

            print(trackedImage.referenceImage.name);

            if (trackedImage.referenceImage.name != "MDESWhite" && trackedImage.referenceImage.name != "MDESBlue")
            {
                continue;
            }

            //if (seq_idx >= sequence.Count)
            //{
            //    print("Sequence out of index" + sequence.Count);
            //    continue;
            //}


            if (gameobjectDict.ContainsKey(trackedImage.referenceImage.name))
            {
                print("seen " + trackedImage.referenceImage.name);
                if (trackedImage.referenceImage.name == "MDESWhite")
                {
                    // Shows a color that ISN'T the current sequence

                    if (sequence[global_seq_idx] == "R")
                    {
                        //Changes color to green 
                        gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(0, 255, 0);

                    }
                    else if (sequence[global_seq_idx] == "B")
                    {
                        //Changes color to red 
                        gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(255, 0, 0);

                    }
                    else
                    {
                        //Changes color to blue 
                        gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                    }
                }
                else if (trackedImage.referenceImage.name == "MDESBlue")
                {
                    // Shows 1 of the next 2 colors
                    // Randomly choose the next 2 colors
                    float random = UnityEngine.Random.Range(0.0f, 2.0f);
                    string next_seq;

                    if (random >= 1.0f)
                    {
                        // If index is out of range, then just choose the next color 
                        next_seq = sequence[Mathf.Min(global_seq_idx + 1, sequence.Count - 1)];
                    }
                    else
                    {
                        next_seq = sequence[global_seq_idx];
                    }


                    if (next_seq == "R")
                    {
                        //Changes color to green 
                        gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(255, 0, 0);

                    }
                    else if (next_seq == "B")
                    {
                        //Changes color to red 
                        gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(0, 0, 255);

                    }
                    else
                    {
                        //Changes color to blue 
                        gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(0, 255, 0);
                    }


                }
                continue;

            }

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

            print("global_seq_idx: " + global_seq_idx);

            if (trackedImage.referenceImage.name == "MDESWhite")
            {
                // Shows a color that ISN'T the current sequence

                if (sequence[global_seq_idx] == "R")
                {
                    //Changes color to green 
                    cube.GetComponent<Renderer>().material.color = new Color(0, 255, 0);

                }
                else if (sequence[global_seq_idx] == "B")
                {
                    //Changes color to red 
                    cube.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

                }
                else
                {
                    //Changes color to blue 
                    cube.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                }
            }
            else if (trackedImage.referenceImage.name == "MDESBlue")
            {
                // Shows 1 of the next 2 colors
                // Randomly choose the next 2 colors
                float random = UnityEngine.Random.Range(0.0f, 2.0f);
                string next_seq;

                if(random >= 1.0f)
                {
                    // If index is out of range, then just choose the next color 
                    next_seq = sequence[Mathf.Min(global_seq_idx + 1, sequence.Count - 1)];
                } else
                {
                    next_seq = sequence[global_seq_idx];
                }


                if (next_seq == "R")
                {
                    //Changes color to green 
                    cube.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

                }
                else if (next_seq == "B")
                {
                    //Changes color to red 
                    cube.GetComponent<Renderer>().material.color = new Color(0, 0, 255);

                }
                else
                {
                    //Changes color to blue 
                    cube.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
                }

            }
    
            gameobjectDict.Add(trackedImage.referenceImage.name, cube);
            prevTrackingState = trackedImage.trackingState; //For newer versions of ARFoundation
        }

        //eventArgs.removed contains all the trackedImages that were not found by the AR camera, either because it was removed from the camera's views or becuase the camera could not detect it.
        foreach (var trackedImage in eventArgs.removed)
        {

            if (trackedImage.referenceImage.name != "MDESWhite" && trackedImage.referenceImage.name != "MDESBlue")
            {
                continue;
            }

            //If we loose tracking of the image, we destroy the conrresponding cube and remove the image name's entry from the dictionary
            Destroy(gameobjectDict[trackedImage.referenceImage.name]);
            gameobjectDict.Remove(trackedImage.referenceImage.name);

            prevTrackingState = trackedImage.trackingState; //For newer versions of ARFoundation
        }


        //eventArgs.updated contains all the trackedImages which are currently being tracked, but its position and/or rotation changed
        foreach (var trackedImage in eventArgs.updated)
        {

            if (trackedImage.referenceImage.name != "MDESWhite" && trackedImage.referenceImage.name != "MDESBlue")
            {
                continue;
            }

            //trackedImage.transform.position -> Updated Position of the tracked image in the real world 
            //trackedImage.transform.rotation -> Updated Rotation of the tracked image in the real world 

            //if tracked image moves, we move the corresponding gameobject to match it's position.
            gameobjectDict[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
            gameobjectDict[trackedImage.referenceImage.name].transform.rotation = trackedImage.transform.rotation;

            //Add the Below if else code  block For newer versions of ARFoundation
            if (prevTrackingState == TrackingState.Tracking && trackedImage.trackingState != prevTrackingState)
            {
                if (gameobjectDict.ContainsKey(trackedImage.referenceImage.name))
                {
                    print("seen " + trackedImage.referenceImage.name);
                    if (trackedImage.referenceImage.name == "MDESWhite")
                    {
                        // Shows a color that ISN'T the current sequence

                        if (sequence[global_seq_idx] == "R")
                        {
                            //Changes color to green 
                            gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(0, 255, 0);

                        }
                        else if (sequence[global_seq_idx] == "B")
                        {
                            //Changes color to red 
                            gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(255, 0, 0);

                        }
                        else
                        {
                            //Changes color to blue 
                            gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                        }
                    }
                    else if (trackedImage.referenceImage.name == "MDESBlue")
                    {
                        // Shows 1 of the next 2 colors
                        // Randomly choose the next 2 colors
                        float random = UnityEngine.Random.Range(0.0f, 2.0f);
                        string next_seq;

                        if (random >= 1.0f)
                        {
                            // If index is out of range, then just choose the next color 
                            next_seq = sequence[Mathf.Min(global_seq_idx + 1, sequence.Count - 1)];
                        }
                        else
                        {
                            next_seq = sequence[global_seq_idx];
                        }


                        if (next_seq == "R")
                        {
                            //Changes color to green 
                            gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(255, 0, 0);

                        }
                        else if (next_seq == "B")
                        {
                            //Changes color to red 
                            gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(0, 0, 255);

                        }
                        else
                        {
                            //Changes color to blue 
                            gameobjectDict[trackedImage.referenceImage.name].GetComponent<Renderer>().material.color = new Color(0, 255, 0);
                        }


                    }
                    continue;
                }

                //We lost and then regained tracking so we add cube again

                print("updated cube object");

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                cube.transform.position = trackedImage.transform.position;
                cube.transform.rotation = trackedImage.transform.rotation;

                if (trackedImage.referenceImage.name == "MDESWhite")
                {
                    // Shows a color that ISN'T the current sequence

                    if (sequence[global_seq_idx] == "R")
                    {
                        //Changes color to green 
                        cube.GetComponent<Renderer>().material.color = new Color(0, 255, 0);

                    }
                    else if (sequence[global_seq_idx] == "B")
                    {
                        //Changes color to red 
                        cube.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

                    }
                    else
                    {
                        //Changes color to blue 
                        cube.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                    }
                }
                else if (trackedImage.referenceImage.name == "MDESBlue")
                {
                    // Shows 1 of the next 2 colors
                    // Randomly choose the next 2 colors
                    float random = UnityEngine.Random.Range(0.0f, 2.0f);
                    string next_seq;

                    if (random >= 1.0f)
                    {
                        // If index is out of range, then just choose the next color 
                        next_seq = sequence[Mathf.Min(global_seq_idx + 1, sequence.Count - 1)];
                    }
                    else
                    {
                        next_seq = sequence[global_seq_idx];
                    }


                    if (next_seq == "R")
                    {
                        //Changes color to green 
                        cube.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

                    }
                    else if (next_seq == "B")
                    {
                        //Changes color to red 
                        cube.GetComponent<Renderer>().material.color = new Color(0, 0, 255);

                    }
                    else
                    {
                        //Changes color to blue 
                        cube.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
                    }


                }
                gameobjectDict.Add(trackedImage.referenceImage.name, cube);

            }
            else if (prevTrackingState != TrackingState.Tracking && trackedImage.trackingState != prevTrackingState)
            {
                //We lost tracking so we remove the cube
                print("destroyed game object");
                Destroy(gameobjectDict[trackedImage.referenceImage.name]);
                gameobjectDict.Remove(trackedImage.referenceImage.name);
            }

            prevTrackingState = trackedImage.trackingState; //For newer versions of ARFoundation

        }
    }
}

