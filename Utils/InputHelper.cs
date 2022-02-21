using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Imaginosia
{
	public static class KeyHelper
	{
		public static KeyboardState oldKeyboard;

		public static void Update()
		{
			oldKeyboard = Keyboard.GetState();
		}

		public static bool Pressed(Keys key) //Returns true on the frame the key specified was pressed
		{
			return Down(key) && !Down(key, true);
		}

		public static bool Released(Keys key) //Returns true on the frame the key specified was released
		{
			return !Down(key) && Down(key, true);
		}

		public static bool Down(Keys key, bool old = false) //Looks cleaner than Keyboard.GetState().IsKeyDown(key)
		{
			if (old)
			{
				return oldKeyboard.IsKeyDown(key);
			}
			return Keyboard.GetState().IsKeyDown(key);
		}
	}

	public static class MouseHelper
	{
		public static MouseState oldMouse;

		public static void Update()
		{
			oldMouse = Mouse.GetState();
		}

		public static bool Pressed(MouseButton button)
		{
			return Down(button) && !Down(button, true);
		}

		public static bool Released(MouseButton button)
		{
			return !Down(button) && Down(button, true);
		}

		public static bool Down(MouseButton button, bool old = false)
		{
			MouseState state = Mouse.GetState();
			if (old)
			{
				state = oldMouse;
			}
			switch (button)
			{
				case MouseButton.Left:
					return state.LeftButton == ButtonState.Pressed;
				case MouseButton.Right:
					return state.RightButton == ButtonState.Pressed;
				case MouseButton.Middle:
					return state.MiddleButton == ButtonState.Pressed;
				case MouseButton.X1:
					return state.XButton1 == ButtonState.Pressed;
				case MouseButton.X2:
					return state.XButton2 == ButtonState.Pressed;
			}
			return false;
		}

		public static float Scroll()
		{
			return Mouse.GetState().ScrollWheelValue - oldMouse.ScrollWheelValue;
		}

		public static Vector2 Position
		{
			get => Mouse.GetState().Position.ToVector2() / Game1.ScreenScalingFactor + Game1.ScreenOriginOffset.ToVector2();
		}
	}

	public enum MouseButton
	{
		Left,
		Right,
		Middle,
		X1,
		X2,
	}

	public static class GamePadHelper
	{
		public static GamePadState[] oldGamePad = new GamePadState[4];

		public static void Update()
		{
			oldGamePad[0] = GamePad.GetState(0);
		}

		public static bool Pressed(Buttons button, PlayerIndex gamepad) //Returns true on the frame the key specified was pressed
		{
			return Down(button, gamepad) && !Down(button, gamepad, true);
		}

		public static bool Released(Buttons button, PlayerIndex gamepad) //Returns true on the frame the button specified was released
		{
			return !Down(button, gamepad) && Down(button, gamepad, true);
		}

		public static Vector2 ThumbSticks(PlayerIndex gamepad, bool right)
		{
			if (right)
			{
				return GamePad.GetState(gamepad, GamePadDeadZone.Circular).ThumbSticks.Right;
			}
			return GamePad.GetState(gamepad, GamePadDeadZone.Circular).ThumbSticks.Left;
		}

		public static bool TriggerHit(PlayerIndex gamepad, bool right, float value) //Returns true on the frame the trigger passes or hits the specified value at all.
		{
			return TriggerRise(gamepad, right, value) || TriggerFall(gamepad, right, value) || (Triggers(gamepad, right) == value && Triggers(gamepad, right, true) != value);
		}

		public static bool TriggerRise(PlayerIndex gamepad, bool right, float value) //Returns true on the frame the trigger rises above the specified value.
        {
			return Triggers(gamepad, right) > value && Triggers(gamepad, right, true) < value;
        }

		public static bool TriggerFall(PlayerIndex gamepad, bool right, float value) //Returns true on the frame the trigger falls below the specified value.
        {
			return Triggers(gamepad, right) < value && Triggers(gamepad, right, true) > value;
		}

		public static float Triggers(PlayerIndex gamepad, bool right, bool old = false)
		{
			if (old)
			{
				if (right)
				{
					return oldGamePad[(int)gamepad].Triggers.Right;
				}
				return oldGamePad[(int)gamepad].Triggers.Left;
			}
			else
            {
				if (right)
				{
					return GamePad.GetState(gamepad).Triggers.Right;
				}
				return GamePad.GetState(gamepad).Triggers.Left;
			}
		}

		public static bool Down(Buttons button, PlayerIndex gamepad, bool old = false)
		{
			if (old)
			{
				return oldGamePad[(int)gamepad].IsButtonDown(button);
			}
			return GamePad.GetState(gamepad).IsButtonDown(button);
		}
	}
}
