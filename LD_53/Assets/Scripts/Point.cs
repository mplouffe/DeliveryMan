using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField]
    private List<Point> m_deliveryPoints;

    [SerializeField]
    private List<Point> m_pickUpPoints;
}
