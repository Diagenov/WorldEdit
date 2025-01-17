﻿using Microsoft.Xna.Framework;
using Regions;
using System;
using System.Linq;
using Terraria;
using TShockAPI;
using WorldEdit.Extensions;

namespace WorldEdit.Commands
{
	public abstract class WECommand
	{
		public TSPlayer plr;
		public Selection select;
		public int x;
		public int x2;
		public int y;
		public int y2;
        public bool minMaxPoints;

        protected WECommand(int x, int y, int x2, int y2, TSPlayer plr, bool minMaxPoints = true)
		{
			this.plr = plr;
			this.select = plr.GetPlayerInfo().Select ?? WorldEdit.Selections["normal"];
			this.x = x;
			this.x2 = x2;
			this.y = y;
			this.y2 = y2;
            this.minMaxPoints = minMaxPoints;
		}

		public abstract void Execute();
		public void Position(bool force = false)
		{
            if (!force && !minMaxPoints) { return; }
			int temp;
			x = Math.Max(x, 0);
			y = Math.Max(y, 0);
			x2 = Math.Min(x2, Main.maxTilesX - 1);
			y2 = Math.Min(y2, Main.maxTilesY - 1);
			
			if (x > x2)
			{
				temp = x2;
				x2 = x;
				x = temp;
			}
			if (y > y2)
			{
				temp = y2;
				y2 = y;
				y = temp;
			}
		}
		public void ResetSection()
        {
            int left = Math.Min(x, x2), right = Math.Max(x, x2);
            int top = Math.Min(y, y2), bottom = Math.Max(y, y2);
            int sX = Netplay.GetSectionX(left), sX2 = Netplay.GetSectionX(right);
            int sY = Netplay.GetSectionY(top), sY2 = Netplay.GetSectionY(bottom);

            int w = (right - left + 1), h = (bottom - top + 1);
            bool SendWholeSections = ((w > 200) || (h > 150));

            if (SendWholeSections)
                foreach (RemoteClient sock in Netplay.Clients.Where(s => s.IsActive))
                    for (int i = sX; i <= sX2; i++)
                        for (int j = sY; j <= sY2; j++)
                            sock.TileSections[i, j] = false;
            else
            {
                NetMessage.SendData(10, -1, -1, null, left, top, w, h);
                NetMessage.SendData(11, -1, -1, null, sX, sY, sX2, sY2);
            }
        }
		public void SetTile(int i, int j, int tileType)
		{
			var tile = Main.tile[i, j];
			switch (tileType)
			{
				case -1:
					tile.active(false);
					tile.frameX = -1;
					tile.frameY = -1;
					tile.liquidType(0);
					tile.liquid = 0;
					tile.type = 0;
					return;
				case -2:
					tile.active(false);
					tile.liquidType(1);
					tile.liquid = 255;
					tile.type = 0;
					return;
				case -3:
					tile.active(false);
					tile.liquidType(2);
					tile.liquid = 255;
					tile.type = 0;
					return;
				case -4:
					tile.active(false);
					tile.liquidType(0);
					tile.liquid = 255;
					tile.type = 0;
					return;
				default:
					if (Main.tileFrameImportant[tileType])
						WorldGen.PlaceTile(i, j, tileType);
					else
					{
						tile.active(true);
						tile.frameX = -1;
						tile.frameY = -1;
						tile.liquidType(0);
						tile.liquid = 0;
						tile.type = (ushort)tileType;
					}
					return;
			}
		}
		public bool TileSolid(int x, int y)
		{
			return x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY || (Main.tile[x, y].active() && Main.tileSolid[Main.tile[x, y].type]);
		}

        public bool CanUseCommand(string permission, Func<RegionInfo, bool> function = null) => CanUseCommand(x, y, x2, y2, permission, function);
        public bool CanUseCommand(int x, int y, int x2, int y2, string permission, Func<RegionInfo, bool> function = null)
        {
            return Tools.CheckPoints(plr, x, y, x2, y2, permission, function);
        }
    }
}