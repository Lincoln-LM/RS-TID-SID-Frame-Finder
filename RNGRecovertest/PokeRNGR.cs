using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RNGRecovertest
{
    internal class PokeRNGR
    {
        public uint seed;

        public PokeRNGR(uint seed)
        {
            this.seed = seed;
        }

        public uint nextUInt()
        {
            seed = seed * 0xEEB9EB65 + 0xA3561A1;
            return seed;
        }
        public uint nextUShort()
        {
            return nextUInt() >> 16;
        }
    }
}