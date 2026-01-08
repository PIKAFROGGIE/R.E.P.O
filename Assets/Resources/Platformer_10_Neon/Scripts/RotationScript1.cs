using UnityEngine;

namespace ithappy
{
    public class RotationScript1 : MonoBehaviour
    {
        public enum RotationAxis
        {
            X,
            Y,
            Z
        }

        public RotationAxis ProprotationAxis = RotationAxis.Y;
        public float ProprotationSpeed = 50.0f;
        public float rotateDuration = 5.0f; // 旋转时长（秒）

        private float timer = 0f;

        void Update()
        {
            // 超过设定时间就停止旋转
            if (timer >= rotateDuration)
                return;

            timer += Time.deltaTime;

            float rotationValue = ProprotationSpeed * Time.deltaTime;

            Vector3 axis = Vector3.zero;
            switch (ProprotationAxis)
            {
                case RotationAxis.X:
                    axis = Vector3.right;
                    break;
                case RotationAxis.Y:
                    axis = Vector3.up;
                    break;
                case RotationAxis.Z:
                    axis = Vector3.forward;
                    break;
            }

            transform.Rotate(axis, rotationValue);
        }
    }
}
