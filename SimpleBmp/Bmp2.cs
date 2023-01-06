using System.Drawing;

namespace SimpleBmp
{
    public class Bmp2
    {
        //entetes

        /// <summary>
        /// Nombre magique placé en début d'entête du fichier généré.
        /// Correspond en texte à "Bmp2"
        /// </summary>
        public int MagicNumber { get { return 846228802; } }
        
        /// <summary>
        /// Largeur de l'image
        /// </summary>
        public short Width { get; private set; }
        
        /// <summary>
        /// Hauteur de l'image
        /// </summary>
        public short Height { get; private set; }
        
        /// <summary>
        /// Nombre d'octet 
        /// Par défaut 1
        /// </summary>
        public short BytesPerPixel { get; private set; }
        
        /// <summary>
        /// Nombre d'octets utilisés pour définir les couleurs avec canaux R, G, B, A
        /// Par défaut 4
        /// </summary>
        public short BytesPerColor { get; private set; }
        
        /// <summary>
        /// Nombre de couleurs définies dans la palette actuelle
        /// </summary>
        public int PaletteCount { get { return Palette.Count; } }

        /// <summary>
        /// Palette définie (données brutes)
        /// Chaque tableau de byte contient les canaux R G B A
        /// </summary>
        public List<byte[]> Palette = new List<byte[]>();
        
        /// <summary>
        /// Grille de pixel (données brutes)
        /// </summary>
        public byte[] Data = new byte[1];

        /// <summary>
        /// Constructeur
        /// </summary>
        public Bmp2()
        {
            Init();
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public Bmp2(short width, short height)
        {
            Init(width, height);
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public Bmp2(short width, short height, short BytesPerPixel, short BytesPerColor)
        {
            Init(width, height, BytesPerPixel, BytesPerColor);
        }

        /// <summary>
        /// Constructeur complet
        /// </summary>
        public Bmp2(short width, short height, short BytesPerPixel, short BytesPerColor, 
            List<byte[]> palette, byte[] data)
        {
            Init(width, height, BytesPerPixel, BytesPerColor, palette, data);
        }

        /// <summary>
        /// Constructeur complet
        /// </summary>
        private void Init(short width = 24, short height = 24, short BytesPerPixel = 1, 
            short BytesPerColor = 4, List<byte[]>? palette = null, byte[]? data = null)
        {
            this.BytesPerPixel = BytesPerPixel;
            this.BytesPerColor = BytesPerColor;
            this.Height = height;
            this.Width = width;

            this.Palette = palette ?? InitPalette();
            this.Data = data ?? new byte[Width * Height];
        }

        /// <summary>
        /// Lit un fichier bmp2 et recrée l'objet correspondant
        /// </summary>
        /// <param name="filename">Fichier à lire</param>
        /// <returns>L'objet bmp2 correspondant</returns>
        /// <exception cref="Exception">Si le nombre magique bmp2 n'est pas détecté</exception>
        public static Bmp2 FromFile(string filename)
        {
            int PaletteCount;
            short height, width, BytesPerColor, BytesPerPixel;
            List<byte[]> palettes = new List<byte[]>();
            List<byte> ldata = new List<byte>();

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                byte[] data = new byte[4];
                fs.Read(data, 0, data.Length);
                if (BitConverter.ToInt32(data) != 846228802) throw new Exception("Oops bad format");

                data = new byte[2];
                fs.Read(data, 0, data.Length);
                width = BitConverter.ToInt16(data);

                fs.Read(data, 0, data.Length);
                height = BitConverter.ToInt16(data);

                fs.Read(data, 0, data.Length);
                BytesPerPixel = BitConverter.ToInt16(data);

                fs.Read(data, 0, data.Length);
                BytesPerColor = BitConverter.ToInt16(data);

                data = new byte[4];
                fs.Read(data, 0, data.Length);
                PaletteCount = BitConverter.ToInt16(data);

                for (int i = 0; i < PaletteCount; i++)
                {
                    data = new byte[4];
                    fs.Read(data, 0, data.Length);
                    palettes.Add(data);
                }

                do {
                    data = new byte[BytesPerPixel];
                    fs.Read(data, 0, data.Length);
                    ldata.AddRange(data);
                } while (fs.Position < fs.Length);

                //version courte (BytesPerPixel = 1)
                /*
                data = new byte[fs.Length - fs.Position];
                fs.Read(data, 0, data.Length);
                bdata = data;
                */
            }

            return new Bmp2(width, height, BytesPerPixel, BytesPerColor, palettes, ldata.ToArray());
        }

        /// <summary>
        /// Lit un fichier texte et importe son contenu dans la grille de pixel.
        /// Remplace les données de l'objet actuel.
        /// </summary>
        /// <param name="filename">Fichier à importer</param>
        public void ImportTxt(string filename)
        {
            short height = 0, width = 0;
            List<byte> lData = new List<byte>();

            using (StreamReader sr = new StreamReader(filename))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] tab = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < tab.Length; i++) lData.Add(byte.Parse(tab[i]));

