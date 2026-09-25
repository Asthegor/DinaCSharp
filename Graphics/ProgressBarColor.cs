using DinaCSharp.Core;
using DinaCSharp.Core.Enums;
using DinaCSharp.Core.Interfaces;
using DinaCSharp.Extensions;
using DinaCSharp.Services;
using DinaCSharp.Services.Keys;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaCSharp.Graphics
{
    /// <summary>
    /// Represents a progress bar rendered entirely using solid colors and primitive rectangles.
    /// </summary>
    public class ProgressBarColor : Base, IProgressBar, ICopyable<ProgressBarColor>, IDisposable
    {
        private bool _visible;
        private float _value;
        private float _minValue;
        private float _maxValue;
        private int _borderThickness;

        private Color _frontColor;
        private Color _backColor;
        private Color _borderColor;

        private Rectangle _borderRectangle;
        private Rectangle _innerRectangle;
        private Rectangle _frontRectangle;

        private ProgressDirection _mode;

        private bool _autoIncrement;

        // --- Smooth Animation ---
        private float _targetValue;
        private float _animationSpeed; // Units per second
        private bool _isAnimating;

        // --- Step Interval Progress ---
        private float _timer;
        private float _delay;
        private float _increment;

        private bool _disposed;

        private readonly Texture2D? _pixelTexture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBarColor"/> class.
        /// </summary>
        /// <param name="value">The initial value of the progress bar.</param>
        /// <param name="minValue">The minimum value of the progress bar.</param>
        /// <param name="maxValue">The maximum value of the progress bar.</param>
        /// <param name="position">The position of the progress bar on screen.</param>
        /// <param name="dimensions">The overall width and height of the progress bar.</param>
        /// <param name="frontColor">The fill color representing current progress.</param>
        /// <param name="borderColor">The border outline color.</param>
        /// <param name="backColor">The background container color behind the progress fill.</param>
        /// <param name="borderThickness">The thickness of the outer border in pixels.</param>
        /// <param name="mode">The progress fill direction mode.</param>
        /// <param name="zorder">The rendering depth order (Z-Order).</param>
        /// <exception cref="InvalidOperationException">Thrown when the required 1x1 pixel texture service is missing.</exception>
        public ProgressBarColor(
            float value,
            float minValue,
            float maxValue,
            Vector2 position,
            Vector2 dimensions,
            Color frontColor,
            Color borderColor,
            Color backColor,
            int borderThickness = 1,
            ProgressDirection mode = ProgressDirection.LeftToRight,
            int zorder = 0)
            : base(position, dimensions, zorder)
        {
            _visible = true;
            _mode = mode;
            _maxValue = maxValue;
            _minValue = minValue;
            _borderThickness = borderThickness;

            _frontColor = frontColor;
            _borderColor = borderColor;
            _backColor = backColor;

            Value = value;

            _pixelTexture = ServiceLocator.Get<Texture2D>(DinaServiceKeys.Texture1px)
                ?? throw new InvalidOperationException("The Texture1px service is not available.");
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
        /// Gets or sets the fill color representing the current progress.
        /// </summary>
        public Color FrontColor { get => _frontColor; set => _frontColor = value; }

        /// <summary>
        /// Gets or sets the background color behind the progress bar fill.
        /// </summary>
        public Color BackColor { get => _backColor; set => _backColor = value; }

        /// <summary>
        /// Gets or sets the color of the outer border frame.
        /// </summary>
        public Color BorderColor { get => _borderColor; set => _borderColor = value; }

        /// <summary>
        /// Gets or sets the thickness of the outer border in pixels.
        /// </summary>
        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = Math.Max(0, value);
                UpdateRectangles();
            }
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
        /// <param name="durationInSeconds">The animation duration in seconds.</param>
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

            // 1. Smooth fill animation
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

            // 2. Automatic step interval progress
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
        /// Draws the colored progress bar rectangles using the provided <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spritebatch">The sprite batch renderer.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (!_visible || _pixelTexture == null)
                return;

            // 1. Draw inner background area
            spritebatch.DrawRectangle(_pixelTexture, _innerRectangle, _backColor, isFilled: true);

            // 2. Draw front progress fill
            if (_frontRectangle.Width > 0 && _frontRectangle.Height > 0)
            {
                spritebatch.DrawRectangle(_pixelTexture, _frontRectangle, _frontColor, isFilled: true);
            }

            // 3. Draw outer border frame on top
            if (_borderThickness > 0)
            {
                spritebatch.DrawRectangle(_pixelTexture, _borderRectangle, _borderColor, thickness: _borderThickness, isFilled: false);
            }
        }

        #endregion

        #region Internal Logic

        /// <summary>
        /// Recalculates and updates inner background, border, and front fill rectangles based on current values.
        /// </summary>
        private void UpdateRectangles()
        {
            int posX = (int)Position.X;
            int posY = (int)Position.Y;
            int width = (int)Dimensions.X;
            int height = (int)Dimensions.Y;

            // Overall boundary rectangle (outer border)
            _borderRectangle = new Rectangle(posX, posY, width, height);

            // Interior area inside border padding
            int innerX = posX + _borderThickness;
            int innerY = posY + _borderThickness;
            int innerWidth = Math.Max(0, width - (_borderThickness * 2));
            int innerHeight = Math.Max(0, height - (_borderThickness * 2));

            _innerRectangle = new Rectangle(innerX, innerY, innerWidth, innerHeight);

            // Progress ratio [0..1]
            float range = _maxValue - _minValue;
            float ratio = range > 0f ? (_value - _minValue) / range : 0f;

            float frontX = innerX;
            float frontY = innerY;
            float frontWidth = innerWidth;
            float frontHeight = innerHeight;

            switch (_mode)
            {
                case ProgressDirection.LeftToRight:
                    frontWidth = innerWidth * ratio;
                    break;

                case ProgressDirection.RightToLeft:
                    frontWidth = innerWidth * ratio;
                    frontX = innerX + innerWidth - frontWidth;
                    break;

                case ProgressDirection.TopToBottom:
                    frontHeight = innerHeight * ratio;
                    break;

                case ProgressDirection.BottomToTop:
                    frontHeight = innerHeight * ratio;
                    frontY = innerY + innerHeight - frontHeight;
                    break;
            }

            _frontRectangle = new Rectangle(
                (int)Math.Round(frontX),
                (int)Math.Round(frontY),
                (int)Math.Round(frontWidth),
                (int)Math.Round(frontHeight)
            );
        }

        #endregion

        #region ICopyable & IDisposable

        /// <summary>
        /// Creates a deep copy of the current <see cref="ProgressBarColor"/> instance.
        /// </summary>
        /// <returns>A new <see cref="ProgressBarColor"/> instance with matching properties.</returns>
        public ProgressBarColor Copy()
        {
            var copy = new ProgressBarColor(
                _value, _minValue, _maxValue, Position, Dimensions,
                _frontColor, _borderColor, _backColor,
                _borderThickness, _mode, ZOrder)
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
        /// Releases resources used by the <see cref="ProgressBarColor"/> instance.
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