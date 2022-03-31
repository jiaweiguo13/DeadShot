using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public CharacterController charCon;

    private Vector3 moveInput;

    public Transform camTrans;

    public Animator anim;


    [Header("Gravity")]
    public float gravityModifier;

    [Header("Movement Controls")]
    public float moveSpeed;
    public float runSpeed;
    public float jumpPower;
    private bool canJump, canDoubleJump;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;

    [Header("Camera Controls")]
    public float mouseSensitivity;
    public bool invertX;
    public bool invertY;

    public Transform firePoint;

    public Gun activeGun;
    public List<Gun> allGuns = new List<Gun>();
    public List<Gun> unlockableGuns = new List<Gun>();
    public int currentGun;

    public Transform adsPoint, gunHolder;
    private Vector3 gunStartPos;
    public float adsSpeed = 2f;

    public GameObject muzzleFlash;

    public AudioSource footstepFast, footstepSlow;

    private float bounceAmount;
    private bool bounce;

    public float maxViewAngle = 60f;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentGun--;
        SwitchGun();

        gunStartPos = gunHolder.localPosition;
    }

    void Update()
    {
        if (!UIController.instance.pauseScreen.activeInHierarchy )
        {
            //moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            //moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

            //guardar Y velocity
            float yStore = moveInput.y;

            Vector3 vertMove = transform.forward * Input.GetAxis("Vertical");
            Vector3 horiMove = transform.right * Input.GetAxis("Horizontal");

            moveInput = horiMove + vertMove;
            moveInput.Normalize();

            if (Input.GetButton("Run"))
            {
                moveInput = moveInput * runSpeed;
            }
            else
            {
                moveInput = moveInput * moveSpeed;
            }


            moveInput.y = yStore;

            // Gravedad
            moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

            if (charCon.isGrounded)
            {
                moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
            }

            //canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatIsGround).Length > 0;

            canJump = charCon.isGrounded;

            if (canJump)
            {
                canDoubleJump = false;
            }
            // Salto del Jugador
            if (Input.GetButtonDown("Jump") && canJump)
            {
                moveInput.y = jumpPower;

                canDoubleJump = true;

               

            }
            else if (canDoubleJump && (Input.GetButtonDown("Jump")))
            {
                moveInput.y = jumpPower;

                canDoubleJump = false;

                
            }

            if (bounce)
            {
                bounce = false;
                moveInput.y = bounceAmount;

                canDoubleJump = true;
            }

            charCon.Move(moveInput * Time.deltaTime);
            // Control Rotacion Camara
            Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

            if (invertX)
            {
                mouseInput.x = -mouseInput.x;
            }
            if (invertY)
            {
                mouseInput.y = -mouseInput.y;
            }

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

            camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));

            if (camTrans.rotation.eulerAngles.x > maxViewAngle && camTrans.rotation.eulerAngles.x < 180f)
            {
                camTrans.rotation = Quaternion.Euler(maxViewAngle, camTrans.rotation.eulerAngles.y, camTrans.rotation.eulerAngles.z);
            }
            else if (camTrans.rotation.eulerAngles.x > 180f && camTrans.rotation.eulerAngles.x < 360f - maxViewAngle)
            {
                camTrans.rotation = Quaternion.Euler(-maxViewAngle, camTrans.rotation.eulerAngles.y, camTrans.rotation.eulerAngles.z);
            }


            muzzleFlash.SetActive(false);

            // Shooting
            // Disparo Singular
            if (Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f))
                {
                    if (Vector3.Distance(camTrans.position, hit.point) > 2f)
                    {
                        firePoint.LookAt(hit.point);
                    }
                }
                else
                {
                    firePoint.LookAt(camTrans.position + (camTrans.forward * 30f));
                }

                FireShot();
            }

            // Disparo Automatico
            if (Input.GetMouseButton(0) && activeGun.canAutoFire)
            {
                if (activeGun.fireCounter <= 0)
                {
                    FireShot();
                }
            }

            if (Input.GetButtonDown("Switch Gun"))
            {
                SwitchGun();
            }

            if (Input.GetMouseButtonDown(1))
            {
                CameraController.instance.ZoomIn(activeGun.zoomAmount);
            }

            if (Input.GetMouseButton(1))
            {
                gunHolder.position = Vector3.MoveTowards(gunHolder.position, adsPoint.position, adsSpeed * Time.deltaTime);
            }
            else
            {
                gunHolder.localPosition = Vector3.MoveTowards(gunHolder.localPosition, gunStartPos, adsSpeed * Time.deltaTime);
            }

            if (Input.GetMouseButtonUp(1))
            {
                CameraController.instance.ZoomOut();
            }

            anim.SetFloat("moveSpeed", moveInput.magnitude);
            anim.SetBool("onGround", canJump);
        }
    }

    public void FireShot()
    {
        if (activeGun.currentAmmo > 0)
        {
            activeGun.currentAmmo--;

            Instantiate(activeGun.bullet, firePoint.position, firePoint.rotation);

            activeGun.fireCounter = activeGun.fireRate;

            UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;

            muzzleFlash.SetActive(true);
        }

    }

    public void SwitchGun()
    {
        activeGun.gameObject.SetActive(false);

        currentGun++;

        if (currentGun >= allGuns.Count)
        {
            currentGun = 0;
        }

        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);

        UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;

        firePoint.position = activeGun.firePoint.position;
    }

    public void AddGun(string gunToAdd)
    {
        bool gunUnlocked = false;

        if (unlockableGuns.Count > 0)
        {
            for (int i = 0; i < unlockableGuns.Count; i++)
            {
                gunUnlocked = true;

                allGuns.Add(unlockableGuns[i]);

                unlockableGuns.RemoveAt(i);

                i = unlockableGuns.Count;
            }
        }

        if (gunUnlocked)
        {
            currentGun = allGuns.Count - 2;
            SwitchGun();
        }
    }

    public void Bounce(float bounceForce)
    {
        bounceAmount = bounceForce;
        bounce = true;
    }
}


