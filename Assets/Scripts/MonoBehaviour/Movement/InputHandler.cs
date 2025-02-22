using UnityEngine;
using UnityEngine.InputSystem;

namespace Native.MonoBehaviour.Movement
{
	public class InputHandler : UnityEngine.MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool crouch;

		private bool ignoreInput;
		private static bool focusActionsSetUp;

		private void Start()
		{
			if (focusActionsSetUp) return;
#if UNITY_EDITOR
			var ignoreInputAction = new InputAction(binding: "/Keyboard/escape");
			ignoreInputAction.performed += _ => ignoreInput = true;
			ignoreInputAction.Enable();

			var enableInputAction = new InputAction(binding: "/Mouse/leftButton");
			enableInputAction.performed += _ => ignoreInput = false;
			enableInputAction.Enable();
#endif
				
			var touchFocus = new InputAction(binding: "<pointer>/press");
			touchFocus.Enable();
				
			focusActionsSetUp = true;
		}

		public void OnMove(InputValue value) 
			=> move = ignoreInput ? Vector2.zero : value.Get<Vector2>();

		public void OnLook(InputValue value)
			=> look = ignoreInput ? Vector2.zero : value.Get<Vector2>();

		public void OnJump(InputValue value)
			=> jump = value.isPressed;

		public void OnSprint(InputValue value)
			=> sprint = value.isPressed;

		public void OnCrouch(InputValue value)
			=> crouch = value.isPressed;
	}
}