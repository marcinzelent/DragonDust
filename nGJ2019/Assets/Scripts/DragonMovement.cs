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
	
	public HealthBar healthBar;
	
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
		while(state == State.slim && swarm.Collapse < 0.8f)
		{
			swarm.Collapse += 0.05f;
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
		while(state == State.normal && swarm.Collapse > 0)
		{
			swarm.Collapse -= 0.07f;
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
			swarm.Noise -= 0.12f;
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
	
	private void turnAntiSwirl()
	{
		if(state == State.swirl)
		{
			state = State.normal;
			turnVisible(true);
			swarm.activate(false);
		}
	}
	
	private void turnAntiSlim()
	{
		if(state == State.slim)
		{
			state = State.normal;
			StartCoroutine("transformAntiSlim");
		}
	}
	
	private void turnAntiSpread()
	{
		if(state == State.spread)
		{
			state = State.normal;
			StartCoroutine("transformAntiSpread");
		}
	}
	
	private void resetTurn()
	{
		swarm.Collapse = 0;
		swarm.Noise = 0;
	}
	
	void Start()
	{
		swarm = GetComponent<SwarmSystem>();
		swarm.activate(false);
	}
	
	void Update()
	{
		// keyboard scheme
		if(Input.GetKey("w"))
			moveUp();
		if(Input.GetKey("a"))
			moveLeft();
		if(Input.GetKey("s"))
			moveDown();
		if(Input.GetKey("d"))
			moveRight();
		
		if(Input.GetKeyDown("i"))
			turnSwirl();
		if(Input.GetKeyUp("i"))
			turnAntiSwirl();
		
		if(Input.GetKeyDown("j"))
			turnSlim();
		if(Input.GetKeyUp("j"))
			turnAntiSlim();
		
		if(Input.GetKeyDown("k"))
			turnSpread();
		if(Input.GetKeyUp("k"))
			turnAntiSpread();
		
		
		// xbox scheme
		if(Input.GetAxis("JoystickY") > 0.5f)
			moveUp();
		if(Input.GetAxis("JoystickX") < -0.5f)
			moveLeft();
		if(Input.GetAxis("JoystickY") < -0.5f)
			moveDown();
		if(Input.GetAxis("JoystickX") > 0.5f)
			moveRight();
		
		if(Input.GetButtonDown("X"))
			turnSwirl();
		if(Input.GetButtonUp("X"))
			turnAntiSwirl();
		
		if(Input.GetButtonDown("A"))
			turnSlim();
		if(Input.GetButtonUp("A"))
			turnAntiSlim();
		
		if(Input.GetButtonDown("Y"))
			turnSpread();
		if(Input.GetButtonUp("Y"))
			turnAntiSpread();
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(2*horizontalBound, 2*verticalBound, 0));
	}
	
	void OnTriggerEnter(Collider other)
	{
		EnemyCollider enemy = other.gameObject.GetComponent<EnemyCollider>();
		if(enemy != null)
		{
			healthBar.health--;
		}
	}
	
}