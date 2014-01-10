using System;
using System.Drawing;
using System.Web;
using System.Drawing.Drawing2D;

namespace Com.BaseLibrary.Web
{
	public class ValidationCodeUtility
	{
		public void GenerateGoogleStyleValidateCodeImage(string sesstionKey)
		{
			HttpContext httpContext = System.Web.HttpContext.Current;
			string code = CreateVerifyCode();
			httpContext.Session[sesstionKey] = code.ToLower();//ȡ�����
			CreateImageOnPage(code, httpContext);        // ���ͼƬ

		}
		#region ��֤�볤��(Ĭ��4����֤��ĳ���)
		int length = 4;
		public int Length
		{
			get { return length; }
			set { length = value; }
		}
		#endregion

		#region ��֤�������С(Ϊ����ʾŤ��Ч����Ĭ��40���أ����������޸�)
		int fontSize = 14;
		public int FontSize
		{
			get { return fontSize; }
			set { fontSize = value; }
		}
		#endregion

		#region �߿�(Ĭ��1����)
		int padding = 6;
		public int Padding
		{
			get { return padding; }
			set { padding = value; }
		}
		#endregion

		#region �Ƿ�������(Ĭ�ϲ����)
		bool chaos = true;
		public bool Chaos
		{
			get { return chaos; }
			set { chaos = value; }
		}
		#endregion

		#region ���������ɫ(Ĭ�ϻ�ɫ)
		Color chaosColor = Color.LightGray;
		public Color ChaosColor
		{
			get { return chaosColor; }
			set { chaosColor = value; }
		}
		#endregion

		#region �Զ��屳��ɫ(Ĭ�ϰ�ɫ)
		Color backgroundColor = Color.White;
		public Color BackgroundColor
		{
			get { return backgroundColor; }
			set { backgroundColor = value; }
		}
		#endregion

		#region �Զ��������ɫ����
		Color[] colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
		public Color[] Colors
		{
			get { return colors; }
			set { colors = value; }
		}
		#endregion

		#region �Զ�����������
		string[] fonts = { "Arial", "Georgia" };
		public string[] Fonts
		{
			get { return fonts; }
			set { fonts = value; }
		}
		#endregion

		#region �Զ���������ַ�������(ʹ�ö��ŷָ�)
		string codeSerial = "2,3,4,5,6,7,9,a,b,c,d,e,f,g,h,j,k,m,n,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z";
		public string CodeSerial
		{
			get { return codeSerial; }
			set { codeSerial = value; }
		}
		#endregion

		#region ���������˾�Ч��

        //private const double PI = 0;
        //private const double PI2 = -1;
		private const double PI = 3.1415926535897932384626433832795;
		private const double PI2 = -3.1415926535897932384626433832795;

		/// <summary>
		/// ��������WaveŤ��ͼƬ��Edit By 51aspx.com��
		/// </summary>
		/// <param name="srcBmp">ͼƬ·��</param>
		/// <param name="bXDir">���Ť����ѡ��ΪTrue</param>
		/// <param name="nMultValue">���εķ��ȱ�����Խ��Ť���ĳ̶�Խ�ߣ�һ��Ϊ3</param>
		/// <param name="dPhase">���ε���ʼ��λ��ȡֵ����[0-2*PI)</param>
		/// <returns></returns>
		public System.Drawing.Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
		{
			System.Drawing.Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);

			// ��λͼ�������Ϊ��ɫ
			System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(destBmp);
			graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, destBmp.Width + 2, destBmp.Height + 2);
			graph.Dispose();

			double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;

			for (int i = 0; i < destBmp.Width; i++)
			{
				for (int j = 0; j < destBmp.Height; j++)
				{
					double dx = 0;
					dx = bXDir ? (PI2 * (double)j) / dBaseAxisLen : (PI2 * (double)i) / dBaseAxisLen;
					dx += dPhase;
					double dy = Math.Sin(dx);

					// ȡ�õ�ǰ�����ɫ
					int nOldX = 0, nOldY = 0;
					nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
					nOldY = bXDir ? j : j + (int)(dy * dMultValue);

					System.Drawing.Color color = srcBmp.GetPixel(i, j);
					if (nOldX >= 0 && nOldX < destBmp.Width
					 && nOldY >= 0 && nOldY < destBmp.Height)
					{
						destBmp.SetPixel(nOldX, nOldY, color);
					}
				}
			}

