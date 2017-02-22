using Microsoft.Xna.Framework;

namespace Tetris
{
    public class Transform
    {
        private bool _isDirty = true;

        private Vector3 _position = Vector3.Zero;
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; _isDirty = true; }
        }

        private Quaternion _rotation = Quaternion.Identity;
        public Quaternion Rotation
        {
            get { return _rotation; }
            set { _rotation = value; _isDirty = true; }
        }

        private Vector3 _scale = Vector3.One;
        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; _isDirty = true; }
        }

        private Matrix _cachedMatrix;
        public Matrix Matrix
        {
            get
            {
                if (_isDirty)
                {
                    CalculateMatrix();
                }

                return _cachedMatrix;
            }

            set
            {
                _cachedMatrix = value;
                _position = value.Translation;
                _rotation = value.Rotation;
                _scale = value.Scale;
            }
        }

        private void CalculateMatrix()
        {
            _cachedMatrix =
                Matrix.CreateTranslation(_position) *
                Matrix.CreateFromQuaternion(_rotation) *
                Matrix.CreateScale(_scale);

            _isDirty = false;
        }

        public Transform()
        {
        }

        public Transform(Matrix matrix)
        {
            Matrix = matrix;
        }
    }
}