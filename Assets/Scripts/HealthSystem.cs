using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    public int health = 100;


    public void UpdateHealth(int newHealth)
    {
        if(newHealth <= 200)
        {
            health = newHealth;
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}