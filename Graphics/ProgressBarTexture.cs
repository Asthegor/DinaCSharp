using DinaCSharp.Core;
using DinaCSharp.Enums;
using DinaCSharp.Core.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaCSharp.Graphics
{
    /// <summary>
    /// Represents a texture-based progress bar rendered using source rect clipping.
    /// </summary>
    public class ProgressBarTexture : Base, IProgressBar, ICopyable<ProgressBarTexture>, IDisposable
    {
        private bool _visible;
        private float _value;
        private float _minValue;
        private float _maxValue;

        private Texture2D _backTexture;
        private Texture2D _frontTexture;
        private Texture2D? _overlayTexture;

        private Rectangle _backDestinationRect;
        private Rectangle _frontDestinationRect;
        private Rectangle _frontSourceRect;
        private Rectangle _overlayDestinationRect;

        private ProgressDirection _mode;

        // --- Smooth Animation ---
        private float _targetValue;
        private float _animationSpeed;
        private bool _isAnimating;

        // --- Step Interval Progress ---
        private float _timer;
        private float _delay;
        private float _increment;
        private bool _autoIncrement;

        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBarTexture"/> class.
        /// </summary>
        /// <param name="value">The initial value of the progress bar.</param>
        /// <param name="minValue">The minimum value of the progress bar.</param>
        /// <param name="maxValue">The maximum value of the progress bar.</param>
        /// <param name="position">The position of the progress bar on screen.</param>
        /// <param name="dimensions">The overall dimensions. If <see cref="Vector2.Zero"/>, dimensions are resolved from the background or fill texture.</param>
        /// <param name="frontTexture">The texture used for the progress fill bar.</param>
        /// <param name="backTexture">The texture used for the background element.</param>
        /// <param name="overlayTexture">Optional frame or overlay texture rendered on top.</param>
        /// <param name="mode">The progress fill direction direction.</param>
        /// <param name="zorder">The rendering depth order (Z-Order).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="frontTexture"/> or <paramref name="backTexture"/> is null.</exception>
        public ProgressBarTexture(
            float value,
            float minValue,
            float maxValue,
            Vector2 position,
            Vector2 dimensions,
            Texture2D frontTexture,
            Texture2D backTexture,
            Texture2D? overlayTexture = null,
            ProgressDirection mode = ProgressDirection.LeftToRight,
            int zorder = 0)
            : base(position, ResolveDimensions(dimensions, backTexture, frontTexture), zorder)
        {
            _frontTexture = frontTexture ?? throw new ArgumentNullException(nameof(frontTexture));
            _backTexture = backTexture ?? throw new ArgumentNullException(nameof(backTexture));
            _overlayTexture = overlayTexture;

            _visible = true;
            _mode = mode;
            _maxValue = maxValue;
            _minValue = minValue;

            _targetValue = Math.Clamp(value, _minValue, _maxValue);
            Value = value;
        }

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether automatic interval step incrementation is active.
        /// </summary>
        public bool AutoIncrement { get => _autoIncrement; set => _autoIncrement = value; }

        /// <summary>
        /// Gets or sets a value indicating whether the progress bar is visible.
        /// </summary>
        public bool Visible { get => _visible; set => _visible = value; }

        /// <summary>
        /// Gets or sets the delay in seconds between each step when <see cref="AutoIncrement"/> is active.
        /// </summary>
        public float Delay { get => _delay; set => _delay = value; }

        /// <summary>
        /// Gets or sets the value added at each step interval when <see cref="AutoIncrement"/> is active.
        /// </summary>
        public float Increment { get => _increment; set => _increment = value; }

        /// <summary>
        /// Gets or sets the fill direction mode for the progress bar.
        /// </summary>
        public ProgressDirection Mode { get => _mode; set => _mode = value; }

        /// <summary>
        /// Gets a value indicating whether the progress bar is currently undergoing a smooth fill animation.
        /// </summary>
        public bool IsAnimating => _isAnimating;

        /// <summary>
        /// Gets or sets the background texture.
        /// </summary>
        public Texture2D BackTexture
        {
            get => _backTexture;
            set { _backTexture = value; UpdateRectangles(); }
        }

        /// <summary>
        /// Gets or sets the front fill texture.
        /// </summary>
        public Texture2D FrontTexture
        {
            get => _frontTexture;
            set { _frontTexture = value; UpdateRectangles(); }
        }

        /// <summary>
        /// Gets or sets the optional frame or overlay texture.
        /// </summary>
        public Texture2D? OverlayTexture
        {
            get => _overlayTexture;
            set { _overlayTexture = value; UpdateRectangles(); }
        }

        /// <summary>
        /// Gets or sets the screen position of the progress bar.
        /// </summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                UpdateRectangles();
            }
        }

        /// <summary>
        /// Gets or sets the overall dimensions of the progress bar.
        /// </summary>
        public new Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                UpdateRectangles();
            }
        }

        /// <summary>
        /// Gets or sets the minimum allowed value.
        /// </summary>
        public float MinValue
        {
            get => _minValue;
            set
            {
                if (value < _maxValue)
                {
                    _minValue = value;
                    Value = _value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum allowed value.
        /// </summary>
        public float MaxValue
        {
            get => _maxValue;
            set
            {
                if (value > _minValue)
                {
                    _maxValue = value;
                    Value = _value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current value of the progress bar.
        /// Assigning a value directly cancels any active smooth animation.
        /// </summary>
        public float Value
        {
            get => _value;
            set
            {
                _value = Math.Clamp(value, _minValue, _maxValue);
                _targetValue = _value;
                _isAnimating = false;
                UpdateRectangles();
            }
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Smoothly animates the progress bar toward a target value over a specified duration.
        /// </summary>
        /// <param name="targetValue">The target value to reach.</param>
        /// <param name="durationInSeconds">The duration of the animation in seconds.</param>
        public void AnimateTo(float targetValue, float durationInSeconds)
        {
            _targetValue = Math.Clamp(targetValue, _minValue, _maxValue);

            if (durationInSeconds <= 0f || Math.Abs(_targetValue - _value) < 0.0001f)
            {
                Value = _targetValue;
                return;
            }

            _animationSpeed = (_targetValue - _value) / durationInSeconds;
            _isAnimating = true;
            _autoIncrement = false;
        }

        /// <summary>
        /// Configures automatic step-by-step interval progress.
        /// </summary>
        /// <param name="delay">Time in seconds between each step interval.</param>
        /// <param name="increment">Amount added to the progress bar value at each interval.</param>
        public void ProgressInterval(float delay, float increment = 1f)
        {
            if (delay > 0 && increment != 0)
            {
                _delay = delay;
                _increment = increment;
                _autoIncrement = true;
                _isAnimating = false;
            }
        }

        /// <summary>
        /// Updates the progress bar logic (smooth animation or automatic step interval logic).
        /// </summary>
        /// <param name="gametime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gametime)
        {
            ArgumentNullException.ThrowIfNull(gametime);

            if (!_visible)
                return;

            float deltaTime = (float)gametime.ElapsedGameTime.TotalSeconds;

            if (_isAnimating)
            {
                _value += _animationSpeed * deltaTime;

                if ((_animationSpeed > 0 && _value >= _targetValue) ||
                    (_animationSpeed < 0 && _value <= _targetValue))
                {
                    _value = _targetValue;
                    _isAnimating = false;
                }

                UpdateRectangles();
                return;
            }

            if (_autoIncrement && _delay > 0)
            {
                _timer += deltaTime;
                if (_timer >= _delay)
                {
                    _timer -= _delay;
                    Value += _increment;
                }
            }
        }

        /// <summary>
        /// Draws the progress bar using the provided <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spritebatch">The XNA/MonoGame sprite batch renderer.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            if (!_visible)
                return;

            // 1. Background texture
            if (_backTexture != null)
            {
                spritebatch.Draw(_backTexture, _backDestinationRect, Color.White);
            }

            // 2. Fill texture (using clipped source rect)
            if (_frontTexture != null && _frontDestinationRect.Width > 0 && _frontDestinationRect.Height > 0)
            {
                spritebatch.Draw(_frontTexture, _frontDestinationRect, _frontSourceRect, Color.White);
            }

            // 3. Optional border / frame texture
            if (_overlayTexture != null)
            {
                spritebatch.Draw(_overlayTexture, _overlayDestinationRect, Color.White);
            }
        }
        #endregion

        #region Internal Logic

        /// <summary>
        /// Recalculates and updates destination and source clipping rectangles based on current value and direction mode.
        /// </summary>
        private void UpdateRectangles()
        {
            int posX = (int)Position.X;
            int posY = (int)Position.Y;
            int width = (int)Dimensions.X;
            int height = (int)Dimensions.Y;

            _backDestinationRect = new Rectangle(posX, posY, width, height);
            _overlayDestinationRect = _backDestinationRect;

            float range = _maxValue - _minValue;
            float ratio = range > 0f ? (_value - _minValue) / range : 0f;

            int texWidth = _frontTexture.Width;
            int texHeight = _frontTexture.Height;

            float destX = posX;
            float destY = posY;
            float destW = width;
            float destH = height;

            float srcX = 0;
            float srcY = 0;
            float srcW = texWidth;
            float srcH = texHeight;

            switch (_mode)
            {
                case ProgressDirection.LeftToRight:
                    destW = width * ratio;
                    srcW = texWidth * ratio;
                    break;

                case ProgressDirection.RightToLeft:
                    destW = width * ratio;
                    destX = posX + width - destW;

                    srcW = texWidth * ratio;
                    srcX = texWidth - srcW;
                    break;

                case ProgressDirection.TopToBottom:
                    destH = height * ratio;
                    srcH = texHeight * ratio;
                    break;

                case ProgressDirection.BottomToTop:
                    destH = height * ratio;
                    destY = posY + height - destH;

                    srcH = texHeight * ratio;
                    srcY = texHeight - srcH;
                    break;
            }

            _frontDestinationRect = new Rectangle(
                (int)Math.Round(destX),
                (int)Math.Round(destY),
                (int)Math.Round(destW),
                (int)Math.Round(destH)
            );

            _frontSourceRect = new Rectangle(
                (int)Math.Round(srcX),
                (int)Math.Round(srcY),
                (int)Math.Round(srcW),
                (int)Math.Round(srcH)
            );
        }

        /// <summary>
        /// Automatically resolves initial dimensions if zero values are passed, defaulting to texture sizes.
        /// </summary>
        private static Vector2 ResolveDimensions(Vector2 dimensions, Texture2D backTex, Texture2D frontTex)
        {
            if (dimensions != Vector2.Zero)
                return dimensions;

            if (backTex != null)
                return new Vector2(backTex.Width, backTex.Height);

            if (frontTex != null)
                return new Vector2(frontTex.Width, frontTex.Height);

            return Vector2.Zero;
        }

        #endregion

        #region ICopyable & IDisposable

        /// <summary>
        /// Creates a deep copy of the current <see cref="ProgressBarTexture"/> instance.
        /// </summary>
        /// <returns>A new <see cref="ProgressBarTexture"/> instance with matching properties.</returns>
        public ProgressBarTexture Copy()
        {
            var copy = new ProgressBarTexture(
                _value, _minValue, _maxValue, Position, Dimensions,
                _frontTexture, _backTexture, _overlayTexture,
                _mode, ZOrder)
            {
                Visible = _visible,
                AutoIncrement = _autoIncrement,
                Delay = _delay,
                Increment = _increment
            };

            if (_isAnimating)
            {
                float remainingTime = Math.Abs((_targetValue - _value) / _animationSpeed);
                copy.AnimateTo(_targetValue, remainingTime);
            }

            return copy;
        }

        /// <summary>
        /// Releases resources used by the <see cref="ProgressBarTexture"/> instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and optionally managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        #endregion
    }
}