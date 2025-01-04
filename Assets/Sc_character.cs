using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_character : MonoBehaviour
{
    private static Sc_character instance;
    public float fallMultiplier = 2.5f;
    private int delayMulai = 3;
    public bool statMulai = false;
    private Rigidbody rb;
    private Animator animator;
    public float forwardSpeed = 5f; // Kecepatan awal karakter
    public float maxSpeed = 10f; // Kecepatan maksimum
    public float laneDistance = 2.5f; // Jarak antar jalur
    private int lane = 1; // 0 = Kiri, 1 = Tengah, 2 = Kanan
    public float jumpDuration = 0.3f; // Durasi lompat
    public float slideDuration = 0.3f;
    public float jumpForce = 20f; // Meningkatkan jumpForce untuk mempercepat lompat
    public bool isGrounded = true;
    public LayerMask groundLayer;
    public Transform groundCheck;
    private bool isSliding = false;
    private bool isJumping = false;
    public bool isGameOver = false;
    // Particle system untuk efek tabrakan
    public ParticleSystem hitParticleEffect;

    bool toggle = false;

    void Start()
    {
        Invoke("mulai_main", delayMulai);
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Pastikan partikel dimatikan di awal
        if (hitParticleEffect != null)
        {
            hitParticleEffect = GetComponentInChildren<ParticleSystem>();
        }
    }

    private void mulai_main()
    {
        animator.Play("runStart");
        statMulai = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!statMulai || isGameOver) return;

        // Mengatur pergerakan ke kiri/kanan menggunakan A dan D
        if (Input.GetKeyDown(KeyCode.A) && lane > 0)
        {
            lane--; // Pindah ke jalur kiri
            animator.SetBool("Moving", true);
        }
        else if (Input.GetKeyDown(KeyCode.D) && lane < 2)
        {
            lane++; // Pindah ke jalur kanan
            animator.SetBool("Moving", true);
        }

        // Tentukan posisi target sesuai dengan jalur saat ini
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (lane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (lane == 2)
            targetPosition += Vector3.right * laneDistance;

        // Pindah karakter ke targetPosition secara bertahap
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * forwardSpeed);

        // Pergerakan ke depan (z-axis)
        if (toggle)
        {
            toggle = false;
            if (forwardSpeed < maxSpeed)
                forwardSpeed += 0.1f * Time.deltaTime;
        }
        else
        {
            toggle = true;
            if (Time.timeScale < 2f)
                Time.timeScale += 0.005f * Time.deltaTime;
        }

        // Menambah kecepatan karakter saat bergerak
        Vector3 move = transform.forward * forwardSpeed;

        if (rb.velocity.y < 0) // Jika karakter sedang jatuh
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // Input lompat menggunakan W, dengan reset animasi setelah lompat selesai
        if (Input.GetKeyDown(KeyCode.W) && !isJumping) // Mengubah Space ke W
        {
            StartCoroutine(Jump()); // Gunakan Coroutine untuk Jump
        }

        // Input sliding
        if (Input.GetKeyDown(KeyCode.S) && !isSliding)
        {
            StartCoroutine(Slide()); // Slide dijalankan meskipun lompat sedang berlangsung
        }
    }

    void MoveLane()
    {
        Vector3 targetPosition = transform.position;

        if (lane == 0)
            targetPosition = new Vector3(-laneDistance, transform.position.y, transform.position.z);
        else if (lane == 1)
            targetPosition = new Vector3(0, transform.position.y, transform.position.z);
        else if (lane == 2)
            targetPosition = new Vector3(laneDistance, transform.position.y, transform.position.z);

        // Perpindahan instan tanpa animasi untuk lane kiri/kanan
        transform.position = targetPosition;
    }

    IEnumerator Jump()
    {
        Debug.Log("jumping");
        isJumping = true;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Menambahkan kekuatan lompat
        animator.SetBool("Jumping", true); // Set boolean animasi lompat
        

        // Tunggu selama durasi lompat, lalu reset
        yield return new WaitForSeconds(jumpDuration);
        
        isJumping = false; // Reset isJumping setelah lompat selesai
        reset_lompat(); // Reset animasi lompat
        if (!isSliding) // Pastikan tidak sedang sliding
        {
            animator.SetBool("Running", true); // Kembali ke animasi lari
        }
    }

    IEnumerator Slide()
    {
        // Mengecek apakah karakter sedang lompat, jika iya, langsung mulai sliding
        isSliding = true;
        animator.SetBool("Sliding", true); // Set boolean animasi sliding
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        col.height /= 2; // Turunkan ukuran collider
        yield return new WaitForSeconds(slideDuration);
        col.height *= 2; // Kembalikan collider ke ukuran normal
        isSliding = false; // Reset isSliding setelah selesai
        animator.SetBool("Sliding", false); // Reset boolean sliding
        if (!isJumping) // Pastikan tidak sedang lompat
        {
            animator.SetBool("Running", true); // Kembali ke animasi lari
        }
    }

    public void reset_lompat()
    {
        animator.SetBool("Jumping", false); // Mengembalikan animasi lompat ke kondisi semula
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) // Pastikan obstacle memiliki tag "Obstacle"
        {
            animator.SetTrigger("Hit");
            Debug.Log("Obstacle Touched!");

            // Mainkan efek partikel tanpa membuat clone
            if (hitParticleEffect != null)
            {
                hitParticleEffect.transform.position = transform.position; // Posisikan partikel di karakter
                hitParticleEffect.Play();
            }

            // Matikan karakter
            statMulai = false;
        }
    }
}
