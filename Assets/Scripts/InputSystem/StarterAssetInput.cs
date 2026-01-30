using UnityEngine;
using UI_InputSystem.Base;
using UnityEngine.UI;

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public float lookSpeed = 20f;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[Header("Mobile Settings")]
		public Button JumpBtn;
		public Button SprintBtn;
		public Image SprintingImage;

		private float sprintTime = 0f;

        private void Start()
        {
			GameSettingDataSingleton.Instance.LoadData();
			lookSpeed = GameSettingDataSingleton.Instance.playerLookSpeed;

			JumpBtn.onClick.AddListener(() => {
				if (!jump)
				{
					JumpInput(true);
				}
			});

			SprintBtn.onClick.AddListener(() =>
			{
				SprintingImage.enabled = !sprint;
				SprintInput(!sprint);

				//if (sprint)
				//{
				//	if (sprintTime >= 0 && sprintTime < 3f)
				//	{
				//		SprintInput(true);
				//	}
				//	else
				//	{
				//		SprintInput(false);
				//		sprintTime = -3f;
				//	}
				//}
				//else
				//{
				//	if (sprintTime >= 0)
				//	{
				//		SprintInput(true);
				//	}
				//}
			});

		}


        public void OnMove(Vector2 value)
		{
			MoveInput(value);
		}

		public void OnLook(Vector2 value)
		{
			if(cursorInputForLook)
			{
				LookInput(value);
			}
		}

		public void OnJump(bool value)
		{
			JumpInput(value);
		}

		public void OnSprint(bool value)
		{
			SprintInput(value);
		}



		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			//SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

        private void Update()
        {
//#if UNITY_ANDROID || UNITY_IOS || UNITY_OPENHARMONY
//			CheckMobileInput();
//#else
//			//CheckKeyPadInput();
//			CheckInput();
//#endif
            if (GameSettingDataSingleton.Instance.isPCMode)
            {
                CheckInput();
            }
            else
            {
                //CheckKeyPadInput();
                CheckMobileInput();
            }


        }

		void CheckInput()
		{
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");
			MoveInput(new Vector2(h, v));

			//float x = Input.GetAxis("Mouse X");
			//float y = Input.GetAxis("Mouse Y");
			//LookInput(new Vector2(x == 0 ? 0 : (x > 0 ? 1 : -1), y == 0 ? 0 : (y > 0 ? 1 : -1)) * lookSpeed);

			float x = UIInputSystem.ME.GetAxisHorizontal(JoyStickAction.CameraLook);
			float y = UIInputSystem.ME.GetAxisVertical(JoyStickAction.CameraLook);
			LookInput(new Vector2(x, -y) * GameSettingDataSingleton.Instance.playerLookSpeed);

			if (Input.GetKeyDown(KeyCode.Space))
			{
				if (!jump)
				{
					JumpInput(true);
				}
			}

			if (Input.GetButtonUp("Sprint"))
			{
				SprintingImage.enabled = !sprint;
				SprintInput(!sprint);
			}

			//if(Input.GetButtonDown("Sprint"))
			//         {
			//             if (sprint)
			//             {
			//		if (sprintTime >= 0 && sprintTime < 3f)
			//		{
			//			SprintInput(true);
			//		}
			//		else
			//                 {
			//			SprintInput(false);
			//			sprintTime = -3f;
			//		}
			//	}
			//	else
			//             {
			//		if(sprintTime >= 0)
			//                 {
			//			SprintInput(true);
			//		}
			//	}

			//         }

			sprintTime += Time.deltaTime;

		}

		void CheckKeyPadInput()
        {
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");
			MoveInput(new Vector2(h, v));

			float x = Input.GetAxis("Mouse X");
			float y = Input.GetAxis("Mouse Y");
			LookInput(new Vector2(x == 0 ? 0 : (x > 0 ? 1 : -1), y == 0 ? 0 : (y > 0 ? 1 : -1)) * GameSettingDataSingleton.Instance.playerLookSpeed);

			if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!jump)
                {
					JumpInput(true);
				}
            }

			if(Input.GetButtonUp("Sprint"))
            {
				SprintingImage.enabled = !sprint;
				SprintInput(!sprint);
            }

			//if(Input.GetButtonDown("Sprint"))
   //         {
   //             if (sprint)
   //             {
			//		if (sprintTime >= 0 && sprintTime < 3f)
			//		{
			//			SprintInput(true);
			//		}
			//		else
   //                 {
			//			SprintInput(false);
			//			sprintTime = -3f;
			//		}
			//	}
			//	else
   //             {
			//		if(sprintTime >= 0)
   //                 {
			//			SprintInput(true);
			//		}
			//	}

   //         }

			sprintTime += Time.deltaTime;

		}

		void CheckMobileInput()
        {
			float h = UIInputSystem.ME.GetAxisHorizontal(JoyStickAction.Movement);
			float v = UIInputSystem.ME.GetAxisVertical(JoyStickAction.Movement);
			MoveInput(new Vector2(h, v));

			float x = UIInputSystem.ME.GetAxisHorizontal(JoyStickAction.CameraLook);
			float y = UIInputSystem.ME.GetAxisVertical(JoyStickAction.CameraLook);
			LookInput(new Vector2(x, -y) * GameSettingDataSingleton.Instance.playerLookSpeed);
		}
    }

}