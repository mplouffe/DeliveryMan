using lvl_0;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryPointsManager : MonoBehaviour
{
    public static DeliveryPointsManager Instance;

    [SerializeField]
    private List<Point> m_startingPoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Point GetRandomStartingPoint()
    {
        var randomIndex = Random.Range(0, m_startingPoints.Count);
        Point startingPoint = m_startingPoints[randomIndex];
        startingPoint.ActivatePoint(PointState.Pickup);
        return startingPoint;
    }
}
