using System;
using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands
{
    public class Paste : WECommand
	{
		private readonly int alignment;
		private readonly Expression expression;
        private readonly bool mode_MainBlocks;
        private readonly string path;
        private readonly bool prepareUndo;

        public Paste(int x, int y, TSPlayer plr, string path, int alignment, Expression expression, bool mode_MainBlocks, bool prepareUndo)
			: base(x, y, int.MaxValue, int.MaxValue, plr)
		{
			this.alignment = alignment;
			this.expression = expression;
            this.mode_MainBlocks = mode_MainBlocks;
            this.path = path;
            this.prepareUndo = prepareUndo;
        }

		public override void Execute()
		{
            var data = Tools.LoadWorldData(path);
			var width = data.Width - 1;
			var height = data.Height - 1;

			if ((alignment & 1) == 0)
			{
				x2 = x + width;
			}
			else
			{
				x2 = x;
				x -= width;
			}
			if ((alignment & 2) == 0)
			{
				y2 = y + height;
			}
			else
			{
				y2 = y;
				y -= height;
			}

			x = Math.Min(Math.Max(0, x), Main.maxTilesX - 1);
            y = Math.Min(Math.Max(0, y), Main.maxTilesY - 1);
            x2 = Math.Min(Math.Max(0, x2), Main.maxTilesX - 1);
            y2 = Math.Min(Math.Max(0, y2), Main.maxTilesY - 1);

            if (!CanUseCommand("worldedit.clipboard.paste")) 
			{ 
				return; 
			}
            if (prepareUndo) 
			{ 
				Tools.PrepareUndo(x, y, x2, y2, plr); 
			}

			for (var i = x; i <= x2; i++)
			{
				for (var j = y; j <= y2; j++)
                {
                    var index1 = i - x;
                    var index2 = j - y;

                    if (i < 0 || j < 0 || i >= Main.maxTilesX || j >= Main.maxTilesY ||
						expression != null && 
						!expression.Evaluate(mode_MainBlocks ? Main.tile[i, j] : data.Tiles[index1, index2]))
					{
						continue;
					}

					Main.tile[i, j] = data.Tiles[index1, index2];
				}
			}

            Tools.LoadWorldSection(data, x, y, false);
            ResetSection();
            plr.SendSuccessMessage("Pasted clipboard to selection.");
		}
	}
}