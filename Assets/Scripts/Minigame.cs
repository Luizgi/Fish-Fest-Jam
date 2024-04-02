using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    [SerializeField] GameObject enemyBar;

    [SerializeField] int state = 1;

    public float speed = 2f;

    [SerializeField] private GameObject minY;
    [SerializeField] private GameObject maxY;
    public float _minY;
    public float _maxY;

    int dir = 1;

    private void Start()
    {
        _minY = minY.transform.position.y;
        _maxY = maxY.transform.position.y; 
    }

    private void Update()
    {
        switch (state)
        {
            case (1):
                Basic();
                break;
        }
        
    }

    void Basic()
    {
        enemyBar.transform.Translate(Vector3.up * speed * dir * Time.deltaTime);
        
        if(enemyBar.transform.position.y >= _maxY)
        {
            dir = -1;
        }
        else if(enemyBar.transform.position.y <= _minY)
        {
            dir = 1;
        }

    }
}
