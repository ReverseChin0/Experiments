using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [SerializeField] float Limits = 4.0f;
    [SerializeField] float speed = 2.5f;
    [SerializeField] Transform player = default;
    float posX = 0.0f;
    float posY = 0.0f;
    Vector3 newDirection; 

    private void Awake()
    {
        posY = player.position.y;
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        
        float newPos = posX + (horizontal * speed * Time.deltaTime);
        
        if(newPos < Limits &&  newPos > -Limits) 
        {
            posX = newPos;
        }

        player.position = new Vector3(posX, posY, 0);
        newDirection = new Vector3(posX - horizontal, posY, 2);
        player.rotation = Quaternion.LookRotation(newDirection - player.position, Vector3.up);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(newDirection, 1);
    }
}
