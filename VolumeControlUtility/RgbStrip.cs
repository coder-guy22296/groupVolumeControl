using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace VolumeControlUtility
{
    public class RgbStrip
    {
        RGBSerialDriverWrapper.RGBSerialDriverWrapperClass driver;

        public RgbStrip()
        {
            /*comPort = new SerialPort("COM5", 9600);
            comPort.Parity = Parity.None;
            comPort.DataBits = 8;
            comPort.StopBits = StopBits.One;
            comPort.Handshake = Handshake.None;
            comPort.Open();
            strip_size = 60;*/
            Console.WriteLine("initializing rgb serial driver");
            driver = new RGBSerialDriverWrapper.RGBSerialDriverWrapperClass();
            Console.WriteLine("rgb serial driver init complete");
            Thread.Sleep(300);
        }

        public void barGraphVisual(Byte percent)
        {
            /*int             leds;
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
            }*/
            Console.WriteLine("setting bar graph: {0}", (SByte)percent);
            driver.barx(127, 127, 127, (SByte)percent);
            Console.WriteLine("setting bar graph done");
        }
    }
}
