using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    [SerializeField] GameObject enemyBar;

    [SerializeField] int state = 1;

    public float speed = 4f;
    public float waitTime = 2f;

    [SerializeField] private GameObject minY;
    [SerializeField] private GameObject maxY;
    public float _minY;
    public float _maxY;


    int dir = 1;


    private void Update()
    {
        

        switch (state)
        {
            case (1):
                Basic();
                break;

                //Fazer mais estados
                //maximo 3 para jam
        }
        
    }

    void Basic()
    {
        waitTime -= Time.deltaTime;

        if(waitTime <= 0)
        {
            enemyBar.transform.Translate(Vector3.up * speed * dir * Time.deltaTime);

            _minY = minY.transform.position.y;
            _maxY = maxY.transform.position.y;

            if (enemyBar.transform.position.y >= _maxY - 0.6f) // adicionei apenas um ajuste aqui para barra ficar na pos certa
            {
                dir = -1;
            }
            else if (enemyBar.transform.position.y <= _minY + 0.3f) // aqui tbm
            {
                dir = 1;
            }
        }

     }
}
