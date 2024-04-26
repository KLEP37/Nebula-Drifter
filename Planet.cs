using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Planet : SpaceObject
{
    public StarSystem.AU orbitDistance;

    public Planet(float earthMasses, float orbitDistance, SpaceObjectType type, float density)
    {
        FindParentSystem();

        this.mass = earthMasses;
        this.orbitDistance.SetAU(orbitDistance);
        this.type = type;
        radius.SetEarthSizes(earthMasses/density);
        GenerateBody();
    }

    void GenerateBody()
    {
        body = new GameObject("Planet");
        var visual = body.AddComponent<VisualObject>();
        visual.spaceObject = this;
        visual.Initiate();
        Debug.Log("Mass: " + mass + "; Orbit Distance: " + orbitDistance.GetAU() + " (Unity Units) " + orbitDistance.GetUnityUnits());
        body.transform.position = Vector3.right * orbitDistance.GetUnityUnits() + Vector3.right * starSystem.star.radius.GetUnityUnits() * starSystem.GetScale(starSystem.star.radius.GetEarthSizes());
        body.transform.localScale = Vector3.one * 2 * radius.GetUnityUnits() * starSystem.GetScale(radius.GetEarthSizes());

        MeshRenderer meshRenderer = body.GetComponent<MeshRenderer>();
        meshRenderer.material = starSystem.planetMaterial;
    }
}