			return destBmp;
		}



		#endregion

		#region ����У����ͼƬ
		public Bitmap CreateImageCode(string code)
		{
			int fSize = FontSize;
			int fWidth = fSize + Padding;

			int imageWidth = (int)(code.Length * fWidth) + 4 + Padding * 2;
			int imageHeight = fSize * 2 + Padding;

			System.Drawing.Bitmap image = new System.Drawing.Bitmap(imageWidth, imageHeight);

			Graphics g = Graphics.FromImage(image);

			g.Clear(BackgroundColor);

			Random rand = new Random();

			//���������������ɵ����
			if (this.Chaos)
			{

				Pen pen = new Pen(ChaosColor, 0);
				int c = Length * 10;

				for (int i = 0; i < c; i++)
				{
					int x = rand.Next(image.Width);
					int y = rand.Next(image.Height);

					g.DrawRectangle(pen, x, y, 1, 1);
				}
			}

			int left = 0, top = 0, top1 = 1, top2 = 1;

			int n1 = (imageHeight - FontSize - Padding * 2);
			int n2 = n1 / 4;
			top1 = n2;
			top2 = n2 * 2;

			Font f;
			Brush b;

			int cindex, findex;

			//����������ɫ����֤���ַ�
			for (int i = 0; i < code.Length; i++)
			{
				cindex = rand.Next(Colors.Length - 1);
				findex = rand.Next(Fonts.Length - 1);

				f = new System.Drawing.Font(Fonts[findex], fSize, System.Drawing.FontStyle.Strikeout);
				b = new System.Drawing.SolidBrush(Colors[cindex]);

				if (i % 2 == 1)
				{
					top = top2;
				}
				else
				{
					top = top1;
				}

				left = i * fWidth + 6;

				g.DrawString(code.Substring(i, 1), f, b, left, top);
			}

			//��һ���߿� �߿���ɫΪColor.Gainsboro
			g.DrawRectangle(new Pen(Color.Gainsboro, 0), 0, 0, image.Width - 1, image.Height - 1);
			g.Dispose();

			//�������Σ�Add By 51aspx.com��
			image = TwistImage(image, true, 8, 4);

			return image;
		}
		#endregion

		#region �������õ�ͼƬ�����ҳ��
		public void CreateImageOnPage(string code, HttpContext context)
		{
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			Bitmap image = this.CreateImageCode(code);

			image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

			context.Response.ClearContent();
			context.Response.ContentType = "image/Jpeg";
			context.Response.BinaryWrite(ms.GetBuffer());

			ms.Close();
			ms = null;
			image.Dispose();
			image = null;
		}
		#endregion

		#region ��������ַ���
		public string CreateVerifyCode(int codeLen)
		{
			if (codeLen == 0)
			{
				codeLen = Length;
			}

			string[] arr = CodeSerial.Split(',');

			string code = "";

			int randValue = -1;

			Random rand = new Random(unchecked((int)DateTime.Now.Ticks));

			for (int i = 0; i < codeLen; i++)
			{
				randValue = rand.Next(0, arr.Length - 1);

				code += arr[randValue];
			}

			return code;
		}
		public string CreateVerifyCode()
		{
			return CreateVerifyCode(0);
		}
		#endregion


        public Bitmap CreateCheckCodeImage(string checkCode)
        {
            var bitmap = new Bitmap(checkCode.Length+100, 40);
            Graphics g = Graphics.FromImage(bitmap);
            int fSize = FontSize;
            int fWidth = fSize + Padding;

            int imageWidth = (int)(checkCode.Length * fWidth) + 4 + Padding * 2;
            int imageHeight = fSize * 2 + Padding;
            try
            {
                var random = new Random();
                g.Clear(Color.White);
                
                
                //����������ɫ����֤���ַ�
                int left = 0, top = 0, top1 = 1, top2 = 1;

                int n1 = (imageHeight - FontSize - Padding * 2);
                int n2 = n1 / 4;
                top1 = n2;
                top2 = n2 * 2;
                for (int i = 0; i < checkCode.Length; i++)
                {
                    Point[] points = new Point[2];
                    points[0] = new Point(random.Next(0, imageWidth), random.Next(0, imageHeight));
                    points[1] = new Point(random.Next(0, imageHeight), random.Next(0, imageWidth));
                    g.DrawLines(new Pen(Color.Coral, 3), points);
                }
                for (int i = 0; i < checkCode.Length; i++)
                {

                    var brush = new LinearGradientBrush(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                    Colors[random.Next(Colors.Length - 1)], Colors[random.Next(Colors.Length - 1)], 1.2f, true);
                    var font = new Font(Fonts[random.Next(Fonts.Length - 1)], 20, FontStyle.Bold);
                    if (i % 2 == 1)
                    {
                        top = top2;
                    }
                    else
                    {
                        top = top1;
                    }

                    left = i * fWidth + 6;
                    g.DrawString(checkCode.Substring(i, 1), font, brush, left, top);

                }
                

                //��ͼƬ��ǰ��������
                for (int i = 0; i < 300; i++)
                {
                    int x = random.Next(bitmap.Width);
                    int y = random.Next(bitmap.Height);
                    bitmap.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //��ͼƬ�ı߿���
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, bitmap.Width - 1, bitmap.Height - 1);
                Bitmap bt = TwistImage(bitmap, true, 10, 0);
                return bt;
            }
            finally
            {
                g.Dispose();
                bitmap.Dispose();
            }
        }

	}
}
