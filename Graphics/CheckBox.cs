using DinaCSharp.Core;
using DinaCSharp.Enums;
using DinaCSharp.Events;
using DinaCSharp.Extensions;
using DinaCSharp.Interfaces;
using DinaCSharp.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace DinaCSharp.Graphics
{
    /// <summary>
    /// Classe représentant une case à cocher graphique interactive.
    /// </summary>
    public class CheckBox : Base, IUpdate, IDraw, IVisible, ICopyable<CheckBox>, IUIStateful
    {
        private Rectangle _checkBoxRect;
        private Texture2D? _checkedTexture;
        private Texture2D? _uncheckedTexture;
        private MouseState _oldMouseState;

        /// <summary>
        /// Initialise une nouvelle instance de la classe CheckBox avec des couleurs.
        /// </summary>
        /// <param name="checkedColor">Couleur de la case cochée.</param>
        /// <param name="uncheckedColor">Couleur de la case non cochée.</param>
        /// <param name="position">Position de la case à cocher.</param>
        /// <param name="dimensions">Dimensions de la case à cocher.</param>
        /// <param name="zorder">Ordre de dessin de la case.</param>
        public CheckBox(Color checkedColor, Color uncheckedColor, Vector2 position, Vector2 dimensions, int zorder = 0) :
            base(position, dimensions, zorder)
        {
            CheckedColor = checkedColor;
            UncheckedColor = uncheckedColor;
            Position = position;
            Initialize();
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe CheckBox avec des textures.
        /// </summary>
        /// <param name="uncheckedTexture">Texture utilisée pour la case non cochée.</param>
        /// <param name="checkedTexture">Texture utilisée pour la case cochée.</param>
        /// <param name="position">Position de la case à cocher.</param>
        /// <param name="dimensions">Dimensions de la case à cocher.</param>
        /// <param name="zorder">Ordre de dessin de la case.</param>
        public CheckBox(Texture2D checkedTexture, Texture2D uncheckedTexture, Vector2 position, Vector2 dimensions, int zorder = 0) :
            base(position, dimensions, zorder)
        {
            _uncheckedTexture = uncheckedTexture;
            _checkedTexture = checkedTexture;
            Position = position;
            Initialize();
        }
        private void Initialize()
        {
            LockedColor = Color.White * 0.75f;
            Visible = true;
            State = CheckBoxState.Unchecked;
            UIState = UIState.Normal;
        }
        /// <summary>
        /// Position de la case à cocher.
        /// </summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                _checkBoxRect = new Rectangle(base.Position.ToPoint(), base.Dimensions.ToPoint());
            }
        }
        /// <summary>
        /// Dimensions de la case à cocher.
        /// </summary>
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                _checkBoxRect = new Rectangle(base.Position.ToPoint(), base.Dimensions.ToPoint());
            }
        }

        /// <summary>
        /// Indique si la case à cocher est visible.
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// Indique si la case à cocher est verrouillée.
        /// </summary>
        public UIState UIState { get; set; }
        /// <summary>
        /// Couleur à utiliser lorsque la case est verrouillée.
        /// </summary>
        public Color LockedColor { get; set; }
        /// <summary>
        /// État actuel de la case à cocher (cochée ou non cochée).
        /// </summary>
        public CheckBoxState State { get; set; }
        /// <summary>
        /// Couleur à utiliser quand la case est survolée/focusée.
        /// </summary>
        public Color HoverColor { get; set; }

        /// <summary>
        /// Événement déclenché lorsqu'on clique sur la case à cocher. Utile pour réagir immédiatement au changement d'état par l'utilisateur.
        /// </summary>
        public event EventHandler<CheckBoxEventArgs>? OnClicked;
        /// <summary>
        /// Événement déclenché lorsque la case à cocher est survolée.
        /// </summary>
        public event EventHandler<CheckBoxEventArgs>? OnHovered;

        /// <summary>
        /// Obtient ou définit l'état coché de la case. True si la case est cochée, false sinon.
        /// Cette propriété simplifie l'accès à l'état en évitant de manipuler l'énumération CheckBoxState directement.
        /// </summary>
        public bool IsChecked
        {
            get => State == CheckBoxState.Checked;
            set => State = value ? CheckBoxState.Checked : CheckBoxState.Unchecked;
        }
        /// <summary>
        /// Couleur quand la case est cochée.
        /// </summary>
        public Color CheckedColor { get; set; } = Color.White;
        /// <summary>
        /// Couleur quand la case est décochée.
        /// </summary>
        public Color UncheckedColor { get; set; } = Color.White;

        /// <summary>
        /// Texture quand la case est cochée.
        /// </summary>
        public Texture2D? CheckedTexture { get => _checkedTexture; set => _checkedTexture = value; }
        /// <summary>
        /// Texture quand la case est décochée.
        /// </summary>
        public Texture2D? UncheckedTexture { get => _uncheckedTexture; set => _uncheckedTexture = value; }

        /// <summary>
        /// Met à jour l'état de la case à cocher (gestion des clics et des interactions).
        /// </summary>
        /// <param name="gametime">Temps de jeu écoulé depuis la dernière mise à jour.</param>
        public void Update(GameTime gametime)
        {
            MouseState ms = Mouse.GetState();
            if (UIState == UIState.Locked)
            {
                _oldMouseState = ms;
                return;
            }

            bool IsMouseOver = _checkBoxRect.Contains(new Point(ms.X, ms.Y));

            if (IsMouseOver)
            {
                if (ms.LeftButton == ButtonState.Pressed)
                    UIState = UIState.Pressed;
                else
                {
                    OnHovered?.Invoke(this, new CheckBoxEventArgs(this));
                    UIState = UIState.Hovered;
                }

                if (_oldMouseState.LeftButton == ButtonState.Pressed && ms.LeftButton == ButtonState.Released)
                {
                    IsChecked = !IsChecked; // on inverse l'état
                    OnClicked?.Invoke(this, new CheckBoxEventArgs(this));
                }
            }
            else
            {
                if (UIState != UIState.Normal)
                    UIState = UIState.Normal;
            }
            _oldMouseState = ms;
        }
        /// <summary>
        /// Dessine la case à cocher sur l'écran.
        /// </summary>
        /// <param name="spritebatch">Objet SpriteBatch utilisé pour dessiner la case.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            if (!Visible)
                return;

            Texture2D? texture = State == CheckBoxState.Checked ? _checkedTexture : _uncheckedTexture;

            Color color;

            switch (UIState)
            {
                case UIState.Locked:
                    color = LockedColor;
                    break;
                case UIState.Hovered:
                case UIState.Pressed:
                    color = HoverColor;
                    break;
                case UIState.Normal:
                    color = State == CheckBoxState.Checked ? CheckedColor : UncheckedColor;
                    break;
                default:
                    color = UncheckedColor;
                    break;
            }

            bool filled = State == CheckBoxState.Checked && texture == null;

            Texture2D pixel = ServiceLocator.Get<Texture2D>(ServiceKeys.Texture1px)
                ?? throw new InvalidOperationException("Texture1px n'est pas enregistré dans le ServiceLocator.");
            spritebatch.DrawRectangle(pixel, _checkBoxRect, color, isFilled: filled);

            if (texture != null)
                spritebatch.Draw(texture, _checkBoxRect, color);
        }

        /// <summary>
        /// Crée une copie de la case à cocher actuelle.
        /// </summary>
        /// <returns>Nouvelle instance de CheckBox avec les mêmes propriétés.</returns>
        public CheckBox Copy()
        {
            return new CheckBox()
            {
                _checkBoxRect = _checkBoxRect,
                CheckedColor = CheckedColor,
                _checkedTexture = _checkedTexture,
                UncheckedColor = UncheckedColor,
                _uncheckedTexture = _uncheckedTexture,
                Dimensions = Dimensions,
                Position = Position,
                State = State,
                Visible = Visible,
                ZOrder = ZOrder,
                UIState = UIState,
                HoverColor = HoverColor,
                LockedColor = LockedColor,
            };
        }
        private CheckBox() { }
    }
}
