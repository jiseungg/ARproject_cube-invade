using Intel.RealSense;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenCVForUnityExample
{
    public class SegmentCanavas : MonoBehaviour
    {
        public RsDevice rsDevice;
        float max_depth = 2;
        Mat rgbaMat, rgbMat, dst;
        Color32[] colors;
        Texture2D texture, rsTexture;
        public float min_binary;
        public int setupcount = 0;
        public List<Point> Canvas, rec;
        
        // Use this for initialization
        void Start()
        {
            texture = (Texture2D)this.transform.GetComponent<RawImage>().mainTexture;
            rsTexture = (Texture2D)GameObject.Find("RawImage").GetComponent<RawImage>().mainTexture;
            rgbaMat = new Mat(new Size(640,480), CvType.CV_8UC3);
            rgbMat = new Mat(rgbaMat.size(), CvType.CV_8UC3);
            Canvas = new List<Point>();
        }
        void Update()
        {
            if(texture != null && setupcount < 150)
            {
                rsTexture = (Texture2D)GameObject.Find("RawImage").GetComponent<RawImage>().mainTexture;
                rec = new List<Point>();
                // Making Original Matrix: RealSense 카메라로 들어오는 원본 RGBA Mat 생성.
                Utils.texture2DToMat(rsTexture, rgbaMat, false);
                rgbMat = new Mat(rgbaMat.size(), CvType.CV_8UC3);
                //this.transform.GetComponent<RawImage>().mainTexture = rsTexture;
                Debug.Log(rsTexture.name);
                Imgproc.cvtColor(rgbaMat,rgbMat,Imgproc.COLOR_RGB2HSV);
                dst = rgbaMat.clone();
                
                Mat screen = new Mat(rgbaMat.size(), CvType.CV_8UC3);
                Mat binary = screen.clone();
                screen = rgbaMat.clone();
                
                Core.inRange(rgbMat, new Scalar(0, 100, 20), new Scalar (10, 255, 255), binary);

                List<MatOfPoint> contours = new List<MatOfPoint>();

                Mat hierarchy = new Mat();

                Imgproc.findContours(binary, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);

                
                List<MatOfPoint> new_contours = new List<MatOfPoint>();
                
                // Core.flip(screen,screen,0);
                foreach (MatOfPoint c in contours)
                {
                    //MatOfPoint를 float들로 형변환
                    MatOfPoint2f c_2f = new MatOfPoint2f(c.toArray());
                    //MatOfPoint2f(윤곽선)의 길이 및 영역을 계산
                    double length = Imgproc.arcLength(c_2f, true);
                    double area = Imgproc.contourArea(c_2f,true);
                    // houghline
                    MatOfPoint2f approx = new MatOfPoint2f();
                    if(length > 20)
                    {
                        // 중심점과 반경을 저장할 변수들을 생성
                        Point center = new Point();
                        float[] radius = new float[1];
                        // Debug.Log(c_2f.depth());
                        Imgproc.minEnclosingCircle(c_2f,center,radius);
                        rec.Add(center);
                    }
                }
                if (rec.Count >= 2)
                {
                    Imgproc.rectangle(screen, rec[0], rec[1], new Scalar(255, 0, 0), 5);
                    Imgproc.rectangle(binary, rec[0], rec[1], new Scalar(255, 0, 0), 5);
                }

                Utils.matToTexture2D(binary, texture, colors);
                setupcount++;
            }
            if(setupcount == 150)
            {
                Canvas = rec;
                setupcount++;   
            }
        }
    }
}