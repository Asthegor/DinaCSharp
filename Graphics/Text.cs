using DinaCSharp.Core;
using DinaCSharp.Core.Interfaces;
using DinaCSharp.Enums;
using DinaCSharp.Events;
using DinaCSharp.Extensions;
using DinaCSharp.Services.Localization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Text;

namespace DinaCSharp.Graphics
{
    /// <summary>
    /// Représente un texte à afficher avec des options de temporisation, d'alignement et de retour à la ligne.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1724:TypeNamesShouldNotMatchNamespaces",
        Justification = "Text est clair dans le contexte du framework. System.Drawing n'est pas référencé.")]
    public class Text : Base, IUpdate, IDraw, IColor, IVisible, IText, ICopyable<Text>, IDrawingElement, IDisposable
    {
        // ── Rendu ─────────────────────────────────────────────────────────────
        private SpriteFont _font;
        private string     _content;

        /// <summary>
        /// Cache de la traduction de <see cref="_content"/>.
        /// Mis à jour dans <see cref="RefreshContent"/> — jamais recalculé à chaque frame.
        /// </summary>
        private string _cachedTranslation = string.Empty;

        private string _wrappedContent = string.Empty;
        private Color  _color;
        private bool   _visible;

        // ── Alignement / position ─────────────────────────────────────────────
        private HorizontalAlignment _halign;
        private VerticalAlignment   _valign;
        private Vector2             _displayposition;

        // ── Effets de rendu ───────────────────────────────────────────────────
        private float                  _rotation;
        private Vector2                _origin  = Vector2.Zero;
        private readonly SpriteEffects _effects = SpriteEffects.None;

        // ── Temporisation ─────────────────────────────────────────────────────
        /// <summary>
        /// État complet du timer d'affichage.
        /// Étant une struct, son assignation produit une copie profonde — simplifie <see cref="Copy"/>.
        /// </summary>
        internal TextTimer Timer;

        // ── Cycle de vie ──────────────────────────────────────────────────────
        private bool _disposed;

        // ─────────────────────────────────────────────────────────────────────
        // Propriétés publiques
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Le contenu du texte.</summary>
        public virtual string Content
        {
            get => _content;
            set
            {
                _content = value;
                RefreshContent();
                NotifyPropertyChanged();
            }
        }

        /// <summary>La couleur du texte.</summary>
        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        /// <summary>Indique si le texte est visible.</summary>
        public virtual bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                Timer.Reset(); // Remet à zéro les accumulateurs, mais pas la machine d'état
                NotifyPropertyChanged();
            }
        }

        /// <summary>La position du texte sur l'écran.</summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                UpdateDisplayPosition();
                NotifyPropertyChanged();
            }
        }

        /// <summary>Dimensions du conteneur de texte.</summary>
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                RefreshContent();
                UpdateDisplayPosition();
                NotifyPropertyChanged();
            }
        }

        /// <summary>La police du texte.</summary>
        public virtual SpriteFont Font
        {
            get => _font;
            set
            {
                _font = value;
                RefreshContent();
                UpdateDisplayPosition();
                NotifyPropertyChanged();
            }
        }

        /// <summary>Angle de rotation du texte en radians.</summary>
        public float Rotation
        {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }

        /// <summary>Active ou désactive le retour automatique à la ligne.</summary>
        public bool Wrap { get; set; }

        /// <summary>Dimensions réelles du texte rendu (indépendantes du conteneur).</summary>
        public Vector2 TextDimensions => _font?.MeasureString(_wrappedContent) ?? Vector2.Zero;

        // ─────────────────────────────────────────────────────────────────────
        // Événements
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Déclenché chaque frame où la souris survole la zone du texte.</summary>
        public event EventHandler<TextEventArgs>? OnHovered;

        // ─────────────────────────────────────────────────────────────────────
        // Constructeurs
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Initialise un texte blanc, positionné à l'origine, aligné en haut à gauche.
        /// </summary>
        public Text(SpriteFont font, string content)
            : this(font, content, Color.White, Vector2.Zero, HorizontalAlignment.Left, VerticalAlignment.Top, zorder: 0) { }

        /// <summary>
        /// Initialise un texte avec tous les paramètres.
        /// </summary>
        /// <param name="font">Police de caractères.</param>
        /// <param name="content">Contenu textuel.</param>
        /// <param name="color">Couleur du texte.</param>
        /// <param name="position">Position à l'écran.</param>
        /// <param name="horizontalalignment">Alignement horizontal (défaut : Left).</param>
        /// <param name="verticalalignment">Alignement vertical (défaut : Top).</param>
        /// <param name="zorder">Ordre de superposition (défaut : 0).</param>
        public Text(SpriteFont font, string content, Color color, Vector2 position = default,
                    HorizontalAlignment horizontalalignment = HorizontalAlignment.Left,
                    VerticalAlignment   verticalalignment   = VerticalAlignment.Top,
                    int zorder = 0)
        {
            ArgumentNullException.ThrowIfNull(font);

            _font    = font;
            _content = content;
            _color   = color;
            _visible = true;
            Timer    = new TextTimer();

            Position = position;

            // Initialise les dimensions sur la taille mesurée du texte initial.
            // base.Dimensions est utilisé ici délibérément pour éviter de déclencher
            // RefreshContent() via le setter surchargé avant que tout soit initialisé.
            base.Dimensions = _font.MeasureString(LocalizationManager.GetTranslation(content));

            RefreshContent();
            _halign = horizontalalignment;
            _valign = verticalalignment;
            UpdateDisplayPosition();
            ZOrder = zorder;
        }

        // ─────────────────────────────────────────────────────────────────────
        // API publique
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Configure les temporisations d'affichage du texte.
        /// </summary>
        /// <param name="waitTime">Délai avant affichage (s). ≤ 0 : aucun délai.</param>
        /// <param name="displayTime">Durée d'affichage (s). ≤ 0 : illimité.</param>
        /// <param name="nbLoops">Nombre de cycles. -1 : infini.</param>
        public virtual void SetTimers(float waitTime = -1f, float displayTime = -1f, int nbLoops = -1)
            => Timer.Configure(waitTime, displayTime, nbLoops);

        /// <summary>Définit les alignements horizontal et vertical du texte.</summary>
        public virtual void SetAlignments(HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
                                          VerticalAlignment   verticalAlignment   = VerticalAlignment.Top)
        {
            _halign = horizontalAlignment;
            _valign = verticalAlignment;
            UpdateDisplayPosition();
        }

        /// <summary>Dessine le texte à l'écran.</summary>
        public virtual void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            if (!_visible || !Timer.IsDisplayed)
                return;

            if (Wrap)
            {
                spritebatch.DrawString(_font, _wrappedContent, _displayposition, _color.PreMultiply());
            }
            else
            {
                // _cachedTranslation évite un appel à GetTranslation() à chaque frame
                float zorder = ZOrder > 0
                    ? 0.5f + (ZOrder / MAX_ZORDER)
                    : ZOrder / MIN_ZORDER;

                spritebatch.DrawString(_font, _cachedTranslation, _displayposition, _color.PreMultiply(),
                                       _rotation, _origin, 1f, _effects, zorder);
            }
        }

        /// <summary>Met à jour l'état du texte en fonction du temps écoulé.</summary>
        public virtual void Update(GameTime gametime)
        {
            ArgumentNullException.ThrowIfNull(gametime);

            if (!_visible)
                return;

            Timer.Update((float)gametime.ElapsedGameTime.TotalSeconds);

            if (Timer.IsDisplayed)
                CheckHover();
        }

        /// <summary>
        /// Crée une copie complète de l'instance, état des timers inclus.
        /// </summary>
        public Text Copy()
        {
            var copy = new Text(_font, _content, _color, Position, _halign, _valign, ZOrder)
            {
                _displayposition = _displayposition,
                // Assignation directe du champ pour éviter que le setter de Visible
                // ne réinitialise les accumulateurs via Timer.Reset().
                _visible = _visible,
                Wrap = Wrap,
                Rotation = _rotation,
                Dimensions = Dimensions,
                // Copie complète de l'état du timer (struct → copie par valeur)
                Timer = Timer
            };

            return copy;
        }

        // ─────────────────────────────────────────────────────────────────────
        // IDisposable
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Libère les ressources de l'instance.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Désabonne tous les gestionnaires d'événements.</summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                OnHovered = null;

            _disposed = true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes privées
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Rafraîchit le cache de traduction et recalcule le wrapping.
        /// Appelé à chaque changement de contenu, de police ou de dimensions — jamais en cours de rendu.
        /// </summary>
        private void RefreshContent()
        {
            _cachedTranslation = LocalizationManager.GetTranslation(_content ?? string.Empty);
            WrapText();
        }

        /// <summary>
        /// Calcule la position d'affichage en appliquant les décalages d'alignement.
        /// </summary>
        private void UpdateDisplayPosition()
        {
            if (_font == null)
                return;

            Vector2 offset = Vector2.Zero;

            switch (_halign)
            {
                case HorizontalAlignment.Center:
                    offset.X = (base.Dimensions.X - TextDimensions.X) / 2f;
                    break;
                case HorizontalAlignment.Right:
                    offset.X = base.Dimensions.X - TextDimensions.X;
                    break;
            }

            switch (_valign)
            {
                case VerticalAlignment.Center:
                    offset.Y = (base.Dimensions.Y - TextDimensions.Y) / 2f;
                    break;
                case VerticalAlignment.Bottom:
                    offset.Y = base.Dimensions.Y - TextDimensions.Y;
                    break;
            }

            _displayposition = base.Position + offset;
        }

        /// <summary>
        /// Découpe le texte en lignes pour respecter la largeur du conteneur.
        /// Utilise <see cref="StringBuilder"/> pour éviter les allocations de chaînes intermédiaires.
        /// </summary>
        private void WrapText()
        {
            if (_font == null || string.IsNullOrEmpty(_content))
            {
                _wrappedContent = _content ?? string.Empty;
                return;
            }

            float maxWidth = base.Dimensions.X;

            if (maxWidth <= 0f)
            {
                _wrappedContent = _cachedTranslation;
                return;
            }

            string[] words = _cachedTranslation.Split(' ');
            var      sb    = new StringBuilder(_cachedTranslation.Length);
            string   line  = string.Empty;

            foreach (string word in words)
            {
                string testLine = string.IsNullOrEmpty(line) ? word : line + " " + word;

                if (_font.MeasureString(testLine).X > maxWidth && !string.IsNullOrEmpty(line))
                {
                    sb.Append(line).Append('\n');
                    line = word;
                }
                else
                {
                    line = testLine;
                }
            }

            sb.Append(line);
            _wrappedContent = sb.ToString();

            // Agrandit la hauteur du conteneur si le texte wrappé dépasse.
            // base.Dimensions est utilisé délibérément : passer par le setter surchargé
            // provoquerait une récursion (Dimensions → WrapText → Dimensions → ...).
            Vector2 measured = _font.MeasureString(_wrappedContent);
            if (base.Dimensions.Y < measured.Y)
                base.Dimensions = new Vector2(base.Dimensions.X, measured.Y);

            UpdateDisplayPosition();
        }

        /// <summary>
        /// Vérifie si la souris survole la zone du texte et déclenche <see cref="OnHovered"/> si c'est le cas.
        /// </summary>
        private void CheckHover()
        {
            var bounds = new Rectangle(Position.ToPoint(), Dimensions.ToPoint());
            if (bounds.Contains(Mouse.GetState().Position))
                OnHovered?.Invoke(this, new TextEventArgs(this));
        }
    }
}
