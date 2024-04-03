using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Food : MonoBehaviour
{
    [Header("Hook Variables")]
    [SerializeField] float chanceHook;
    [SerializeField] GameObject Minigame;
    [SerializeField] GameObject MinigameCanva;

    [SerializeField] Player player;
    [SerializeField] Rigidbody2D rb2d;

    [Header("Attributes")]
    float difficulty;


    [Header("Sei lá configs pontocom")]
    bool arrived = false;
    float minY = -2;
    float maxY = 2;
    Vector2 endedPos;

    private void Awake()
    {   
        rb2d = GetComponent<Rigidbody2D>();
        difficulty = Random.Range(0f, 1f);
        player = FindAnyObjectByType<Player>();

    }

    private void Start()
    {
        endedPos = new Vector2(transform.position.x, Random.Range(minY, maxY));

        if(!arrived)
            rb2d.velocity = Vector2.down * 2;
    }

    private void Update()
    {
        if(transform.position.y <= endedPos.y)
        {
            arrived = true;
            rb2d.velocity = Vector2.zero;
        }
    }

    public void Sort()
    {
        chanceHook = Random.Range(0f, 1f) * difficulty;

        if (chanceHook > 0.5f)
        {
            HookMinigame();
            player.canMove = false;
            player.isMinigaming = true;
        }
    }

    private void HookMinigame()
    {   
        Debug.Log("Hooked " + "difficulty: " + difficulty);
        Minigame.SetActive(true);
        MinigameCanva.SetActive(true);
    }
}
