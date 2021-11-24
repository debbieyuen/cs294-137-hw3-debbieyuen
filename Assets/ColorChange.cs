using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    // This is the number of states that user memorizes
    public int num_states;
    public List<string> sequence;

    // Boolean to check whether sequence is done
    public bool isCompleted;

    //Time remaining until next sequence

    const float maxDelta = 5.0f; 
    private float delta = maxDelta;
    public float rps = 1.0f;
    private int seq_idx;

    AudioSource audioData;

    public void reset()
    {
        sequence.Clear(); 
        //Populate sequence 
        for (int i = 0; i < num_states; i++)
        {
            float random = Random.Range(0.0f, 3.0f);
            // If 0 <= random < 1
            // Assign Red
            if (random >= 0 && random < 1.0f)
            {
                sequence.Add("R");

            }
            else if (random >= 1.0f && random < 2.0f)
            {
                sequence.Add("B");

            }
            else
            {
                sequence.Add("G");
            }
        }


        audioData = GetComponent<AudioSource>();
        seq_idx = 0;
        isCompleted = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioData = GetComponent<AudioSource>();
        isCompleted = true; 
    }
    // Update is called once per frame
    void Update()
    { 
        if (isCompleted)
        {
            //Do nothing until user presses 'reset' button
            return; 
        }

        if(delta > 0.0f)
        {
            delta -= Time.deltaTime * rps;
            return;
        }

        GetComponent<Renderer>().material.color = new Color(0, 0, 0);   

        //Change color of Cube
        if (sequence[seq_idx] == "R")
        {
            audioData.Play(0);
            //Changes color to red 
            GetComponent<Renderer>().material.color = new Color(255, 0, 0);

        } else if (sequence[seq_idx] == "B")
        {
            audioData.Play(0);
            //Changes color to blue 
            GetComponent<Renderer>().material.color = new Color(0, 0, 255);

        } else
        {
            audioData.Play(0);
            //Changes color to green 
            GetComponent<Renderer>().material.color = new Color(0, 255, 0);
        }

        seq_idx += 1;
        delta = maxDelta;

        if (seq_idx >= sequence.Count)
        {
            isCompleted = true; 
        }

    }

    IEnumerator Reset()
    {
        // your process
        yield return new WaitForSeconds(1);
        // continue process
    }
}
