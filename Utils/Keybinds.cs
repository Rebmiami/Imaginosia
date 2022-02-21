using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia
{
	public static class Keybinds
	{
		public static bool UseItem(ControllerType controller, PlayerIndex gamepadNumber)
		{
			if (controller == ControllerType.Keyboard)
			{
				return MouseHelper.Pressed(MouseButton.Left);
			}
			else
			{
				return GamePadHelper.Pressed(Buttons.RightTrigger, gamepadNumber);
			}    
		}

		public static bool UseItemHold(ControllerType controller, PlayerIndex gamepadNumber)
		{
			if (controller == ControllerType.Keyboard)
			{
				return MouseHelper.Down(MouseButton.Left);
			}
			else
			{
				return GamePadHelper.Down(Buttons.RightTrigger, gamepadNumber);
			}
		}

		public static bool PickUpItem(ControllerType controller, PlayerIndex gamepadNumber)
		{
			if (controller == ControllerType.Keyboard)
			{
				return KeyHelper.Pressed(Keys.E);
			}
			else
			{
				return GamePadHelper.Pressed(Buttons.LeftTrigger, gamepadNumber);
			}
		}

		public static int SwapItem(ControllerType controller, PlayerIndex gamepadNumber)
		{
			if (controller == ControllerType.Keyboard)
			{
				if (Math.Abs(MouseHelper.Scroll()) > 1)
				{
					return -Math.Sign(MouseHelper.Scroll());
				}
			}
			else
			{
				if (GamePadHelper.Pressed(Buttons.DPadLeft, gamepadNumber))
				{
					return 1;
				}
				else if (GamePadHelper.Pressed(Buttons.DPadRight, gamepadNumber))
				{
					return -1;
				}
			}
			return 0;
		}

		public static int UseActive(ControllerType controller, PlayerIndex gamepadNumber)
		{
			if (controller == ControllerType.Keyboard)
			{
				if (KeyHelper.Pressed(Keys.Z))
					return 1;
				if (KeyHelper.Pressed(Keys.X))
					return 2;
				if (KeyHelper.Pressed(Keys.C))
					return 3;
			}
			else
			{
				if (GamePadHelper.Pressed(Buttons.X, gamepadNumber))
					return 1;
				if (GamePadHelper.Pressed(Buttons.Y, gamepadNumber))
					return 2;
				if (GamePadHelper.Pressed(Buttons.B, gamepadNumber))
					return 3;
			}
			return 0;
		}
	}
}
