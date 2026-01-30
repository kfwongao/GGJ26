using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public GameObject followTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(followTarget != null)
        {
            transform.position = new Vector3(followTarget.transform.position.x, transform.position.y, followTarget.transform.position.z);
        }
        else
        {
            Debug.LogError("MiniMapCamera::Update => followTarget is null");
        }
    }
}
