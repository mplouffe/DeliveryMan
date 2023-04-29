using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lvl_0
{
    public struct Duration
    {
        private float m_currentDuration;
        private float m_totalDuration;

        public Duration(float totalDuration)
        {
            m_currentDuration = 0;
            m_totalDuration = totalDuration;
        }

        public void Update(float duration)
        {
            m_currentDuration += duration;
        }

        public bool Elapsed()
        {
            return m_currentDuration >= m_totalDuration;
        }

        public void Reset(float newDuration = -1f)
        {
            if (newDuration > -1)
            {
                m_totalDuration = newDuration;
            }
            m_currentDuration = 0;
        }

        public float Delta()
        {
            return m_currentDuration / m_totalDuration;
        }
    }
}
