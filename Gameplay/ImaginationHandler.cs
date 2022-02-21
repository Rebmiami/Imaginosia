using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Gameplay
{
	public static class ImaginationHandler
	{
		public static bool IsImagination { get; private set; }

		static ImaginationHandler()
		{
			IsImagination = false;
		}

		public static void EnterImagination()
		{
			IsImagination = true;
		}

		public static void LeaveImagination()
		{
			IsImagination = false;
		}

		public static void SwitchImagination()
		{
			if (IsImagination)
			{
				LeaveImagination();
			}
			else
			{
				EnterImagination();
			}
		}
	}
}
