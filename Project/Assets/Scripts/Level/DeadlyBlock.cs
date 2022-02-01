using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyBlock : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if(player != null && player.isLiving && !player.permaDead)
        {
            player.PermaDie();
        }
    }
}
