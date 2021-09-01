using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System.Diagnostics;

namespace mongodb{
    public class Mongo : MonoBehaviour
    {

        static string connString = "mongodb://203.255.57.92:27017";         // db ip/포트 지정

        static MongoClient cli = new MongoClient(connString);

        static MongoDatabase testdb = cli.GetServer().GetDatabase("test");   // db 데이터 지정

        static FlightData predata;

         void Start()
        {
        // ###################데이터 가져올 때 필요한 부분##############################
            FlightData result = testdb.GetCollection<FlightData>("tests").FindAll().SetSortOrder(SortBy.Descending("_id")).First();
            predata = result;
        // #################################################################
        }

        // // Update is called once per frame
        void Update()
        {
            //#################### db에서 데이터 가져오기 ##############################
            FlightData result = testdb.GetCollection<FlightData>("tests").FindAll().SetSortOrder(SortBy.Descending("_id")).First();
            if (result != null && !predata.Id.Equals(result.Id))
            {
                UnityEngine.Debug.Log(result.ToString()); 
                //UnityEngine.Debug.Log(result.lastUpdate);  reslut.접근할 데이터 형식으로 FlightData클래스에서 사용할 데이터만 추출 가능
                //UnityEngine.Debug.Log(result.Rolldata);
                // ...
                predata = result;
            }
           //#######################################################################

            //############################ db에 데이터 입력 (현재 임의의 데이터 입력함)#####################################
            // InputData cust1 = new InputData { lastUpdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), Name = "Kim", Age1 = 30, Age2 = 30, Age3 = 30, Age4 = 30, Age5 = 30, Age6 = 30, Age7 = 30, Age8 = 30 };
            // testdb.GetCollection<FlightData>("unity_to_raspberry").Insert(cust1);
            //##################################################################################
        }
    
    }
    // ###################### db에서 가져오는 데이터 포맷 지정 #############################
        class FlightData
        {
            public ObjectId Id { get; set; }
            public string lastUpdate { get; set; }
            public double Rolldata { get; set; }
            public double Pitchdata { get; set; }
            public double Yawdata { get; set; }
            public double Altitudedata { get; set; }
            public double Speeddata { get; set; }
            public double latdata { get; set; }
            public double londata { get; set; }
            public double Batterydata { get; set; }

            public override string ToString()
            {
                return lastUpdate + "/" + Rolldata + "/" + Pitchdata + "/" + Yawdata + "/" + Altitudedata + "/" + Speeddata + "/" + latdata + "/" + londata + "/" + Batterydata;
            }
        
        }

        // ###################### db에 입력하는 데이터 포맷 지정 #############################
        class InputData
        {
            public ObjectId Id { get; set; }

            public string lastUpdate { get; set; }
            public string Name { get; set; }
            public int Age1 { get; set; }
            public int Age2 { get; set; }
            public int Age3 { get; set; }
            public int Age4 { get; set; }
            public int Age5 { get; set; }
            public int Age6 { get; set; }
            public int Age7 { get; set; }
            public int Age8 { get; set; }
        
        }
}