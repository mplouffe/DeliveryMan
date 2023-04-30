using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorAnimator : MonoBehaviour
{
    [SerializeField]
    private float m_scrollSpeed;

    private void Update()
    {
        if (transform.position.x <= -13.46)
        {
            transform.position = new Vector2(2.5f, transform.position.y);
        }

        float offset = transform.position.x + (m_scrollSpeed * Time.deltaTime);
        transform.position = new Vector2(offset, transform.position.y);
    }
}
