using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_urban_obs1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   void Update () {
        GameObject objHero = GameObject.Find("character(Clone)");
        float jarak =
        Vector3.Distance(objHero.transform.position,this.transform.position);
        //Debug.Log("Jarak dekat hero : " + jarak);
            if (jarak < 15f) {
            Transform objObs1 = this.transform.Find("Decorations");
            foreach (Transform anak in objObs1)
            {
                if (anak.name == "Car01") {
                anak.Translate(new Vector3(0, 0, 2f) * Time.deltaTime);
                }
            }
        }
    }
}
