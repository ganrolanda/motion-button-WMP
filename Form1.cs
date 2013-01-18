using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videosources;// to store list of video devices
        Camera camera = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //starting a camera
            if (Start.Text == "Start")
            {
                Start.Text = "Stop";
                camera = new Camera(videosources[comboBox1.SelectedIndex].MonikerString);
                camera.FrameEvent += new Camera.FrameEventHandler(camera_FrameEvent);
                camera.Start_Camera();
                button1.Enabled = true;
            }
            else
            {
                Start.Text = "Start";
                camera.Stop_Camera();
                button1.Enabled = false;
            }
        }

        void camera_FrameEvent(object Camera, Bitmap image)
        {
            pictureBox1.Image = image;
            //ImageMatrix IM = new ImageMatrix(img);
            //pictureBox1.Image = IM.ToImage();
            MotionBox1.Text = Convert.ToString(Static_Variables.percentile_motion[0]);
            MotionBox2.Text = Convert.ToString(Static_Variables.percentile_motion[1]);
            MotionBox3.Text = Convert.ToString(Static_Variables.percentile_motion[2]);
            MotionDiffrence1.Text = Convert.ToString(Static_Variables.percentile_motion_diffrence[0]);
            MotionDiffrence2.Text = Convert.ToString(Static_Variables.percentile_motion_diffrence[1]);
            MotionDiffrence3.Text = Convert.ToString(Static_Variables.percentile_motion_diffrence[2]);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //getting camera info
            videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videosources.Count != 0)
            {
                foreach (FilterInfo videosource in videosources)
                {
                    comboBox1.Items.Add(videosource.Name);
                    comboBox1.Enabled = true;
                    Start.Enabled = true;
                }
            }
            else
            {
                comboBox1.Items.Add("No Device Found");
                comboBox1.Enabled = false;
                Start.Enabled = false;
            }
            comboBox1.SelectedIndex = 0;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //closing video source when form is closed
            if (camera != null)
            {
                camera.Stop_Camera();
            }

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label4.Text = "Set Motion Sensivity :  " + Convert.ToString(trackBar1.Value) + " out of 255";
            MotionDetection.each_pixel_threshold = (byte)trackBar1.Value;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Static_Variables.IsDetecting = true;
            button1.Enabled = false;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            Static_Variables.button1_threshold = trackBar3.Value;
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            Static_Variables.button2_threshold = trackBar4.Value;
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            Static_Variables.button3_threshold = trackBar5.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label5.Text = "Average motion over " + Convert.ToString(trackBar2.Value) + " pixels";
            MotionDetection.max_bound_s = trackBar2.Value;
        }


    }
}
