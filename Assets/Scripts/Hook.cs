using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] Minigame minigame;
<<<<<<< HEAD

=======
>>>>>>> 05ad6e3fff151f79fa730b6b7d95175064d3e705
    private void OnTriggerStay2D(Collider2D stay)
    {
        if (stay.CompareTag("PlayerCanva"))
        {
            minigame.speed = .5f;
        }
    }
<<<<<<< HEAD

=======
>>>>>>> 05ad6e3fff151f79fa730b6b7d95175064d3e705
    private void OnTriggerExit2D(Collider2D exit)
    {
        if (exit.CompareTag("PlayerCanva"))
        {
            minigame.speed = 2f;
        }
    }
}
