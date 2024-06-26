using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZombeezGameJam.Entities.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Script References")]
        [SerializeField] private Player _playerScript;

        internal bool isFacingLeft;
        internal float xMoveInput;

        private PlayerActions _playerActions;
        private PlayerActions.Player_MapActions _playerActionsMap;

        #region Unity Methods

        private void Awake()
        {
            _playerActions = new PlayerActions();
            _playerActionsMap = _playerActions.Player_Map;
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += DisableInputs;
            EnableInputs();
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= DisableInputs;
            DisableInputs();
        }

        #endregion Unity Methods

        #region Custom Methods

        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            xMoveInput = context.ReadValue<float>();

            isFacingLeft = xMoveInput < 0;
            _playerScript.UpdatePlayerState(PlayerStates.Run);
        }

        private void OnMovementCanceled(InputAction.CallbackContext context)
        {
            xMoveInput = 0;
            _playerScript.UpdatePlayerState(PlayerStates.Idle);
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _playerScript.movementScript.ExecuteJump();
        }

        private void OnFirePerformed(InputAction.CallbackContext context)
        {
            if (CanPlayerFire())
            {
                _playerScript.weaponScript.FireWeapon();
            }
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Interact button pressed");
            _playerScript.interactor.CheckForInteractions();
        }

        private bool CanPlayerFire()
        {
            bool isUnarmed = _playerScript.currentWeapon == PlayerWeapons.Unarmed;
            PlayerStates[] invalidStates = new PlayerStates[] { PlayerStates.Jump, PlayerStates.Midair, PlayerStates.Land };
            bool isInValidState = invalidStates.Contains(_playerScript.currentState);

            return !(isUnarmed || isInValidState);
        }

        private void EnableInputs()
        {
            _playerActionsMap.Enable();

            _playerActionsMap.Movement.performed += OnMovementPerformed;
            _playerActionsMap.Movement.canceled += OnMovementCanceled;

            _playerActionsMap.Jump.performed += OnJumpPerformed;
            _playerActionsMap.Fire.performed += OnFirePerformed;
            _playerActionsMap.Interact.performed += OnInteractPerformed;
        }

        private void DisableInputs()
        {
            _playerActionsMap.Disable();

            _playerActionsMap.Movement.performed -= OnMovementPerformed;
            _playerActionsMap.Movement.canceled -= OnMovementCanceled;

            _playerActionsMap.Jump.performed -= OnJumpPerformed;
            _playerActionsMap.Fire.performed -= OnFirePerformed;
            _playerActionsMap.Interact.performed -= OnInteractPerformed;
        }

        #endregion Custom Methods
    }
}
