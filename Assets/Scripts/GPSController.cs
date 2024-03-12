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
    public double r = 6378.137; //�ԓ����a(km)

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

        //���N�����ƂȂ����\������肭�����Ȃ��B
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
                LocationInfo location = Input.location.lastData; //�ʒu���擾(float�^)
                float heading = Input.compass.trueHeading; //�[���̌����̊p�x

                //����ꂽ�ʒu����double�^�ɕϊ�
                double originLatitude = (double)location.latitude;
                double originLongitude = (double)location.longitude;
                
                //RealtimeDatabase���瑊��̈ʒu�����擾����B
                targetLatitude = dbGet.latitude;
                targetLongitude = dbGet.longitude;
                //targetLatitude = 34.742314; //����ܓx
                //targetLongitude = 134.988890; //����o�x

                //�ܓx�o�x������6���ɒ���
                double originLatitude_6 = double.Parse(originLatitude.ToString("f6"));
                double originLongitude_6 = double.Parse(originLongitude.ToString("f6"));
                double targetLatitude_6 = double.Parse(targetLatitude.ToString("f6"));
                double targetLongitude_6 = double.Parse(targetLongitude.ToString("f6"));
                
                //�I�u�W�F�N�g�̕\��
                transform.position = CalcPosition((double)originLatitude_6, (double)originLongitude_6, targetLatitude_6, targetLongitude_6, heading);

                //�e�L�X�g�\��
                originLocationText.text = "originLat: " + originLatitude + "\n" + "originLon: " + originLongitude;
                targetLocationText.text = "targetLat: " + targetLatitude + "\n" + "targetLon: " + targetLongitude;
                compassText.text = heading.ToString() + "�x";
                objectText.text = transform.position.ToString();
                distanceText.text = CalcDistance((double)originLatitude_6, (double)originLongitude_6, targetLatitude_6, targetLongitude_6).ToString() + "m";
                targetDigreeText.text = (CalcBearing((double)originLatitude_6, (double)originLongitude_6, targetLatitude_6, targetLongitude_6) - heading).ToString() + "�x";
                originLocation_6Text.text = "originLat(6): " + originLatitude_6 + "\n" + "originLon(6): " + originLongitude_6;
                targetLocation_6Text.text = "targetLat(6): " + targetLatitude_6 + "\n" + "targetLon(6): " + targetLongitude_6;

            }
        }
    }

    //�\������I�u�W�F�N�g�̍��W�����
    private Vector3 CalcPosition(double originLat, double originLon, double targetLat, double targetLon, float heading)
    {
        double distance = CalcDistance(originLat, originLon, targetLat, targetLon); //���݈ʒu����ڕW�܂ł̋���
        double bearing = CalcBearing(originLat, originLon, targetLat, targetLon); //���݈ʒu����ڕW�܂ł̊p�x(�k��0��)

        double angle = ToRadian(bearing - heading); // �[���̕�������ڕW�ʒu�̕��p

        return new Vector3(
            (float)(Math.Sin(angle) * distance),
            (float)altitude,
            (float)(Math.Cos(angle) * distance)
        );

    }

    //2�n�_�̈ܓx�o�x���狗�����v�Z���ĕԂ�
    public double CalcDistance(double originLat, double originLon, double targetLat, double targetLon)
    {
        double dlat1 = originLat * Deg2Rad;
        double dlon1 = originLon * Deg2Rad;
        double dlat2 = targetLat * Deg2Rad;
        double dlon2 = targetLon * Deg2Rad;

        //Haversine��
        double d1 = Math.Sin(dlat1) * Math.Sin(dlat2);
        double d2 = Math.Cos(dlat1) * Math.Cos(dlat2) * Math.Cos(dlon2 - dlon1);
        double distance = r * Math.Acos(d1 + d2);
        return distance * 1000; //m�P�ʂɒ����ĕԂ�
    }

    public static double Deg2Rad { get { return Math.PI / 180.0; } }

    //2�n�_�̈ܓx�o�x����p�x���v�Z���ĕԂ�
    public double CalcBearing(double originLat, double originLon, double targetLat, double targetLon)
    {
        double ��1 = ToRadian(originLat);
        double ��2 = ToRadian(targetLat);
        double ��1 = ToRadian(originLon);
        double ��2 = ToRadian(targetLon);

        double y = Math.Sin(��2 - ��1) * Math.Cos(��2);
        double x = Math.Cos(��1) * Math.Sin(��2) - Math.Sin(��1) * Math.Cos(��2) * Math.Cos(��2 - ��1);
        double �� = Math.Atan2(y, x);
        double bearing = (ToDegree(��) + 360) % 360;

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
