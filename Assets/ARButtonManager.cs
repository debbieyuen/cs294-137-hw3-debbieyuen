using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using cs294_137.hw2; //TO_COMMENT
using UnityEngine.XR.ARFoundation; //TO_ADD
using UnityEngine.XR.ARSubsystems; //TO_ADD
using UnityEngine.UI;



public class ARButtonManager : MonoBehaviour
{
    private Camera arCamera;
    private PlaceGameBoard placeGameBoard;
    public GameObject cube;
    public Text messageText;
    private List<string> sequence;
    private bool isCompeleted;
    public int seq_idx;
    private bool isWin;
    public float debounceTime = 0.3f;
    private float remainingDebounceTime;

    void Start()
    {
        // Here we will grab the camera from the AR Session Origin.
        // This camera acts like any other camera in Unity.
        //arCamera = FindObjectOfType<ARCamera>().GetComponent<Camera>();//TO_COMMENT
        arCamera = GetComponent<ARSessionOrigin>().camera; //TO_ADD
        // We will also need the PlaceGameBoard script to know if
        // the game board exists or not.
        placeGameBoard = GetComponent<PlaceGameBoard>();

        remainingDebounceTime = 0;

        sequence = cube.GetComponent<ColorChange>().sequence;
        isCompeleted = cube.GetComponent<ColorChange>().isCompleted;
        seq_idx = 0;
        isWin = false;
    }

    void Update()
    {
        if (remainingDebounceTime > 0)
        {
            remainingDebounceTime -= Time.deltaTime;
            return;
        }

        isCompeleted = cube.GetComponent<ColorChange>().isCompleted;

        // Update sequence from cube 
        sequence = cube.GetComponent<ColorChange>().sequence;

        //if (placeGameBoard.Placed() && Input.GetMouseButtonDown(0))//TO_COMMENT
        if (placeGameBoard.Placed() && Input.touchCount > 0) //TO_ADD
        {
            //Vector2 touchPosition = Input.mousePosition; //TO_COMMENT
            Vector2 touchPosition = Input.GetTouch(0).position; //TO_ADD
            // Convert the 2d screen point into a ray.
            Ray ray = arCamera.ScreenPointToRay(touchPosition);
            // Check if this hits an object within 100m of the user.

            /*TO_COMMENT
                        RaycastHit[] hits;
                        hits = Physics.RaycastAll(ray, 100.0F);
                        for (int i = 0; i < hits.Length; i++)
                        {
                            // Check that the object is interactable.
                            if(hits[i].transform.tag=="Interactable")
                                // Call the OnTouch function.
                                // Note the use of OnTouch3D here lets us
                                // call any class inheriting from OnTouch3D.
                                hits[i].transform.GetComponent<OnTouch3D>().OnTouch();
                        }
            */

            //TO_ADD ================================================
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                print(hit.transform.tag);

                if (hit.transform.tag == "Untagged")
                    return;
                // Check that the object is interactable.
                //if (hit.transform.tag == "Interactable")
                //    // Call the OnTouch function.
                //    // Note the use of OnTouch3D here lets us
                //    // call any class inheriting from OnTouch3D.
                //    hit.transform.GetComponent<OnTouch3D>().OnTouch();

                // If a touch is found and we are not waiting,
                if (remainingDebounceTime <= 0)
                {
                    remainingDebounceTime = debounceTime;
                }



                if (hit.transform.gameObject.tag == "reset_button")
                {
                    messageText.gameObject.SetActive(true);
                    messageText.text = "Starting Game!";

                    //cube.gameObject.SetActive(false);
                    cube.GetComponent<ColorChange>().reset();
                    cube.GetComponent<Renderer>().material.color = new Color(255, 255, 255);

                    isCompeleted = false;
                    seq_idx = 0;
                    return;
                }

                if (hit.transform.gameObject.tag == "red_button")
                {

                    if (!isCompeleted)
                    {
                        // Don't do anything if the sequence isn't completed
                        return;
                    }

                    if (sequence[seq_idx] == "R")
                    {
                        // User is correct
                        seq_idx += 1;
                        messageText.gameObject.SetActive(true);
                        messageText.text = "Correct!";
                    }
                    else
                    {
                        // User is incorrect
                        messageText.gameObject.SetActive(true);
                        messageText.text = "Incorrect!";
                        seq_idx = 0;
                    }

                    if (seq_idx >= sequence.Count)
                    {
                        messageText.gameObject.SetActive(true);
                        messageText.text = "YOU WIN!";
                    }
                    return;
                }

                if (hit.transform.gameObject.tag == "blue_button")
                {
                    if (!isCompeleted)
                    {
                        // Don't do anything if the sequence isn't completed
                        return;
                    }

                    if (sequence[seq_idx] == "B")
                    {
                        // User is correct
                        messageText.gameObject.SetActive(true);
                        messageText.text = "Correct!";
                        seq_idx += 1;
                    }
                    else
                    {
                        // User is incorrect
                        messageText.gameObject.SetActive(true);
                        messageText.text = "Incorrect!";
                        seq_idx = 0;
                    }

                    if (seq_idx >= sequence.Count)
                    {
                        messageText.gameObject.SetActive(true);
                        messageText.text = "YOU WIN!";
                    }
                    return;
                }

                if (hit.transform.gameObject.tag == "green_button")
                {
                    if (!isCompeleted)
                    {
                        // Don't do anything if the sequence isn't completed
                        return;
                    }

                    if (sequence[seq_idx] == "G")
                    {
                        // User is correct
                        messageText.gameObject.SetActive(true);
                        messageText.text = "Correct!";
                        seq_idx += 1;
                    }
                    else
                    {
                        // User is incorrect
                        messageText.gameObject.SetActive(true);
                        messageText.text = "Incorrect!";
                        seq_idx = 0;
                    }

                    if (seq_idx >= sequence.Count)
                    {
                        messageText.gameObject.SetActive(true);
                        messageText.text = "YOU WIN!";
                    }
                    return;
                }



            }
            //================================================
        }
    }
}
