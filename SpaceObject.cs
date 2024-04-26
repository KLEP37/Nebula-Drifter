using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceObject
{
    public float mass;
	public SpaceObjectType type;
	public GameObject body;
	public StarSystem starSystem;
    public StarSystem.AU radius;

    public SpaceObject()
	{

	}
	public SpaceObject(float mass)
	{
		this.mass = mass;
	}

	public enum SpaceObjectType
	{
		TerrestrialPlanet,
		GasGiant,
		Star,
		Moon
	}

	public void FindParentSystem()
	{
		starSystem = GameObject.FindGameObjectWithTag("StarSystem").GetComponent<StarSystem>();
	}
}