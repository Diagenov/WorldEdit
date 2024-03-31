using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands
{
	public class SetWall : WECommand
	{
		private Expression expression;
		private int wallType;

		public SetWall(int x, int y, int x2, int y2, TSPlayer plr, int wallType, Expression expression)
			: base(x, y, x2, y2, plr)
		{
			this.expression = expression ?? new TestExpression(new Test(t => true));
			this.wallType = wallType;
		}

		public override void Execute()
        {
            if (!CanUseCommand("worldedit.region.setwall")) 
			{ 
				return; 
			}
            if (!WorldEdit.BuildWalls.Contains(wallType))
            {
				plr.SendErrorMessage("You are not determined enough to use this!");
                return;
            }
            Tools.PrepareUndo(x, y, x2, y2, plr);
			int edits = 0;
			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					if (Tools.CanSet(false, Main.tile[i, j], wallType, select, expression, i, j, plr))
                    {
                        Main.tile[i, j].wall = (ushort)wallType;
						edits++;
					}
				}
			}
			ResetSection();

			string wallName = wallType == 0 ? "air" : "wall " + wallType;
			plr.SendSuccessMessage("Set walls to {0}. ({1})", wallName, edits);
		}
	}
}