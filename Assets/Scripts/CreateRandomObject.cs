using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Poly_Gone
{
    public class CreateRandomObject : MonoBehaviour
    {
        public Material[] cubeMaterial = new Material[7];
        public GameObject cubePrefab;
        [HideInInspector] public List<GameObject> instantiatedObjects = new List<GameObject>();
        [HideInInspector] public static int ID = 1;
        GameObject cubeObject;
        [TextArea]
        public string Json;
        private void Awake()
        {
            GetCubeId();
        }
        void GetCubeId()
        {
            //ID=...
        }
        void Start()
        {
            cubeObject = Instantiate(cubePrefab);
            StartCoroutine(IntitJson());
        }
        IEnumerator IntitJson()
        {
            WWWForm form = new WWWForm();
            WWW www = new("http://localhost/sqlconnect/loaddata.php", form);
            yield return www;
            string[] webresults = www.text.Split('\t');
            if (webresults[0] == "0")
            {
                Debug.Log("Json Successfully Initialized");
                Debug.Log("Json file is:\n" + webresults[1]);
                Json = webresults[1];
            }
            else
            {
                Debug.Log("IntitJson Failed! Error #" + www.text);
            }
        }

        IEnumerator SaveDataToSQLServer()
        {
            WWWForm form = new WWWForm();
            form.AddField("cubejson", Json);
            //UnityWebRequest.Post("http://localhost/sqlconnect/savedata.php", form);
            WWW www = new("http://localhost/sqlconnect/savedata.php", form);
            yield return www;
            if (www.text =="0")
            {
                Debug.Log("Cube Created Successfully");
            }
            else
            {
                Debug.Log("Cube Create Failed! Error #"+www.text);
            }
        }
        public void CreateObject()
        {
            //save
            StartCoroutine(SaveDataToSQLServer());

            //create instance
            CubeData data = new CubeData();
            data.CubeGameObj = cubePrefab;

            //randomize
            CubeData randomData = CreateRandom(data);

            //attach random values
            SetCubeValues(randomData);

            //set json for save before next create
            Json = JsonUtility.ToJson(randomData);
        }
        private CubeData CreateRandom(CubeData data)
        {
            data.Rotation = Quaternion.Euler(
                UnityEngine.Random.Range(0f, 360f),
                UnityEngine.Random.Range(0f, 360f),
                UnityEngine.Random.Range(0f, 360f));
            data.Scale = new Vector3(
                UnityEngine.Random.Range(0.0f, 4.0f),
                UnityEngine.Random.Range(0.0f, 4.0f),
                UnityEngine.Random.Range(0.0f, 4.0f));

            data.CubeMaterial = cubeMaterial[UnityEngine.Random.Range(0, 7)];

            data.CubeGameObj.GetComponent<Transform>().rotation = data.Rotation;
            data.CubeGameObj.GetComponent<Transform>().localScale = data.Scale;
            data.CubeGameObj.GetComponent<MeshRenderer>().material = data.CubeMaterial;
            return data;
        }
        public void SetCubeValues(CubeData data)
        {
            cubeObject.GetComponent<Transform>().rotation = data.Rotation;
            cubeObject.GetComponent<Transform>().localScale = data.Scale;
            cubeObject.GetComponent<MeshRenderer>().material = data.CubeMaterial;
        }
        public string GetJson()
        {
            return Json;
        }
    }

    [System.Serializable]
    public class CubeData
    {
        public Quaternion Rotation;
        public Vector3 Scale;
        public Material CubeMaterial;
        public GameObject CubeGameObj;
    }
}
