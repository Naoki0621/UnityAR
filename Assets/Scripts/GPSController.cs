using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class GPSController : MonoBehaviour
{

    public DBGet dbGet;
    public double targetLatitude = 0;
    public double targetLongitude = 0;
    public double altitude = -1.50;
    public double r = 6378.137; //赤道半径(km)

    public Text originLocationText;
    public Text objectText;
    public Text distanceText;
    public Text compassText;
    public Text targetDigreeText;
    public Text targetLocationText;
    public Text originLocation_6Text;
    public Text targetLocation_6Text;

    // Start is called before the first frame update
    void Start()
    {
        Input.compass.enabled = true;
        Input.location.Start();

        //即起動だとなぜか表示が上手くいかない。
        Invoke("ARIndication", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ARIndication()
    {
        if (Input.location.isEnabledByUser)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                LocationInfo location = Input.location.lastData; //位置情報取得(float型)
                float heading = Input.compass.trueHeading; //端末の向きの角度

                //得られた位置情報をdouble型に変換
                double originLatitude = (double)location.latitude;
                double originLongitude = (double)location.longitude;
                
                //RealtimeDatabaseから相手の位置情報を取得する。
                targetLatitude = dbGet.latitude;
                targetLongitude = dbGet.longitude;
                //targetLatitude = 34.742314; //自宅緯度
                //targetLongitude = 134.988890; //自宅経度

                //緯度経度を小数6桁に直す
                double originLatitude_6 = double.Parse(originLatitude.ToString("f6"));
                double originLongitude_6 = double.Parse(originLongitude.ToString("f6"));
                double targetLatitude_6 = double.Parse(targetLatitude.ToString("f6"));
                double targetLongitude_6 = double.Parse(targetLongitude.ToString("f6"));
                
                //オブジェクトの表示
                transform.position = CalcPosition((double)originLatitude_6, (double)originLongitude_6, targetLatitude_6, targetLongitude_6, heading);

                //テキスト表示
                originLocationText.text = "originLat: " + originLatitude + "\n" + "originLon: " + originLongitude;
                targetLocationText.text = "targetLat: " + targetLatitude + "\n" + "targetLon: " + targetLongitude;
                compassText.text = heading.ToString() + "度";
                objectText.text = transform.position.ToString();
                distanceText.text = CalcDistance((double)originLatitude_6, (double)originLongitude_6, targetLatitude_6, targetLongitude_6).ToString() + "m";
                targetDigreeText.text = (CalcBearing((double)originLatitude_6, (double)originLongitude_6, targetLatitude_6, targetLongitude_6) - heading).ToString() + "度";
                originLocation_6Text.text = "originLat(6): " + originLatitude_6 + "\n" + "originLon(6): " + originLongitude_6;
                targetLocation_6Text.text = "targetLat(6): " + targetLatitude_6 + "\n" + "targetLon(6): " + targetLongitude_6;

            }
        }
    }

    //表示するオブジェクトの座標を特定
    private Vector3 CalcPosition(double originLat, double originLon, double targetLat, double targetLon, float heading)
    {
        double distance = CalcDistance(originLat, originLon, targetLat, targetLon); //現在位置から目標までの距離
        double bearing = CalcBearing(originLat, originLon, targetLat, targetLon); //現在位置から目標までの角度(北を0°)

        double angle = ToRadian(bearing - heading); // 端末の方向から目標位置の方角

        return new Vector3(
            (float)(Math.Sin(angle) * distance),
            (float)altitude,
            (float)(Math.Cos(angle) * distance)
        );

    }

    //2地点の緯度経度から距離を計算して返す
    public double CalcDistance(double originLat, double originLon, double targetLat, double targetLon)
    {
        double dlat1 = originLat * Deg2Rad;
        double dlon1 = originLon * Deg2Rad;
        double dlat2 = targetLat * Deg2Rad;
        double dlon2 = targetLon * Deg2Rad;

        //Haversine式
        double d1 = Math.Sin(dlat1) * Math.Sin(dlat2);
        double d2 = Math.Cos(dlat1) * Math.Cos(dlat2) * Math.Cos(dlon2 - dlon1);
        double distance = r * Math.Acos(d1 + d2);
        return distance * 1000; //m単位に直して返す
    }

    public static double Deg2Rad { get { return Math.PI / 180.0; } }

    //2地点の緯度経度から角度を計算して返す
    public double CalcBearing(double originLat, double originLon, double targetLat, double targetLon)
    {
        double φ1 = ToRadian(originLat);
        double φ2 = ToRadian(targetLat);
        double λ1 = ToRadian(originLon);
        double λ2 = ToRadian(targetLon);

        double y = Math.Sin(λ2 - λ1) * Math.Cos(φ2);
        double x = Math.Cos(φ1) * Math.Sin(φ2) - Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(λ2 - λ1);
        double θ = Math.Atan2(y, x);
        double bearing = (ToDegree(θ) + 360) % 360;

        return bearing;
    }

    private double ToRadian(double degree)
    {
        return degree * Math.PI / 180;
    }

    private double ToDegree(double radian)
    {
        return radian * 180 / Math.PI;
    }

}
