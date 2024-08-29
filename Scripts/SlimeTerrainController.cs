using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeTerrainController : MonoBehaviour
{
    //its not working rewrite it
    public Transform player;
    public Material slimeMaterial;
    public float recedeSpeed = 1.0f;
    public float maxAlpha = 0.8f;
    public float minAlpha = 0.0f;
    public float distanceThreshold = 10.0f;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        
        float alpha = Mathf.Lerp(maxAlpha, minAlpha, distance / distanceThreshold);

        
        Color color = slimeMaterial.color;

        
        color.a = alpha;

       
        slimeMaterial.color = color;
    }
}
