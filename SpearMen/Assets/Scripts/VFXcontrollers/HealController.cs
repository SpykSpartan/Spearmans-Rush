using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealController : MonoBehaviour
{
    public float sineTimer;
    public float intensityRange;

    Light HPLight;
    // Start is called before the first frame update
    void Start()
    {
        HPLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        float intensity = intensityRange * Mathf.Sin(Time.time * sineTimer) + (intensityRange * 2f);
        HPLight.intensity = intensity;
    }
}
