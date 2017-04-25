﻿using EngineName.Components.Renderable;
using EngineName.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using EngineName.Components;
using Microsoft.Xna.Framework;

namespace EngineName.Systems
{
    public class Rendering2DSystem : EcsSystem
    {
        private GraphicsDevice mGraphicsDevice;
        private SpriteBatch mSpriteBatch;

        public override void Init()
        {
            mGraphicsDevice = Game1.Inst.GraphicsDevice;
            mSpriteBatch = new SpriteBatch(mGraphicsDevice);
            base.Init();
        }
        public override void Update(float t, float dt)
        {
            base.Update(t, dt);
        }
        public override void Draw(float t, float dt)
        {
            mSpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
            foreach (CCamera camera in Game1.Inst.Scene.GetComponents<CCamera>().Values)
            {
                foreach (var component in Game1.Inst.Scene.GetComponents<C2DRenderable>())
                {
                    var key = component.Key;
                    if (component.Value.GetType() == typeof(CSprite))
                    {
                        CSprite sprite = (CSprite)component.Value;
                        DrawSprite(t, dt, sprite);
                    }
                    if (component.Value.GetType() == typeof(CText))
                    {
                        CText text = (CText)component.Value;
                        DrawText(t, dt, text);
                    }
                }
            }

            mSpriteBatch.End();
            base.Draw(t, dt);
        }
        private void DrawSprite(float t, float dt, CSprite sprite)
        {
            if (sprite.texture == null) return;
            mSpriteBatch.Draw(sprite.texture, sprite.position, sprite.color);
        }
        private void DrawText(float t, float dt, CText text)
        {
            if (text.format == null) return;
            text.origin = MeasureString(text.font, text.format) / 2; //Center in middle of text

            mSpriteBatch.DrawString(text.font, text.format, text.position, text.color, 0f, text.origin, 1f, SpriteEffects.None, 0f);
        }
        private static Vector2 MeasureString(SpriteFont font, string text)
        {
            return font.MeasureString(text);
        }
    }
}