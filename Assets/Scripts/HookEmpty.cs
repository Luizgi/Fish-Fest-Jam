using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookEmpty : MonoBehaviour
{
    [SerializeField] GameObject maxY;
    float _maxY;
    Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        _maxY = maxY.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < _maxY)
        {
            rb2d.velocity = Vector2.up * 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D enter)
    {
        if (enter.CompareTag("Destroyer"))
        {
            Destroy(gameObject);
        }
    }
}
