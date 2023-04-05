using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Player : NetworkBehaviour
{
    [SerializeField] Vector3 currentJoystick;
    [SerializeField] Vector3 velocity;

    public override void OnNetworkSpawn()
    {
        GameObject.Find("Game Manager").GetComponent<NetcodeManager>().AssignNewPlayerClient(this);
    }

    void Update()
    {
        if (IsOwner)
        {
            UpdateJoystick(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            return;
        }
        MovePlayer();
    }

    public void SetNewClientPosition(Vector3 pos)
    {
        transform.position = pos;
    }


    void MovePlayer()
    {
        //Deceleration
        float xVel = Mathf.Sign(velocity.x);
        float yVel = Mathf.Sign(velocity.x);
        velocity.x += 0.1f * Time.deltaTime * -xVel;
        if(Mathf.Sign(velocity.x) != xVel)
        {
            velocity.x = 0;
        }
        velocity.y += 0.1f * Time.deltaTime * -yVel;
        if (Mathf.Sign(velocity.y) != yVel)
        {
            velocity.y = 0;
        }
        //Move
        velocity += currentJoystick * Time.deltaTime;
        //Max Speed
        velocity = Vector3.Min(velocity, Vector3.one * .01f);
        velocity = Vector3.Max(velocity, -Vector3.one * .01f);

        transform.position += velocity;
    }

    public void UpdateJoystick(Vector2 joystick)
    {
        currentJoystick = joystick.normalized;
    }
}
