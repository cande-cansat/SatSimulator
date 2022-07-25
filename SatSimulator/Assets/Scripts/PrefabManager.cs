using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public GameObject SatellitePrefab;
    public void SpawnSatellite(GameObject prefab, Vector3 _position)
    {
        GameObject satellite = Instantiate(prefab);
        satellite.transform.position = _position;
    }
}
