using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Otaku.Rendering
{
    public class QuadBatchRenderer : IDisposable
    {
        private int _blocksCapacity = 16;
        private int _usedBlocks = 0;

        private bool _vertexBufferDirty;

        private VertexBuffer _vertexBuffer;
        private VertexPositionColor[] _vertices;

        private IndexBuffer _indexBuffer;
        private uint[] _indices;

        readonly BasicEffect _effect;

        public QuadBatchRenderer(GraphicsDevice device)
        {
            var vertexCount = _blocksCapacity * 4;
            _vertices = new VertexPositionColor[vertexCount];

            var indicesCount = _blocksCapacity * 6;
            _indices = new uint[indicesCount];

            UpdateBuffers(device);

            _vertexBufferDirty = true;

            _effect = new BasicEffect(device)
            {
                VertexColorEnabled = true
            };
        }

        public void AddQuad(Vector2 position, Vector2 size, Color color)
        {
            if (_usedBlocks >= _blocksCapacity)
            {
                GrowCapacity();
            }

            var vertexOffset = _usedBlocks * 4;
            _vertices[vertexOffset + 0] = new VertexPositionColor(new Vector3(position.X, position.Y, 0.0f), color);
            _vertices[vertexOffset + 1] = new VertexPositionColor(new Vector3(position.X + size.X, position.Y, 0.0f), color);
            _vertices[vertexOffset + 2] = new VertexPositionColor(new Vector3(position.X + size.X, position.Y + size.Y, 0.0f), color);
            _vertices[vertexOffset + 3] = new VertexPositionColor(new Vector3(position.X, position.Y + size.Y, 0.0f), color);

            var indexOffset = _usedBlocks * 6;
            _indices[indexOffset + 0] = (uint)vertexOffset + 0;
            _indices[indexOffset + 1] = (uint)vertexOffset + 2;
            _indices[indexOffset + 2] = (uint)vertexOffset + 3;
            _indices[indexOffset + 3] = (uint)vertexOffset + 0;
            _indices[indexOffset + 4] = (uint)vertexOffset + 1;
            _indices[indexOffset + 5] = (uint)vertexOffset + 2;

            _vertexBufferDirty = true;

            _usedBlocks += 1;
        }

        public void Render(GraphicsDevice device, Matrix view, Matrix projection)
        {
            if (_vertexBufferDirty)
            {
                UpdateBuffers(device);
            }

            _effect.View = view;
            _effect.Projection = projection;

            device.SetVertexBuffer(_vertexBuffer);
            device.Indices = _indexBuffer;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _usedBlocks * 2);
            }
        }

        public void Clear()
        {
            _usedBlocks = 0;
            _vertices = new VertexPositionColor[_blocksCapacity * 4];
            _indices = new uint[_blocksCapacity * 6];

            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();

            _vertexBuffer = null;
            _indexBuffer = null;
        }

        void UpdateBuffers(GraphicsDevice device)
        {
            if (_vertexBuffer == null || _blocksCapacity * 4 > _vertexBuffer.VertexCount)
            {
                _vertexBuffer?.Dispose();
                _indexBuffer?.Dispose();

                _vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColor), _blocksCapacity * 4, BufferUsage.WriteOnly);
                _indexBuffer = new IndexBuffer(device, typeof(uint), _blocksCapacity * 6, BufferUsage.WriteOnly);
            }

            _vertexBuffer.SetData(_vertices);
            _indexBuffer.SetData(_indices);

            _vertexBufferDirty = false;
        }

        void GrowCapacity()
        {
            _blocksCapacity = _blocksCapacity * 4;
            Array.Resize(ref _vertices, _blocksCapacity * 4);
            Array.Resize(ref _indices, _blocksCapacity * 6);
        }

        public void Dispose()
        {
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
        }
    }
}