﻿using System;
using TShockAPI;

namespace WorldEdit.Commands
{
	public class UndoCommand : WECommand
	{
		private int steps;

		public UndoCommand(TSPlayer plr, int steps)
			: base(0, 0, 0, 0, plr)
		{
			this.steps = steps;
		}

		public override void Execute()
		{
			int i = 0;
			for (; WorldEdit.GetPlayerInfo(plr).undoLevel != -1 && i < steps; i++)
			{
				Tools.Undo(plr);
			}
			plr.SendSuccessMessage(String.Format("Undid last {0}action{1}.", i == 1 ? "" : i + " ", i == 1 ? "" : "s"));
		}
	}
}