                    if (width == 0) width = (short)tab.Length;
                    height += 1;
                }
            }

            this.Height = height;
            this.Width = width;

            Data = lData.ToArray();
        }

        /// <summary>
        /// Convertit un objet bmp2 en bmp classique
        /// </summary>
        /// <returns></returns>
        public Bitmap ConvertToBitmap()
        {
            using (Bitmap bmp = new Bitmap(Width, Height))
            {
                int x = 0, y = 0;

                for (int i = 0; i < Data.Length; i++)
                {
                    if (i > 0 && i % Width == 0) { x = 0; y++; }
                    bmp.SetPixel(x, y, IdPaletteToColor(Data[i]));
                }

                return bmp;
            }
        }

        /// <summary>
        /// Définit la couleur d'un pixel dans la grille
        /// </summary>
        /// <param name="idPalette">Index de la couleur dans la palette courante</param>
        public void SetPixel(int x, int y, byte idPalette)
        {
            int idx = x * Width + y;
            Data[idx] = idPalette;
        }

        /// <summary>
        /// Retourne une couleur de la palette courante en tant que structure Color
        /// </summary>
        /// <param name="idPalette">Index de la couleur dans la palette courante</param>
        /// <returns></returns>
        private Color IdPaletteToColor(short idPalette)
        {
            byte[] data = Palette[idPalette];
            return Color.FromArgb(data[3], data[0], data[1], data[2]);
        }

        /// <summary>
        /// Initialise la palette par défaut
        /// </summary>
        /// <returns></returns>
        private List<byte[]> InitPalette()
        {
            return new List<byte[]>
            {
                new byte[] { 255, 255, 255, 0 },
                new byte[] { 198, 0, 0, 255 },
                new byte[] { 143, 60, 31, 255 },
                new byte[] { 102, 102, 243, 255 },
                new byte[] { 247, 214, 181, 255 },
                new byte[] { 248, 251, 30, 255 },
                new byte[] { 0, 0, 0, 255 }
            };
        }

        /// <summary>
        /// Compile la palette en tableau de byte pour sauvegarde binaire
        /// </summary>
        /// <returns></returns>
        private byte[] BuildPalette()
        {
            byte[] palette = new byte[Palette.Count * BytesPerColor];
            int idx = 0;

            for (int i = 0; i < Palette.Count; i++)
                for (int j = 0; j < Palette[i].Length; j++)
                    Array.Copy(BitConverter.GetBytes(Palette[i][j]), 0, palette, idx++, 1);

            return palette;
        }

        /// <summary>
        /// Compile les entetes en tableau de byte pour sauvegarde binaire
        /// </summary>
        /// <returns></returns>
        private byte[] BuildHeaders()
        {
            byte[] headers = new byte[16];
            Array.Copy(BitConverter.GetBytes(MagicNumber), 0, headers, 0, 4);
            Array.Copy(BitConverter.GetBytes(Width), 0, headers, 4, 2);
            Array.Copy(BitConverter.GetBytes(Height), 0, headers, 6, 2);
            Array.Copy(BitConverter.GetBytes(BytesPerPixel), 0, headers, 8, 2);
            Array.Copy(BitConverter.GetBytes(BytesPerColor), 0, headers, 10, 2);
            Array.Copy(BitConverter.GetBytes(PaletteCount), 0, headers, 12, 4);
            return headers;
        }

        /// <summary>
        /// Sauvegarde l'image au format bmp2 dans un fichier
        /// </summary>
        /// <param name="filename">Nom du fichier à sauvegarder</param>
        public void SaveImage(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                byte[] headers = BuildHeaders();
                fs.Write(headers, 0, headers.Length);

                byte[] palette = BuildPalette();
                fs.Write(palette, 0, palette.Length);

                fs.Write(Data, 0, Data.Length);
            }
        }

    }

}