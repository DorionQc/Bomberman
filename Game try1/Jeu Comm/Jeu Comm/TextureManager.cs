/************************
 * Samuel Goulet
 * Novembre 2016
 * Classe Singleton TextureManager, pour s'occuper des textures du jeu
 ***********************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Jeu_Comm
{
    // Classe Singleton TextureManager, pour s'occuper des textures du jeu
    public class TextureManager
    {
        // Déclaration des textures possibles

        public Image TextureCaseVide;
        public Image TextureCaseWall;
        public Image TextureCaseSolidWall;
        public Image[,] tTextureCaseBonus;

        public Image[,] tTexturePlayerLeft;
        public Image[,] tTexturePlayerRight;
        public Image[,] tTexturePlayerUp;
        public Image[,] tTexturePlayerDown;

        public Image[] tTextureFire;
        public Image[,] tTextureBomb;

        // Déclaration de l'instance unique de la classe
        private static TextureManager INSTANCE;

        // Constructeur privé
        private TextureManager(Color[] Colors)
        {
            LoadAllTextures(Colors);
        }

        /// <summary>
        /// Propriété publique pour accéder à l'instance (qui doit avoir été préalablement créée)
        /// </summary>
        public static TextureManager Instance
        {
            get
            {
                if (INSTANCE == null)
                    throw new ArgumentNullException("Singleton instance not created");
                return INSTANCE;
            }
        }

        /// <summary>
        /// Création de l'instance
        /// </summary>
        /// <param name="Colors"></param>
        public static void InitInstance(Color[] Colors)
        {
            if (INSTANCE != null)
                INSTANCE.LoadAllTextures(Colors);
            else
                INSTANCE = new TextureManager(Colors);
        }

        /// <summary>
        /// Recherche toutes les textures et les store en mémoire, en modifiant celles nécessaires avec
        /// les couleurs spécifiées
        /// </summary>
        /// <param name="Colors"></param>
        private void LoadAllTextures(Color[] Colors)
        {
            // Création des tableaux
            tTexturePlayerLeft = new Image[4, 4];
            tTexturePlayerRight = new Image[4, 4];
            tTexturePlayerUp = new Image[4, 4];
            tTexturePlayerDown = new Image[4, 4];
            tTextureCaseBonus = new Image[6, 2];
            tTextureFire = new Image[2];
            tTextureBomb = new Image[4, 4];


            // Textures de cases
            TextureCaseVide = Properties.Resources.TextureCaseVide;
            TextureCaseWall = Properties.Resources.TextureCaseWall;
            TextureCaseSolidWall = Properties.Resources.TextureCaseSolidWall;

            tTextureFire[0] = Properties.Resources.TextureCaseFire1;
            tTextureFire[1] = Properties.Resources.TextureCaseFire2;

            // Textures de joueurs et de bombes
            for (int i = 0; i < 4; i++)
            {
                tTextureBomb[i, 0] = ChangeColor(Colors[i], Properties.Resources.TextureBomb1);
                tTextureBomb[i, 1] = ChangeColor(Colors[i], Properties.Resources.TextureBomb2);
                tTextureBomb[i, 3] = ChangeColor(Colors[i], Properties.Resources.TextureBomb4);
                tTextureBomb[i, 2] = tTextureBomb[i, 0];

                tTexturePlayerUp[i, 0] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerBack1);
                tTexturePlayerUp[i, 1] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerBack2);
                tTexturePlayerUp[i, 3] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerBack4);
                tTexturePlayerUp[i, 2] = tTexturePlayerUp[i, 0];
                tTexturePlayerDown[i, 0] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerFront1);
                tTexturePlayerDown[i, 1] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerFront2);
                tTexturePlayerDown[i, 3] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerFront4);
                tTexturePlayerDown[i, 2] = tTexturePlayerDown[i, 0];
                tTexturePlayerRight[i, 0] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerRight1);
                tTexturePlayerRight[i, 1] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerRight2);
                tTexturePlayerRight[i, 3] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerRight4);
                tTexturePlayerRight[i, 2] = tTexturePlayerRight[i, 0];
                tTexturePlayerLeft[i, 0] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerLeft1);
                tTexturePlayerLeft[i, 1] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerLeft2);
                tTexturePlayerLeft[i, 3] = ChangeColor(Colors[i], Properties.Resources.TexturePlayerLeft4);
                tTexturePlayerLeft[i, 2] = tTexturePlayerLeft[i, 0];
            }
            
            // Textures des bonus
            tTextureCaseBonus[0, 0] = Properties.Resources.TextureBonusExtraBomb;
            tTextureCaseBonus[0, 1] = Properties.Resources.TextureBonusExtraBomb2;
            tTextureCaseBonus[1, 0] = Properties.Resources.TextureBonusPower;
            tTextureCaseBonus[1, 1] = Properties.Resources.TextureBonusPower2;
            tTextureCaseBonus[2, 0] = Properties.Resources.TextureBonusSpeed;
            tTextureCaseBonus[2, 1] = Properties.Resources.TextureBonusSpeed2;
            tTextureCaseBonus[3, 0] = Properties.Resources.TextureBonusShoot;
            tTextureCaseBonus[3, 1] = Properties.Resources.TextureBonusShoot2;
            tTextureCaseBonus[4, 0] = Properties.Resources.TextureBonusKick;
            tTextureCaseBonus[4, 1] = Properties.Resources.TextureBonusKick2;
            tTextureCaseBonus[5, 0] = Properties.Resources.TextureBonusMaxExplosion;
            tTextureCaseBonus[5, 1] = Properties.Resources.TextureBonusMaxExplosion2;

        }
        
        /// <summary>
        /// Modifie une image pour y remplacer le bleu pour la couleur spécifiée
        /// </summary>
        /// <param name="NewColor"></param>
        /// <param name="OldImage"></param>
        /// <returns></returns>
        public static Bitmap ChangeColor(Color NewColor, Image OldImage)
        {
            Bitmap bm = new Bitmap(OldImage.Width, OldImage.Height);
            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(new ColorMatrix(
                new float[][]
                {
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {NewColor.R / 255f, NewColor.G / 255f, NewColor.B / 255f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1},
                }
                ));
            // Écriture de l'image dans un Bitmap, avec sa couleur modifiée
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.DrawImage(OldImage, new Rectangle(0, 0, OldImage.Width, OldImage.Height), 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel, ia);
            }
            return bm;
        }

    }
}
