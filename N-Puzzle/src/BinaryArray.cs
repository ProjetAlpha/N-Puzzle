using System;
namespace NPuzzle.src
{
    public class BinaryArray
    {
        private bool _is64Bits;

        public BinaryArray()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                _is64Bits = true;
            }
        }

        /*public makeIntArray()
        {

        }*/
    }
}
