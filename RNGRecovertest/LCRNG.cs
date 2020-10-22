using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RNGRecovertest
{
    internal class LCRNG
    {
        public int seed;

        public LCRNG(int seed)
        {
            this.seed = seed;
        }

        public int nextUInt()
        {
            this.seed = (int)((this.seed * 0xEEB9EB65 + 0xA3561A1) & 0xffffffff);
            return this.seed;
        }
        public int nextUShort()
        {
            return (int)((uint)this.nextUInt() >> 16);
        }
    }
}