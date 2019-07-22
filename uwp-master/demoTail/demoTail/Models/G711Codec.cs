using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoTail.Models
{
    public class G711Codec
    {
        public const int BIAS = 0x84; //132; or 1000 0100
        public const int MAX = 32635; // 32767 ( MAX 15-BIT integer) minus BIAS
    }
}
