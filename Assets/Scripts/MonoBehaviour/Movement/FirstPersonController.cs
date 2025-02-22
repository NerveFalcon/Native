using System;
using Native.MonoBehaviour.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputHandler))]
public class FirstPersonController : MonoBehaviour
{
	public PlayerState state = PlayerState.Move;
	public float moveSpeed = 0.3f;
	public float sprintCoefficient = 1.5f;
	public float crouchCoefficient = 0.5f;
	
	public LayerMask GroundLayer = 1; 
	public float jumpForce = 2f;

	public float mouseSensitivity = 2f;

	private Camera playerCamera;
	private InputHandler input;
	private Rigidbody rigidBody;
	private CapsuleCollider collider;

	private float verticalRotation = 0f;
	private Vector3 playerVelocity;
	private bool isGrounded;

	void Start()
	{
		input = GetComponent<InputHandler>();
		playerCamera = GetComponentInChildren<Camera>();

		Cursor.lockState = CursorLockMode.Locked;
		
		collider = GetComponentInChildren<CapsuleCollider>();
		rigidBody = GetComponent<Rigidbody>();
		
		//  Защита от дурака
		if (GroundLayer == gameObject.layer)
			Debug.LogError("Player SortingLayer must be different from Ground SourtingLayer!");
	}

	void Update()
	{
		SetPlayerState();
		CameraRotation();
	}

	private void FixedUpdate()
	{
		GravityHandle();
		JumpHandle();
		MoveHandle();
	}

	private void GravityHandle()
	{
		var bottomCenterPoint = new Vector3(collider.bounds.center.x, collider.bounds.min.y, collider.bounds.center.z);

		//создаем невидимую физическую капсулу и проверяем не пересекает ли она обьект который относится к полу

		//collider.bounds.size.x / 2 * 0.9f -- эта странная конструкция берет радиус обьекта.
		// был бы обязательно сферой -- брался бы радиус напрямую, а так пишем по-универсальнее

		isGrounded = Physics.CheckCapsule(collider.bounds.center, bottomCenterPoint, collider.bounds.size.x / 2 * 0.9f, GroundLayer);
		// если можно будет прыгать в воздухе, то нужно будет изменить коэффициент 0.9 на меньший.
	}

	private void SetPlayerState()
	{
		if (!isGrounded)
		{
			state = PlayerState.InAir;
			return;
		}
		
		if (input.sprint)
		{
			state = PlayerState.Sprint;
			return;
		}
		
		if (input.crouch)
		{
		    state = PlayerState.Crouch;
		    return;
		}
		
		state = PlayerState.Move;
	}

	private void JumpHandle()
	{
		if (state != PlayerState.InAir && input.jump)
		{
			rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
	}

	private void MoveHandle()
	{
		var speed = state switch
		{
			PlayerState.Move => moveSpeed,
			PlayerState.Sprint => moveSpeed * sprintCoefficient,
			PlayerState.Crouch => moveSpeed * crouchCoefficient,
			_ => 0,
		};
		var moveX = input.move.x * speed;
		var moveZ = input.move.y * speed;

		var move = (transform.right * moveX + transform.forward * moveZ);
		rigidBody.AddForce(move, ForceMode.Force);
	}

	private void CameraRotation()
	{
		var mouseX = input.look.x * mouseSensitivity;
		var mouseY = input.look.y * mouseSensitivity;

		verticalRotation -= mouseY;
		verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

		playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
		transform.Rotate(Vector3.up * mouseX);
	}
}