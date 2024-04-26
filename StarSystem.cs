using System.Collections.Generic;
using UnityEngine;

public class StarSystem : MonoBehaviour
{
    // testing seed is 4685132, 4444292, 4267610 and 4444444
    // edge case: 4489068

    public static float AUToUnityUnits;

    [SerializeField] int seed;
    [SerializeField] float AUScale;

    public Star star;
    List<Planet> planets = new List<Planet>();
    System.Random rng;
    public float habitableZoneStart;
    public float habitableZoneEnd;

    [SerializeField] public Material starMaterial;
    [SerializeField] public Material planetMaterial;

    [SerializeField] public Transform user;

    // Start is called before the first frame update
    void Start()
    {
        StarSystem.AUToUnityUnits = AUScale;
        rng = new System.Random(seed);
        GenerateStarSystem();
    }

    void GenerateStarSystem()
    {
        star = new Star(0.3f + (float)rng.NextDouble() * 2);
        GeneratePlanets(0);
    }

    void GeneratePlanets(int generatedPlanetsCount)
    {
        float generationTreshold = 1 / (0.05f * generatedPlanetsCount + 1.05f);
        if (rng.NextDouble() > generationTreshold)
        {
            return;
        }

        float planetMass = 1;
        float planetDensity = 3;
        SpaceObject.SpaceObjectType type;
        int random = (int)(rng.NextDouble() * 100);
        if (random <= 25)
        {
            type = SpaceObject.SpaceObjectType.GasGiant;
        }
        else
        {
            type = SpaceObject.SpaceObjectType.TerrestrialPlanet;
        }


        //Distance
        AU distance = new AU();
        distance.SetAU(Mathf.Clamp(1 / (1 - Mathf.Pow((float)rng.NextDouble(), 2)) - 0.99f, 0.1f, 50));

        for (int i = 0; i < generatedPlanetsCount; i++)
        {
            if (Mathf.Abs(distance.GetAU() - planets[i].orbitDistance.GetAU()) < 0.1f)
            {
                GeneratePlanets(generatedPlanetsCount);
                return;
            }
        }

        //Size
        if (type == SpaceObject.SpaceObjectType.TerrestrialPlanet)
        {
            float x = (float)rng.NextDouble();
            planetMass = Mathf.Clamp(1 / (1 - Mathf.Pow(x, 3)) + Mathf.Log(x, 10 * Mathf.Exp(1)) + Mathf.PI / 10, 0.1f, 10);
        }
        else if(type == SpaceObject.SpaceObjectType.GasGiant)
        {
            float x = (float)rng.NextDouble();
            planetMass = Mathf.Clamp(2 / (1 - Mathf.Pow(x, 3)) + 10 * Mathf.Sqrt(x) - 1, 1, 40);
        }

        planets.Add(new Planet(planetMass, distance.GetAU(), type, planetDensity));
        GeneratePlanets(generatedPlanetsCount + 1);
    }

    public float GetScale(float earthSizes)
    {
        return 250 / Mathf.Pow(earthSizes, 0.5f);
    }

    public struct AU
    {
        //Sizes of bodies are their radii

        float value;

        public float GetAU()
        {
            return value;
        }
        public float GetUnityUnits()
        {
            return value * StarSystem.AUToUnityUnits;
        }
        public float GetSunSizes()
        {
            return value * 215.032f;
        }
        public float GetEarthSizes()
        {
            return value * 23454.8f;
        }

        public void SetAU(float value)
        {
            this.value = value;
        }
        public void SetUnityUnits(float value)
        {
            this.value = value / StarSystem.AUToUnityUnits;
        }
        public void SetSunSizes(float value)
        {
            this.value = value / 215.032f;
        }
        public void SetEarthSizes(float value)
        {
            this.value = value / 23454.8f;
        }
    }
}