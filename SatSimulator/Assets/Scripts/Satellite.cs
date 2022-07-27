using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : MonoBehaviour
{
    // Start is called before the first frame update
    float pos_sat_x, pos_sat_y, pos_sat_z;
    

    private Rigidbody satelliteRigidbody;
    
    void Start()
    {
        satelliteRigidbody = GetComponent<Rigidbody>();
        TCP_Server.Instance.SetGpsPositionCallback(new CallbackGpsPosition(OnPositionChange));
        pos_sat_z = (float)0.5;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 destination = new Vector3(transform.position.x, pos_sat_z, transform.position.z);
        // Debug.LogFormat("current : {0}",transform.position);
        // Debug.LogFormat("Dest : {0}",destination);
        transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime);
    }
    void OnPositionChange(float sat_x, float sat_y, float sat_z, float target_x, float target_y, float target_z)
    {
        pos_sat_x = sat_x;
        pos_sat_y = sat_y;
        pos_sat_z = sat_z + (float)0.5;
    }
}
