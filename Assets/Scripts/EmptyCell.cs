using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyCell : MonoBehaviour
{
    public int id;
    public GameObject camera;

    public void OnMouseDown()
    {
        camera.GetComponent<GameControl>().CreateShape(this.gameObject, id);
    }
}
