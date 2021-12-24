using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace histogram_sikistirma
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            chart1.Series.Clear();
            chart2.Series.Clear();
            chart2.Series.Add("Pixel");
            chart1.Series.Add("Pixel");
        }

        private void dosyaAcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Resim Dosyası |*.jpg";
            if (file.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(file.FileName);
                if (fi.Exists)
                {
                    string DosyaYolu = file.FileName;
                    image1.Image = new Bitmap(DosyaYolu);
                }
                else 
                {
                    //Hata
                }
            }
        }

        private void donustur_Click(object sender, EventArgs e)
        {
            //hedef resim bitmap olarak resim1 değişkeni içerisine atılır.
            Bitmap resim1 = (Bitmap)image1.Image;

            //eğer resim1 seçilmemiş yüklenmemişse hata verdir.
            if (resim1 == null)
            {
                MessageBox.Show("Lütfen resmi seçtiğinizden emin olunuz!", "Resim Bulanamadı",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //aynı boyutta histogram sıkıştırma sonucunu çizdirmek adına boş bir bitmap resmi açılır. Aynı boyutta fakat boştur.
            Bitmap resim2 = new Bitmap(resim1.Width, resim1.Height);

            //tüm pixeller gezilmek adına toplam pixel sayısı alınır
            double pixelsayisi = resim1.Width * resim1.Height;

            //histogram bir grafiktir. Her bir pixelin 0-255 aralığındaki renklerini almak için 255 boyutunda bir histogram dizisi oluşturulur.
            //0-255 aralığında her bir pixellin rengi sayılmak için
            double[] hist = new double[256];

            double[] c = new double[256];

            //tüm pixeller gezilir. ve histogram elde edilir.
            for (int i = 0; i < resim1.Width; i++)
            {
                for (int j = 0; j < resim1.Height; j++)
                {
                    //pixelin rengi alınır.
                    Color renk = resim1.GetPixel(i, j);
                    //Red green ve blue değerlerinin ortalaması alınır. her bir renk 0-255 arasında değer alır.
                    //Tüm renkleri toplar 3 e bölerek, yine 0-255 aralığında bir yere sığdırılır
                    int ort = (renk.R + renk.B + renk.G) / 3;
                    // o rengin sayısı 1 arttırılır. Amaç hangi renkte ne kadar pixel var onu saymaktır.
                    hist[ort] = hist[ort] + 1;
                }
            }
            // yukarıdaki iki for döngüleri bitince histogram elde edilmiş olur.


            //chart1 yani grafik 1 birinci resmin histogram sıkıştırma yapılmamış remin renk uzayındaki grafiğini çizer
            for (int l = 0; l < 256; l++)
            {
                chart1.Series["Pixel"].Points.AddXY(l, hist[l]);
            }

            //normalizasyon adına tüm c değerleri toplam pixel sayısına bölünür. Amaç normalize edilmiş renklerdir
            for (int i = 0; i < 255; i++)
            {
                c[i] = hist[i] / pixelsayisi;
            }

            for (int i = 0; i < 256; i++)
            {
                if (i == 0)
                {
                    //normalizasyon için istenilen maksimum renk aralığı ile çarpıyoruz. Normalizasyonun son adımıdır.
                    c[i] = Math.Floor(c[i] * 255);
                }
                else
                {
                    // 2 . adım kümülatif histogram kendinden önceki pixel ile renkleri ile toplanır. Sıkıştırma yapılır.
                    c[i] = Math.Floor(c[i] * 255 + c[i - 1]);
                }
            }



            //elde edilen normalizasyon uygulanmış kümülatif histogramı boş tuvalimiz olan yukarıda resim2 olarak tanımlanan bitmape çiziyoruz.
            for (int i = 0; i < resim1.Width; i++)
            {
                for (int j = 0; j < resim1.Height; j++)
                {
                    //resim 1 deki renk alınır. Red green blue değerleri
                    Color renk = resim1.GetPixel(i, j);
                    int r = renk.R;
                    int g = renk.G;
                    int b = renk.B;

                    //resim2 ye çizmek için setPixel yani i satırı j sütunundaki pixel içerisine renk uzayı ile birlikte renkler kümülatif histogramdan
                    //alınarak o pixele 0-255 aralığındaki gri tonu yüklenir.
                    resim2.SetPixel(i, j, Color.FromArgb(Convert.ToInt32(c[r] * 255 / 256), Convert.ToInt32(c[g] * 255 / 256), Convert.ToInt32(c[b] * 255 / 256)));

                }
            }
            image2.Image = resim2;

            //Aşağıda yeni sıkıştırma yapılmış resmin histogramı yeniden çıkarılır

            int[] hist2 = new int[255];
            for (int i = 0; i < resim2.Width; i++)
            {
                for (int j = 0; j < resim2.Height; j++)
                {
                    Color renk = resim2.GetPixel(i, j);
                    int deger = (renk.R + renk.B + renk.G) / 3;
                    hist2[deger] = hist2[deger] + 1;
                }
            }

            //chart 2 yani ikinci grafikte yeni resmin histogramı çizdirilir. Histogram zaten bir grafiktir.
            for (int i = 0; i < 255; i++)
            {
                chart2.Series["Pixel"].Points.AddXY(i, hist2[i]);
            }

        }

        private void createdByAybalaCambazToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}