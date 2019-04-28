using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragonMovement : MonoBehaviour
{
	public List<Renderer> solids;
	
	public GameObject hurtPrefab;
	
	public float horizontalSpeed = 0.1f;	
	public float verticalSpeed = 0.2f;	
	
	private Vector3 direcV = Vector3.zero;
	private Vector3 deltaV = Vector3.zero;
	
	public float horizontalBound = 6f;	
	public float verticalBound = 4f;
	
	private float slitherPhase = 0;
	
	public HealthBar healthBar;
	
	private float hurtCooldown = 0;
	
	private SwarmSystem swarm;
	
	private enum State {normal, swirl, slim, spread};
	private State state = State.normal;
	
	private void applyMotion(Vector3 direc)
	{
		direcV = direcV + direc;
	}
		
	private void moveUp()
	{
		applyMotion(Vector3.up * verticalSpeed);
	}
	
	private void moveDown()
	{
		applyMotion(Vector3.down * verticalSpeed);
	}
	
	private void moveLeft()
	{
		applyMotion(Vector3.left * horizontalSpeed);
	}
	
	private void moveRight()
	{
		applyMotion(Vector3.right * horizontalSpeed);
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
	
	void FixedUpdate()
	{
		transform.Translate(deltaV);
		deltaV = 0.6f*deltaV + 0.4f*direcV;
		direcV = Vector3.zero;
		
		if(transform.position.y > verticalBound)
			transform.position = new Vector3(transform.position.x, verticalBound, transform.position.z);
		
		if(transform.position.y < -verticalBound)
			transform.position = new Vector3(transform.position.x, -verticalBound, transform.position.z);
		
		if(transform.position.x < -horizontalBound)
			transform.position = new Vector3(-horizontalBound, transform.position.y, transform.position.z);
		
		if(transform.position.x > horizontalBound)
			transform.position = new Vector3(horizontalBound, transform.position.y, transform.position.z);
		
		slitherPhase += 0.1f;
		foreach(Transform t in swarm.meshRender.bones)
		{
			t.Translate(new Vector3(0, Mathf.Sin(t.position.x-transform.position.x+slitherPhase)*0.002f, 0), Space.World);
		}
		foreach(Renderer r in solids)
		{
			r.transform.Translate(new Vector3(0, Mathf.Sin(r.transform.position.x-transform.position.x+slitherPhase+0.2f)*-0.008f, 0), Space.World);
		}
	}
	
	void Update()
	{
		if(hurtCooldown > 0)
			hurtCooldown -= Time.deltaTime;
		
		if(healthBar.health <= 0)
			return;
		
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
		if(Input.GetAxis("JoystickY") < -0.5f)
			moveUp();
		if(Input.GetAxis("JoystickX") < -0.5f)
			moveLeft();
		if(Input.GetAxis("JoystickY") > 0.5f)
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
	
	private IEnumerator makeGoAway(Transform t, float spread)
	{
		Vector3 delta = Random.onUnitSphere * Random.Range(0.03f, 0.1f);
		Vector3 start = t.position;
		while((start-t.position).magnitude < spread)
		{
			t.Translate(delta);
			yield return new WaitForSeconds(0.01f);
		}
		Destroy(t.gameObject);
	}
	
	private void getHurt()
	{
		if(hurtCooldown <= 0)
		{
			healthBar.health--;
			if(healthBar.health == 0)
			{
				for(int i=0; i<200; i++)
				{
					Transform t = ((GameObject)Instantiate(hurtPrefab, transform.position + swarm.collapseCenter, Quaternion.identity)).transform;
					StartCoroutine(makeGoAway(t, 6));
				}
				turnVisible(false);
				swarm.activate(false);
			}
			else if(healthBar.health > 0)
			{
				hurtCooldown = 1.5f;
				
				for(int i=0; i<30; i++)
				{
					Transform t = ((GameObject)Instantiate(hurtPrefab, transform.position + swarm.collapseCenter, Quaternion.identity)).transform;
					StartCoroutine(makeGoAway(t, 3));
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		EnemyCollider enemy = other.gameObject.GetComponent<EnemyCollider>();
		if(enemy != null)
		{
			switch(enemy.type)
			{
				case ObstacleType.caveWalls:
				case ObstacleType.rockTop:
				case ObstacleType.rockBottom:
				case ObstacleType.rockJaws:
					getHurt();
					break;
				case ObstacleType.narrowPassage:
					if(state != State.slim)
						getHurt();
					break;
				case ObstacleType.net:
					if(state != State.spread)
						getHurt();	
					break;
			}
		}
	}
	
}