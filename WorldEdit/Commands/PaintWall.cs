using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands
{
	public class PaintWall : WECommand
	{
		private int color;
		private bool coating;

        private Expression expression;

		public PaintWall(int x, int y, int x2, int y2, TSPlayer plr, int color, bool coating, Expression expression)
			: base(x, y, x2, y2, plr)
		{
			this.color = color;
            this.coating = coating;
            this.expression = expression ?? new TestExpression(new Test(t => true));
		}

		public override void Execute()
        {
            if (!CanUseCommand("worldedit.region.paintwall")) { return; }
            Tools.PrepareUndo(x, y, x2, y2, plr);
			int edits = 0;
			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					var tile = Main.tile[i, j];
					if (tile.wall > 0 && tile.wallColor() != color && select(i, j, plr) && expression.Evaluate(tile))
					{
                        if (color == 0)
                        {
                            tile.wallColor(0);
                            tile.invisibleWall(false);
                            tile.fullbrightWall(false);
                        }
                        else if (coating)
							WorldGen.paintCoatWall(i, j, (byte)color);
						else
							tile.wallColor((byte)color);
						edits++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Painted walls. ({0})", edits);
		}
	}
}
