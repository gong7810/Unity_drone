using System;
using System.Collections;
using System.Collections.Generic;
using AirSimUnity.DroneStructs;
using UnityEngine;
using System.IO;
using System.Net;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace AirSimUnity {
    /*
     * Drone component that is used to control the drone object in the scene. This is based on Vehicle class that is communicating with AirLib.
     * This class depends on the AirLib's drone controllers based on FastPhysics engine. The controller is being used based on setting.json file in Documents\AirSim
     * The drone can be controlled either through keyboard or through client api calls.
     * This data is being constantly exchanged between AirLib and Unity through PInvoke delegates.
     */
    public class Drone : Vehicle {
        public Transform[] rotors;
        private List<RotorInfo> rotorInfos = new List<RotorInfo>();
        private float rotationFactor = 0.1f;
        
        public List<Transform> waypoints = new List<Transform>();
        private Transform targetWaypoint;
        //private int targetWaypointIndex = 0;
        private float minDistance = 0.1f; //waypoint에 도착했는지 판별
        private int lastWaypointIndex = 7;

        private float movementStep;
        private float rotationStep;
        private float movementSpeed = 1.0f;
        private float ratationSpeed = 3.0f;

        private float startTime;
        private float journeyLength;

        int i = 0;
        private int index = 0;
        private float[,] points = new float[8, 3];
        //private float[,] arr;
        static WebClient webClient = new System.Net.WebClient();

        static Stopwatch sw = new Stopwatch();
        
        private new void Start() {

            webClient.Credentials = new NetworkCredential("ditto", "ditto");
            string ServerData = webClient.DownloadString("http://203.255.57.124:8080/api/2/things/org.eclipse.ditto:waypoint/features/FlightData/properties");
            JObject jsons = JObject.Parse(ServerData);

            //lastWaypointIndex = waypoints.Count - 1; // 전체 waypoint의 갯수, 인덱스이므로 -1 해줌
            //targetWaypoint = waypoints[targetWaypointIndex];

            while (i < 1)
            {
                UnityEngine.Debug.Log("waypoint1[0] = " + jsons["waypoint1"][0]);
                UnityEngine.Debug.Log("waypoint1[1] = " + jsons["waypoint1"][1]);
                UnityEngine.Debug.Log("waypoint1[2] = " + jsons["waypoint1"][2]);
                i += 1;
            };
            /*
            while (true)
            {
                arr = new float[,] {jsons["waypoint1"][0].ToObject<float>(), jsons["waypoint1"][1].ToObject<float>(), jsons["waypoint1"][2].ToObject<float>()};

            }
            */
            points[0, 0] = jsons["waypoint1"][0].ToObject<float>();
            points[0, 1] = jsons["waypoint1"][1].ToObject<float>();
            points[0, 2] = jsons["waypoint1"][2].ToObject<float>();

            points[1, 0] = jsons["waypoint2"][0].ToObject<float>();
            points[1, 1] = jsons["waypoint2"][1].ToObject<float>();
            points[1, 2] = jsons["waypoint2"][2].ToObject<float>();

            points[2, 0] = jsons["waypoint3"][0].ToObject<float>();
            points[2, 1] = jsons["waypoint3"][1].ToObject<float>();
            points[2, 2] = jsons["waypoint3"][2].ToObject<float>();

            points[3, 0] = jsons["waypoint4"][0].ToObject<float>();
            points[3, 1] = jsons["waypoint4"][1].ToObject<float>();
            points[3, 2] = jsons["waypoint4"][2].ToObject<float>();

            points[4, 0] = jsons["waypoint5"][0].ToObject<float>();
            points[4, 1] = jsons["waypoint5"][1].ToObject<float>();
            points[4, 2] = jsons["waypoint5"][2].ToObject<float>();

            points[5, 0] = jsons["waypoint6"][0].ToObject<float>();
            points[5, 1] = jsons["waypoint6"][1].ToObject<float>();
            points[5, 2] = jsons["waypoint6"][2].ToObject<float>();

            points[6, 0] = jsons["waypoint7"][0].ToObject<float>();
            points[6, 1] = jsons["waypoint7"][1].ToObject<float>();
            points[6, 2] = jsons["waypoint7"][2].ToObject<float>();

            points[7, 0] = jsons["waypoint8"][0].ToObject<float>();
            points[7, 1] = jsons["waypoint8"][1].ToObject<float>();
            points[7, 2] = jsons["waypoint8"][2].ToObject<float>();

            targetWaypoint = waypoints[0];
            UnityEngine.Debug.Log("targetWaypoint.position = " + targetWaypoint.position);
            targetWaypoint.position = new Vector3(points[0, 0], points[0, 1], points[0, 2]);
            UnityEngine.Debug.Log("targetWaypoint.position = " + targetWaypoint.position);

            startTime = Time.deltaTime;

            journeyLength = Vector3.Distance(transform.position, targetWaypoint.position);

            base.Start(); 

            for (int i = 0; i < rotors.Length; i++) {
                rotorInfos.Add(new RotorInfo());
            }
        }

        private new void FixedUpdate() {
            if (isServerStarted)
            {
                if (resetVehicle)
                {
                    rcData.Reset();
                    currentPose = poseFromAirLib;
                    resetVehicle = false;
                }

                base.FixedUpdate();

                DataManager.SetToUnity(poseFromAirLib.position, ref position);
                DataManager.SetToUnity(poseFromAirLib.orientation, ref rotation);

                transform.position = position;
                transform.rotation = rotation;

                for (int i = 0; i < rotors.Length; i++)
                {
                    float rotorSpeed = (float) (rotorInfos[i].rotorSpeed * rotorInfos[i].rotorDirection * 180 /
                                                Math.PI * rotationFactor);
                    rotors[i].Rotate(Vector3.up, rotorSpeed * Time.deltaTime, Space.Self);
                }
            }
        }

        private new void LateUpdate() {

            /*
            float X_point1 = jsons["waypoint1"][0].ToObject<float>();
            float Y_point1 = jsons["waypoint1"][1].ToObject<float>();
            float Z_point1 = jsons["waypoint1"][2].ToObject<float>();

            float X_point2 = jsons["waypoint2"][0].ToObject<float>();
            float Y_point2 = jsons["waypoint2"][1].ToObject<float>();
            float Z_point2 = jsons["waypoint2"][2].ToObject<float>();

            float X_point3 = jsons["waypoint3"][0].ToObject<float>();
            float Y_point3 = jsons["waypoint3"][1].ToObject<float>();
            float Z_point3 = jsons["waypoint3"][2].ToObject<float>();

            float X_point4 = jsons["waypoint4"][0].ToObject<float>();
            float Y_point4 = jsons["waypoint4"][1].ToObject<float>();
            float Z_point4 = jsons["waypoint4"][2].ToObject<float>();
            */
            //List <float> points = new List<float>();


            movementStep = movementSpeed * Time.deltaTime;
            rotationStep = ratationSpeed * Time.deltaTime;

            Vector3 directionToTarget = targetWaypoint.position - transform.position;
            Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget); // gimbal lock 현상을 해결하기 위한 개념 : quaternion

            transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, rotationStep*2);

            float distance = Vector3.Distance(transform.position, targetWaypoint.position); // 현재 enemy의 위치와 waypoint간의 거리를 계산
            CheckDistanceToWaypoint(distance, points);

            float distCovered = (Time.time - startTime) * movementSpeed;
            float fracJourney = distCovered / journeyLength;
            
            transform.position = Vector3.Lerp(transform.position, targetWaypoint.position, fracJourney);

            //transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, movementStep);// 현재위치, 목표위치, 속도

            if (isServerStarted)
            {
                //Image capture is being done in base class
                base.LateUpdate();
            }
        }

        void CheckDistanceToWaypoint(float currentDistance, float[,] points)
        {
            if (currentDistance <= minDistance)
            {
                UnityEngine.Debug.Log("index = " + index);
                UnityEngine.Debug.Log("targetWaypoint.position = " + targetWaypoint.position);
                index += 1; // targetwaypoint의 index를 1씩 늘리는것

                //targetWaypointIndex++; // 목표 waypointindex를 1씩 늘리는것
                UpdateTargetWaypoint(points);
            }
        }

        void UpdateTargetWaypoint(float[,] points) // 현재 진행중인 waypoint를 업데이트함
        {
            /*if (targetWaypointIndex > lastWaypointIndex) // 마지막 waypoint는 3인데 이것보다 targetwaypoint가 크다면 0으로 리턴
            {
                targetWaypointIndex = 0;
            }*/
            if (index > lastWaypointIndex) // 마지막 waypoint는 7인데 이것보다 targetwaypoint가 크다면 0으로 리턴
            {
                index = 0;
                //targetWaypoint.position = new Vector3(points[0, 0], points[0, 1], points[0, 2]);
            }
            startTime = Time.time;
            targetWaypoint.position = new Vector3(points[index, 0], points[index, 1], points[index, 2]);
            //targetWaypoint = waypoints[targetWaypointIndex]; // 다음 index의 waypoint로 목표지점 update
        }

        public KinemticState GetKinematicState() {
            return airsimInterface.GetKinematicState();
        }

        #region IVehicleInterface implementation

        // Sets the animation for rotors on the drone. This is being done by AirLib through Pinvoke calls
        public override bool SetRotorSpeed(int rotorIndex, RotorInfo rotorInfo) {
            rotorInfos[rotorIndex] = rotorInfo;
            return true;
        }

        //Gets the data specific to drones for saving in the text file along with the images at the time of recording
        public override DataRecorder.ImageData GetRecordingData() {
            DataRecorder.ImageData data;
            data.pose = currentPose;
            data.carData = new CarStructs.CarData();
            data.image = null;
            return data;
        }

        #endregion IVehicleInterface implementation
    }
}