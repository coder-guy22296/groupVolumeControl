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
        //RGBSerialDriverWrapper.RGBSerialDriverWrapperClass driver;

        public RgbStrip()
        {
            //Console.WriteLine("initializing rgb serial driver");
            //driver = new RGBSerialDriverWrapper.RGBSerialDriverWrapperClass();
            //Console.WriteLine("rgb serial driver init complete");
            //Thread.Sleep(300);
        }

        public void barGraphVisual(Byte percent)
        {
            //Console.WriteLine("setting bar graph: {0}", (SByte)percent);
            //driver.barx(127, 127, 127, (SByte)percent);
            //Console.WriteLine("setting bar graph done");
        }
    }
}
