﻿using TShockAPI;

namespace WorldEdit.Commands
{
	public class Undo : WECommand
	{
		private int accountID;
		private int steps;

		public Undo(TSPlayer plr, int accountID, int steps)
			: base(0, 0, 0, 0, plr)
		{
			this.accountID = accountID;
			this.steps = steps;
		}

		public override void Execute()
		{
			int i = -1;
			while (++i < steps && Tools.Undo(plr, accountID));
			if (i == 0)
				plr.SendErrorMessage("Failed to undo any actions.");
			else
				plr.SendSuccessMessage("Undid {0}'s last {1}action{2}.", 
					accountID == 0 ? "ServerConsole" : TShock.UserAccounts.GetUserAccountByID(accountID).Name, 
					i == 1 ? "" : i + " ", 
					i == 1 ? "" : "s");
		}
	}
}