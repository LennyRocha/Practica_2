using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float velocidadRotacion = 180f;

    private Rigidbody2D rb;

    public float fuerzaImpulso = 5f;

    public float velocidadMaxima = 8f;

    public GameObject balaPrefab;
    public Transform puntoDisparo; 

    public GameObject explosionPrefab;

    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;

    public ParticleSystem propulsor;

    public float tiempoEntreDisparos = 0.6f;

    private float siguienteDisparo = 0f;

    private bool gameOver = false;

    public AudioClip sonidoDisparo;
    public AudioClip sonidoMuerte;
    public AudioClip sonidoReinicio;
    public AudioClip sonidoChoque;

    private AudioSource audioSource;

    private bool invulnerable = false;

    public float tiempoInvulnerable = 3f;

    public float velocidadParpadeo = 0.15f;

      IEnumerator Reaparecer()
    {
        invulnerable = true;

        spriteRenderer.enabled = false;
        playerCollider.enabled = false;

        propulsor.Stop();

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        yield return new WaitForSeconds(1f);

        spriteRenderer.enabled = true;

        yield return StartCoroutine(Parpadear());

        playerCollider.enabled = true;

        invulnerable = false;
    }

    IEnumerator Parpadear()
    {
        float tiempo = 0f;

        while (tiempo < tiempoInvulnerable)
        {
            spriteRenderer.enabled =
                !spriteRenderer.enabled;

            yield return new WaitForSeconds(
                velocidadParpadeo
            );

            tiempo += velocidadParpadeo;
        }

        spriteRenderer.enabled = true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!gameOver &&
            Keyboard.current.spaceKey.isPressed &&
            Time.time >= siguienteDisparo)
        {
            Disparar();

            siguienteDisparo =
                Time.time + tiempoEntreDisparos;
        }

        if (gameOver){
                GameManager gameManager =
                FindFirstObjectByType<GameManager>();
                gameManager.GameOver();
                if(Keyboard.current.rKey.wasPressedThisFrame)
                    {
                        Time.timeScale = 1f;

                        SceneManager.LoadScene(
                            SceneManager.GetActiveScene().buildIndex
                        );
                }
        }
    }

    void FixedUpdate()
    {
        float rotacion = 0;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            rotacion = 1;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            rotacion = -1;
        }

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
        {
            rb.AddForce(transform.up * fuerzaImpulso);
            if (!propulsor.isPlaying)
            {
                propulsor.Play();
            }
        } else {
            if (propulsor.isPlaying)
            {
                propulsor.Stop();
            }
        }

        rb.MoveRotation(
            rb.rotation +
            rotacion *
            velocidadRotacion *
            Time.fixedDeltaTime
        );

         if (rb.linearVelocity.magnitude > velocidadMaxima)
            {
                rb.linearVelocity =
                    rb.linearVelocity.normalized * velocidadMaxima;
            }
    }

    void Disparar()
    {
        Instantiate(
            balaPrefab,
            puntoDisparo.position,
            puntoDisparo.rotation
        );
        audioSource.PlayOneShot(sonidoDisparo);
        Debug.Log("Bala");
    }

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (collision.gameObject.CompareTag("Asteroid") && !invulnerable)
        {
        GameManager gameManager = FindFirstObjectByType<GameManager>();

        audioSource.PlayOneShot(sonidoChoque);
        gameManager.PerderVida();

        Destroy(collision.gameObject);

        if (gameManager.vidas <= 0)
        {
            GameOver();
        }
        else
        {
            StartCoroutine(Reaparecer());
        }
        }
    }

    void ReiniciarJugador()
    {
        audioSource.PlayOneShot(sonidoReinicio);

        transform.position = Vector3.zero;

        rb.linearVelocity = Vector2.zero;

        rb.angularVelocity = 0f;
    }

    void GameOver(){
        audioSource.PlayOneShot(sonidoMuerte);

        Debug.Log("GAME OVER");

        gameOver = true;

        Time.timeScale = 0f;
    }
}