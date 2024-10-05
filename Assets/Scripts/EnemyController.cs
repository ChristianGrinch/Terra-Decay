using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

	private int damage;
	private float speed;
	public int exp;
	public int health;
	private float detectionRadius = 15;
	private bool followingPlayer = false;
	private int difficulty = 1;
	private float damageRate = 0.4f;
	private bool isDamagingPlayer = false;
	private int jumpForce = 10;
	private bool isRunningJump;

	private Rigidbody enemyRb;
	private GameObject player;
	private HealthSystem healthSystem;

    public GameObject ringPrefab;
    private float ringExpandTime = 1f;
    private float ringMaxSize = 45f;
    private float spawnRingCooldown = 10f;
    private float lastSpawnRingTime = 0f;

    // Start is called before the first frame update
    void Start()
	{
		enemyRb = GetComponent<Rigidbody>();
		player = GameObject.Find("Player");
		healthSystem = gameObject.GetComponent<HealthSystem>();
		AssignStats();
		healthSystem.health = health;
    }
	IEnumerator Jump(float jumpHeight)
	{
		isRunningJump = true;
		yield return new WaitForSeconds(Random.Range(5, 14)); // Delay between jumps

		enemyRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        enemyRb.useGravity = true;

        yield return new WaitForSeconds(0.2f); // Wait 0.2 seconds so Y is > jumpHeight

        while (transform.position.y > jumpHeight)
        {
            yield return null;
        }

        enemyRb.useGravity = false;
		isRunningJump = false;
    }
    IEnumerator SpawnRing() // not my code, chatgpt
    {
        lastSpawnRingTime = Time.time;
        GameObject ring = Instantiate(ringPrefab, transform.position, Quaternion.identity);

        ring.transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);

        float currentTime = 0f;
        Vector3 initialScale = ring.transform.localScale;

        while (currentTime < ringExpandTime)
        {
            float progress = currentTime / ringExpandTime;
            float scale = Mathf.Lerp(1f, ringMaxSize, progress);
            ring.transform.localScale = new Vector3(scale, ring.transform.localScale.y, scale);

            currentTime += Time.deltaTime;
            yield return null;
        }

        Destroy(ring, 0.5f);
    }
    // Update is called once per frame
    void Update()
	{
		if (!isRunningJump && gameObject.name == "Boss 1")
		{
			StartCoroutine(Jump(2.1f));
		}

		if (!followingPlayer
			&& Vector3.Distance(player.transform.position, transform.position) < detectionRadius
			&& UIManager.Instance.isGameActive)
		{
			MoveEnemy();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();

			if (playerHealth != null && !isDamagingPlayer)
			{
				StartCoroutine(DamagePlayer(collision, playerHealth));
			}
			else
			{
				Debug.LogError("Unable to locate HealthSystem on player!");
			}
				
		}

		if (collision.gameObject.CompareTag("Bullet"))
		{
			if (!followingPlayer)
			{
				StartCoroutine(MoveForTime(5));
			}
		}

        if (collision.gameObject.CompareTag("Ground") && gameObject.name == "Boss 1")
        {
            if(Time.time - lastSpawnRingTime >= spawnRingCooldown)
            {
                StartCoroutine(SpawnRing());
            }
        }
    }
	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			isDamagingPlayer = false;
			StopAllCoroutines();
		}
	}

	IEnumerator DamagePlayer(Collision collision, HealthSystem playerHealth)
	{
		isDamagingPlayer = true;

		int initialHealth = playerHealth.health - damage;
		playerHealth.UpdateHealth(initialHealth);

		while (playerHealth.health > 0)
		{
			int modifiedHealth = playerHealth.health - damage;
			yield return new WaitForSeconds(damageRate);
			playerHealth.UpdateHealth(modifiedHealth);
			Debug.Log("running DamagePlayer in if");
		}
		isDamagingPlayer = false;
	}

	IEnumerator MoveForTime(float duration)
	{
		followingPlayer = true;
		float timePassed = 0;

		while (timePassed < duration)
		{
			// Move the enemy once per frame
			MoveEnemy();
			timePassed += Time.deltaTime;
			yield return null;  // Wait for the next frame
		}
		followingPlayer = false;
	}

	void MoveEnemy()
	{
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;

        enemyRb.AddForce(Time.deltaTime * speed * direction, ForceMode.Impulse);
	}

	void AssignStats()
	{
		difficulty = UIManager.Instance.difficulty;

		switch (difficulty)
		{
			case 1:
				SetStats(20, 12, 10, 40, "Enemy 1");
				SetStats(25, 10, 15, 60, "Enemy 2");
				SetStats(50, 10, 20, 100, "Enemy 3");
				SetStats(90, 5, 200, 450, "Boss 1");
				break;
			case 2:
				SetStats(25, 12, 15, 55, "Enemy 1");
				SetStats(30, 10, 20, 75, "Enemy 2");
				SetStats(55, 10, 25, 130, "Enemy 3");
				SetStats(95, 5.5f, 250, 675, "Boss 1");
				break;
			case 3:
				SetStats(35, 14.5f, 18, 68, "Enemy 1");
				SetStats(45, 13, 25, 100, "Enemy 2");
				SetStats(70, 13, 35, 156, "Enemy 3");
				SetStats(130, 6.5f, 325, 800, "Boss 1");
				break;
			default:
				Debug.LogError("Invalid difficulty level.");
				break;
		}
	}
	void SetStats(int enemyDamage, float enemySpeed, int enemyExp, int enemyHealth, string enemyName)
	{
		if(gameObject.name == enemyName)
		{
			damage = enemyDamage;
			speed = enemySpeed;
			exp = enemyExp;
			health = enemyHealth;
		}
	}
}
