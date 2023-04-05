using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Player : NetworkBehaviour
{
    [SerializeField] Vector3 currentJoystick;
    [SerializeField] Vector3 oldPos;
    [SerializeField] Vector3 velocity;
    const float maxSpeed = 3;
    const float deceleration = 5;
    const float acceleration = 10;

    public override void OnNetworkSpawn()
    {
        GameObject.Find("Game Manager").GetComponent<NetcodeManager>().AssignNewPlayerClient(this);
    }

    void Update()
    {
        if (IsOwner)
        {
            UpdateJoystick(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        }
        MovePlayer();
    }

    public void SetNewClientPosition(Vector3 pos)
    {
        if(!IsOwner && !IsHost)
        {
            //Clientside prediction
            Vector3 dir = pos - oldPos;
            if (dir.magnitude > 0.1f)
            {
                currentJoystick = dir.normalized;
            }
            else
            {
                currentJoystick = Vector3.zero;
            }
        }
        transform.position = pos;
        oldPos = pos;
    }

    void MovePlayer()
    {
        //Deceleration
        float xVel = Mathf.Sign(velocity.x);
        float yVel = Mathf.Sign(velocity.y);
        velocity.x += deceleration * Time.deltaTime * -xVel;
        if(Mathf.Sign(velocity.x) != xVel)
        {
            velocity.x = 0;
        }
        velocity.y += deceleration * Time.deltaTime * -yVel;
        if (Mathf.Sign(velocity.y) != yVel)
        {
            velocity.y = 0;
        }
        //Move
        velocity += currentJoystick * acceleration * Time.deltaTime;
        //Max Speed
        velocity = Vector3.Min(velocity, Vector3.one * maxSpeed);
        velocity = Vector3.Max(velocity, -Vector3.one * maxSpeed);

        transform.position += velocity * Time.deltaTime;
    }

    public void UpdateJoystick(Vector2 joystick)
    {
        currentJoystick = joystick.normalized;
    }
}
