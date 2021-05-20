using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballShooti : MonoBehaviour
{
    public GameObject bullet = default;
    bool canShoot = true;
    [SerializeField]float strength = 50;
    private void Update()
    {
        if (Input.GetMouseButton(0) && canShoot){
            canShoot = false;
            GameObject go = Instantiate(bullet,transform.position,Quaternion.identity);
            Rigidbody launchable = go.GetComponent<Rigidbody>();
            launchable.AddForce(transform.forward * strength, ForceMode.VelocityChange);
            StartCoroutine(canshootAgain());
        }

    }

    IEnumerator canshootAgain()
    {
        yield return new WaitForSeconds(0.5f);
        canShoot = true;
    }
}
