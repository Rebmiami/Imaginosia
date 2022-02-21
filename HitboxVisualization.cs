using Imaginosia.Gameplay;
using Imaginosia.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia
{
	public static class HitboxVisualization
	{
		public static void DrawEntityHitbox(Entity entity, SpriteBatcher spriteBatcher)
		{
			FloatRectangle hitboxRectangle = entity.Hitbox;
			hitboxRectangle.Location = PositionHelper.ToScreenPosition(hitboxRectangle.Location);
			hitboxRectangle.Size = PositionHelper.ToScreenPosition(hitboxRectangle.Size, true);


			spriteBatcher.Draw(Assets.Tex["special"], new Rectangle(hitboxRectangle.Location.ToPoint(), hitboxRectangle.Size.ToPoint()), Color.White * 0.5f);
		}
	}
}
