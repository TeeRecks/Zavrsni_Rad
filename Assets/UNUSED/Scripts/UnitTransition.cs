using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTransition : MonoBehaviour
{
    Vector3 oldPos;
    Vector3 newPos;

    Vector3 currentVelocity;
    private float smoothSpeed = 0.15f;

    //private int lerpSpeed = 8;

    private void Start()
    {
        oldPos = this.transform.position;
        newPos = this.transform.position;
    }

    private void Update()
    {
        //this.transform.position = Vector3.Lerp(this.transform.position, newPos, lerpSpeed * Time.deltaTime);
        this.transform.position = Vector3.SmoothDamp(this.transform.position, newPos, ref currentVelocity, smoothSpeed);
    }

    public void Transition(Tile oldTile, Tile newTile)
    {
        oldPos = oldTile.Position();
        newPos = newTile.Position();
        //koordinate
        //Debug.Log(oldPos + " -> " + newPos);
    }
}
