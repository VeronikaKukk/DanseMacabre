using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCamera : MonoBehaviour
{
     public GameObject target;
    private Bounds targetBounds;
    public void Start()
    {
        targetBounds = target.GetComponent<Renderer>().bounds;
    }
    void LateUpdate()
    {

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = targetBounds.size.x / targetBounds.size.y;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = targetBounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = targetBounds.size.y / 2 * differenceInSize;
        }

        transform.position = new Vector3(targetBounds.center.x, targetBounds.center.y, -1f);

    }
}
