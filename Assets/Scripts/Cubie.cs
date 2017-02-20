using UnityEngine;
using System.Collections;

public class Cubie
{
    public bool isCenter;
    private GameObject gameObject;

    public GameObject getGameObject() { return gameObject; }

	public Cubie(GameObject gameObject)
    {
        this.gameObject = gameObject;
        isCenter = false;
    }
}