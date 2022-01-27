using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInteractionControlsScript : MonoBehaviour
{
    public TileMap tileMap;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.transform.gameObject.tag == "Tile")
                    {
                        //Debug.Log(hitInfo.collider.transform.parent.name);
                        //tileMap.MovePlayer(tileMap.gameObjectToTile[hitInfo.collider.transform.parent.transform.parent.gameObject]);
                        GameObject hitGO = hitInfo.collider.transform.parent.gameObject;
                        tileMap.MovePlayer(hitGO);
                    }
                }
            }
        }
    }
}
