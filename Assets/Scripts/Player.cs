using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Player : NetworkBehaviour
{
    [SerializeField] Vector3 oldPos1, oldPos2;
    [SerializeField] float predictionTimer;

    public override void OnNetworkSpawn()
    {
        GameObject.Find("Game Manager").GetComponent<NetcodeManager>().AssignNewPlayerClient(this);
    }

    void Update()
    {
        PredictMovement();
    }

    public void SetNewClientPosition(Vector3 pos)
    {
        oldPos2 = transform.position;
        oldPos1 = pos;
        predictionTimer = 1;
    }

    /// <summary>
    /// Predicts movements of client-side players, slows down to halt after 1 second without update
    /// </summary>
    void PredictMovement()
    {
        transform.position = Vector3.Lerp(oldPos2, oldPos1, 1 - predictionTimer);
        if (predictionTimer > 0)
        {
            predictionTimer = Mathf.Max(0, predictionTimer - (Time.deltaTime * predictionTimer));
        }
    }
}
