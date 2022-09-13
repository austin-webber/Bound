using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRb;
    [SerializeField] private float jumpPower;
    [SerializeField] private float secondJumpPower;
    [SerializeField] private float hangingJumpPower = 5f;
    private bool grounded;
    [SerializeField] private float jumpChargeInitial = 0.5f;
    [SerializeField] private float jumpCharge;
    [SerializeField] private float jumpChargeScale = 1.5f;
    private float jumpCap = 2.25f;
    private Vector2 jumpDirVector;
    private Vector2 secondJumpDirVector;
    private Vector2 hangingJumpDirVector;
    [SerializeField] private float leftRightJumpAngle = 1f;
    [SerializeField] private float secondLeftRightJumpAngle = 1f;
    [SerializeField] private float hangingJumpAngle = 0.5f;
    [SerializeField] private float hangingDrag = 5f;
    private int jumpCount = 0;

    private bool aimingLeft;
    private bool aimingRight;
    private bool notAiming;
    private bool hanging;
    private bool clawsOut;

    [SerializeField] private GameObject jump2;
    [SerializeField] private GameObject jump1;

    [SerializeField] private GameObject staminaBarSlider;
    [SerializeField] private float staminaLossRate = 0.1f;
    [SerializeField] private float staminaRechargeRate = 1f;

    [SerializeField] private Animator animator;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip jumpIconSound;
    [SerializeField] private AudioClip wallGrabSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip finishSound;
    private bool landSoundPlayed = false;
    private bool grabSoundPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = gameObject.GetComponent<Rigidbody2D>();

        jumpCharge = jumpChargeInitial;
        notAiming = true;
        hanging = false;
        clawsOut = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerRb.IsSleeping())
        {
            playerRb.WakeUp();
        }

        if (!hanging)
        {
            playerRb.drag = 0.05f;
        }

        if (playerRb.velocity.y < 0)
        {
            animator.SetBool("isJumpingUp", false);
            animator.SetBool("isFalling", true);
        }
        else if (playerRb.velocity.y > 0.001)
        {
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumpingUp", true);
        }
        else
        {
            if (!hanging & !clawsOut)
            {
                animator.SetBool("grounded", true);
                animator.SetBool("isJumpingUp", false);
                animator.SetBool("isFalling", false);
            }
        }

        CheckForInputs();

        if (aimingLeft && grounded && transform.localScale.x > 0)
        {
            gameObject.transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
        else if (aimingRight && grounded && transform.localScale.x < 0)
        {
            gameObject.transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

        CheckForJump();

        CheckForDoubleJump();

        CheckForClaws();

        CheckForHanging();

        UpdateJumps();
    }

    IEnumerator ResetJumpCharge(GameObject gameObject)
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (jumpCount > 0 && collision.gameObject.CompareTag("JumpCharge"))
        {
            audioSource.PlayOneShot(jumpIconSound);
            jumpCount -= 1;
            collision.gameObject.SetActive(false);
            StartCoroutine(ResetJumpCharge(collision.gameObject));
        }    

        if (collision.gameObject.CompareTag("Finish"))
        {
            audioSource.PlayOneShot(finishSound);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (clawsOut && collision.transform.CompareTag("Wall"))
        {
            if (!grabSoundPlayed)
            {
                audioSource.PlayOneShot(wallGrabSound);
                grabSoundPlayed = true;
            }
            if (staminaBarSlider.GetComponent<Image>().fillAmount >= 0)
            {
                hanging = true;
                animator.SetBool("hanging", true);
            }
        }

        if (collision.transform.CompareTag("Ground"))
        {
            if (playerRb.velocity.y == 0)
            {
                if (!landSoundPlayed)
                {
                    audioSource.PlayOneShot(landSound);
                    landSoundPlayed = true;
                }
                grounded = true;
                animator.SetBool("grounded", true);
                staminaBarSlider.GetComponent<Image>().fillAmount += staminaRechargeRate * Time.deltaTime;
                jumpCount = 0;
            }
        } 
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
        animator.SetBool("grounded", false);
        landSoundPlayed = false;
        hanging = false;
        animator.SetBool("hanging", false);
        grabSoundPlayed = false;
    }

    private void CheckForInputs()
    {
        if (Input.GetKey(KeyCode.A))
        {
            aimingLeft = true;
            notAiming = false;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            aimingRight = true;
            notAiming = false;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            aimingLeft = false;
            notAiming = true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            aimingRight = false;
            notAiming = true;
        }
    }

    private void CheckForJump()
    {
        if (grounded && Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("jumpCharging", true);

            if (jumpCharge < jumpCap)
            {
                jumpCharge += Time.deltaTime * jumpChargeScale;
            }
            else
            {
                jumpCharge = jumpCap;
            }
        }

        if (grounded && Input.GetKeyUp(KeyCode.Space))
        {
            audioSource.PlayOneShot(jumpSound);
            animator.SetBool("jumpCharging", false);
            animator.SetBool("isJumpingUp", true);
            jumpCount += 1;

            if (aimingLeft)
            {
                jumpDirVector = new Vector2(-leftRightJumpAngle, 1f);
            }
            else if (aimingRight)
            {
                jumpDirVector = new Vector2(leftRightJumpAngle, 1f);
            }
            else if (notAiming)
            {
                jumpDirVector = new Vector2(0f, 1f);
            }

            playerRb.AddForce(jumpDirVector * jumpPower * jumpCharge, ForceMode2D.Impulse);
            jumpCharge = jumpChargeInitial;
        }
    }

    private void CheckForDoubleJump()
    {
        if (!grounded && !hanging && Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            audioSource.PlayOneShot(jumpSound);
            playerRb.velocity = new Vector2(0f, 0f);
            playerRb.angularVelocity = 0f;
            jumpCount += 1;

            if (aimingLeft)
            {
                if (transform.localScale.x > 0)
                {
                    gameObject.transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                }

                secondJumpDirVector = new Vector2(-secondLeftRightJumpAngle, 1);
                playerRb.AddForce(secondJumpDirVector * secondJumpPower, ForceMode2D.Impulse);
            }
            else if (aimingRight)
            {
                if (transform.localScale.x < 0)
                {
                    gameObject.transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                }

                secondJumpDirVector = new Vector2(secondLeftRightJumpAngle, 1);
                playerRb.AddForce(secondJumpDirVector * secondJumpPower, ForceMode2D.Impulse);
            }
            else if (notAiming)
            {
                secondJumpDirVector = new Vector2(0f, 1);
                playerRb.AddForce(secondJumpDirVector * secondJumpPower, ForceMode2D.Impulse);
            }
        }
    }

    private void CheckForClaws()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            clawsOut = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            clawsOut = false;
            hanging = false;
            animator.SetBool("hanging", false);
        }
    }

    private void CheckForHanging()
    {
        if (hanging)
        {
            // stick to wall
            playerRb.drag = hangingDrag;
            staminaBarSlider.GetComponent<Image>().fillAmount -= staminaLossRate * Time.deltaTime;

            if (staminaBarSlider.GetComponent<Image>().fillAmount <= 0)
            {
                hanging = false;
                animator.SetBool("hanging", false);
            }
        }

        if (hanging && Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.PlayOneShot(jumpSound);
            playerRb.drag = 0.05f;
            hanging = false;
            animator.SetBool("hanging", false);
            // if you wall jump right when your stamina runs out, this line below would for some reason send you flying
            // staminaBarSlider.GetComponent<Image>().fillAmount -= 0.05f;

            if (aimingLeft)
            {
                if (transform.localScale.x > 0)
                {
                    gameObject.transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                }

                hangingJumpDirVector = new Vector2(-hangingJumpAngle, 1f);
                playerRb.AddForce(hangingJumpDirVector * hangingJumpPower, ForceMode2D.Impulse);
            }
            else if (aimingRight)
            {
                if (transform.localScale.x < 0)
                {
                    gameObject.transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                }

                hangingJumpDirVector = new Vector2(hangingJumpAngle, 1f);
                playerRb.AddForce(hangingJumpDirVector * hangingJumpPower, ForceMode2D.Impulse);
            }
        }
    }

    private void UpdateJumps()
    {
        if (jumpCount == 0)
        {
            jump2.SetActive(true);
            jump1.SetActive(true);
        }
        else if (jumpCount == 1)
        {
            jump2.SetActive(false);
            jump1.SetActive(true);
        }
        else if (jumpCount == 2)
        {
            jump1.SetActive(false);
        }
    }
}
