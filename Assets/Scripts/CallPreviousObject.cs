using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poly_Gone
{

    public class CallPreviousObject : MonoBehaviour
    {

        public CreateRandomObject createRandomObject;
        public void CallAndSetObject()
        {
            
            //load
            //CubeData calledData = JsonUtility.FromJson<CubeData>(createRandomObject.GetJson());
            //createRandomObject.SetCubeValues(calledData);
            StartCoroutine(LoadDataToSQLServer());
        }

        IEnumerator LoadDataToSQLServer()
        {
            WWWForm form = new WWWForm();
            WWW www = new("http://localhost/sqlconnect/loaddata.php", form);
            yield return www;
            string[] webresults = www.text.Split('\t');
            if (webresults[0] == "0")
            {
                Debug.Log("Cube Called Successfully");
                Debug.Log("Json file is:\n"+ webresults[1]);
                CubeData calledData = JsonUtility.FromJson<CubeData>(webresults[1]);
                createRandomObject.SetCubeValues(calledData);
            }
            else
            {
                Debug.Log("Cube Call Failed! Error #" + www.text);
            }
        }

    }
}