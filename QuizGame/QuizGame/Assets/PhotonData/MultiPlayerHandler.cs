using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayerHandler : MonoBehaviour
{
    public PhotonView pv;
    public GameObject cam;
    public Object[] destroyAbles;
    public GameObject LookAtObj;
    private void Awake()
    {
        if(!pv.IsMine)
        {
            foreach(Object obj in destroyAbles)
            {
                Destroy(obj);
            }
            cam.SetActive(false);

        }
        else
        {
            LookAtObj.SetActive(true);
        }
    }
}
