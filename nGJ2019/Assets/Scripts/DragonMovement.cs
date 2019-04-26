using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragonMovement : MonoBehaviour
{
	public List<Renderer> solids;
	
	public float horizontalSpeed = 0.1f;	
	public float verticalSpeed = 0.2f;	
	
	public float horizontalBound = 6f;	
	public float verticalBound = 4f;
	
	private SwarmSystem swarm;
	
	private enum State {normal, swirl, slim, spread};
	private State state = State.normal;
		
	private void moveUp()
	{
		transform.Translate(Vector3.up * verticalSpeed);
		if(transform.position.y > verticalBound)
			transform.position = new Vector3(transform.position.x, verticalBound, transform.position.z);
	}
	
	private void moveDown()
	{
		transform.Translate(Vector3.down * verticalSpeed);
		if(transform.position.y < -verticalBound)
			transform.position = new Vector3(transform.position.x, -verticalBound, transform.position.z);
	}
	
	private void moveLeft()
	{
		transform.Translate(Vector3.left * horizontalSpeed);
		if(transform.position.x < -horizontalBound)
			transform.position = new Vector3(-horizontalBound, transform.position.y, transform.position.z);
	}
	
	private void moveRight()
	{
		transform.Translate(Vector3.right * horizontalSpeed);
		if(transform.position.x > horizontalBound)
			transform.position = new Vector3(horizontalBound, transform.position.y, transform.position.z);
	}
	
	private void turnVisible(bool visible)
	{
		foreach(Renderer solid in solids)
		{
			solid.enabled = visible;
		}
	}
	
	private IEnumerator transformSlim()
	{
		while(state == State.slim && transform.localScale.y > 0.2f)
		{
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y-0.03f, transform.localScale.z-0.03f);
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	private IEnumerator transformSpread()
	{
		while(state == State.spread && swarm.Noise < 3)
		{
			swarm.Noise += 0.05f;
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	private IEnumerator transformAntiSlim()
	{
		while(state == State.normal && transform.localScale.y < 1)
		{
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y+0.05f, transform.localScale.z+0.05f);
			yield return new WaitForSeconds(0.01f);
		}
		if(state == State.normal)
		{
			turnVisible(true);
			swarm.activate(false);
		}
	}
	
	private IEnumerator transformAntiSpread()
	{
		while(state == State.normal && swarm.Noise > 0)
		{
			swarm.Noise -= 0.08f;
			yield return new WaitForSeconds(0.01f);
		}
		if(state == State.normal)
		{
			turnVisible(true);
			swarm.activate(false);
		}
	}
	
	private void turnSwirl()
	{
		resetTurn();
		turnVisible(false);
		swarm.activate(true);
		state = State.swirl;
	}
	
	private void turnSlim()
	{
		resetTurn();
		turnVisible(false);
		swarm.activate(true);
		state = State.slim;
		StartCoroutine("transformSlim");
	}
	
	private void turnSpread()
	{
		resetTurn();
		turnVisible(false);
		swarm.activate(true);
		state = State.spread;
		StartCoroutine("transformSpread");
	}
	
	private void turnNormal()
	{
		State prev = state;
		state = State.normal;
		if(prev == State.swirl)
		{
			turnVisible(true);
			swarm.activate(false);
		}
		
		if(prev == State.slim)
			StartCoroutine("transformAntiSlim");
		
		if(prev == State.spread)
			StartCoroutine("transformAntiSpread");
	}
	
	private void resetTurn()
	{
		transform.localScale = Vector3.one;
		swarm.Noise = 0;
	}
	
	void Start()
	{
		swarm = GetComponent<SwarmSystem>();
		swarm.activate(true);
	}
	
	void Update()
	{
		if(Input.GetKey("w"))
			moveUp();
		if(Input.GetKey("a"))
			moveLeft();
		if(Input.GetKey("s"))
			moveDown();
		if(Input.GetKey("d"))
			moveRight();
		
		if(Input.GetKeyDown("r"))
			turnSwirl();
		if(Input.GetKeyUp("r"))
			turnNormal();
		
		if(Input.GetKeyDown("f"))
			turnSlim();
		if(Input.GetKeyUp("f"))
			turnNormal();
		
		if(Input.GetKeyDown("e"))
			turnSpread();
		if(Input.GetKeyUp("e"))
			turnNormal();
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(2*horizontalBound, 2*verticalBound, 0));
	}
	
}