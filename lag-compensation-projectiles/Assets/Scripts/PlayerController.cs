using FishNet.Object;
using UnityEngine;

//This is made by Bobsi Unity - Youtube
namespace VersausSurvival
{
	public class PlayerController : NetworkBehaviour
	{
		[Header("Base setup")]
		public float Speed = 7.5f;
		public float JumpSpeed = 8.0f;
		public float Gravity = 20.0f;
		public float LookSpeed = 2.0f;
		public float LookXLimit = 45.0f;

		CharacterController _characterController;
		Vector3 _moveDirection = Vector3.zero;
		float _rotationX;

		[HideInInspector]
		public bool CanMove = true;

		[SerializeField] float _cameraYOffset = 0.4f;
		Camera _playerCamera;

		public override void OnStartClient()
		{
			base.OnStartClient();
			if (IsOwner)
			{
				_playerCamera = Camera.main;
				_playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + _cameraYOffset, transform.position.z);
				_playerCamera.transform.SetParent(transform);
			}
			else
			{
				Destroy(this);
			}
		}

		void Start()
		{
			_characterController = GetComponent<CharacterController>();

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		void Update()
		{
			var forward = transform.TransformDirection(Vector3.forward);
			var right = transform.TransformDirection(Vector3.right);

			var curSpeedX = CanMove ? Speed * Input.GetAxis("Vertical") : 0;
			var curSpeedY = CanMove ? Speed * Input.GetAxis("Horizontal") : 0;
			var movementDirectionY = _moveDirection.y;
			_moveDirection = (forward * curSpeedX) + (right * curSpeedY);

			if (Input.GetButton("Jump") && CanMove && _characterController.isGrounded)
			{
				_moveDirection.y = JumpSpeed;
			}
			else
			{
				_moveDirection.y = movementDirectionY;
			}

			if (!_characterController.isGrounded)
			{
				_moveDirection.y -= Gravity * Time.deltaTime;
			}

			_characterController.Move(_moveDirection * Time.deltaTime);

			if (CanMove && _playerCamera != null)
			{
				_rotationX += -Input.GetAxis("Mouse Y") * LookSpeed;
				_rotationX = Mathf.Clamp(_rotationX, -LookXLimit, LookXLimit);
				_playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
				transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * LookSpeed, 0);
			}
		}
	}
}
