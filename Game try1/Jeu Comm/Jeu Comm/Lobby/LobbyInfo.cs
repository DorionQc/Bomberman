using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;

using Jeu_Comm.Network;

namespace Jeu_Comm.Lobby
{
    public struct LobbyInfo
    {
        public IPAddress Host;
        public string NomPartie;

        public string[] NomJoueurs;
        public IPAddress[] IPJoueurs;

        public Color[] Colors;

        public bool[] SpotsTaken;
        public bool[] PlayerReady;

        public LobbyInfo(IPAddress Host)
        {
            PlayerReady = new bool[4];
            SpotsTaken = new bool[4];
            NomPartie = "";
            IPJoueurs = new IPAddress[4] { IPAddress.None, IPAddress.None, IPAddress.None, IPAddress.None };
            Colors = new Color[4] { Color.White, Color.White, Color.White, Color.White };
            this.Host = Host;
            NomJoueurs = new string[4] { "", "", "", "" };
        }

        public byte GetFirstAvailableSpot()
        {
            byte i = 0;
            while (i < 4 && SpotsTaken[i])
                i++;
            if (i == 4)
                return 255;
            return i;
        }

        public bool AddPlayer(string Nom, byte ID, IPAddress IP, Color Color)
        {
            if (ID >= 0 && ID <= 3 && SpotsTaken[ID] == false && IP != IPAddress.Any && IP != IPAddress.Broadcast)
            {
                NomJoueurs[ID] = Nom;
                IPJoueurs[ID] = IP;
                Colors[ID] = Color;
                SpotsTaken[ID] = true;
                return true;
            }
            return false;
        }

        public bool RemovePlayer(byte ID)
        {
            if (ID >= 0 && ID <= 3)
            {
                NomJoueurs[ID] = "";
                IPJoueurs[ID] = IPAddress.None;
                Colors[ID] = Color.Black;
                SpotsTaken[ID] = false;
                return true;
            }
            return false;
        }

        
        public byte[] ToByteArray()
        {
            int Length = 42;
            int[] TextLength = new int[4];
            int NomLength = Encoding.Default.GetByteCount(NomPartie);
            Length += NomLength;
            for (int i = 0; i < 4; i++)
            {
                TextLength[i] = Encoding.Default.GetByteCount(NomJoueurs[i]);
                Length += TextLength[i];
            }
            byte[] ret = new byte[Length];
            Host.GetAddressBytes().CopyTo(ret, 0);
            ret[4] = (byte)NomLength;
            int Pos = 5;
            Encoding.Default.GetBytes(NomPartie).CopyTo(ret, Pos);
            Pos += NomLength;
            for (int i = 0; i < 4; i++)
            {
                ret[Pos] = (byte)TextLength[i];
                Encoding.Default.GetBytes(NomJoueurs[i]).CopyTo(ret, Pos + 1);
                Pos += TextLength[i] + 1;
            }
            for (int i = 0; i < 4; i++)
            {
                IPJoueurs[i].GetAddressBytes().CopyTo(ret, Pos);
                Pos += 4;
            }
            for (int i = 0; i < 4; i++)
            {
                BitConverter.GetBytes(Colors[i].ToArgb()).CopyTo(ret, Pos);
                Pos += 4;
            }
            ret[Pos] = (byte)(
                (PlayerReady[0] ? 8 : 0) |
                (PlayerReady[1] ? 4 : 0) |
                (PlayerReady[2] ? 2 : 0) |
                (PlayerReady[3] ? 1 : 0));
            return ret;
        }

        // Host | (length) Nom Partie | (length) Nom Joueur x4 | IP Joueur x4 | Couleur Joueur x4 // POSSIBLY TO REDO
        public bool FromByteArray(byte[] Data, ref int Index)
        {
            int LengthSoFar = 41 + Index;
            if (Data.Length < LengthSoFar)
                return false;
            NomJoueurs = new string[4];
            IPJoueurs = new IPAddress[4];
            PlayerReady = new bool[4];
            Colors = new Color[4];
            SpotsTaken = new bool[4];

            Host = new IPAddress(NetworkUtils.SubArray(Data, Index, 4));
            NomPartie = Encoding.Default.GetString(Data, 5 + Index, Data[4 + Index]);
            int Pos = 5 + Data[4 + Index];
            LengthSoFar += Data[4 + Index];
            if (Data.Length < LengthSoFar)
                return false;
            int j = 0;
            while (j < 4 && LengthSoFar < Data.Length)
            {
                NomJoueurs[j] = Encoding.Default.GetString(Data, Pos + Index + 1, Data[Pos + Index]);
                Pos += Data[Pos + Index] + 1;
                LengthSoFar += Data[Pos + Index];
                if (NomJoueurs[j] != "")
                    SpotsTaken[j] = true;
                else
                    SpotsTaken[j] = false;
                j++;
            }
            if (Pos > LengthSoFar - 32 || j != 4)
                return false;
            for (int i = 0; i < 4; i++)
            {
                IPJoueurs[i] = new IPAddress(NetworkUtils.SubArray(Data, Pos + Index, 4));
                Pos += 4;
            }
            for (int i = 0; i < 4; i++)
            {
                Colors[i] = Color.FromArgb(BitConverter.ToInt32(Data, Pos + Index));
                Pos += 4;
            }
            byte flag = 8;
            for (int i = 0; i < 4; i++)
            {
                PlayerReady[i] = ((flag & Data[Pos + Index]) > 0);
                flag >>= 1;
            }
            Index += Pos + 1;
            return true;
        }
    }
}
