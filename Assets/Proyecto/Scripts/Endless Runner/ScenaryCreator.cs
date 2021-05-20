using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenaryCreator : MonoBehaviour
{
    public Transform[] cityTiles;
    [SerializeField] float endZ = 30.0f;
    [SerializeField] float beginZ = -10.0f;
    [SerializeField] float speed = 0.5f;
    int cantidadTiles = 0;
    //int currentIndex = 0;
    private void Start()
    {
        cantidadTiles = cityTiles.Length;
    }

    private void Update()
    {
        foreach(Transform t in cityTiles) 
        {
            if(t.position.z > endZ) 
            {
                t.position = new Vector3(0, 0, beginZ);
            }
            t.transform.Translate(new Vector3(0, 0, speed) * Time.deltaTime);
        }
    }
}
