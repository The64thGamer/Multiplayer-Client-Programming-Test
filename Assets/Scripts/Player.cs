using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Player : NetworkBehaviour
{
    [SerializeField] Vector2 oldPos1, oldPos2;
    [SerializeField] float oldTime1, oldTime2, timeBetweenUpdates;
    [SerializeField] float predictionTimer;
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
            MovePlayer();
            return;
        }
        PredictMovement();
    }

    public void SetNewClientPosition(Vector3 pos)
    {
        oldPos2 = transform.position;
        oldTime2 = oldTime1;
        oldPos1 = pos;
        oldTime1 = Time.time;
        predictionTimer = 1;
        timeBetweenUpdates =  oldTime1 - oldTime2;
        transform.position = pos;
    }

    /// <summary>
    /// Predicts movements of client-side players, slows down to halt after 1 second without update
    /// </summary>
    void PredictMovement()
    {
        transform.position = Vector3.Lerp(oldPos2, oldPos1, 1 - predictionTimer);
        if (predictionTimer > 0)
        {
            if(timeBetweenUpdates > 0)
            {
                predictionTimer = Mathf.Max(0, predictionTimer - (Time.deltaTime * predictionTimer / timeBetweenUpdates));
                return;
            }
            predictionTimer = Mathf.Max(0, predictionTimer - (Time.deltaTime * predictionTimer));
        }
    }

    void MovePlayer()
    {
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
        velocity += currentJoystick * Time.deltaTime;
        transform.position += velocity;
    }

    public void UpdateJoystick(Vector2 joystick)
    {
        currentJoystick = joystick.normalized;
    }
}
