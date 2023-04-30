using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryController : MonoBehaviour
{
    [SerializeField]
    private Transform m_directionArrow;

    [SerializeField]
    private float m_arrowSpeed;

    [SerializeField]
    private Point m_targetPoint;

    void Update()
    {
        if (m_targetPoint != null)
        {
            Vector3 targetDir = m_targetPoint.transform.position - transform.position;
            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            m_directionArrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            m_directionArrow.Rotate(Vector3.forward, m_arrowSpeed * Time.deltaTime);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            var point = collision.gameObject.GetComponent<Point>();

        }
    }
}
