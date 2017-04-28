using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeu_Comm.Entities;

namespace Jeu_Comm.Entities
{
    public struct PlayerPositions
    {
        public int[] X;
        public int[] Y;
        public float[] VelX;
        public float[] VelY;

        public PlayerPositions(Joueur[] Players)
        {
            X = new int[4];
            Y = new int[4];
            VelX = new float[4];
            VelY = new float[4];

            for (int i = 0; i < 4; i++)
            {
                if (Players[i] != null)
                {
                    X[i] = Players[i].X;
                    Y[i] = Players[i].Y;
                    VelX[i] = Players[i].VelX;
                    VelY[i] = Players[i].VelY;
                }
            }
        }

        public void Update(byte ID, int x, int y, float velx, float vely)
        {
            X[ID] = x;
            Y[ID] = y;
            VelX[ID] = velx;
            VelY[ID] = vely;
        }

        public byte[] ToByteArray()
        {
            byte[] ret = new byte[64];
            for (int i = 0; i < 4; i++)
            {
                BitConverter.GetBytes(X[i]).CopyTo(ret, i * 4);
                BitConverter.GetBytes(Y[i]).CopyTo(ret, 16 + i * 4);
                BitConverter.GetBytes(VelX[i]).CopyTo(ret, 32 + i * 4);
                BitConverter.GetBytes(VelY[i]).CopyTo(ret, 48 + i * 4);
            }
            return ret;
        }

        public bool Decode(byte[] Data, ref int Pos)
        {
            if (Data.Length < Pos + 64)
                return false;
            X = new int[4];
            Y = new int[4];
            VelX = new float[4];
            VelY = new float[4];
            for (int i = 0; i < 4; i++)
            {
                X[i] =    BitConverter.ToInt32(Data, Pos + i * 4);
                Y[i] =    BitConverter.ToInt32(Data, 16 + Pos + i * 4);
                VelX[i] = BitConverter.ToSingle(Data, 32 + Pos + i * 4);
                VelY[i] = BitConverter.ToSingle(Data, 48 + Pos + i * 4);
            }
            Pos += 64;
            return true;
        }
    } 
}
