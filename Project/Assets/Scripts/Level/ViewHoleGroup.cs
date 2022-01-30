using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewHoleGroup : MonoBehaviour
{
    public float activationInterval = 2;
    public ScientistAnimations[] scientists;
    public Transform character;

    private int activeIndex;
    private float activationTime = 0;

    void Start()
    {
        
    }

    void Update()
    {
        int closestIndex = 0;
        float closestDistance = Mathf.Infinity;
        for(int i=0; i<scientists.Length; i++)
        {
            float distance = Vector3.Distance(scientists[i].startPosition, character.position);
            if(closestDistance > distance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }
        if(closestIndex != activeIndex)
        {
            if(activationTime <= 0)
            {
                activeIndex = closestIndex;
                
            }
            else
            {
                activationTime -= Time.deltaTime;
                scientists[activeIndex].visible = false;
            }
        }
        else
        {
            scientists[activeIndex].visible = true;
            activationTime += Time.deltaTime;
            if(activationTime > activationInterval)
                activationTime = activationInterval;
        }
    }
}
