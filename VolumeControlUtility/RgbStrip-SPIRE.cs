using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;

namespace VolumeControlUtility
{
    public class RgbStrip
    {
        static SerialPort   comPort;
        int                 strip_size;

        public RgbStrip()
        {
            comPort = new SerialPort("COM5", 9600);
            comPort.Parity = Parity.None;
            comPort.DataBits = 8;
            comPort.StopBits = StopBits.One;
            comPort.Handshake = Handshake.None;
            comPort.Open();
            strip_size = 60;
        }

        public void barGraphVisual(int percent)
        {
            int             leds;
            Byte[]          buffer;
            MemoryStream    memStream;
            BinaryWriter    binWriter;


            leds = (int)((percent / 100.0f) * strip_size);
            Console.WriteLine("leds to illuminate: " + leds);

            for (int px = 0; px < leds; px++)
            {
                memStream = new MemoryStream();
                binWriter = new BinaryWriter(memStream);
                //header
                binWriter.Write((char)11);
                binWriter.Write((char)7);
                //data
                binWriter.Write((int)7);
                binWriter.Write((char)255);
                binWriter.Write((char)0);
                binWriter.Write((char)0);
                //Write to a Buffer
                buffer = memStream.GetBuffer();
                Console.WriteLine("bytes in stream: " + (int)memStream.Length);
                comPort.Write(buffer, 0, (int)memStream.Length - 1);
                Console.WriteLine("done writing");
            }
        }
    }
}
