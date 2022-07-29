using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField countSatellite;
    public TMP_InputField positionType;
    public TMP_Text statusLog;
    public GameObject CanSAT;

    public GameObject PrefabManager;
    public static List<GameObject> satList = new List<GameObject>();
    private GameObject pmInstance;

    void Start()
    {
        
    
    }

    public void OnClickApply()
    {
        int cnt = int.Parse(countSatellite.text);
        int tide = (int)Mathf.Sqrt(cnt);
        int center_x = tide / 2;
        int center_y = tide / 2;
        
        for(int i=0; i<satList.Count; i++){
            Destroy(satList[i]);
        }
        satList.Clear();
        
        for(int i=0; i<tide; i++){
            for (int j=0; j<tide; j++){
                GameObject satellite = Instantiate(CanSAT);
                satellite.transform.position = new Vector3(5*(i-center_x), (float)0.5, 5*(j-center_y));
                satList.Add(satellite);
            }
        }
        printLog("Generate " + tide*tide + " satellites.");
        
    }

    public void OnClickStart()
    {
        printLog("Start Simulation.");
        if(pmInstance != null){
            GameObject.Destroy(pmInstance);
        }
        pmInstance = Instantiate(PrefabManager, new Vector3(0,0,0), Quaternion.identity);
    }

    public void printLog(string s){
        statusLog.text += s;
        statusLog.text += "\n";
    }
}
