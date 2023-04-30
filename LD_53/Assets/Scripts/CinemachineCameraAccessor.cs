using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineCameraAccessor : MonoBehaviour
{
    public static CinemachineCameraAccessor Instance;

    [SerializeField]
    private CinemachineVirtualCamera m_cinemachineCamera;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    public CinemachineVirtualCamera GetCamera()
    {
        return m_cinemachineCamera;
    }
}
