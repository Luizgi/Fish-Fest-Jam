using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Componentes do jogador
    [Header("Components")]
    Rigidbody2D rb2d;
    SpriteRenderer spr;

    // Entradas do jogador
    [Header("Inputs")]
    float horizontal;
    float vertical;

    // UI
    [Header("UI")]
    [SerializeField] private Image mySaciety;
    [SerializeField] private Image sacietyBG;
    [SerializeField] private Image myTimeEating;
    [SerializeField] private int maxSaciety = 100;
    [SerializeField] private float sacietyDecreaseSpeed = 0.5f;
    [SerializeField] private float sacietyRecoverSpeed = 1.0f;

    // Variáveis locais
    [Header("Local")]
    float moveLim = .7f;
    float swimSpeed = 5f;
    float rotSpeed = 150f;
    int saciety;
    int lostLife;
    public bool canMove = true;

    // Variáveis de comer
    [Header("Eat Variables")]
    public bool canEat = false;
    [SerializeField] GameObject possibleEat;

    // Testes
    [Header("Tests")]
    [SerializeField] bool testing = true;
    [SerializeField] bool changeMove = false;

    // Controle de tempo de pressionar o botão
    [Header("Hold Time")]
    public float requiredHoldTime = 1f;
    [SerializeField] private bool buttonHeld = false;
    public float currentHoldTime = 0f;

    // Partículas
    [Header("Particles")]
    [SerializeField] private GameObject bubbles;

    private void Awake()
    {
        // Inicializa os componentes
        rb2d = GetComponent<Rigidbody2D>();
        //spr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        // Inicializa a saciedade
        saciety = maxSaciety;
    }

    private void Update()
    {

        if (canMove == false)
        {
            rb2d.velocity = new Vector3(0f, 0f, 0f);
        }

        // Rotaciona o jogador
        Rotate();

        // Obtém as entradas do jogador
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        // Atualiza a barra de saciedade na UI
        mySaciety.fillAmount = ((float)saciety / (float)maxSaciety);

        // Testes
        if (Input.GetButtonDown("Fire1") && testing == true)
        {
            LostSaciety(10);
        }

        if (canEat == true)
        {
            Eat();
        }
    }

    private void Rotate()
    {
        // Rotaciona o jogador conforme a escala
        float rotationZ = transform.rotation.eulerAngles.z;
        float scaleY = 1f;

        if (rotationZ >= 0f && rotationZ <= 90f)
        {
            scaleY = 1f;
        }
        else if (rotationZ > 90f && rotationZ <= 270f)
        {
            scaleY = -1f;
        }
        else if (rotationZ > 270f && rotationZ <= 360f)
        {
            scaleY = 1f;
        }

        transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);
    }

    private void Eat()
    {

        // Comer ao manter o botão pressionado
        if (Input.GetButton("Fire2") && testing == true)
        {
            if (!buttonHeld)
            {
                currentHoldTime = 0f;
                buttonHeld = true;
                if(myTimeEating!= null)
                {
                    myTimeEating.enabled = true;
                    myTimeEating.fillAmount = currentHoldTime;
                }
            }

            currentHoldTime += Time.deltaTime;
            if(myTimeEating != null)
                myTimeEating.fillAmount = currentHoldTime;


            if (currentHoldTime >= requiredHoldTime)
            {
                Destroy(possibleEat);
                RecoverSaciety(10);
                buttonHeld = false;
            }
        }
        else
        {
            if(myTimeEating!= null)
                myTimeEating.enabled = false;
            buttonHeld = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Verifica se o jogador está perto de comida
        if (collision.CompareTag("Food"))
        {
            canEat = true;
            var food = collision.GetComponent<Food>();
            food.Sort();
            possibleEat = collision.gameObject;
        }
    }

    void LostSaciety(int lost)
    {
        // Perde saciedade
        saciety -= lost;
        StartCoroutine(DecreaseSaciety());
    }

    void RecoverSaciety(int recover)
    {
        // Recupera saciedade
        int remainigRecover = maxSaciety - saciety;
        int actualRecover = Mathf.Min(remainigRecover, recover);

        StartCoroutine(IncreaseSaciety(actualRecover));
    }

    private void FixedUpdate()
    {
        // Movimentação do jogador
        if (changeMove == false && canMove == true)
            Move();
        if (testing == true && changeMove)
            MoveTest();
    }

    private void MoveTest()
    {
        // Movimentação de teste
        if (horizontal != 0)
        {
            rb2d.velocity = new Vector2(horizontal * swimSpeed, vertical * swimSpeed);
            horizontal *= moveLim;
            vertical *= moveLim;
        }

        rb2d.velocity = new Vector2(horizontal * swimSpeed, vertical * swimSpeed);
    }

    private void Move()
    {
        // Movimentação padrão
        transform.Rotate(Vector3.forward * -horizontal * rotSpeed * Time.deltaTime);
        Vector2 moveDir = new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad));
        rb2d.velocity = moveDir * vertical * swimSpeed * moveLim;

        // Ativa ou desativa as partículas de bolhas
        Vector2 velocity = rb2d.velocity;

        if (velocity.magnitude == 0)
        {
            bubbles.SetActive(true);
        }
        else
        {
            bubbles.SetActive(false);
        }
    }

    IEnumerator DecreaseSaciety()
    {
        // Reduz a saciedade gradualmente
        float targetFillAmount = (float)saciety / (float)maxSaciety;

        while (sacietyBG.fillAmount > targetFillAmount)
        {
            sacietyBG.fillAmount -= sacietyDecreaseSpeed * Time.deltaTime;
            yield return null;
        }

        sacietyBG.fillAmount = targetFillAmount;
    }

    IEnumerator IncreaseSaciety(int recoverAmount)
    {
        // Aumenta a saciedade gradualmente
        float targetFillAmount = ((float)saciety + recoverAmount) / ((float)maxSaciety);

        while (sacietyBG.fillAmount < targetFillAmount)
        {
            sacietyBG.fillAmount += sacietyRecoverSpeed * Time.deltaTime;
            yield return null;
        }

        sacietyBG.fillAmount = targetFillAmount;
        saciety += recoverAmount;
    }
}
