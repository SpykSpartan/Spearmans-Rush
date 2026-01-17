using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public string zoneID;

    public List<Zone> zonesToLoadTogether;

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
