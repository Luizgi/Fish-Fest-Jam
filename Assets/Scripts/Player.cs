using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D rb2d;

    [Header("Inputs")]
    float horizontal;
    float vertical;

    [Header("UI")]
    [SerializeField] private Image mySaciety;
    [SerializeField] private Image sacietyBG;
    [SerializeField] private int maxSaciety = 100;
    [SerializeField] private float sacietyDecreaseSpeed = 0.5f;
    [SerializeField] private float sacietyRecoverSpeed = 1.0f;

    [Header("Local")]
    float moveLim = .7f;
    float swimSpeed = 5f;
    float rotSpeed = 150f;
    int saciety;
    int lostLife;

    [Header("Eat Variables")]
    [SerializeField] bool canEat = false;
    [SerializeField] GameObject possibleEat;


    [Header("Tests")]
    [SerializeField] bool testing = true;
    [SerializeField] bool changeMove = false;

    [Header("Hold Time")]
    public float requiredHoldTime = 2f;
    [SerializeField] private bool buttonHeld = false;
    [SerializeField] private float currentHoldTime = 0f;

    [Header("Particles")]
    [SerializeField] private GameObject bubbles;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        saciety = maxSaciety;
    }
    private void Update()
    {
        //



        //Setting Inputs
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        //Transforming saciety to fill
        mySaciety.fillAmount = ((float)saciety / (float)maxSaciety);

        //Testing:
        if (Input.GetButtonDown("Fire1") && testing == true)
        {
            LostSaciety(10);
        }

        if(canEat == true)
        {
            Eat();
        }
    }

    private void Eat()
    {
        if (Input.GetButton("Fire2") && testing == true)
        {
            if (!buttonHeld)
            {
                currentHoldTime = 0f;
                buttonHeld = true;
            }
            currentHoldTime += Time.deltaTime;

            if (currentHoldTime >= requiredHoldTime)
            {
                Destroy(possibleEat);
                RecoverSaciety(10);
                buttonHeld = false;
            }
        }
        else
        {
            buttonHeld = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            canEat = true;
            possibleEat = collision.gameObject;
        }
    }

    void LostSaciety(int lost)
    {
        saciety -= lost;
        StartCoroutine(DecreaseSaciety());
    }

    void RecoverSaciety(int recover)
    {
        int remainigRecover = maxSaciety - saciety;
        int actualRecover = Mathf.Min(remainigRecover, recover);

        StartCoroutine(IncreaseSaciety(actualRecover));
    }


    private void FixedUpdate()
    {
        if(changeMove == false)
            Move();
        if(testing == true && changeMove)
            MoveTest();
    }

    private void MoveTest()
    {
        //First Movimentation
        if(horizontal != 0)
        {
            rb2d.velocity = new Vector2(horizontal * swimSpeed, vertical * swimSpeed);
            horizontal *= moveLim;
            vertical *= moveLim;
        }

        rb2d.velocity = new Vector2(horizontal * swimSpeed, vertical * swimSpeed);
    }

    private void Move()
    {
        //Second Movimentation
        transform.Rotate(Vector3.forward * -horizontal * rotSpeed * Time.deltaTime);
        Vector2 moveDir = new Vector2 (Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad));
        rb2d.velocity = moveDir * vertical * swimSpeed * moveLim;


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
        float targetFillAmount = ((float)saciety + recoverAmount) / ((float)maxSaciety);
        
        while(sacietyBG.fillAmount < targetFillAmount)
        {
            sacietyBG.fillAmount += sacietyRecoverSpeed * Time.deltaTime;
            yield return null;
        }

        sacietyBG.fillAmount = targetFillAmount;
        saciety += recoverAmount;
    }
}
