using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class FlyingObjects : MonoBehaviour
{
    // Start is called before the first frame update
    float pos_sat_x, pos_sat_y, pos_sat_z;
    float pos_target_x, pos_target_y, pos_target_z;
    
    public Boolean type;
    private bool isInitialized = false;
    private Rigidbody satelliteRigidbody;
    private long recentTime;
    private long currentTime;
    private TimeSpan deltaTime;
    void Start()
    {
        recentTime = DateTime.Now.Ticks;
        satelliteRigidbody = GetComponent<Rigidbody>();
        TCP_Server.Instance.SetGpsPositionCallback(new CallbackGpsPosition(OnPositionChange));
        pos_sat_z = (float)0.5;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 destination = new Vector3(0,0,0);
        if(type){
            destination = new Vector3(transform.position.x, pos_sat_z, transform.position.z);
        }
        else if(isInitialized){
            destination = new Vector3(pos_target_x, pos_target_z, pos_target_y);
        }
        // Debug.LogFormat("current : {0}",transform.position);
        // Debug.LogFormat("Dest : {0}",destination);
        transform.position = Vector3.MoveTowards(transform.position, destination, Math.Abs(pos_sat_z - transform.position.y)/(float)(deltaTime.TotalSeconds * 60));
    }
    void OnPositionChange(float sat_x, float sat_y, float sat_z, float target_x, float target_y, float target_z)
    {
        currentTime = DateTime.Now.Ticks;
        deltaTime = new TimeSpan(currentTime - recentTime);
        recentTime = currentTime;
        pos_sat_x = sat_x;
        pos_sat_y = sat_y;
        pos_sat_z = sat_z + (float)0.5;

        if(target_x != -1){
            isInitialized = true;
            pos_target_x = target_x;
            pos_target_y = target_y;
            pos_target_z = target_z;
        }
    }
}
