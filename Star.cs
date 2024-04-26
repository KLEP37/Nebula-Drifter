using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : SpaceObject
{
    public float temperature;
    public Color color;

    MeshRenderer meshRenderer;

    public Star(float mass)
    {
        FindParentSystem();

        this.mass = mass;
        CalculateFromMass();
        type = SpaceObjectType.Star;
        CreateBody();
    }

    void CreateBody()
    {
        body = new GameObject("Star");
        var visual = body.AddComponent<VisualObject>();
        visual.spaceObject = this;
        visual.Initiate();
        meshRenderer = body.GetComponent<MeshRenderer>();
        meshRenderer.material = starSystem.starMaterial;
        Color emissionColor = color;
        emissionColor.a = .5f;
        meshRenderer.material.color = color;
        meshRenderer.material.SetColor("_EmissionColor", emissionColor);

        body.transform.localScale = Vector3.one * 2 * radius.GetUnityUnits() * starSystem.GetScale(radius.GetEarthSizes());
    }

    void CalculateFromMass()
    {
        temperature = Mathf.Sqrt(Mathf.Sqrt(Mathf.Pow(mass, 2.5f) * 983449600000000));
        radius.SetSunSizes(Mathf.Pow(mass, 0.8f));
        color = Mathf.CorrelatedColorTemperatureToRGB(temperature);
        starSystem.habitableZoneStart = Mathf.Sqrt(mass * 0.95f);
        starSystem.habitableZoneEnd = Mathf.Sqrt(mass * 1.37f);
    }
}
