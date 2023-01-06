using System.Drawing;
using System.Windows.Forms;

namespace SimpleBmp
{
    public class Bmp2Viewer
    {
        private Bmp2? currentBmp;
        private Form f;
        private bool showed = false;

        FlowLayoutPanel? fpanel;
        int taillePanel = 10;

        /// <summary>
        /// Constructeur du viewer
        /// </summary>
        public Bmp2Viewer()
        {
            f = new Form();
            fpanel = new FlowLayoutPanel();
        }

        /// <summary>
        /// (Re) initialise la fenêtre et le flowLayoutPanel
        /// </summary>
        /// <param name="b"></param>
        private void InitDraw(Bmp2 b)
        {
            currentBmp = b;
            f.Size = new Size(b.Width * (taillePanel + 7), b.Height * (taillePanel + 8));
            fpanel.Size = new Size(b.Width * (taillePanel + 6), b.Height * (taillePanel + 6));

            f.Controls.Clear();
            f.Controls.Add(fpanel);

            if (!showed)
            {
                f.Show();
                showed = true;
            }
        }

        /// <summary>
        /// Dessinne les pixels du BMP2 dans les panels du viewer
        /// </summary>
        /// <param name="b"></param>
        public void Draw(Bmp2 b)
        {
            InitDraw(b);

            Color PaletteToColor(int idPalette)
            {
                byte[] data = currentBmp.Palette[idPalette];
                return Color.FromArgb(data[3], data[0], data[1], data[2]);
            }

            for (int i = 0; i < b.Width * b.Height; i++)
            {
                Panel p = new Panel();
                p.Size = new Size(taillePanel, taillePanel);
                fpanel.Controls.Add(p);
            }

            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    int idx = b.Width * i + j;
                    fpanel.Controls[idx].BackColor = PaletteToColor(b.Data[idx]);
                }
            }
        }


        /// <summary>
        /// Ferme la fenêtre de visualisation
        /// </summary>
        public void Close()
        {
            f.Close();
        }

    }

}
