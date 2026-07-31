using DinaCSharp.Core.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaCSharp.Graphics
{
    /// <summary>
    /// Framework component that animates an <see cref="IText"/> instance (such as <c>Text</c> or <c>ShadowText</c>)
    /// by interpolating its position and adjusting its color and alpha transparency over a specified lifetime.
    /// </summary>
    public class FloatingText : IDraw, IUpdate
    {
        private readonly IText _text;

        /// <summary>
        /// Gets or sets the starting screen position of the floating text.
        /// </summary>
        public Vector2 StartPosition { get; set; }

        /// <summary>
        /// Gets or sets the target ending screen position of the floating text.
        /// </summary>
        public Vector2 EndPosition { get; set; }

        /// <summary>
        /// Gets or sets the base color applied to the text element.
        /// </summary>
        public Color BaseColor { get; set; }

        /// <summary>
        /// Gets or sets the initial alpha transparency factor (ranging from 0.0 to 1.0). Default value is 1.0f.
        /// </summary>
        public float StartAlpha { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the final alpha transparency factor at the end of the text's lifetime (ranging from 0.0 to 1.0). Default value is 0.5f.
        /// </summary>
        public float EndAlpha { get; set; } = 0.5f;

        /// <summary>
        /// Gets the remaining lifetime of the floating text animation in seconds.
        /// </summary>
        public float Lifetime { get; private set; }

        /// <summary>
        /// Gets or sets the maximum duration of the floating text animation in seconds.
        /// </summary>
        public float MaxLifetime { get; set; }

        /// <summary>
        /// Gets a value indicating whether the floating text animation is still active.
        /// </summary>
        public bool IsAlive => Lifetime > 0f;

        /// <summary>
        /// Gets the underlying encapsulated <see cref="IText"/> element.
        /// </summary>
        public IText TextElement => _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingText"/> class using an existing <see cref="IText"/> object.
        /// </summary>
        /// <param name="text">The text element to animate.</param>
        /// <param name="startPosition">The starting position on screen.</param>
        /// <param name="endPosition">The target ending position on screen.</param>
        /// <param name="color">The base display color.</param>
        /// <param name="duration">The total duration of the animation in seconds. Default is 0.8s.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
        public FloatingText(IText text, Vector2 startPosition, Vector2 endPosition, Color color, float duration = 0.8f)
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
            StartPosition = startPosition;
            EndPosition = endPosition;
            BaseColor = color;
            MaxLifetime = duration;
            Lifetime = duration;

            _text.Position = startPosition;
            _text.Color = color * StartAlpha;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingText"/> class, automatically instantiating a <c>Text</c> or <c>ShadowText</c> element based on shadow parameters.
        /// </summary>
        /// <param name="font">The font resource used for rendering text.</param>
        /// <param name="content">The text content to display.</param>
        /// <param name="startPosition">The starting position on screen.</param>
        /// <param name="endPosition">The target ending position on screen.</param>
        /// <param name="color">The base display color.</param>
        /// <param name="shadowColor">Optional color for the drop shadow. If specified alongside <paramref name="shadowOffset"/>, a <c>ShadowText</c> is created.</param>
        /// <param name="shadowOffset">Optional offset vector for the drop shadow.</param>
        /// <param name="duration">The total duration of the animation in seconds. Default is 0.8s.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="font"/> is null.</exception>
        public FloatingText(
            SpriteFont font,
            string content,
            Vector2 startPosition,
            Vector2 endPosition,
            Color color,
            Color? shadowColor = null,
            Vector2? shadowOffset = null,
            float duration = 0.8f)
        {
            ArgumentNullException.ThrowIfNull(font);

            StartPosition = startPosition;
            EndPosition = endPosition;
            BaseColor = color;
            MaxLifetime = duration;
            Lifetime = duration;

            if (shadowColor.HasValue && shadowOffset.HasValue)
            {
                _text = new ShadowText(font, content, color, shadowColor.Value, shadowOffset.Value, startPosition);
            }
            else
            {
                _text = new Text(font, content, color, startPosition);
            }

            _text.Color = color * StartAlpha;
        }

        /// <summary>
        /// Updates the animation state, recalculating the position, remaining lifetime, and alpha channel of the text element.
        /// </summary>
        /// <param name="gametime">Provides a snapshot of timing values.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="gametime"/> is null.</exception>
        public void Update(GameTime gametime)
        {
            ArgumentNullException.ThrowIfNull(gametime);

            if (!IsAlive)
                return;

            float deltaTime = (float)gametime.ElapsedGameTime.TotalSeconds;
            Lifetime -= deltaTime;
            if (Lifetime < 0f)
                Lifetime = 0f;

            float progress = 1.0f - (Lifetime / MaxLifetime);

            // Update position and color alpha
            _text.Position = Vector2.Lerp(StartPosition, EndPosition, progress);

            float currentAlpha = MathHelper.Lerp(StartAlpha, EndAlpha, progress);
            _text.Color = BaseColor * currentAlpha;
        }

        /// <summary>
        /// Renders the floating text to the screen using the provided <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spritebatch">The sprite batch used for drawing graphics.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (!IsAlive)
                return;

            _text?.Draw(spritebatch);
        }
    }
}