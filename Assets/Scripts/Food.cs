using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Food : MonoBehaviour
{
    [Header("Hook Variables")]
    [SerializeField] float chanceHook;
    [SerializeField] GameObject Minigame;

    [SerializeField] Player player;


    [Header("Attributes")]
    [Tooltip("Try put one case after point")][Range(0f, 1f)] public float difficulty;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
    }

    public void Sort()
    {
        chanceHook = Random.Range(0f, 1f) * difficulty;

        if (chanceHook > 0.5f)
        {
            HookMinigame();
        }
    }

    private void HookMinigame()
    {
        player.canMove = false;
        Debug.Log("Hooked " + "difficulty: " + difficulty);
        Minigame.SetActive(true);
    }
}
