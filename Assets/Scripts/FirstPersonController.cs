using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
	public PlayerState state = PlayerState.Move;
	public float moveSpeed = 5f;
	public float sprintCoefficient = 1.5f;
	public float crouchCoefficient = 5f;
	
	public float mouseSensitivity = 2f;
	public float jumpForce = 2f;
	public float gravity = -19.62f;

	private CharacterController controller;
	private Camera playerCamera;
	private PlayerInput input;
	private float verticalRotation = 0f;
	private Vector3 playerVelocity;
	private bool isGrounded;

	void Start()
	{
		controller = GetComponent<CharacterController>();
		input = GetComponent<PlayerInput>();
		playerCamera = GetComponentInChildren<Camera>();
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		isGrounded = controller.isGrounded;

		SetPlayerState();
		MouseHandle();
		VerticalVelocityHandle();
		var playerMove = MoveHandle();

		controller.Move((playerMove + playerVelocity) * Time.deltaTime);
	}

	private void SetPlayerState()
	{
		// if (!isGrounded)
		// {
		// 	state = PlayerState.InAir;
		// 	return;
		// }
		//
		// if (Input.GetButton("Crouch"))
		// {
		//     state = PlayerState.Crouch;
		//     return;
		// }
		//
		// if (Input.GetButton("Sprint"))
		// {
		//     state = PlayerState.Sprint;
		//     return;
		// }
		
		state = PlayerState.Move;
	}

	private void VerticalVelocityHandle()
	{
		if (isGrounded && playerVelocity.y < 0)
		{
			playerVelocity.y = -2f;
		}

		if (isGrounded && Input.GetButtonDown("Jump"))
		{
			playerVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
		}

		playerVelocity.y += gravity * Time.deltaTime;
	}

	private Vector3 MoveHandle()
	{
		var speed = state switch
		{
			PlayerState.Move => moveSpeed,
			PlayerState.Sprint => moveSpeed * sprintCoefficient,
			PlayerState.Crouch => moveSpeed * crouchCoefficient,
			_ => 0,
		};
		var moveX = Input.GetAxis("Horizontal") * speed;
		var moveZ = Input.GetAxis("Vertical") * speed;

		var move = (transform.right * moveX + transform.forward * moveZ);
		return move;
	}

	private void MouseHandle()
	{
		var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
		var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

		verticalRotation -= mouseY;
		verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

		playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
		transform.Rotate(Vector3.up * mouseX);
	}
}