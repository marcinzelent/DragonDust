using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject crystalPrefab;
	public Material liveCrystal;
	public Material deadCrystal;
	
	public int lives;
	
	private int currentHealth;
	public int health
	{
		get{return currentHealth;}
		set
		{
			currentHealth = value;
			updateCrystals();
		}
	}
	
	private List<Renderer> crystals = new List<Renderer>();
	
	private void updateCrystals()
	{
		for(int i=0; i<lives; i++)
		{
			if(health > i)
				crystals[i].material = liveCrystal;
			else
				crystals[i].material = deadCrystal;
		}
	}
	
    void Start()
    {
        for(int i=0; i<lives; i++)
		{
			GameObject crystal = Instantiate(crystalPrefab, transform.position + Vector3.right*0.8f*i, Quaternion.identity);
			crystals.Add(crystal.GetComponentInChildren<Renderer>());
		}
		
		health = lives;
		updateCrystals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
