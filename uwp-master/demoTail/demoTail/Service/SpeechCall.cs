using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoTail.Service
{
    public class SpeechCall
    {
        /** = 320 <br> The number of bytes (NOT shorts) of raw audio generated in 20 ms. */
        public static readonly int RAW_AUDIO_BYTES = 320;

        /** = 220 <br> The number of bytes in a RTP datagram with extension and payload (not including its UDP header) */
        public static readonly int DATAGRAM_LENGTH = 220;

        // package visibility
/*        internal TxInfo txInfo;*/
    }
}
