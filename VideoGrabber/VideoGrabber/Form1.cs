//Important!: Select x64 at Solution Platform before running this sample.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intel.RealSense;
namespace VideoGrabber
{
    public partial class Form1 : Form
    {
        
        Config cfg;
        Pipeline pipeline;
        Colorizer colorizer;

        public Form1()
        {
            InitializeComponent();
            InitRealSense();
            timer1.Start();
        }

        private void InitRealSense()
        {
            cfg = new Config();
            pipeline = new Pipeline();
            //Initialization of running mode for Color camera
            cfg.EnableStream(Stream.Color, 640, 480, Format.Bgr8);
            cfg.EnableStream(Stream.Depth, 640, 480, Format.Z16);
            pipeline.Start(cfg);
            colorizer = new Colorizer();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            PointCloud pc = new PointCloud();
            using (var frames = pipeline.WaitForFrames())
            {
                //Acquiring ColorFrame and DepthFrame
                var colorFrame = frames.ColorFrame.DisposeWith(frames);
                var depthFrame = frames.DepthFrame.DisposeWith(frames);

                //Reffering the depth value at (320,240)
                this.Text = "Depth at (320,240): "+depthFrame.GetDistance(320, 240) + "m";

                // Colorization the depth frame for visualization purposes, .
                var colorizedDepth = colorizer.Process<VideoFrame>(depthFrame).DisposeWith(frames);

                var DepthImg = new Bitmap(640, 480, colorizedDepth.Stride, PixelFormat.Format24bppRgb, colorizedDepth.Data);
                var ColorImg = new Bitmap(640, 480, colorFrame.Stride, PixelFormat.Format24bppRgb, colorFrame.Data);
                pictureBox1.Image = DepthImg;
                pictureBox2.Image = ColorImg;

            }
        }
    }
}
