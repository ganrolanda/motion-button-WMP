using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Diagnostics;

namespace WindowsFormsApplication2
{
    class Camera
    {

        private VideoCaptureDevice videosource = null;//video device to be used

        bool brushes_updated = false;
        int count = 0;

        Pen p = new Pen(Color.Black, 2);
        SolidBrush[] sb = { new SolidBrush(Color.FromArgb(50, Color.Blue)), new SolidBrush(Color.FromArgb(70, Color.Red)), new SolidBrush(Color.FromArgb(70, Color.Green)) };
        
        private MotionDetection MD1 = new MotionDetection();
        private MotionDetection MD2 = new MotionDetection();
        private MotionDetection MD3 = new MotionDetection();

        public void Restore_brush()
        {
            sb[0] = new SolidBrush(Color.FromArgb(50, Color.Blue));
            sb[1] = new SolidBrush(Color.FromArgb(70, Color.Red));
            sb[2] = new SolidBrush(Color.FromArgb(70, Color.Green));
        }

        public void Update_brush(int i)
        {
            brushes_updated = true;
            if (i == 1)
            {
                sb[0] = new SolidBrush(Color.Blue);
            }
            else if (i == 2)
            {
                sb[1] = new SolidBrush(Color.Red);
            }
            else
            {
                sb[2] = new SolidBrush(Color.YellowGreen);
            }
        }
        //initialising
        bool intialize = true;

        // constructor
        public Camera(string MonikerString)
        {
            videosource = new VideoCaptureDevice(MonikerString);
            MD1.MotionEvent += new MotionDetection.MotionEventHandler(MD1_MotionEvent);
            MD2.MotionEvent += new MotionDetection.MotionEventHandler(MD2_MotionEvent);
            MD3.MotionEvent += new MotionDetection.MotionEventHandler(MD3_MotionEvent);
            MD3.threshold = Static_Variables.button3_threshold;
            MD2.threshold = Static_Variables.button2_threshold;
            MD1.threshold = Static_Variables.button1_threshold;
        }

        void MD3_MotionEvent(object MotionDetection, EventArgs eventArgs)
        {
            Update_brush(3);
            MD3.threshold = Static_Variables.button3_threshold;


            WMP_Control wmp = new WMP_Control();
            wmp.ControlMediaPlayer("PreviousSong");
        }

        void MD2_MotionEvent(object MotionDetection, EventArgs eventArgs)
        {
            Update_brush(2);
            MD2.threshold = Static_Variables.button2_threshold;


            WMP_Control wmp = new WMP_Control();
            wmp.ControlMediaPlayer("NextSong");
        }

        void MD1_MotionEvent(object MotionDetection, EventArgs eventArgs)
        {
            Update_brush(1);
            MD1.threshold = Static_Variables.button1_threshold;

            WMP_Control wmp = new WMP_Control();
            wmp.ControlMediaPlayer("Play/Pause");
        }

        //public delegates
        public delegate void FrameEventHandler(object Camera, Bitmap image);

        //public event for the class
        public event FrameEventHandler FrameEvent;

        //event firing method
        public void OnFraneEvent(object Camera, Bitmap image)
        {
            // Check if there are any Subscribers
            if (FrameEvent != null)
            {
                // Call the Event
                FrameEvent(Camera, image);
            }
        }

        //stop the camera source
        public void Stop_Camera()
        {
            if (videosource.IsRunning)
            {
                videosource.Stop();
            }
        }

        //start the camera source
        public void Start_Camera()
        {
            if (!videosource.IsRunning)
            {
                videosource.NewFrame += new NewFrameEventHandler(videosource_NewFrame);
                videosource.Start();
            }
        }

        void videosource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            Bitmap Image = new Bitmap(eventArgs.Frame.Width, eventArgs.Frame.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics _copy = Graphics.FromImage(Image);
            _copy.DrawImage(eventArgs.Frame, new Point(0, 0));
            _copy.Dispose();


            if (intialize)
            {
                Static_Variables.Make_rectangles(Image);
                intialize = false;
                return;
            }
            
            if (Static_Variables.IsDetecting)
            {
                //3 images
                Bitmap[] images = new Bitmap[3];
                for (int i = 0; i < 3; i++)
                {
                    images[i] = (Bitmap)Image.Clone(Static_Variables.rects[i], System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                }

                //update motion variables
                Static_Variables.percentile_motion[0] = MD1.Process(images[0]);
                Static_Variables.percentile_motion[1] = MD2.Process(images[1]);
                Static_Variables.percentile_motion[2] = MD3.Process(images[2]);
                Static_Variables.percentile_motion_diffrence[0] = MD1.average_motion_percentage;
                Static_Variables.percentile_motion_diffrence[1] = MD2.average_motion_percentage;
                Static_Variables.percentile_motion_diffrence[2] = MD3.average_motion_percentage;

            }

            if (brushes_updated)
            {
                count++;
                if (count > 10)
                {
                    count = 0; 
                    Restore_brush();
                    brushes_updated = false;
                }
            }
            int temp = 0;
            Graphics g = Graphics.FromImage(Image);
            foreach (Rectangle rect in Static_Variables.rects)
            {
                g.DrawRectangle(p, rect);
                g.FillRectangle(sb[temp], rect);
                temp++;
            }
            g.Dispose();

            OnFraneEvent(this, Image);

        }
    }
}
