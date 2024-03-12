using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class DBGet : MonoBehaviour
{
    private string baseUrl = "https://location-4c8ca-default-rtdb.firebaseio.com/users/"; //Firebaseのデータベースを作成し使用
    public double latitude = 0;
    public double longitude = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Connect());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Connect()
    {
        string uid = "userA";
        UnityWebRequest request = UnityWebRequest.Get(baseUrl + uid + ".json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("NetWork error : " + request.error);
        }
        else
        {
            Debug.Log("Success : " + request.downloadHandler.text);
            var response = JsonUtility.FromJson<ResponseLocation>(request.downloadHandler.text);
            Debug.Log("endlatitude : " + response.latitude);
            Debug.Log("endlongitude : " + response.longitude);
            Debug.Log("endaltitude : " + response.altitude);

            latitude = response.latitude;
            longitude = response.longitude;

            Debug.Log("Lat : " + latitude + "Lon : " + longitude);
        }
    }
}

[System.Serializable]
public class ResponseLocation
{
    public double latitude;
    public double longitude;
    public double altitude;
}

