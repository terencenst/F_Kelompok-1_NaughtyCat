using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_levels : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] objLevels;
    public GameObject[] objObstacle;
    private GameObject objLastLevels;

    public GameObject objHero;
    private GameObject preHero;

    private float kecLari = 10f;
    private float faktorPercepatan = 0.3f; 

    private bool levelAwal = true;
    void Start()
    {
        objLastLevels = this.gameObject;

        preHero = Instantiate(objHero, this.transform.position, this.transform.rotation) as GameObject;
        
        for (int a = 0; a <= 15; a++) {
            buat_level();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (preHero.GetComponent<Sc_character>().statMulai == true) 
        {
            kecLari += faktorPercepatan * Time.deltaTime; 

            // Dapatkan semua child dari GameObject place_levels
            foreach (Transform anak in GameObject.Find("place_levels").transform) 
            {
                anak.transform.Translate(new Vector3(0, 0, -kecLari) * Time.deltaTime);

                Vector3 posAnakLevel = Camera.main.WorldToViewportPoint(anak.transform.position);
                if (posAnakLevel.z < -1f) 
                {
                    Destroy(anak.gameObject);
                    buat_level();
                }
            }
        }
    }


    void buat_level(){

        Vector3 posLevels = objLastLevels.transform.position;
        if (levelAwal == false) {
            posLevels.z += objLastLevels.GetComponent<Renderer>().bounds.size.z;
            }
        int randJenisLevel = Random.Range(0, objLevels.Length);
        GameObject preLevels = Instantiate(objLevels[randJenisLevel], posLevels, this.transform.rotation) as GameObject;
        objLastLevels = preLevels.gameObject;
        objLastLevels.transform.parent = this.transform;

        if (levelAwal == false) {
        int rand = Random.Range(0,5);
            if (rand == 0) {
            buat_obstacle_barrier(preLevels);
            }
        }
        levelAwal = false;
    }

    void buat_obstacle_barrier(GameObject tempatLevels) {
        int jenisBarrier = Random.Range(2, 4);
        GameObject objObs = Instantiate(objObstacle[jenisBarrier],
        tempatLevels.transform.position, tempatLevels.transform.rotation) as GameObject;
        objObs.transform.parent = tempatLevels.transform;
    }
}
