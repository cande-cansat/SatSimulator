using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PrefabManager : MonoBehaviour
{
    public GameObject TargetPrefab;
    public GameObject TCPserverPrefab;
    public GameObject PathCoordPrefab;

    public TMP_Text statusLog;

    float pos_sat_x, pos_sat_y, pos_sat_z;
    float pos_target_x, pos_target_y, pos_target_z;
    
    private Rigidbody satelliteRigidbody;
    private long recentTime;
    private long currentTime;
    private TimeSpan deltaTime;
    private GameObject targetMissile;
    private GameObject TCPserver;

    private PathCalculator pathCalculator;
    private Coordinate targetCoordinate;
    private Coordinate satelliteCoordinate;

    private List<Coordinate> targetPath;
    private List<GameObject> pathCrdList = new List<GameObject>();
    

    private Boolean isTargetVisual = false;
    private void Start() {
        if(TCPserver == null){
            TCPserver = GameObject.Instantiate(TCPserverPrefab, new Vector3(0,0,0), Quaternion.identity);
        }
        pathCalculator = new PathCalculator();
        recentTime = DateTime.Now.Ticks;
        satelliteRigidbody = GetComponent<Rigidbody>();
        TCP_Server.Instance.SetGpsPositionCallback(new CallbackGpsPosition(OnPositionChange));
        pos_sat_z = (float)0.5;
    }

    void Update()
    {
        // Vector3 destination = new Vector3(0,0,0);
        // if(type){
        //     destination = new Vector3(transform.position.x, pos_sat_z, transform.position.z);
        // }
        // else if(isInitialized){
        //     destination = new Vector3(pos_target_x, pos_target_z, pos_target_y);
        // }
        // // Debug.LogFormat("current : {0}",transform.position);
        // // Debug.LogFormat("Dest : {0}",destination);
        // transform.position = Vector3.MoveTowards(transform.position, destination, Math.Abs(pos_sat_z - transform.position.y)/(float)(deltaTime.TotalSeconds * 60));
        Vector3 destination = new Vector3(0,pos_sat_z,0);
        foreach(GameObject cansat in MainMenu.satList){
            destination = new Vector3(cansat.transform.position.x, pos_sat_z, cansat.transform.position.z);
            cansat.transform.position = Vector3.MoveTowards(cansat.transform.position, destination, Math.Abs(pos_sat_z - cansat.transform.position.y)/(float)(deltaTime.TotalSeconds * 60));
        }
        if(isTargetVisual){
            if(targetPath != null){
                if(pathCrdList != null){
                    foreach(var point in pathCrdList){
                        Destroy(point);
                    }
                }
                foreach(var pathCrd in targetPath){
                    GameObject pathCrdPoint = Instantiate(PathCoordPrefab);
                    pathCrdPoint.transform.position = new Vector3(pathCrd.item1, pathCrd.item3, pathCrd.item2);
                    pathCrdList.Add(pathCrdPoint);
                }
            }

            if(targetMissile == null){
                targetMissile = GameObject.Instantiate(TargetPrefab, new Vector3(pos_target_x, pos_target_z, pos_target_y), Quaternion.identity);
                Variables.logMessages.Enqueue(String.Format("[CanSAT] Target detected, Lat : {0}, Long : {1}, Alt : {2}\n", pos_target_x, pos_target_y, pos_target_z));
            }
            destination = new Vector3(pos_target_x, pos_target_z, pos_target_y);
            targetMissile.transform.position = Vector3.MoveTowards(targetMissile.transform.position, destination, Math.Abs(pos_sat_z - targetMissile.transform.position.y)/(float)(deltaTime.TotalSeconds * 60));
        }
    }
    private void OnDestroy() {
        GameObject.Destroy(TCPserver);
    }


    void OnPositionChange(float sat_x, float sat_y, float sat_z, float target_x, float target_y, float target_z)
    {
        currentTime = DateTime.Now.Ticks;
        deltaTime = new TimeSpan(currentTime - recentTime);
        recentTime = currentTime;
        pos_sat_x = sat_x;
        pos_sat_y = sat_y;
        pos_sat_z = sat_z + (float)0.5;
        // Variables.logMessages.Enqueue(String.Format("[Cansat] Satellite : {0}, {1}, {2}", sat_x, sat_y, sat_z));

        if(target_x != -1){
            
            pos_target_x = target_x;
            pos_target_y = target_y;
            pos_target_z = target_z;

            if(isTargetVisual){
                pathCalculator.setTimeDiff((float)deltaTime.TotalSeconds);
                targetPath = pathCalculator.calcPath(targetCoordinate, new Coordinate(target_x, target_y, target_z));
            }
            else{
                isTargetVisual = true;
            }
            targetCoordinate = new Coordinate(target_x, target_y, target_z);
        }
    }
}
