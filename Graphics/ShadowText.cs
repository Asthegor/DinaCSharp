using DinaCSharp.Core.Enums;
using DinaCSharp.Core.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DinaCSharp.Graphics
{
    /// <summary>
    /// Représente un texte avec une ombre portée.
    /// Hérite de <see cref="Text"/> pour le texte principal et maintient
    /// une instance <see cref="Text"/> interne pour l'ombre.
    /// </summary>
    /// <remarks>
    /// Initialise un <see cref="ShadowText"/>.
    /// </remarks>
    public class ShadowText : Text, ICopyable<ShadowText>
    {
        private readonly Text _shadow;
        private Vector2 _offset;
        /// <summary>
        /// Initialise une nouvelle instance de la classe ShadowText avec les paramètres spécifiés.
        /// </summary>
        /// <param name="font">Police de caractères.</param>
        /// <param name="content">Contenu textuel.</param>
        /// <param name="color">Couleur du texte principal.</param>
        /// <param name="position">Position du texte principal à l'écran.</param>
        /// <param name="shadowColor">Couleur de l'ombre.</param>
        /// <param name="shadowOffset">Décalage de l'ombre par rapport au texte.</param>
        /// <param name="halign">Alignement horizontal (défaut : Left).</param>
        /// <param name="valign">Alignement vertical (défaut : Top).</param>
        /// <param name="zorder">Ordre de superposition (défaut : 0). L'ombre est toujours à <c>zorder - 1</c>.</param>
        public ShadowText(SpriteFont font, string content, Color color, Color shadowColor, Vector2 shadowOffset,
                          Vector2 position = default, HorizontalAlignment halign = default, VerticalAlignment valign = default, int zorder = 0) : base(font, content, color, position, halign, valign, zorder)
        {
            _shadow = new Text(font, content, shadowColor, position + shadowOffset, halign, valign, zorder - 1);
            _offset = shadowOffset;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Propriétés spécifiques à l'ombre
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Couleur de l'ombre.</summary>
        public Color ShadowColor
        {
            get => _shadow.Color;
            set => _shadow.Color = value;
        }

        /// <summary>
        /// Décalage de l'ombre par rapport au texte principal.
        /// La position de l'ombre est resynchronisée immédiatement après modification.
        /// </summary>
        public Vector2 Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                if (_shadow != null)
                    _shadow.Position = Position + _offset;
            }
        }

        /// <summary>
        /// Encombrement visuel total, c'est-à-dire les dimensions du texte augmentées du décalage de l'ombre.
        /// Utile pour les calculs de mise en page qui doivent tenir compte de l'ombre.
        /// </summary>
        public Vector2 VisualDimensions => Dimensions + _offset;

        /// <summary>
        /// Ordre de superposition du texte principal.
        /// L'ombre est automatiquement placée à <c>ZOrder - 1</c>.
        /// </summary>
        public new int ZOrder
        {
            get => base.ZOrder;
            set
            {
                base.ZOrder = value;
                if (_shadow != null)
                    _shadow.ZOrder = value - 1;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Overrides — synchronisation de l'ombre
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Le contenu du texte et de son ombre.</summary>
        public override string Content
        {
            get => base.Content;
            set
            {
                base.Content = value;
                if (_shadow != null)
                    _shadow.Content = value;
            }
        }

        /// <summary>Indique si le texte et son ombre sont visibles.</summary>
        public override bool Visible
        {
            get => base.Visible;
            set
            {
                base.Visible = value;
                if (_shadow != null)
                    _shadow.Visible = value;
            }
        }

        /// <summary>La police du texte et de son ombre.</summary>
        public override SpriteFont Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                if (_shadow != null)
                    _shadow.Font = value;
            }
        }

        /// <summary>
        /// La position du texte principal à l'écran.
        /// La position de l'ombre est recalculée automatiquement en appliquant <see cref="Offset"/>.
        /// </summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                if (_shadow != null)
                    _shadow.Position = value + _offset;
            }
        }

        /// <summary>Dimensions du conteneur du texte et de son ombre.</summary>
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                if (_shadow != null)
                    _shadow.Dimensions = value;
            }
        }

        /// <summary>Dessine l'ombre puis le texte principal.</summary>
        public override void Draw(SpriteBatch spritebatch)
        {
            _shadow?.Draw(spritebatch);
            base.Draw(spritebatch);
        }

        /// <summary>Met à jour l'état de l'ombre puis du texte principal en fonction du temps écoulé.</summary>
        public override void Update(GameTime gametime)
        {
            _shadow?.Update(gametime);
            base.Update(gametime);
        }

        /// <summary>Configure les temporisations d'affichage du texte et de son ombre.</summary>
        /// <param name="waitTime">Délai avant affichage (s). ≤ 0 : aucun délai.</param>
        /// <param name="displayTime">Durée d'affichage (s). ≤ 0 : illimité.</param>
        /// <param name="nbLoops">Nombre de cycles. -1 : infini.</param>
        public override void SetTimers(float waitTime = -1f, float displayTime = -1f, int nbLoops = -1)
        {
            _shadow?.SetTimers(waitTime, displayTime, nbLoops);
            base.SetTimers(waitTime, displayTime, nbLoops);
        }

        /// <summary>Définit les alignements horizontal et vertical du texte et de son ombre.</summary>
        /// <param name="horizontalAlignment">L'alignement horizontal du texte.</param>
        /// <param name="verticalAlignment">L'alignement vertical du texte.</param>
        public override void SetAlignments(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            _shadow?.SetAlignments(horizontalAlignment, verticalAlignment);
            base.SetAlignments(horizontalAlignment, verticalAlignment);
        }

        // ─────────────────────────────────────────────────────────────────────
        // ICopyable<ShadowText>
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Crée une copie complète de l'instance, état des timers inclus.
        /// </summary>
        public new ShadowText Copy()
        {
            var copy = new ShadowText(Font, Content, Color, ShadowColor, Offset, Position, zorder: ZOrder)
            {
                Dimensions = Dimensions,
                Visible = Visible,
                // TextTimer est une struct : l'assignation produit une copie profonde de l'état.
                // On restaure après Visible= pour annuler le Timer.Reset() déclenché par le setter.
                Timer = Timer
            };
            copy._shadow.Timer = _shadow.Timer;

            return copy;
        }

        // ─────────────────────────────────────────────────────────────────────
        // IDisposable
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Libère les ressources de l'ombre, puis délègue au parent.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _shadow.Dispose();

            base.Dispose(disposing);
        }
    }
}
