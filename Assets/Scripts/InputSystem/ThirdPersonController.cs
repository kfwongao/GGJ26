using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using System.Collections;
using Cinemachine;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
//#if ENABLE_INPUT_SYSTEM
//    [RequireComponent(typeof(PlayerInput))]
//#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        public CinemachineVirtualCamera _virtualCamera;
        private CinemachineBasicMultiChannelPerlin noise;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

//#if ENABLE_INPUT_SYSTEM
//        private PlayerInput _playerInput;
//#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;
        //
        public Button playerAttackBtn;
        public GameObject _attackTarget;
        public float attackTime = 0;
        public AudioClip attackSound;
        public AudioClip hitSound;

        public Slider HPSlider;
        public Slider MPSlider;
        public Slider EXPSlider;
        public Text HPBarText;
        public Text MPBarText;
        public Text EXPBarText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI playerNameText;

        public Button HPRecBtn;
        public Text HPRecText;
        public Button MPRecBtn;
        public Text MPRecText;
        public AudioClip recoverSound;

        public bool isDie = false;

        //Enemy
        public GameObject EnemyInfoUI;
        public Text EnemyHPText;
        public Slider EnemyHPSlider;
        public TextMeshProUGUI EnemyLevelText;
        public TextMeshProUGUI EnemyNameText;

        //Skill Button
        public Button skill01Btn;
        public Button skill02Btn;
        public Button skill03Btn;
        public Button skill04Btn;
        private bool isSpellingSkill = false;

        public GameObject[] skill01ShiFangObjs;
        public bool isOnSkill01Buffer = false;

        public GameObject[] skill02ShiFangObjs;
        public GameObject[] skill02FlyToObjs;
        public float skill02CoolDown = 5f;
        public List<GameObject> skill02AttackEnemies;

        public GameObject[] skill03ShiFangObjs;
        public GameObject[] skill03FlyToObjs;
        public float skill03CoolDown = 3f;
        public List<GameObject> skill03AttackEnemies;

        public GameObject[] skill04ShiFangObjs;
        public GameObject[] skill04FlyToObjs;
        public float skill04CoolDown = 1f;
        //
        public GameObject ValueChange3DDisplayObj;
        public NavMeshAgent _agent;
        public bool enableAINav = false;

        public TextMeshProUGUI charPageTotalCoinText;
        public TextMeshProUGUI charPageTotalDiamondText;

        public TextMeshProUGUI storePageTotalCoinText;
        public TextMeshProUGUI storePageTotalDiamondText;


        //
        public GameObject ReplayDialogUI;
        public GameObject WinDialogUI;

        //Hit Anim
        public GameObject UIPrefabsSpawnHolder;
        public GameObject HitAnimTips;

        private bool IsCurrentDeviceMouse
        {
            get
            {
                return false;
//#if ENABLE_INPUT_SYSTEM
//                return _playerInput.currentControlScheme == "KeyboardMouse";
//#else
//                return false;
//#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            PlayerData.Instance.LoadData();
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            playerAttackBtn.onClick.AddListener(Attack);
            HPRecBtn.onClick.AddListener(UseHPRecItem);
            MPRecBtn.onClick.AddListener(UseMPRecItem);

            skill01Btn.onClick.AddListener(() => { UseSkill(1); });
            skill02Btn.onClick.AddListener(() => { UseSkill(2); });
            skill03Btn.onClick.AddListener(() => { UseSkill(3); });
            skill04Btn.onClick.AddListener(() => { UseSkill(4); });

            skill01Btn.GetComponent<CountDown>().StartCoolDown(0);
            skill02Btn.GetComponent<CountDown>().StartCoolDown(0);
            skill03Btn.GetComponent<CountDown>().StartCoolDown(0);
            skill04Btn.GetComponent<CountDown>().StartCoolDown(0);

            _controller.enabled = false;
            transform.position = PlayerData.Instance.playerLocation;
            _controller.enabled = true;

            _agent = GetComponent<NavMeshAgent>();
            _agent.enabled = false;
            enableAINav = false;

            //#if ENABLE_INPUT_SYSTEM
            //            _playerInput = GetComponent<PlayerInput>();
            //#else
            //            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
            //#endif
            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if(isDie)
            {
                _animator.SetBool("isDie", true);
                ReplayDialogUI.SetActive(true);
                return;
            }

            if(PlayerData.Instance.isGameWin)
            {
                WinDialogUI.SetActive(true);
                return;
            }

            attackTime += Time.deltaTime;

            _hasAnimator = TryGetComponent(out _animator);

            UpdateAvatar();

            if(_input.move != Vector2.zero)
            {
                enableAINav = false;
                _agent.enabled = false;
            }

            if (enableAINav)
            {
                if(_attackTarget != null)
                {
                    if (Vector3.Distance(_attackTarget.transform.position, transform.position) < 2f)
                    {
                        _agent.SetDestination(transform.position);
                        Attack();
                        _animator.SetFloat(_animIDSpeed, 0);
                        _animator.SetFloat(_animIDMotionSpeed, 0);
                    } 
                    else
                    {
                        _agent.SetDestination(_attackTarget.transform.position);
                        if (_hasAnimator)
                        {
                            _animator.SetFloat(_animIDSpeed, _agent.speed);
                            _animator.SetFloat(_animIDMotionSpeed, 1f);
                        }
                    }

                }
            }
            JumpAndGravity();
            GroundedCheck();
            if(!enableAINav && !isSpellingSkill)
            {
                Move();
            }

            CheckMouseClickOnObject();
            AutoSavePlayerLocation();

            skillCoolDown();

        }

        private void skillCoolDown()
        {
            skill02CoolDown -= Time.deltaTime;
            skill03CoolDown -= Time.deltaTime;
            skill04CoolDown -= Time.deltaTime;
        }

        private void UpdateAvatar()
        {
            HPBarText.text = $"{Mathf.Round(PlayerData.Instance.currentHP)}/{PlayerData.Instance.GetLevelMaxHP()}";
            MPBarText.text = $"{Mathf.Round(PlayerData.Instance.currentMP)}/{PlayerData.Instance.GetLevelMaxMP()}";
            EXPBarText.text = $"{Mathf.Round(PlayerData.Instance.playerExp)}/{PlayerData.Instance.GetLevelExpUp()}";

            HPSlider.value = PlayerData.Instance.currentHP / PlayerData.Instance.GetLevelMaxHP();
            MPSlider.value = PlayerData.Instance.currentMP / PlayerData.Instance.GetLevelMaxMP();
            EXPSlider.value = PlayerData.Instance.playerExp / (float)PlayerData.Instance.GetLevelExpUp();

            levelText.text = PlayerData.Instance.playerLevel.ToString();
            playerNameText.text = PlayerData.Instance.playerName;


            if (_attackTarget != null)
            {
                EnemyAI ai = _attackTarget.GetComponent<EnemyAI>();
                EnemyInfoUI.SetActive(true);
                EnemyHPText.text = $"{Mathf.Round(ai.currentHP)}/{Mathf.Round(ai.maxHP)}";
                EnemyHPSlider.value = ai.currentHP / (float)ai.maxHP;
                EnemyLevelText.text = $"{ai.EnemyLevel}";
                EnemyNameText.text = ai.EnemyName;
            }
            else
            {
                EnemyInfoUI.SetActive(false);
            }

        }

        private void AutoSavePlayerLocation()
        {
            PlayerData.Instance.playerLocation = transform.position;
            if (Vector3.Distance(transform.position, PlayerData.Instance.playerLocation) > 20f)
            {
                PlayerData.Instance.playerLocation = transform.position;
                PlayerData.Instance.SaveLocation();
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
            //Inventory.Instance.UpdateUI();
            HPRecText.text = $"{Inventory.Instance.hp_water_count}";
            MPRecText.text = $"{Inventory.Instance.mp_water_count}";
            charPageTotalCoinText.text = $"{PlayerData.Instance.totalCoin}";
            charPageTotalDiamondText.text = $"{PlayerData.Instance.totalDiamond}";
            storePageTotalCoinText.text = $"{PlayerData.Instance.totalCoin}";
            storePageTotalDiamondText.text = $"{PlayerData.Instance.totalDiamond}";
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void CheckMouseClickOnObject()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Pressed left click, casting ray.");
                CastRay();
            }
        }

        void CastRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500))
            {
                Debug.DrawLine(ray.origin, hit.point);
                //Debug.Log("Hit object: " + hit.collider.gameObject.name);
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    if (!hit.collider.gameObject.GetComponent<EnemyAI>().isDie)
                    {
                        _attackTarget = hit.collider.gameObject;
                    }
                }
            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            MapsDataSingleton.Instance.isGrounded = Grounded;

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);

            float fov = _virtualCamera.m_Lens.FieldOfView;
            fov = Mathf.Clamp(fov + _input.look.y * Time.deltaTime, 15, 70);
            _virtualCamera.m_Lens.FieldOfView = fov;
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        public void ReceieveDamage(GameObject attacker, float damage)
        {
            StartCoroutine(_ProcessShake(5f, 0.5f));
            if (hitSound != null)
            {
                AudioManager.Instance.audioSource.PlayOneShot(hitSound);
            }
            if (_attackTarget == null)
            {
                _attackTarget = attacker;
            }
            float tempHP = PlayerData.Instance.currentHP;
            float calc = PlayerData.Instance.CalculateReceieveDamage(damage);
            tempHP = tempHP - calc;
            if (HitAnimTips != null)
            {
                ShowLocationChangeUIAnim(((int)(-1 * calc)).ToString(), Color.red, Camera.main.WorldToScreenPoint(transform.position));
                //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText((int)(-1 * calc), false, Color.red);
            }
            PlayerData.Instance.currentHP = tempHP > 0 ? tempHP : 0;
            PlayerData.Instance.SaveData();

            UpdateAvatar();

            if (tempHP <= 0)
            {
                isDie = true;
            }
        }

        private IEnumerator _ProcessShake(float shakeIntensity = 5f, float shakeTiming = 0.5f)
        {
            Noise(0.25f, shakeIntensity);
            yield return new WaitForSeconds(shakeTiming);
            Noise(0, 0);
        }

        public void Noise(float amplitudeGain, float frequencyGain)
        {
            noise.m_AmplitudeGain = amplitudeGain;
            noise.m_FrequencyGain = frequencyGain;
        }

        public void Attack()
        {
            if(Grounded)
            {
                if(attackTime > 1f)
                {
                    attackTime = 0;
                    if (_attackTarget != null)
                    {
                        if (MapsDataSingleton.Instance.LocationAreaName == "Space Ship")
                        {
                            if (GameSettingDataSingleton.Instance.localization_index == 0)
                            {
                                // English
                                ShowLocationChangeUIAnim($"Cannot Fight in Space Ship", Color.red, Camera.main.WorldToScreenPoint(transform.position));
                            }
                            else
                            {
                                ShowLocationChangeUIAnim($"不能在战舰中战斗", Color.red, Camera.main.WorldToScreenPoint(transform.position));
                            }
                            return;
                        }
                        if (Vector3.Distance(_attackTarget.transform.position, transform.position) < 2f)
                        {
                            _agent.enabled = false;
                            enableAINav = false;
                            _animator.Play("Attack");
                            if(attackSound != null)
                            {
                                AudioManager.Instance.audioSource.PlayOneShot(attackSound);
                            }
                            transform.rotation = Quaternion.LookRotation(_attackTarget.transform.position - transform.position);

                            EnemyAI enemyai = _attackTarget.GetComponent<EnemyAI>();
                            if (enemyai != null)
                            {
                                enemyai.ReceieveAttack(PlayerData.Instance.CalculateAttack(enemyai.defenceValue));
                            }
                        }
                        else
                        {
                            _agent.enabled = true;
                            _agent.SetDestination(_attackTarget.transform.position);
                            _agent.velocity = _controller.velocity;
                            enableAINav = true;
                        }
                    }
                }
            }
        }

        public void ReceieveExp(int exp)
        {
            int LVBeforeCalcExp = PlayerData.Instance.playerLevel;
            PlayerData.Instance.CalculateExp(exp);
            int LVAfterCalcExp = PlayerData.Instance.playerLevel;
            if(LVAfterCalcExp > LVBeforeCalcExp)
            {
                if (HitAnimTips != null)
                {
                    if (GameSettingDataSingleton.Instance.localization_index == 0)
                    {
                        // English
                        ShowLocationChangeUIAnim($"Level UP to {LVAfterCalcExp}", Color.green, Camera.main.WorldToScreenPoint(transform.position));
                    }
                    else
                    {
                        ShowLocationChangeUIAnim($"等级升到 {LVAfterCalcExp}", Color.green, Camera.main.WorldToScreenPoint(transform.position));
                    }
                    
                    //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText($"Level UP to {LVAfterCalcExp}", Color.green);
                    // TODO: Add special popup, do rewards popup, do level up suggestion
                    PlayerData.Instance.remaindBonusBasePoint += (PlayerData.Instance.GetLevelMaxBonusBasePoint(LVAfterCalcExp) - PlayerData.Instance.GetLevelMaxBonusBasePoint(LVBeforeCalcExp));
                    PlayerData.Instance.remaindBonusSkillPoint += (PlayerData.Instance.GetLevelMaxBonusSkillPoint(LVAfterCalcExp) - PlayerData.Instance.GetLevelMaxBonusSkillPoint(LVBeforeCalcExp));
                    PlayerData.Instance.SaveData();
                }
            } else
            {
                if (HitAnimTips != null)
                {
                    if (GameSettingDataSingleton.Instance.localization_index == 0)
                    {
                        // English
                        ShowLocationChangeUIAnim($"+ Exp {exp}", Color.yellow, Camera.main.WorldToScreenPoint(transform.position));
                    }
                    else
                    {
                        ShowLocationChangeUIAnim($"+ 经验 {exp}", Color.yellow, Camera.main.WorldToScreenPoint(transform.position));
                    }
                    
                    //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText($"+ Exp {exp}", Color.yellow);
                }
            }
        }

        public void UseHPRecItem()
        {
            if(PlayerData.Instance.currentHP >= PlayerData.Instance.GetLevelMaxHP())
            {
                return;
            }

            if(Inventory.Instance.hp_water_count > 0)
            {
                Inventory.Instance.UseItem("HP_water");
                if (recoverSound != null)
                {
                    AudioManager.Instance.audioSource.PlayOneShot(recoverSound);
                }
                PlayerData.Instance.currentHP = Mathf.Clamp(PlayerData.Instance.currentHP + 100f, 0, PlayerData.Instance.GetLevelMaxHP());
                if (HitAnimTips != null)
                {
                    ShowLocationChangeUIAnim($"+ HP {100}", Color.green, Camera.main.WorldToScreenPoint(transform.position));
                    //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText($"+ HP {100}", Color.green);
                }
            }
        }

        public void UseMPRecItem()
        {
            if (PlayerData.Instance.currentMP >= PlayerData.Instance.GetLevelMaxMP())
            {
                return;
            }

            if (Inventory.Instance.mp_water_count > 0)
            {
                Inventory.Instance.UseItem("MP_water");
                if (recoverSound != null)
                {
                    AudioManager.Instance.audioSource.PlayOneShot(recoverSound);
                }
                PlayerData.Instance.currentMP = Mathf.Clamp(PlayerData.Instance.currentMP + 100f, 0, PlayerData.Instance.GetLevelMaxMP());
                if (HitAnimTips != null)
                {
                    ShowLocationChangeUIAnim($"+ MP {100}", Color.green, Camera.main.WorldToScreenPoint(transform.position));
                    //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText($"+ MP {100}", Color.green);
                }
            }
        }

        public void UseSkill(int id)
        {
            if (checkSkillCanSpawn(id))
            {
                isSpellingSkill = true;
                _agent.enabled = false;
                enableAINav = false;
                float stopRecieveMoveTime = 0f;
                float useMP = PlayerData.Instance.GetSkillUseMP(id);
                switch (id)
                {
                    case 1:
                        // buffer
                        // TODO: play animation
                        _animator.Play("SkillArea");
                        PlayerData.Instance.currentMP = Mathf.Clamp(PlayerData.Instance.currentMP - useMP, 0, PlayerData.Instance.GetLevelMaxMP());
                        Instantiate(skill01ShiFangObjs[PlayerData.Instance.GetSkillIndex(id)], transform).GetComponent<skill_shifang>().PlayEffect();
                        PlayerData.Instance.skillBuffPoints = PlayerData.Instance.GetBufferSkillPoints();
                        isOnSkill01Buffer = true;
                        Invoke(nameof(ResetSkill01Buffer), 50f);
                        skill01Btn.GetComponent<CountDown>().StartCoolDown(60f);
                        break;
                    case 2:
                        // poison magic attack
                        int canPoisonNum = PlayerData.Instance.GetMaxNumberOfPoisonSkillAttack();

                        // Using Physics.OverlapSphere
                        Collider[] castPoisonColliders = Physics.OverlapSphere(transform.position, 15f);
                        if (castPoisonColliders.Length > 0)
                        {
                            foreach (Collider obj in castPoisonColliders)
                            {
                                if (obj.transform.gameObject.CompareTag("Enemy"))
                                {
                                    _attackTarget = obj.transform.gameObject;
                                    break;
                                }
                            }

                            skill02AttackEnemies.Clear();
                            for (int i = 0; i < castPoisonColliders.Length; i++)
                            {
                                if (castPoisonColliders[i].transform.gameObject.CompareTag("Enemy"))
                                {
                                    skill02AttackEnemies.Add(castPoisonColliders[i].transform.gameObject);
                                }
                            }
                        }

                        if (skill02AttackEnemies.Count > 0)
                        {
                            skill02AttackEnemies.Sort((a, b) => {
                                float distance_a = Vector3.Distance(a.transform.position, transform.position);
                                float distance_b = Vector3.Distance(b.transform.position, transform.position);
                                return distance_a.CompareTo(distance_b);
                            });

                            // magic wind shifang anim
                            _animator.Play("SkillArea");

                            PlayerData.Instance.currentMP = Mathf.Clamp(PlayerData.Instance.currentMP - useMP, 0, PlayerData.Instance.GetLevelMaxMP());

                            for (int i = 0; i < skill02AttackEnemies.Count; i++)
                            {
                                if (i > canPoisonNum) break;

                                if (skill02AttackEnemies[i] != null)
                                {
                                    if (i == 0)
                                    {
                                        transform.rotation = Quaternion.LookRotation(skill02AttackEnemies[i].transform.position - transform.position);
                                    }
                                    // spwan a list of wind magic and fly to target
                                    Instantiate(skill02ShiFangObjs[PlayerData.Instance.GetSkillIndex(id)], transform).GetComponent<skill_shifang>().PlayEffect();
                                    skill_flyToTarget flyto2 = Instantiate(skill02FlyToObjs[PlayerData.Instance.GetSkillIndex(id)], transform.position, Quaternion.identity).GetComponent<skill_flyToTarget>();
                                    flyto2.SetTarget(skill02AttackEnemies[i], gameObject, PlayerData.Instance.GetSkillDamage(2));
                                    stopRecieveMoveTime = flyto2.delayFly;
                                }
                            }
                            //skill03CoolDown = 3f;
                            skill02Btn.GetComponent<CountDown>().StartCoolDown(5f);
                        }
                        else
                        {
                            if (HitAnimTips != null)
                            {
                                if (GameSettingDataSingleton.Instance.localization_index == 0)
                                {
                                    // English
                                    ShowLocationChangeUIAnim($"No target", Color.red, Camera.main.WorldToScreenPoint(transform.position));
                                }
                                else
                                {
                                    ShowLocationChangeUIAnim($"没有目标", Color.red, Camera.main.WorldToScreenPoint(transform.position));
                                }
                                
                                //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText($"No target", Color.red);
                            }
                        }
                        break;
                    case 3:
                        // wind magic attack
                        int canAttackNum = PlayerData.Instance.GetMaxNumberOfWindSkillAttack();

                        // Using Physics.OverlapSphere
                        Collider[] castColliders = Physics.OverlapSphere(transform.position, 10f);
                        if (castColliders.Length > 0)
                        {
                            foreach (Collider obj in castColliders)
                            {
                                if (obj.transform.gameObject.CompareTag("Enemy"))
                                {
                                    _attackTarget = obj.transform.gameObject;
                                    break;
                                }
                            }

                            skill03AttackEnemies.Clear();
                            for (int i = 0; i < castColliders.Length; i++)
                            {
                                if (castColliders[i].transform.gameObject.CompareTag("Enemy"))
                                {
                                    skill03AttackEnemies.Add(castColliders[i].transform.gameObject);
                                }
                            }
                        }

                        if (skill03AttackEnemies.Count > 0)
                        {
                            skill03AttackEnemies.Sort((a, b) => { 
                                float distance_a = Vector3.Distance(a.transform.position, transform.position);
                                float distance_b = Vector3.Distance(b.transform.position, transform.position);
                                return distance_a.CompareTo(distance_b);
                            });

                            // magic wind shifang anim
                            _animator.Play("SkillArea");

                            PlayerData.Instance.currentMP = Mathf.Clamp(PlayerData.Instance.currentMP - useMP, 0, PlayerData.Instance.GetLevelMaxMP());

                            for (int i = 0; i < skill03AttackEnemies.Count; i++)
                            {
                                if (i > canAttackNum) break;

                                if (skill03AttackEnemies[i] != null)
                                {
                                    if(i == 0)
                                    {
                                        transform.rotation = Quaternion.LookRotation(skill03AttackEnemies[i].transform.position - transform.position);
                                    }
                                    // spwan a list of wind magic and fly to target
                                    Instantiate(skill03ShiFangObjs[PlayerData.Instance.GetSkillIndex(id)], transform).GetComponent<skill_shifang>().PlayEffect();
                                    skill_flyToTarget flyto3 = Instantiate(skill03FlyToObjs[PlayerData.Instance.GetSkillIndex(id)], transform.position, Quaternion.identity).GetComponent<skill_flyToTarget>();
                                    flyto3.SetTarget(skill03AttackEnemies[i], gameObject, PlayerData.Instance.GetSkillDamage(3));
                                    stopRecieveMoveTime = flyto3.delayFly;
                                }
                            }
                            //skill03CoolDown = 3f;
                            skill03Btn.GetComponent<CountDown>().StartCoolDown(3f);
                        } 
                        else
                        {
                            if (HitAnimTips != null)
                            {
                                if (GameSettingDataSingleton.Instance.localization_index == 0)
                                {
                                    // English
                                    ShowLocationChangeUIAnim($"No target", Color.red, Camera.main.WorldToScreenPoint(transform.position));
                                }
                                else
                                {
                                    ShowLocationChangeUIAnim($"没有目标", Color.red, Camera.main.WorldToScreenPoint(transform.position));
                                }
                                
                                //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText($"No target", Color.red);
                            }
                        }
                        break;
                    case 4:
                        // qigong ball
                        // TODO: play animation
                        _animator.Play("SkillSingle");
                        transform.rotation = Quaternion.LookRotation(_attackTarget.transform.position - transform.position);
                        PlayerData.Instance.currentMP = Mathf.Clamp(PlayerData.Instance.currentMP - useMP, 0, PlayerData.Instance.GetLevelMaxMP());
                        skill_shifang skill_shifang = Instantiate(skill04ShiFangObjs[PlayerData.Instance.GetSkillIndex(id)], transform).GetComponent<skill_shifang>();
                        skill_shifang.PlayEffect();
                        skill_flyToTarget flyto4 = Instantiate(skill04FlyToObjs[PlayerData.Instance.GetSkillIndex(id)], transform.position, Quaternion.identity).GetComponent<skill_flyToTarget>();
                        flyto4.SetTarget(_attackTarget, gameObject, PlayerData.Instance.GetSkillDamage(4));
                        stopRecieveMoveTime = flyto4.delayFly;
                        
                        //skill04CoolDown = 1f;
                        skill04Btn.GetComponent<CountDown>().StartCoolDown(1f);
                        break;
                }
                Invoke(nameof(SpellSkillEnd), stopRecieveMoveTime);
            }
        }

        private void SpellSkillEnd()
        {
            isSpellingSkill = false;
        }

        public bool checkSkillCanSpawn(int id)
        {
            if (MapsDataSingleton.Instance.LocationAreaName == "Space Ship")
            {
                if (GameSettingDataSingleton.Instance.localization_index == 0)
                {
                    // English
                    ShowLocationChangeUIAnim($"Cannot Fight in Space Ship", Color.red, Camera.main.WorldToScreenPoint(transform.position));
                }
                else
                {
                    ShowLocationChangeUIAnim($"不能在战舰中战斗", Color.red, Camera.main.WorldToScreenPoint(transform.position));
                }
                return false;
            }
            float useMP = PlayerData.Instance.GetSkillUseMP(id);
            string errorText = "";
            if (PlayerData.Instance.currentMP - useMP < 0)
            {
                if (GameSettingDataSingleton.Instance.localization_index == 0)
                {
                    // English
                    errorText = $"Not Enough MP";
                }
                else
                {
                    errorText += $"MP 不足";
                }
                
            }
            switch (id)
            {
                case 1:
                    // buffer
                    if (HitAnimTips != null)
                    {
                        ShowLocationChangeUIAnim(errorText, Color.red, Camera.main.WorldToScreenPoint(transform.position));
                        //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText(errorText, Color.red);
                    }
                    return PlayerData.Instance.currentMP - useMP >= 0 && !isOnSkill01Buffer && skill01Btn.GetComponent<CountDown>().canSpawnSkill;
                case 2:
                    // poison magic attack
                    if (HitAnimTips != null)
                    {
                        ShowLocationChangeUIAnim(errorText, Color.red, Camera.main.WorldToScreenPoint(transform.position));
                        //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText(errorText, Color.red);
                    }
                    return PlayerData.Instance.currentMP - useMP >= 0 && skill02Btn.GetComponent<CountDown>().canSpawnSkill;
                case 3:
                    // wind magic attack
                    if (skill03CoolDown > 0)
                    {
                        if (GameSettingDataSingleton.Instance.localization_index == 0)
                        {
                            // English
                            errorText += $"Skill not cool down";
                        }
                        else
                        {
                            errorText += $"技能没有冷却";
                        }
                    }
                    if (HitAnimTips != null)
                    {
                        ShowLocationChangeUIAnim(errorText, Color.red, Camera.main.WorldToScreenPoint(transform.position));
                        //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText(errorText, Color.red);
                    }
                    return PlayerData.Instance.currentMP - useMP >= 0 && skill03Btn.GetComponent<CountDown>().canSpawnSkill;
                case 4:
                    if (_attackTarget == null)
                    {
                        if (HitAnimTips != null)
                        {
                            if (GameSettingDataSingleton.Instance.localization_index == 0)
                            {
                                // English
                                ShowLocationChangeUIAnim($"No target", Color.red, Camera.main.WorldToScreenPoint(transform.position));
                            }
                            else
                            {
                                ShowLocationChangeUIAnim($"没有目标", Color.red, Camera.main.WorldToScreenPoint(transform.position));
                            }
                            
                            //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText($"No target", Color.red);
                        }
                        return false;
                    }
                    // qigong ball
                    if(skill04CoolDown > 0)
                    {
                        if(GameSettingDataSingleton.Instance.localization_index == 0)
                        {
                            // English
                            errorText += $"   Skill not cool down";
                        } else
                        {
                            errorText += $"   技能没有冷却";
                        }
                        
                    }
                    if (HitAnimTips != null)
                    {
                        ShowLocationChangeUIAnim(errorText, Color.red, Camera.main.WorldToScreenPoint(transform.position));
                        //Instantiate(ValueChange3DDisplayObj, transform).GetComponent<ValueChange3DDisplay>().SetText(errorText, Color.red);
                    }
                    return PlayerData.Instance.currentMP - useMP >= 0 && skill04Btn.GetComponent<CountDown>().canSpawnSkill;
                default:
                    break;
            }
            return false;
        }

        private void ResetSkill01Buffer()
        {
            PlayerData.Instance.skillBuffPoints = 0;
            isOnSkill01Buffer = false;
        }

        public void Replay()
        {
            isDie = false;
            PlayerData.Instance.currentHP = PlayerData.Instance.GetLevelMaxHP();
            PlayerData.Instance.playerLocation = new Vector3(-30, 0, 70);
            _controller.enabled = false;
            transform.position = PlayerData.Instance.playerLocation;
            _controller.enabled = true;
            _animator.SetBool("isDie", false);
            ReplayDialogUI.SetActive(false);
        }

        public void ShowLocationChangeUIAnim(string name, Color color, Vector3 pos, int textSize = 60)
        {
            GameObject go = Instantiate(HitAnimTips, UIPrefabsSpawnHolder.transform);
            go.GetComponent<HitAnim>().Init(name, color, pos, textSize);
        }
    }
}