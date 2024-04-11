using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region Variables
    // Componentes do jogador
    [Header("Components")]
    Rigidbody2D rb2d;
    SpriteRenderer spr;
    Animator anim;
    [SerializeField] CameraShake shake;

    // Entradas do jogador
    [Header("Inputs")]
    float horizontal;
    float vertical;

    [Header("Tests")]
    public bool testing = false;

    // UI
    [Header("UI")]
    [SerializeField]  Image mySaciety;
    [SerializeField]  Image sacietyBG;
    [SerializeField]  Image myLife;
    [SerializeField]  Image lifeBG;
    [SerializeField]  Image myTimeEating;
    [SerializeField]  Text myPoints;
    [SerializeField]  Text deathScenePoints;
    [SerializeField]  Text myMaxPoints;
    [SerializeField]  Text ManyPoints;
    [SerializeField]  int maxSaciety = 100;
    [SerializeField]  int maxLife = 100;
    [SerializeField]  float DecreaseSpeed = 0.5f;
    [SerializeField]  float RecoverSpeed = 1.0f;
    [SerializeField] GameObject SpaceClick;
    public bool tuto = false;
    [SerializeField] GameObject YouDie;
    int point;
    int maxPoints;


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
    bool hasFoodArrived = false;
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
    #endregion

    private void Awake()
    {
        // Inicializa os componentes
        anim = GetComponentInChildren<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        spr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if(SoundController.Instance != null)
            SoundController.Instance.PlayMusic("Idle");

        // Inicializa a saciedade
        saciety = maxSaciety;
        life = maxLife;
        maxPoints = PlayerPrefs.GetInt("Max Pontuation", 0);

        mySaciety.fillAmount = ((float)saciety / (float)maxSaciety);
        myLife.fillAmount = ((float)life / (float)maxLife);
    }

    private void Update()
    {
        if(point > maxPoints)
        {
            maxPoints = point;
            PlayerPrefs.SetInt("Max Pontuation", maxPoints);
        }

        bubbles.SetActive(true);
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


        if (canEat == true && isMinigaming == false && hasFoodArrived == true)
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

    private void OnTriggerStay2D(Collider2D stay)
    {
        // Verifica se o jogador está perto de comida
        if (stay.CompareTag("Food"))
        {
            
            var food = stay.GetComponent<Food>();
            hasFoodArrived = food.isArrived();

            if (hasFoodArrived == true)
            {
                if (isMinigaming == false)
                {
                    canEat = true;
                    possibleEat = stay.gameObject;

                    if (currentHoldTime >= 1f)
                        food.Sort();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D exit)
    {
        if (exit.CompareTag("Food"))
        {
            canEat = false;
        }
    }

    #region Mechanics
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
        if (Input.GetButton("Fire1"))
        {
            if (!buttonHeld)
            {
                currentHoldTime = 0f;
                buttonHeld = true;

                if (myTimeEating!= null)
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
        myTimeEating.enabled = false;
        if(eat == true)
        {
            Instantiate(emptyHook, possibleEat.transform.position, possibleEat.transform.rotation);
            Destroy(possibleEat);
            RecoverSaciety(10);

            Points(20);
        }
        else
        {
            Instantiate(emptyHook, possibleEat.transform.position, possibleEat.transform.rotation);
            Destroy(possibleEat);
            LostLife(10);

            if(point > 0)
            {
                Points(-10);
            }
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
            anim.SetBool("_idle", true);
            anim.SetBool("_swimming", false);
        }
        else
        {
            anim.SetBool("_swimming", true);
            anim.SetBool("_idle", false);
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
        if(life >= 0)
        {
            life -= lost;
            anim.SetTrigger("_damage");
            StartCoroutine(DecreaseLife());
            StartCoroutine(FlashRed());
        }
        else if(life <= 0)
        {
            Die();
        }
    }
    void RecoverLife(int recover)
    {
        int remainingRecover = maxLife - life;
        int actualRecover = Mathf.Min(remainingRecover, recover);

        StartCoroutine(IncreaseLife(actualRecover));
    }

    void Die()
    {
        if(YouDie!= null)
        {
            YouDie.SetActive(true);
            deathScenePoints.text = point.ToString();
            myMaxPoints.text = maxPoints.ToString();
        }

        Destroy(gameObject);
    }

    void Points(int quantity)
    {
        point += quantity;
        StartCoroutine(AppearPoint(quantity));

        myPoints.text = point.ToString();

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
        shake.TriggerShake(.35f);
        yield return new WaitForSeconds(timeFlashing * Time.deltaTime);
        spr.color = normal;
    }

    IEnumerator AppearPoint(int quantity)
    {
        if (quantity < 0)
        {
            ManyPoints.enabled = true;

            ManyPoints.color = Color.red;
            ManyPoints.text = quantity.ToString();

            yield return new WaitForSeconds(.5f);
            ManyPoints.enabled = false;
        }
        if (quantity > 0)
        {
            ManyPoints.enabled = true;

            ManyPoints.color = Color.green;
            ManyPoints.text = quantity.ToString();

            yield return new WaitForSeconds(.5f);
            ManyPoints.enabled = false;
        }
    }
    #endregion
}
