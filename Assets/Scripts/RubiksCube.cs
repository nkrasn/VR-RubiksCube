using UnityEngine;
using System.Collections;

public class RubiksCube : MonoBehaviour
{
    private Cubie[] cubies;
    private Cubie[] centers;
    private Cubie sideToRotate;

    private bool turning;

    public enum Turn
    {
        U, UI,
        D, DI,
        F, FI,
        B, BI,
        L, LI,
        R, RI
    }

    public Cubie[] getCenters() { return centers; }
    public bool isTurning() { return turning; }

    void Start()
	{
        cubies = new Cubie[26];
        centers = new Cubie[6];

        turning = false;

        // Populate the cubies array
        int cubies_i = 0;
        int centers_i = 0;
        foreach(Transform t in transform)
        {
            cubies[cubies_i] = new Cubie(t.gameObject);
            if(t.gameObject.name.Substring(0, 2) == "m_") // Check if it's a center
            {
                // Say it's a center and then add it to the centers array
                cubies[cubies_i].isCenter = true;
                centers[centers_i] = cubies[cubies_i];
                centers_i++;
            }
            cubies_i++;
        }

        sideToRotate = null;
	}

    void Update()
    {
        if(sideToRotate == null)
        {
            if(Input.GetKeyDown(KeyCode.U))
                setSide(Turn.U);
            else if(Input.GetKeyDown(KeyCode.D))
                setSide(Turn.D);
            else if(Input.GetKeyDown(KeyCode.R))
                setSide(Turn.R);
            else if(Input.GetKeyDown(KeyCode.L))
                setSide(Turn.L);
            else if(Input.GetKeyDown(KeyCode.F))
                setSide(Turn.F);
            else if(Input.GetKeyDown(KeyCode.B))
                setSide(Turn.B);
        }

        if(!turning && sideToRotate != null)
        {
            if(Input.GetKey(KeyCode.LeftShift))
                StartCoroutine("TurnSide", false);
            else
                StartCoroutine("TurnSide", true);
        }
    }

    public void beginTurn(bool isClockwise)
    {
        if(!turning && sideToRotate != null)
            StartCoroutine("TurnSide", isClockwise);
    }

    IEnumerator TurnSide(bool clockwise)
    {
        turning = true;
        float delay = 1f / 90f;
        float resolution = 30;
        
        Transform sideToRotateTransform = sideToRotate.getGameObject().transform;
        for(int i = 0; i < resolution; i++)
        {
            if(clockwise)
                sideToRotateTransform.Rotate(sideToRotateTransform.localPosition.normalized, 90f / resolution);
            else
                sideToRotateTransform.Rotate(sideToRotateTransform.localPosition, -90f / resolution);
            
            yield return new WaitForSeconds(delay);
        }

        sideToRotate = null;
        unparentSidePieces();
        turning = false;
    }

    // Figure out how to parent, based on the position of the cubies
    public void setSide(Turn side)
    {
        // Conver the side enum into a lowercase string (ignore inverted)
        string sideToString = side.ToString().ToLower().Substring(0, 1);

        switch(side)
        {
            case Turn.U:
            case Turn.UI:
                parentSidePieces(Vector3.up);
                break;
            case Turn.D:
            case Turn.DI:
                parentSidePieces(Vector3.down);
                break;
            case Turn.F:
            case Turn.FI:
                parentSidePieces(Vector3.right);
                break;
            case Turn.B:
            case Turn.BI:
                parentSidePieces(Vector3.left);
                break;
            case Turn.L:
            case Turn.LI:
                parentSidePieces(Vector3.back);
                break;
            case Turn.R:
            case Turn.RI:
                parentSidePieces(Vector3.forward);
                break;
        }
    }

    // Makes the center of the side = parent of the cubies part of the side
    public void parentSidePieces(Vector3 sideOrientation)
    {
        sideOrientation.Normalize();

        // Find the center of the side
        foreach(Cubie cubie in cubies)
        {
            if(cubie.isCenter)
            {
                // Normalize the cubie position to make it comparable with the side orientation
                Vector3 cubiePos = cubie.getGameObject().transform.localPosition.normalized;

                if(cubiePos == sideOrientation)
                {
                    sideToRotate = cubie;
                    break; // Found the center, no need to look anymore
                }
            }
        }

        // Make the center the parent of the appropriate cubies
        foreach(Cubie cubie in cubies)
        {
            GameObject cubieGameObject = cubie.getGameObject();

            // Masking the cubie's position with the side orientation
            Vector3 cubiePos = cubieGameObject.transform.localPosition;
            cubiePos.x *= Mathf.Abs(sideOrientation.x);
            cubiePos.y *= Mathf.Abs(sideOrientation.y);
            cubiePos.z *= Mathf.Abs(sideOrientation.z);
            cubiePos.Normalize();

            // Parent the cubie with the center if it's a part of the side
            if(cubiePos == sideOrientation && !cubie.isCenter)
            {
                cubieGameObject.transform.parent = sideToRotate.getGameObject().transform;
            }
        }
    }

    private void unparentSidePieces()
    {
        // Make the parent of every cubie the cube/core
        foreach(Cubie cubie in cubies)
        {
            cubie.getGameObject().transform.parent = transform;
        }
    }
}