using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Componentes do jogador
    [Header("Components")]
    Rigidbody2D rb2d;
    SpriteRenderer spr;
    [SerializeField] CameraShake shake;

    // Entradas do jogador
    [Header("Inputs")]
    float horizontal;
    float vertical;

    [Header("Tests")]
    public bool testing = false;

    // UI
    [Header("UI")]
    [SerializeField] private Image mySaciety;
    [SerializeField] private Image sacietyBG;
    [SerializeField] private Image myLife;
    [SerializeField] private Image lifeBG;
    [SerializeField] private Image myTimeEating;
    [SerializeField] private int maxSaciety = 100;
    [SerializeField] private int maxLife = 100;
    [SerializeField] private float DecreaseSpeed = 0.5f;
    [SerializeField] private float RecoverSpeed = 1.0f;
    [SerializeField] GameObject SpaceClick;
    public bool tuto = false;

    // Variáveis locais
    [Header("Local")]
    float moveLim = .7f;
    float swimSpeed = 5f;
    float rotSpeed = 150f;
    int saciety;
    int life;
    int lostLife;
    public bool canMove = true;
    public float timeFlashing;
    Color normal = Color.white;
    Color flash = Color.red;
    

    // Variáveis de comer
    [Header("Eat Variables")]
    public bool canEat = false;
    public bool isMinigaming = false;
    public GameObject possibleEat;
    [SerializeField] float loseTime = 5f;
    [SerializeField] float gainTime = 2f;
    [SerializeField] GameObject emptyHook;


    // Controle de tempo de pressionar o botão
    [Header("Hold Time")]
    public float requiredHoldTime = 1f;
    [SerializeField] private bool buttonHeld = false;
    public float currentHoldTime = 0f;

    // Partículas
    [Header("Particles")]
    [SerializeField] GameObject bubbles;
    [SerializeField] bool canBubble = false;

    private void Awake()
    {
        // Inicializa os componentes
        rb2d = GetComponent<Rigidbody2D>();
        spr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        // Inicializa a saciedade
        saciety = maxSaciety;
        life = maxLife;

        mySaciety.fillAmount = ((float)saciety / (float)maxSaciety);
        myLife.fillAmount = ((float)life / (float)maxLife);
    }

    private void Update()
    {

        // Atualiza a barra de saciedade na UI
        mySaciety.fillAmount = ((float)saciety / (float)maxSaciety);
        myLife.fillAmount = ((float)life / (float)maxLife);

        UpdatingAttributes();

        if (canMove == false)
        {
            rb2d.velocity = new Vector3(0f, 0f, 0f);
        }

        // Rotaciona o jogador
        Rotate();

        // Obtém as entradas do jogador
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");


        if (canEat == true && isMinigaming == false)
        {
            Eat();
        }
    }
    private void FixedUpdate()
    {
        // Movimentação do jogador
        if (canMove == true)
            Move();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Verifica se o jogador está perto de comida
        if (collision.CompareTag("Food"))
        {
            canEat = true;
            var food = collision.GetComponent<Food>();
            possibleEat = collision.gameObject;

            if (currentHoldTime >= 1f)
                food.Sort();

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            canEat = false;
        }
    }

    #region Mechanics
    private void Rotate()
    {
        canBubble = false;
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
        if (Input.GetButton("Fire1"))
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
                EatMethod(true);
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

    public void EatMethod(bool eat)
    {
        if(eat == true)
        {
            Instantiate(emptyHook, possibleEat.transform.position, possibleEat.transform.rotation);
            Destroy(possibleEat);
            RecoverSaciety(10);
        }
        else
        {
            Instantiate(emptyHook, possibleEat.transform.position, possibleEat.transform.rotation);
            Destroy(possibleEat);
            LostLife(10);
        }
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
    #endregion

    #region Attributes

    private void UpdatingAttributes()
    {


        if (tuto == true)
        {
            SpaceClick.SetActive(true);
        }
        else
        {
            SpaceClick.SetActive(false);
        }


        //Perder saciedade ao longo do tempo
        if (saciety >= 0 && testing == false && isMinigaming == false)
        {
            loseTime -= Time.deltaTime;
            if (loseTime <= 0f)
            {
                LostSaciety(10);
                loseTime = 5f;
            }
        }
        else if(saciety <= 0 && isMinigaming== false)
        {
                loseTime -= Time.deltaTime;
                if (loseTime <= 0f)
                {
                    LostLife(10);
                    loseTime = 5f;
                }
        }

        //TESTANDO
        if(saciety >= maxSaciety)
        {
            gainTime -= Time.deltaTime;

            if(gainTime <= 0f && life < maxLife)
            {
                RecoverLife(10);
                gainTime = 3f;
            }
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

    public void LostLife(int lost)
    {
        if(life > 0)
        {
            life -= lost;
            StartCoroutine(DecreaseLife());
            StartCoroutine(FlashRed());
        }
    }
    void RecoverLife(int recover)
    {
        int remainingRecover = maxLife - life;
        int actualRecover = Mathf.Min(remainingRecover, recover);

        StartCoroutine(IncreaseLife(actualRecover));
    }
    #endregion

    #region UiStuffs
    IEnumerator DecreaseSaciety()
    {
        // Reduz a saciedade gradualmente
        float targetFillAmount = (float)saciety / (float)maxSaciety;

        while (sacietyBG.fillAmount > targetFillAmount)
        {
            sacietyBG.fillAmount -= DecreaseSpeed * Time.deltaTime;
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
            sacietyBG.fillAmount += RecoverSpeed * Time.deltaTime;
            yield return null;
        }

        sacietyBG.fillAmount = targetFillAmount;
        saciety += recoverAmount;
    }

    IEnumerator DecreaseLife()
    {
        // Reduz a vida gradualmente
        float targetFillAmount = (float)life / (float)maxLife;

        while (lifeBG.fillAmount > targetFillAmount)
        {
            lifeBG.fillAmount -= DecreaseSpeed * Time.deltaTime;
            yield return null;
        }

        lifeBG.fillAmount = targetFillAmount;
    }

    IEnumerator IncreaseLife(int recoverAmount)
    {
        float targetFillAmount = ((float)life + recoverAmount) / ((float)maxLife);

        while(lifeBG.fillAmount < targetFillAmount)
        {
            lifeBG.fillAmount += RecoverSpeed * Time.deltaTime;
            yield return null;
        }

        lifeBG.fillAmount = targetFillAmount;
        life += recoverAmount;
    }

    IEnumerator FlashRed()
    {
        spr.color = flash;
        shake.TriggerShake(timeFlashing * Time.deltaTime);
        yield return new WaitForSeconds(timeFlashing * Time.deltaTime);
        spr.color = normal;
    }
    #endregion
}
