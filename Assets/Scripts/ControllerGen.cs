using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerGen : MonoBehaviour
{
    [SerializeField] GameObject food;
    Food[] foods;

    [Header("Spawn Options")]
    int qtdSpawned;
    [SerializeField] int qtdToSpawn = 3;
    Vector2 placetoSpawn;

    // Update is called once per frame
    void Update()
    {
        foods = FindObjectsOfType<Food>();
        qtdSpawned = foods.Length + 1;
        placetoSpawn = new Vector2(Random.Range(-4.5f, 4.5f), 5.6f);

        if (qtdSpawned <= qtdToSpawn)
        {
            Instantiate(food, placetoSpawn, transform.rotation);
        }
    }
}
