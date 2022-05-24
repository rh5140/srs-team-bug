using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    float originalY;

    public float floatStrength = 0.05f;
    public float speed = 7;

    void Start()
    {
        this.originalY = this.transform.localPosition.y;
    }

    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x,
            originalY + ((float)Mathf.Sin(Time.time * speed) * floatStrength),
            transform.localPosition.z);
    }

}
