using lvl_0;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField]
    private List<Point> m_deliveryPoints;

    [SerializeField]
    private List<Point> m_pickUpPoints;

    [SerializeField]
    private Animator m_pointAnimator;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer;

    private PointState m_pointState;
    private PlayerSide m_currentPlayerSide;

    private float m_scoreMultiplier = 0.1f;

    private float m_timeAdded;

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetDistance(float distance)
    {
        m_timeAdded = distance;
    }

    public Point PointReached()
    {
        List<Point> m_nextTargetPointsList = m_pointState == PointState.Pickup ? m_deliveryPoints : m_pickUpPoints;
        var randomIndex = Random.Range(0, m_nextTargetPointsList.Count);
        Point nextPoint = m_nextTargetPointsList[randomIndex];
        PointState targetState = m_pointState == PointState.Pickup ? PointState.Delivery : PointState.Pickup;
        nextPoint.ActivatePoint(targetState);
        var distance = Vector3.Distance(transform.position, nextPoint.transform.position);
        nextPoint.SetDistance(distance/2);

        if (m_pointState == PointState.Delivery)
        {
            var score = 10 + (10 * m_scoreMultiplier);
            m_scoreMultiplier += 0.1f;
            PointsManager.Instance.AddPoints((int)score);
            Clock.Instance.AddTimeToClock(m_timeAdded);
            GameManager.Instance.PlaySfx(Sfx.Delivery);
        }
        else
        {
            GameManager.Instance.PlaySfx(Sfx.Pickup);
        }

        gameObject.SetActive(false);
        return nextPoint;
    }

    private void Update()
    {
        if (m_pointState == PointState.Delivery)
        {
            var playerSide = GameManager.Instance.GetPlayerSide(transform.position.x);
            if (playerSide != m_currentPlayerSide)
            {
                if (playerSide == PlayerSide.Left)
                {
                    m_spriteRenderer.flipX = true;
                }
                else
                {
                    m_spriteRenderer.flipX = false;
                }
                m_currentPlayerSide = playerSide;
            }
        }

    }

    public void ActivatePoint(PointState pointState)
    {
        gameObject.SetActive(true);
        m_pointState = pointState;
        var animationIndex = m_pointState == PointState.Pickup ? "Box" : "Delivery";
        m_pointAnimator.SetTrigger(animationIndex);
        m_spriteRenderer.flipX = false;
        m_currentPlayerSide = PlayerSide.Right;
    }
}

public enum PointState
{
    Pickup,
    Delivery
}

public enum PlayerSide
{
    Left,
    Right
}
