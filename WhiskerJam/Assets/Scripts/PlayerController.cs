using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BoxCollider2D playerCollider;
    private Rigidbody2D playerRb;
    [SerializeField] private float jumpPower;
    private bool isGrounded;
    [SerializeField] private float jumpChargeInitial = 0.5f;
    [SerializeField] private float jumpCharge;
    [SerializeField] private float jumpChargeScale = 1.5f;
    private float jumpCap = 2.5f;
    private Vector2 jumpDirectionVector;

    private bool isAimingLeft;
    private bool isAimingRight;
    private bool isNotAiming;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = gameObject.GetComponent<BoxCollider2D>();
        playerRb = gameObject.GetComponent<Rigidbody2D>();

        jumpCharge = jumpChargeInitial;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            isAimingLeft = true;
            isNotAiming = false;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            isAimingLeft = false;
            isNotAiming = true;
            //way too dank
        }
        else if (Input.GetKey(KeyCode.D))
        {
            
        }

        if (isGrounded && Input.GetKey(KeyCode.Space))
        {
            if (jumpCharge < jumpCap)
            {
                jumpCharge += Time.deltaTime * jumpChargeScale;
            }
            else
            {
                jumpCharge = jumpCap;
            }
        }

        if (isGrounded && Input.GetKeyUp(KeyCode.Space))
        {
            playerRb.AddForce(jumpDirectionVector * jumpPower * jumpCharge, ForceMode2D.Impulse);
            jumpCharge = jumpChargeInitial;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("grounded");
        } 
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("not grounded");
        }
    }

}
