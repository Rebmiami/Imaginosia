using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia
{
	public class SpriteBatcher : SpriteBatch
	{
		Dictionary<string, RenderTarget2D> layers;
		RenderTarget2D finalTarget;
		Rectangle dimensions;

		public SpriteBatcher(GraphicsDevice graphicsDevice, Rectangle dimensions) : base(graphicsDevice)
		{
			layers = new Dictionary<string, RenderTarget2D>();
			this.dimensions = dimensions;
		}

		public void LayerBegin(string layerName, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
		{
			RenderTarget2D newLayer = new RenderTarget2D(GraphicsDevice, dimensions.Width, dimensions.Height);
			layers.Add(layerName, newLayer);
			GraphicsDevice.SetRenderTarget(newLayer);

			Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
		}

		public void LayerEnd()
		{
			End();
		}

		public void FrameBegin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
		{
			GraphicsDevice.SetRenderTarget(null);
			Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
		}

		public void FrameEnd()
		{
			End();
		}

		public void DisposeLayer(string layerName)
		{
			layers[layerName].Dispose();
			layers.Remove(layerName);
		}

		public void DrawLayer(string layerName)
		{
			Draw(layers[layerName], Vector2.Zero, Color.White);
		}

		public RenderTarget2D GetLayer(string layerName)
		{
			return layers[layerName];
		}

		// public void DrawToScreen(Rectangle dimensions)
		// {
		// 
		// }
	}
}
