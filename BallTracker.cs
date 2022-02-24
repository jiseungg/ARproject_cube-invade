using Intel.RealSense;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenCVForUnityExample
{
    
    /// <summary>
    /// WebCamTextureToMat Example
    /// An example of converting a WebCamTexture image to OpenCV's Mat format.
    /// </summary>
    public class BallTracker : MonoBehaviour
    {
        public RsDevice rsDevice;
        public RawImage resultTexture;
        public RawImage final;
        public List<GameObject> balls;
        public GameObject ballParents;

        float[,] data = new float[480,848];
        int[,,] c_data = new int[480,640,3];

        // public 으로 돌려서 min,max depth값 수동 조정 가능하게 할 수 있음
        // isInitMaxMinDepth 변수 삭제
        float min_depth = 0, max_depth = 2;
        Align align = new Align(Stream.Color);

        /// <summary>
        /// The rgba mat.
        /// </summary>
        Mat rgbaMat;

        /// <summary>
        /// The colors.
        /// </summary>
        Color32[] colors;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;
        Texture2D f_texture;

        SegmentCanavas segmentCanavas;
        int[] xRange = new int[2];
        int[] yRange = new int[2];
        int sqWidth, sqHeight;
        bool isCanvasUpdated = false;

        void OnNewSample(Frame frame)
        {
            using (var frameSet = frame.AsFrameSet())
            {
                var frames = align.Process(frameSet).AsFrameSet();
                using (var depthFrame = frames.DepthFrame as DepthFrame)
                {
                    if (depthFrame != null)
                    {
                        for (int i = 0; i < depthFrame.Width; i++)
                        {
                            for (int j = 0; j < depthFrame.Height; j++)
                            {
                                data[j,i] = depthFrame.GetDistance(i,j);
                            }
                        }
                        for (int i = 0; i < depthFrame.Width; i++)
                        {
                            for (int j = 0; j < depthFrame.Height; j++)
                            {
                                data[j,i] = depthFrame.GetDistance(i,j);
                            }
                        }
                    }
                }
            }
        }

        float Normalize(float d)
        {
            float ret = d / max_depth;
            return ret;
        }

        byte Mapping(float d)
        {
            byte val = (byte)(d * 255);
            return val;
        }

        // Use this for initialization
        void Start()
        {
            balls = new List<GameObject>();
            for(int i = 0; i < 50; i++)
            {
                balls.Add(Instantiate(GameObject.Find("ball"), Vector3.zero, Quaternion.Euler(Vector3.zero)));
            }
            texture = (Texture2D)resultTexture.mainTexture;
            f_texture = (Texture2D)final.mainTexture;
            rsDevice.OnNewSample += OnNewSample;
            segmentCanavas = GameObject.Find("CanvasSegment").GetComponent<SegmentCanavas>();
        }
        void Update()
        {
            if(!isCanvasUpdated && segmentCanavas.setupcount > 150)
            {
                xRange[0] = (int)(segmentCanavas.Canvas[0].x < segmentCanavas.Canvas[1].x ? segmentCanavas.Canvas[0].x : segmentCanavas.Canvas[1].x);  
                xRange[1] = (int)(segmentCanavas.Canvas[0].x > segmentCanavas.Canvas[1].x ? segmentCanavas.Canvas[0].x : segmentCanavas.Canvas[1].x);  
                yRange[0] = (int)(segmentCanavas.Canvas[0].y < segmentCanavas.Canvas[1].y ? segmentCanavas.Canvas[0].y : segmentCanavas.Canvas[1].y);  
                yRange[1] = (int)(segmentCanavas.Canvas[0].y > segmentCanavas.Canvas[1].y ? segmentCanavas.Canvas[0].y : segmentCanavas.Canvas[1].y);
                sqWidth = xRange[1] - xRange[0];
                sqHeight = yRange[1] - yRange[0];
                // Debug.Log("Width: " + sqWidth.ToString());
                // Debug.Log("Height: " + sqHeight.ToString());
                // Debug.Log("x range: " + xRange[0].ToString() + " // " + xRange[1].ToString());
                // Debug.Log("y range: " + yRange[0].ToString() + " // " + yRange[1].ToString());
                isCanvasUpdated = true;
            }
            if(isCanvasUpdated && texture != null)
            {  
                // Making Original Matrix: RealSense 카메라로 들어오는 원본 RGBA Mat 생성.
                rgbaMat = new Mat(new Size(640,480), CvType.CV_8UC3);
                Utils.texture2DToMat((Texture2D)resultTexture.mainTexture,rgbaMat,false);
                
                // Approx Poly && Contour Related Functions
                // RGBA Mat을 HSV Mat으로 색변환(cvtColor)
                Mat rgbMat = new Mat(rgbaMat.size(), CvType.CV_8UC3);
                Imgproc.cvtColor(rgbaMat,rgbMat,Imgproc.COLOR_RGB2HSV);

                // 파란색 공과 관련된 정보만 담을 blue Mat 생성
                Mat blue = new Mat(rgbaMat.size(), CvType.CV_8UC3);
                Mat dst = rgbaMat.clone();

                //MatofPoint - Point 객체들로 이루어진 Mat(행렬)
                // contours 리스트는 Point객체들이 묶인 MatOfPoint들의 리스트 == 윤곽선들의 리스트
                List<MatOfPoint> contours = new List<MatOfPoint>();
                Mat hierarchy = new Mat();
                
                // Scalar BlueHSVmin = new Scalar(100, 140, 0);
                // Scalar BlueHSVmax = new Scalar(140, 255, 255);
                // blue Mat에 HSV Mat에서 BlueHSVmin 과 BlueHSVmax 사이에 있는 색상값만 추출하여 blue에 저장
                Core.inRange(rgbMat, new Scalar(80,140,100), new Scalar(140,255,255), blue);
                // findContours 함수를 활용하여 색의 윤곽선을 contours 리스트에 저장.
                Imgproc.findContours(blue, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);

                List<MatOfPoint> new_contours = new List<MatOfPoint>();
                
                foreach (MatOfPoint c in contours)
                {
                    //MatOfPoint를 float들로 형변환
                    MatOfPoint2f c_2f = new MatOfPoint2f(c.toArray());
                    //MatOfPoint2f(윤곽선)의 길이 및 영역을 계산
                    double length = Imgproc.arcLength(c_2f,true);
                    // double area = Imgproc.contourArea(c_2f,true);
                    MatOfPoint2f approx = new MatOfPoint2f();
                    if(length > 50)
                    {
                        // 중심점과 반경을 저장할 변수들을 생성
                        Point center = new Point();
                        float[] radius = new float[1];
                        Imgproc.minEnclosingCircle(c_2f,center,radius);
                        // data행렬에서 Center 위치의 Depth값 추출

                        // 화면 아래쪽을 bottomline에 연동시키기 위해 인식 범위 재설정함 
                        // if(xRange[0] <= (int)(center.x) && xRange[1] >= (int)(center.x) && yRange[0] <= (int)(center.y) && yRange[1] >= (int)(center.y)){
                        if(xRange[0] <= (int)(center.x) && xRange[1] >= (int)(center.x) && yRange[0] <= (int)(center.y)){
                            float z_pos = data[(int)center.y, (int)center.x];
                            Imgproc.circle(dst,center,(int)radius[0],new Scalar(255,255,0),2);
                            Imgproc.circle(dst,center, 2, new Scalar(100,100,255), 5);
                            Imgproc.putText(dst,z_pos.ToString(), center, 1, 5, new Scalar(100,100,255),5);
                            // Debug.Log("centerx: " + center.x.ToString() + " centery " + center.y.ToString());
                            Imgproc.approxPolyDP(c_2f, approx, length * 0.01, true);
                            new_contours.Add(new MatOfPoint(approx.toArray()));
                        }   
                    }
                }

                //dst(목표 매트릭스)에 새로운 윤곽선 그리기
                Imgproc.drawContours(dst, new_contours, -1, new Scalar(255,0,0), 2);
                
                //ADD-END
                if(new_contours.Count < 50)
                {
                    int cnt = 0;
                    foreach(MatOfPoint c in new_contours)
                    {
                        MatOfPoint2f c_2f = new MatOfPoint2f(c.toArray());    
                        Point center = new Point();
                        float[] radius = new float[1];
                        Imgproc.minEnclosingCircle(c_2f,center,radius);
                        float x_pos = (float)((center.x - xRange[0]) * 40 / sqWidth - 20f);
                        float y_pos = (float)(20 - (24 * (center.y - yRange[0]) / sqHeight)); 
                        float z_pos = data[(int)center.y, (int)center.x] * 6f;
                        //Debug.Log("x_pos: " + x_pos.ToString() + "y_pos: " + y_pos.ToString() + "z_pos" + z_pos.ToString());
                        balls[cnt++].transform.position = new Vector3(x_pos, y_pos, z_pos);
                    }
                    for(int i = cnt; i < balls.Count; i++)
                    {
                        balls[i].transform.position = Vector3.zero;
                    }
                }


                // 만약 공이 맞자마자 바로 비활성화 되고 나서 배열의 가장 뒷자리로 가게 된다면
                // 떨어지는 동안은 다른 인덱스가 되게 될 테고 이는 안되는 구조다
                // 그러니까 맞게 되면 이 공에 대한 충돌 판정을 특정 몇초간만 꺼버리고
                // 어느정도 내려가게 되면 이를 뒤로 빼는 것이 올바른 구조라고 할 수 있겠다.

                // 그러면 내가 만들어야 할 것은 충돌했을때 일정 시간동안 이를 없애버리는 구조이므로
                // 기존 공에도 충돌 관련 스크립트를 넣어서 일정 시간동안 콜라이더 속성을 꺼버리는 스크립트와
                // 본인이 일정 거리 이하로 내려갔을 시에 본인의 인덱스를 가장 뒤로 밀어버리는 코드 두가지가 필요하겠다

                // 다행히도 이는 충돌되는 공(우리가 던지는 공)에다가 한번에 끼워넣으면 되는 구조이므로
                // 외부 스크립트 하나만 끼워주면 될 거같은데
                // 다들 어떻게 생각하시는지 궁금합니다.

                // 일단 여기서 스크립트를 짜는건 아닌거 같으니까 다른 코드로 들어가 보겠습니다.
                //UPDATE: Junhyung-Choi
                Utils.matToTexture2D(dst, f_texture, colors);
                //Original Code: Utils.matToTexture2D(rgbaMat, texture, colors);
            }
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
    }
}
