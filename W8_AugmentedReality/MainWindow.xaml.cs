using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;


using System.Timers;
using Microsoft.Kinect;
using System.Media;

namespace W8_AugmentedReality
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;

        private Skeleton[] skeletons = null;
        private JointType[] bones = { 
                                      // torso 
                                      JointType.Head, JointType.ShoulderCenter,
                                      JointType.ShoulderCenter, JointType.ShoulderLeft,
                                      JointType.ShoulderCenter, JointType.ShoulderRight,
                                      JointType.ShoulderCenter, JointType.Spine, 
                                      JointType.Spine, JointType.HipCenter,
                                      JointType.HipCenter, JointType.HipLeft, 
                                      JointType.HipCenter, JointType.HipRight,
                                      // left arm 
                                      JointType.ShoulderLeft, JointType.ElbowLeft,
                                      JointType.ElbowLeft, JointType.WristLeft,
                                      JointType.WristLeft, JointType.HandLeft,
                                      // right arm 
                                      JointType.ShoulderRight, JointType.ElbowRight,
                                      JointType.ElbowRight, JointType.WristRight,
                                      JointType.WristRight, JointType.HandRight,
                                      // left leg
                                      JointType.HipLeft, JointType.KneeLeft,
                                      JointType.KneeLeft, JointType.AnkleLeft,
                                      JointType.AnkleLeft, JointType.FootLeft,
                                      // right leg
                                      JointType.HipRight, JointType.KneeRight,
                                      JointType.KneeRight, JointType.AnkleRight,
                                      JointType.AnkleRight, JointType.FootRight,
                                    };

        private DrawingGroup drawingGroup; // Drawing group for skeleton rendering output
        private DrawingImage drawingImg; // Drawing image that we will display

        private byte[] colorData = null;
        private WriteableBitmap colorImageBitmap = null;

        //
        // new
        //

        int screenWid = 720;

        private BitmapImage t_shirt;
        private BitmapImage bug;
        private BitmapImage body;
        private BitmapImage hat;
        private BitmapImage gun;
        private BitmapImage sun;
        private BitmapImage bulletImg;
        Rect sunR = new Rect(260, 400, 200, 170);

        private BitmapImage[] hP = new BitmapImage[5];
        private Rect hpRect = new Rect(50, 380, 150, 85);

        private BitmapImage rightUI;
        private Rect bulletRect = new Rect(530, 380, 120, 65);

        private BitmapImage[] badGhost = new BitmapImage[4];
        private Rect[] badGhostRect = new Rect[4];
        private BitmapImage[] goodGhost = new BitmapImage[5];
        private Rect[] goodGhostRect = new Rect[5];

        bool[] isLevel = new bool[2];

        private SoundPlayer soundPlayer = new SoundPlayer("Images/highNoon.wav");
        private SoundPlayer shootEffect = new SoundPlayer("Images/shoot.wav");
        private SoundPlayer hurtEffect = new SoundPlayer("Images/hurt.wav");

        int[] rd2 = { 1, 3, 4, 5 };
        int[] rd3 = { 2, 3, 4, 5, 6 };

        private int healPoint = 4;
        int score = 0;
        int killNum = 0;
        double angle = 0;

        private WriteableBitmap writeablePhoto = null;
        private BitmapImage photo = new BitmapImage();
        private Point headPoint = new Point(320, 320);


        private double hatWid = 50;

        Bullet bullet = new Bullet();
        int bulletNum = 6;

        Pose reloadPose = new Pose();
        Pose shootPose = new Pose();

        //private Rect r = new Rect(0, 0, 200, 200);
        private Rect bugR = new Rect(320, 0, 50, 50);
        private int speed = 6;

        Rect gunR;

        // Move direction of the bullet
        Vector moveDir;

        bool shoot = false;
        bool isShoot = false;
        bool isGameStart = false;
        bool isSun = false;

        System.Timers.Timer aTimer = new System.Timers.Timer();

        void Init()
        {
            sunR = new Rect(260, 400, 200, 170);
            rd2[0] = 1;
            rd2[1] = 3;
            rd2[2] = 4;
            rd2[3] = 5;

            rd3[0] = 2;
            rd3[1] = 3;
            rd3[2] = 4;
            rd3[3] = 5;
            rd3[4] = 6;

            killNum = 0;
            isLevel[0] = true;
            isLevel[1] = false;

            healPoint = 4;
            score = 0;

            bullet = new Bullet();
            bulletNum = 6;

            shoot = false;
            isShoot = false;
            isGameStart = false;
            isSun = false;

            for (int i = 0; i < badGhostRect.Length; i++)
            {
                badGhostRect[i] = new Rect(-100, 30, 100, 100);
            }

            for (int i = 0; i < goodGhostRect.Length; i++)
            {
                goodGhostRect[i] = new Rect(-100, 230, 100, 100);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            t_shirt = LoadImages("Images/i_love_hk.png");
            bug = LoadImages("Images/bug.png");
            body = LoadImages("Images/body.png");
            hat = LoadImages("Images/hat.png");
            gun = LoadImages("Images/gun.png");
            sun = LoadImages("Images/sun.png");

            bulletImg = LoadImages("Images/bul.png");

            rightUI = LoadImages("Images/UI/uiRight.png");

            isLevel[0] = true;
            isLevel[1] = false;

            for (int i = 0; i < hP.Length; i++)
            {
                String str = "Images/UI/UILeft_" + (i + 1) + ".png";
                hP[i] = LoadImages(str);
                //if (badGhost[i].va) Console.WriteLine(str + " load fail");
            }

            for (int i = 0; i < badGhost.Length; i++)
            {
                String str = "Images/ghost/ghost_" + i + ".png";
                badGhost[i] = LoadImages(str);
                // if (badGhost[i] == null) Console.WriteLine(str + " load fail");
            }

            for (int i = 0; i < goodGhost.Length; i++)
            {
                String str = "Images/ghost/ghost_" + (i + 4) + ".png";
                goodGhost[i] = LoadImages(str);
                // if (goodGhost[i] == null) Console.WriteLine(str + " load fail");
            }

            for (int i = 0; i < badGhostRect.Length; i++)
            {
                badGhostRect[i] = new Rect(-100, 30, 100, 100);
            }

            for (int i = 0; i < goodGhostRect.Length; i++)
            {
                goodGhostRect[i] = new Rect(-100, 230, 100, 100);
            }


            aTimer.Elapsed += new ElapsedEventHandler(shootInternal);

            aTimer.Interval = 1000;    // 1秒 = 1000毫秒

            aTimer.Start();

            // Create the pose;
            ReloadPose();
            ShootPose();

            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("No Kinects detected", "Depth Sensor Basics");
                Application.Current.Shutdown();
            }
            else
            {
                sensor = KinectSensor.KinectSensors[0];
                if (sensor == null)
                {
                    MessageBox.Show("Kinect is not ready to use", "Depth Sensor Basics");
                    Application.Current.Shutdown();
                }
            }

            // -------------------------------------------------------
            // color 
            sensor.ColorStream.Enable();
            // allocate storage for color data 
            colorData = new byte[sensor.ColorStream.FramePixelDataLength];

            // create an empty bitmap with the same size as color frame 
            colorImageBitmap = new WriteableBitmap(
                      sensor.ColorStream.FrameWidth, sensor.ColorStream.FrameHeight,
                      96, 96, PixelFormats.Bgr32, null);


            colorImg.Source = colorImageBitmap;
            // register an event handler 
            sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);

            // skeleton stream 
            sensor.SkeletonStream.Enable();
            sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
            skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];

            // -------------------------------------------------------
            // Create the drawing group we'll use for drawing
            drawingGroup = new DrawingGroup();
            // Create an image source that we can use in our image control
            drawingImg = new DrawingImage(drawingGroup);
            // Display the drawing using our image control
            skeletonImg.Source = drawingImg;
            // prevent drawing outside of our render area
            drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, screenWid, 480));

            // start the kinect
            sensor.Start();
        }

        private void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null) return;

                // get the data 
                colorFrame.CopyPixelDataTo(colorData);
                // write color data to bitmap buffer
                colorImageBitmap.WritePixels(
                    new Int32Rect(0, 0, colorFrame.Width, colorFrame.Height),
                    colorData, colorFrame.Width * colorFrame.BytesPerPixel, 0);

                using (MemoryStream stream = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(colorImageBitmap));
                    encoder.Save(stream);

                    photo.CacheOption = BitmapCacheOption.OnLoad;
                    photo.StreamSource = stream;
                }

            }
        }

        private void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (DrawingContext dc = this.drawingGroup.Open()) // clear the drawing
            {
                // draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, screenWid, 480));

                if (isGameStart && healPoint > 0)
                {
                    MoveGhost();
                    DrawGhost(dc);
                }




                if (bullet.isInit)
                {
                    bullet.MoveBullet(moveDir);

                    if (bullet.getPosision().X < 0 || bullet.getPosision().Y < 0 || bullet.getPosision().X > screenWid || bullet.getPosision().Y > 480)
                    {
                        bullet.isDestroy = true;
                    }

                }

                using (SkeletonFrame frame = e.OpenSkeletonFrame())
                {
                    if (frame != null)
                    {
                        frame.CopySkeletonDataTo(skeletons);

                        // Add your code below 

                        // Find the closest skeleton 
                        Skeleton skeleton = GetPrimarySkeleton(skeletons);

                        if (skeleton == null) return;
                        //DrawSkeleton(skeleton, dc, Brushes.GreenYellow, new Pen(Brushes.DarkGreen, 6));
                        //
                        // new
                        //                       

                        if (isGameStart && restartGame(skeleton))
                        {
                            bulletNum = 6;

                        }

                        if (isGameStart && restartGame(skeleton) && healPoint <= 0)
                        {
                            Init();
                        }

                        if (isMatched(skeleton, reloadPose) && !isSun)
                        {
                            isSun = true;
                        }

                        String str = " " + bulletNum;
                        FormattedText ft = new FormattedText(str, System.Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 32, Brushes.White);

                        if (isGameStart)
                        {
                            // draw the hat
                            DrawHat(skeleton, dc);
                            // draw the body
                            DrawBody(skeleton, dc);
                        }

                        if (killNum >= 5)
                        {
                            isLevel[0] = false;
                            isLevel[1] = true;
                        }
                        else
                        {
                            isLevel[0] = true;
                            isLevel[1] = false;
                        }
                        MovingSun(dc, skeleton);

                        if (healPoint <= 0)
                        {
                            FormattedText ft2 = new FormattedText("Game Over, Your score: " + score + " Two hand together to restart", System.Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 16, Brushes.White);
                            dc.DrawText(ft2, new Point(screenWid / 2 - ft2.Width / 2, 480 / 2 - ft2.Height / 2));
                        }
                        if (bulletNum == 0 && healPoint > 0)
                        {
                            FormattedText ft2 = new FormattedText("Reload!!! Tow Hand Together!", System.Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 32, Brushes.Red);
                            dc.DrawText(ft2, new Point(screenWid / 2 - ft2.Width / 2, 480 / 2 - ft2.Height / 2));
                        }
                        if (isGameStart)
                        {
                            if (healPoint > 0)
                            {
                                dc.DrawText(ft, new Point(580, 365));

                                ShootBullet(skeleton, dc);
                            }
                            DrawGun(skeleton, dc);
                        }

                    }
                }
            }
        }

        private Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;

            if (skeletons != null)
            {
                //Find the closest skeleton       
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null) skeleton = skeletons[i];
                        else if (skeleton.Position.Z > skeletons[i].Position.Z)
                            skeleton = skeletons[i];
                    }
                }
            }

            return skeleton;
        }

        public int getNum(int[] arrNum, int tmp, int minValue, int maxValue, Random ra)
        {
            int n = 0;
            while (n <= arrNum.Length - 1)
            {
                if (arrNum[n] == tmp)
                {
                    tmp = ra.Next(minValue, maxValue);
                    getNum(arrNum, tmp, minValue, maxValue, ra);
                }
                n++;
            }
            return tmp;
        }
        // get random number in to a array
        public int[] getRandomNum(int num, int minValue, int maxValue)
        {
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
            int[] arrNum = new int[num];
            int tmp = 0;
            for (int i = 0; i <= num - 1; i++)
            {

                tmp = ra.Next(minValue, maxValue);
                arrNum[i] = getNum(arrNum, tmp, minValue, maxValue, ra);
            }
            return arrNum;
        }

        void DrawGhost(DrawingContext dc)
        {
            for (int i = 0; i < badGhost.Length; i++)
            {
                dc.DrawImage(badGhost[i], badGhostRect[i]);
            }
            for (int i = 0; i < goodGhost.Length; i++)
            {
                dc.DrawImage(goodGhost[i], goodGhostRect[i]);
            }


            dc.DrawImage(hP[healPoint], hpRect);


            dc.DrawImage(rightUI, bulletRect);

        }
        void MoveGhost()
        {
            int[] rd = getRandomNum(9, 0, 430);

            angle += 10;
            if (angle >= 180)
            {
                angle = 0;
            }

            for (int i = 0; i < badGhostRect.Length; i++)
            {
                if (badGhostRect[i].Contains(bullet.getPosision()))
                {
                    bullet.isDestroy = true;
                    score += 20;
                    rd3 = getRandomNum(5, 1, 7);
                    badGhostRect[i].X = -100;
                    badGhostRect[i].Y = rd[i];
                    killNum++;
                }
                if (isLevel[0])// level 1
                {
                    badGhostRect[i].X += rd2[i] / 2;
                }
                if (isLevel[1]) // level2 
                {
                    badGhostRect[i].X += 1 + rd2[i] / 2;
                    badGhostRect[i].Y += 50 * Math.Sin(angle);
                }
                if (badGhostRect[i].X > screenWid)
                {
                    if (healPoint > 0)
                    {
                        healPoint--;
                    }
                    rd2 = getRandomNum(4, 1, 6);
                    badGhostRect[i].X = -100;
                    badGhostRect[i].Y = rd[i];

                }
            }

            for (int i = 0; i < goodGhostRect.Length; i++)
            {
                if (goodGhostRect[i].Contains(bullet.getPosision()))
                {
                    bullet.isDestroy = true;
                    score -= 5;
                    rd3 = getRandomNum(5, 1, 7);
                    goodGhostRect[i].X = -100;
                    goodGhostRect[i].Y = rd[i + 4];
                    hurtEffect.Play();
                }

                goodGhostRect[i].X += rd3[i] / 2;
                if (goodGhostRect[i].X > screenWid)
                {

                    rd3 = getRandomNum(5, 1, 7);
                    goodGhostRect[i].X = -100;
                    goodGhostRect[i].Y = rd[i + 4];
                }
            }
        }

        private bool restartGame(Skeleton skeleton)
        {
            Point hLeft = SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandLeft].Position);
            Point hRight = SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandRight].Position);

            if (HitTest(hLeft, hRight, 20))
            {
                return true;
            }
            else
                return false;
        }

        private void DrawSkeleton(Skeleton skeleton, DrawingContext dc, Brush jointBrush, Pen bonePen)
        {
            for (int i = 0; i < bones.Length; i += 2)
                DrawBone(skeleton, dc, bones[i], bones[i + 1], bonePen);

            // Render joints
            foreach (Joint j in skeleton.Joints)
            {
                if (j.TrackingState == JointTrackingState.NotTracked) continue;

                dc.DrawEllipse(jointBrush, null, SkeletonPointToScreenPoint(j.Position), 5, 5);
            }
        }

        private void DrawBone(Skeleton skeleton, DrawingContext dc, JointType jointType0, JointType jointType1, Pen bonePen)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked) return;

            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred) return;

            //dc.DrawLine(new Pen(Brushes.Red, 5),
            dc.DrawLine(bonePen,
                SkeletonPointToScreenPoint(joint0.Position),
                SkeletonPointToScreenPoint(joint1.Position));
        }

        private Point SkeletonPointToScreenPoint(SkeletonPoint sp)
        {
            ColorImagePoint pt = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(
                sp, ColorImageFormat.RgbResolution640x480Fps30);
            return new Point(pt.X, pt.Y);
        }

        //
        // new
        //

        private BitmapImage LoadImages(String url)
        {
            BitmapImage img;
            img = new BitmapImage(new Uri(url, UriKind.Relative));

            return img;
        }

        private void MovingSun(DrawingContext dc, Skeleton skeleton)
        {
            dc.DrawImage(sun, sunR);


            if (sunR.Y > -85 && isSun && !isGameStart)
            {
                sunR.Y -= 4;
            }
            else if (sunR.Y <= -85 && !isGameStart)
            {
                isGameStart = true;
                soundPlayer.Play();
            }
            // Console.WriteLine(sunR.Y);
        }



        private void DrawHat(Skeleton skeleton, DrawingContext dc)
        {
            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                Point sLeft = SkeletonPointToScreenPoint(skeleton.Joints[JointType.ShoulderLeft].Position);
                Point sRight = SkeletonPointToScreenPoint(skeleton.Joints[JointType.ShoulderRight].Position);
                Point head = SkeletonPointToScreenPoint(skeleton.Joints[JointType.Head].Position);

                headPoint = head;

                double w = Math.Abs(sLeft.X - sRight.X);
                double h = w * hat.Height / hat.Width;
                double x = head.X - w / 2;
                double y = head.Y - 1.5 * h;
                hatWid = w;
                Rect r = new Rect(x, y, w, h);
                dc.DrawImage(hat, r);
            }
        }



        private void DrawBody(Skeleton skeleton, DrawingContext dc)
        {
            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                Point sLeft = SkeletonPointToScreenPoint(skeleton.Joints[JointType.ShoulderLeft].Position);
                Point sRight = SkeletonPointToScreenPoint(skeleton.Joints[JointType.ShoulderRight].Position);
                //Point head = SkeletonPointToScreenPoint(skeleton.Joints[JointType.Head].Position);
                

                double w = Math.Abs(sLeft.X - sRight.X) * 45 / 20;
                double x = sLeft.X - w * 4 / 45;
                double y = sLeft.Y - w * 11 / 45;

                Rect r = new Rect(x, y, w, w);
                dc.DrawImage(body, r);
            }
        }


        private void DrawGun(Skeleton skeleton, DrawingContext dc)
        {
            Point pt1 = SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandLeft].Position);
            Point pt2 = SkeletonPointToScreenPoint(skeleton.Joints[JointType.WristLeft].Position);
            Point rightHand = SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandRight].Position);
            Point elbowLeft = SkeletonPointToScreenPoint(skeleton.Joints[JointType.ElbowLeft].Position);

            double w = hatWid;
            double x = pt1.X - w * 3 / 4;
            double y = pt1.Y - w * 1 / 4;

            gunR = new Rect(x, y, w, w / 2);

            float angle = GetAngle(skeleton, JointType.HandLeft, JointType.WristLeft);
            // Rotate
            dc.PushTransform(new RotateTransform(angle, pt1.X, pt1.Y));
            dc.DrawImage(gun, gunR);


        }

        private void shootInternal(object source, ElapsedEventArgs e)
        {
            isShoot = false;
        }

        private void ReloadPose()
        {
            reloadPose.Title = "reloadPose-pose";

            PoseAngle[] angles = new PoseAngle[2];
            angles[0] = new PoseAngle(JointType.ShoulderRight,
                         JointType.ElbowRight, 305, 20);
            angles[1] = new PoseAngle(JointType.ElbowRight,
                         JointType.WristRight, 270, 30);

            reloadPose.Angles = angles;
        }

        private void ShootPose()
        {
            shootPose.Title = "Shoot-pose";

            PoseAngle[] angles = new PoseAngle[2];
            angles[0] = new PoseAngle(JointType.ShoulderRight,
                         JointType.ElbowRight, 0, 20);
            angles[1] = new PoseAngle(JointType.ElbowRight,
                         JointType.WristRight, 270, 30);

            shootPose.Angles = angles;
        }

        private void ShootBullet(Skeleton skeleton, DrawingContext dc)
        {
            Point pt1 = SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandLeft].Position);
            Point pt2 = SkeletonPointToScreenPoint(skeleton.Joints[JointType.WristLeft].Position);
            Point rightHand = SkeletonPointToScreenPoint(skeleton.Joints[JointType.HandRight].Position);
            Point elbowLeft = SkeletonPointToScreenPoint(skeleton.Joints[JointType.ElbowLeft].Position);

            if (isMatched(skeleton, shootPose))//HitTest(rightHand, elbowLeft, 50))
            {
                shoot = true;
            }
            else
            {
                shoot = false;
            }


            if (shoot && !isShoot && bulletNum > 0)
            {
                isShoot = true;
                shoot = false;

                double w = hatWid;

                moveDir = new Vector(pt1.X - pt2.X, pt1.Y - pt2.Y);
                moveDir.Normalize();

                bullet = new Bullet(new Point(pt1.X - w * 3 / 4, pt1.Y - w * 1 / 4));

                shootEffect.Play();

                bulletNum--;

                Console.WriteLine(bullet.returnNumber().ToString());
            }
            else if (bullet.isInit && bullet.isDestroy)
            {
                bullet = new Bullet();
            }

            if (bullet.isInit)
            {
                bullet.DrawBullet(dc, bulletImg);
            }
        }


        private bool HitTest(Point pt1, Point pt2, double radious)
        {
            if ((pt1.Y - pt2.Y) * (pt1.Y - pt2.Y) + (pt2.X - pt1.X) * (pt2.X - pt1.X) < radious * radious)
                return true;
            else
                return false;
        }

        // 
        // Angle part
        //

        Boolean isMatched(Skeleton skeleton, Pose pose)
        {
            for (int i = 0; i < pose.Angles.Length; i++)
            {

                if (AngularDifference(GetAngle(skeleton, pose.Angles[i].StartJoint, pose.Angles[i].EndJoint), pose.Angles[i].Angle) > pose.Angles[i].Threshold)
                    return false;
            }
            return true;
        }

        float AngularDifference(float a1, float a2)
        {
            float abs_diff = Math.Abs(a1 - a2);
            return Math.Min(abs_diff, 360 - abs_diff);
        }

        private float GetAngle(Skeleton s, JointType js, JointType je)
        {
            Point sp = SkeletonPointToScreenPoint(s.Joints[js].Position);
            Point ep = SkeletonPointToScreenPoint(s.Joints[je].Position);

            float angle = (float)(
                Math.Atan2(ep.Y - sp.Y, ep.X - sp.X) * 180 / Math.PI);

            angle = (angle + 360) % 360;

            return angle;
        }


    }
}