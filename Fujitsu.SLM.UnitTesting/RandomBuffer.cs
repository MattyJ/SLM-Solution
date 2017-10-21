using System;

namespace Fujitsu.SLM.UnitTesting
{
    public class RandomBuffer
    {
        private readonly Random _random = new Random();
        private readonly byte[] _seedBuffer;

        public RandomBuffer(int maxBufferSize)
        {
            _seedBuffer = new byte[maxBufferSize];
            _random.NextBytes(_seedBuffer);
        }

        public byte[] GenerateBufferFromSeed(int size)
        {
            var randomWindow = _random.Next(0, size);

            var buffer = new byte[size];

            Buffer.BlockCopy(_seedBuffer, randomWindow, buffer, 0, size - randomWindow);
            Buffer.BlockCopy(_seedBuffer, 0, buffer, size - randomWindow, randomWindow);

            return buffer;
        }
    }
}