using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] Minigame minigame;
    private void OnTriggerStay2D(Collider2D stay)
    {
        if (stay.CompareTag("PlayerCanva"))
        {
            minigame.speed = .5f;
        }
    }
    private void OnTriggerExit2D(Collider2D exit)
    {
        if (exit.CompareTag("PlayerCanva"))
        {
            minigame.speed = 2f;
        }
    }
}
