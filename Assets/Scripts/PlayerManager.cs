using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IDamageable
{
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider hungerSlider;

    public static PlayerManager instance;

    public CharacterController controller;

    public float speed = 5;
    public float gravity = -9.18f;
    public float jumpHeight = 3f;

    public float hungerSpeed = 0.1f;


    private float speedRef;
    private float health = 100f;
    private float hunger = 100f;

    public Transform handHolder;

    Vector3 velocity;
    bool isGrounded;

    public bool isDead = false;

    public void AddHealth(float health)
    {
        ChangeHunger(-health * 1.5f);
        this.health += health;
        this.health = Mathf.Clamp(this.health, 0, 100);
        healthSlider.value = this.health;
    }

    public void ChangeHunger(float value)
    {
        hunger -= value;
        hunger = Mathf.Clamp(hunger, 0, 100f);
        hungerSlider.value = hunger;
    }

    private bool isInBoost = false;

    public void ChangeSpeed(float spd, bool bost)
    {
        if(isInBoost)
        {
            return;
        }
        else
        {
            if (bost)
            {
                speedRef *= spd;
            }
            else
            {
                speedRef /= spd;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        DamageFX.instance.ToogleDamageFX(0.5f);
        CameraShake.instance.ShakeCamera();
        Debug.Log(damage);
        health -= damage;
        health = Mathf.Clamp(health, 0, 100);
        healthSlider.value = health;
        if (health <= 0)
        {
            isDead = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            GameManager.instance.ActivateDeathScreen();
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(!controller)
            controller = GetComponent<CharacterController>();
        healthSlider.value = health;
        speedRef = speed;
    }

    private float frequency = 5f;

    void Update()
    {
        if (hunger > 0)
        {
            ChangeHunger(Time.deltaTime * hungerSpeed);
        }
        else
        {
            if (frequency > 0)
            {
                frequency -= Time.deltaTime;
            }
            else
            {
                CameraShake.instance.ShakeCamera();
                DamageFX.instance.ToogleDamageFX(0.5f);
                frequency = 0;
            }
            AddHealth(-1 * ((hungerSpeed) * (Time.deltaTime * 2)));
        }
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKey("left shift") && isGrounded)
        {
            speed = speedRef * 1.5f;
        }
        else
        {
            speed = speedRef;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}