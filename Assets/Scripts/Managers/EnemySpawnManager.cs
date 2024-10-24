using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // When making a new enemy, add them in the places that have the ~~~~~~~~~~ENEMY tag

    //~~~~~~~~~~ENEMY (ADD IN INSPECTOR)
    public GameObject[] enemy;
	public GameObject enemyParent;
	public GameObject player;

	public GameObject[] enemyCountArray;
	private int totalEnemyCount = 0;

	//~~~~~~~~~~ENEMY
	public List<int> enemyCount = new(5);

	public int currentWave = 0;
	public int spawnBufferDistance = 7;

	private Vector3 lastBossSpawnPos;
	private float mapSize = GameManager.mapSize;

	private bool runningAssignEnemiesToLists;

	//~~~~~~~~~~ENEMY
	private enum EnemyType
	{
		Level1,
		Level2,
		Level3,
		Boss1,
		IceZombie
	}
	//~~~~~~~~~~ENEMY
	private Dictionary<EnemyType, int> enemiesToSpawn = new()
	{
		{ EnemyType.Level1, 4 },
		{ EnemyType.Level2, 0 },
		{ EnemyType.Level3, 0 },
		{ EnemyType.Boss1, 0 },
		{ EnemyType.IceZombie, 0 }
	};
	void Update()
	{
		enemyCountArray = GameObject.FindGameObjectsWithTag("Enemy");
		totalEnemyCount = enemyCountArray.Length;

		if (totalEnemyCount == 0 && UIManager.Instance.isGameUnpaused)
		{
			if (GameManager.Instance.didLoadSpawnManager)
			{
				SpawnEnemiesOnLoad();
				GameManager.Instance.didLoadSpawnManager = false;
			}
			else
			{
				currentWave++;

				NumberOfEnemiesToSpawn();

				SpawnEnemyWave();
			}
		}
	}

	public IEnumerator AssignEnemiesToLists()
	{
		yield return null;
		enemyCount = new() { 0, 0, 0, 0, 0 };

		foreach (GameObject enemy in enemyCountArray)
		{
			if (enemy.name.Contains("Enemy 1"))
			{
				enemyCount[0]++;
			}
			else if (enemy.name.Contains("Enemy 2"))
			{
				enemyCount[1]++;
			}
			else if (enemy.name.Contains("Enemy 3"))
			{
				enemyCount[2]++;
			}
			else if (enemy.name.Contains("Boss 1"))
			{
				enemyCount[3]++;
			}
			else if (enemy.name.Contains("Ice Zombie"))
			{
				enemyCount[4]++;
			}
		}
	}

	void NumberOfEnemiesToSpawn()
	{
		if (currentWave % 10 == 0)
		{
			enemiesToSpawn[EnemyType.Boss1] += 1;
		}
		//~~~~~~~~~~ENEMY
		switch (GameManager.Instance.difficulty)
		{
			case 1:
				enemiesToSpawn[EnemyType.Level1] = currentWave + 3;
				enemiesToSpawn[EnemyType.Level2] = currentWave + 1;
				enemiesToSpawn[EnemyType.Level3] = currentWave + -2; // Spawns 1 lvl3 on wave 3, then incriments
				enemiesToSpawn[EnemyType.IceZombie] = currentWave + -3; // Spawns 1 ice zombie on wave 4, then incriments
				break;
			case 2:
				enemiesToSpawn[EnemyType.Level1] = currentWave + 5;
				enemiesToSpawn[EnemyType.Level2] = currentWave + 1;
				enemiesToSpawn[EnemyType.Level3] = currentWave + 0;
				enemiesToSpawn[EnemyType.IceZombie] = currentWave + -2;
				break;
			case 3:
				enemiesToSpawn[EnemyType.Level1] = currentWave + 7;
				enemiesToSpawn[EnemyType.Level2] = currentWave + 3;
				enemiesToSpawn[EnemyType.Level3] = currentWave + 2;
				enemiesToSpawn[EnemyType.IceZombie] = currentWave + 1;
				break;
		}
	}
	private Vector3 GenerateSpawnPosition(int type)
	{
		Vector3 randomSpawnPos = GetRandomNavMeshPosition();

		float posY = type == 3 ? 2 : 0.5f;
		randomSpawnPos.y = posY;

		if (type == 3)
		{
			lastBossSpawnPos = randomSpawnPos;
			return lastBossSpawnPos;
		}

		while (Vector3.Distance(player.transform.position, randomSpawnPos) < spawnBufferDistance 
			|| (Vector3.Distance(lastBossSpawnPos, randomSpawnPos) < 5))
		{
			randomSpawnPos = GetRandomNavMeshPosition();
			randomSpawnPos.y = posY;
		}

		return randomSpawnPos;
	}

	Vector3 GetRandomNavMeshPosition()
	{
		Vector3 randomPosition = new(
			Random.Range(-mapSize, mapSize),
			Random.Range(0, mapSize),
			Random.Range(-mapSize, mapSize)
		);

		NavMeshHit hit;
		if(NavMesh.SamplePosition(randomPosition, out hit, 4, NavMesh.AllAreas)) 
			// the 4 was orginally the spawnBufferDistance, but modifying this number so that the enemies would spawn further away from the player,
			// actually breaks it. so i hardcoded this instead and modified it for the use case above (while loop)
		{
			return hit.position;
		}
		return randomPosition;
	}
	void SpawnEnemyWave()
	{
		//~~~~~~~~~~ENEMY
		switch (currentWave % 10)
		{
			case 0:
				{
					for (int i = 0; i < enemiesToSpawn[EnemyType.Boss1]; i++)
					{
						InstantiateEnemy(3);
					}
					for (int i = 0; i < enemiesToSpawn[EnemyType.Level1]; i++)
					{
						InstantiateEnemy(0);
					}
					for (int i = 0; i < enemiesToSpawn[EnemyType.IceZombie]; i++)
					{
						InstantiateEnemy(4);
					}

					break;
				}

			default:
				{
					for (int i = 0; i < enemiesToSpawn[EnemyType.Level1]; i++)
					{
						InstantiateEnemy(0);
					}
					for (int i = 0; i < enemiesToSpawn[EnemyType.Level2]; i++)
					{
						InstantiateEnemy(1);
					}
					for (int i = 0; i < enemiesToSpawn[EnemyType.Level3]; i++)
					{
						InstantiateEnemy(2);
					}
					for (int i = 0; i < enemiesToSpawn[EnemyType.IceZombie]; i++)
					{
						InstantiateEnemy(4);
					}
					
					break;
				}
		}

		StartCoroutine(AssignEnemiesToLists());
	}

	public void SpawnEnemiesOnLoad()
	{
		//~~~~~~~~~~ENEMY
		for (int i = 0; i < GameManager.Instance.bossLevel1; i++) // must be first so the boss pos can be saved
		{
			InstantiateEnemy(3);
		}
		for (int i = 0; i < GameManager.Instance.enemyLevel1; i++)
		{
			InstantiateEnemy(0);
		}
		for (int i = 0; i < GameManager.Instance.enemyLevel2; i++)
		{
			InstantiateEnemy(1);
		}
		for (int i = 0; i < GameManager.Instance.enemyLevel3; i++)
		{
			InstantiateEnemy(2);
		}
		for (int i = 0; i < GameManager.Instance.iceZombie; i++)
		{
			InstantiateEnemy(4);
		}

		StartCoroutine(AssignEnemiesToLists());
	}

	public void InstantiateEnemy(int type)
	{
		GameObject instantiatedEnemy = Instantiate(enemy[type], GenerateSpawnPosition(type), Quaternion.Euler(90, 0, 0));

		instantiatedEnemy.transform.parent = enemyParent.transform; // Sets parent
		instantiatedEnemy.name = enemy[type].name; // Removes (Clone) from name
	}
	public void InstantiateEnemyDebug()
	{
		GameObject instantiatedEnemy = Instantiate(enemy[0], GenerateSpawnPosition(0), Quaternion.Euler(90, 0, 0));
		instantiatedEnemy.transform.parent = enemyParent.transform; // Sets parent
		instantiatedEnemy.name = enemy[0].name; // Removes (Clone) from name
	}
}
