using lvl_0;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;

public class InstructionsManager : MonoBehaviour
{
    [SerializeField]
    private float m_inputCooldown;

    private Duration m_inputCooldownDuration;

    private InputActions m_inputActions;

    private void Awake()
    {
        m_inputCooldownDuration = new Duration(m_inputCooldown);
        m_inputActions = new InputActions();
    }

    private void OnEnable()
    {
        m_inputActions.Instructions.Enable();
        m_inputActions.Instructions.Back.performed += OnBackSelected;
    }

    private void OnDisable()
    {
        m_inputActions.Instructions.Back.performed -= OnBackSelected;
        m_inputActions.Instructions.Disable();
    }

    private void Update()
    {
        m_inputCooldownDuration.Update(Time.deltaTime);
    }

    private void OnBackSelected(CallbackContext context)
    {
        if (m_inputCooldownDuration.Elapsed())
        {
            m_inputCooldownDuration.Reset();
            GameManager.Instance.LeaveInstructions();
        }
    }
}
