using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesMovementBehaviourScript : MonoBehaviour
{
    public Vector3 TargetPos;
    public int key;

    // Start is called before the first frame update
    void Start()
    {
        TargetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, TargetPos, 0.1f);
    }
}
