using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fish : MonoBehaviour
{

    [Header("Limits")]
    [SerializeField] private GameObject _minY;
    [SerializeField] private GameObject _maxY;
    float minY;
    float maxY;

    [Header("Components")]
    [SerializeField] Minigame minigame;
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] Image _saturation;


    float strenght = 2.5f;
    float saturation = 0.9f;
    bool isHooking = false;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        minY = _minY.transform.position.y;
        maxY = _maxY.transform.position.y;
    }
    private void Update()
    {
        Debug.Log(saturation);

        if(minigame.waitTime <= 0f)
        {
            if (saturation >= 0f && isHooking == false)
            {
                saturation -= 0.0025f;
            }
            _saturation.fillAmount = saturation;
            Move();
        }
    }

    public bool YouWin()
    {
        if (saturation <= 0f)
            return true;
        return false;
    }

    public bool YouLose()
    {
        if (saturation >= 1f)
            return true;
        return false;
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (transform.position.y < maxY)
                rb2d.velocity = new Vector2(0, strenght);
            else
                rb2d.velocity = new Vector2(0, 0);
        }
        else
        {
            if (transform.position.y > minY)
                rb2d.velocity = new Vector2(0, -strenght);
            else
                rb2d.velocity = new Vector2(0, 0);
        }
    }

    private void OnTriggerStay2D(Collider2D stay)
    {
        if (stay.CompareTag("HookCanva"))
        {
            if (saturation <= 1)
            {
                isHooking = true;
                saturation += 0.01f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D exit)
    {
        if (exit.CompareTag("HookCanva"))
        {
            isHooking = false;
        }
    }
}
