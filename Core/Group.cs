using DinaCSharp.Core.Interfaces;
using DinaCSharp.Extensions;
using DinaCSharp.Graphics.Texts;
using DinaCSharp.Helpers;
using DinaCSharp.Services;
using DinaCSharp.Services.Keys;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections;
using System.Collections.Generic;

namespace DinaCSharp.Core
{
    /// <summary>
    /// Représente un groupe d'éléments gérant leur affichage, leur visibilité, leur couleur et leurs interactions.
    /// Les éléments sont automatiquement triés par ordre d'affichage (Z-order) lors de chaque ajout.
    /// </summary>
    public class Group : Base, IDraw, IVisible, IEnumerable<IElement>, ICollide, IUpdate, IColor, IClickable, IHovered, IDisposable
    {
        private const int DEFAULT_FRAMEPADDING = 8;
        private const int DEFAULT_FRAMETHICKNESS = 2;
        private readonly List<IElement> _elements = [];
        private Rectangle _rect;
        private bool _visible;
        private Color _color;
        private readonly Texture2D _pixel;
        private IDrawingElement? _title;
        private Rectangle? _titleRect;
        private bool _hovered;
        private bool _disposed;

        // ─────────────────────────────────────────────────────────────────────
        // Constructeurs
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Initialise un groupe vide.
        /// </summary>
        /// <param name="position">Position initiale du groupe. Par défaut, (0, 0).</param>
        /// <param name="dimensions">Dimensions initiales du groupe. Par défaut, (0, 0).</param>
        /// <param name="zorder">Ordre d'affichage initial. Par défaut, 0.</param>
        /// <exception cref="InvalidOperationException">
        /// Levée si le service <c>Texture1px</c> n'est pas enregistré dans le <see cref="ServiceLocator"/>.
        /// </exception>
        public Group(Vector2 position = default, Vector2 dimensions = default, int zorder = 0)
            : base(position, dimensions, zorder)
        {
            _color = Color.White;
            Visible = true;
            _pixel = ServiceLocator.Get<Texture2D>(DinaServiceKeys.Texture1px)
                ?? throw new InvalidOperationException("Le service Texture1px n'est pas disponible.");
        }

        /// <summary>
        /// Initialise un groupe en copiant les éléments d'un groupe source.
        /// </summary>
        /// <param name="source">Groupe source à copier.</param>
        /// <param name="duplicate">
        /// Si <c>true</c>, chaque élément est dupliqué via son constructeur de copie.
        /// Si <c>false</c>, les références sont partagées entre les deux groupes.
        /// </param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="source"/> est <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Levée si un élément ne possède pas de constructeur de copie, ou si le service <c>Texture1px</c>
        /// n'est pas enregistré dans le <see cref="ServiceLocator"/>.
        /// </exception>
        public Group(Group source, bool duplicate = true)
        {
            ArgumentNullException.ThrowIfNull(source);

            _pixel = ServiceLocator.Get<Texture2D>(DinaServiceKeys.Texture1px)
                ?? throw new InvalidOperationException("Le service Texture1px n'est pas disponible.");

            _elements = [];
            foreach (var item in source._elements)
            {
                if (duplicate)
                {
                    IElement element = (IElement?)Activator.CreateInstance(item.GetType(), item)
                        ?? throw new InvalidOperationException(
                            $"Impossible de dupliquer l'élément de type {item.GetType().Name}. " +
                            $"Vérifiez qu'il possède un constructeur de copie public.");
                    _elements.Add(element);
                }
                else
                {
                    _elements.Add(item);
                }
            }

            Position = source.Position;
            Dimensions = source.Dimensions;
            ZOrder = source.ZOrder;
            Visible = source.Visible;
            _color = Color.White;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Propriétés publiques
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Rectangle représentant la position et les dimensions du groupe dans l'espace 2D.
        /// Mis à jour automatiquement lors des changements de <see cref="Position"/> et <see cref="Dimensions"/>.
        /// </summary>
        public Rectangle Rectangle => _rect;

        /// <summary>
        /// Nombre d'éléments dans le groupe.
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// Obtient ou définit la position du groupe.
        /// Déplacer le groupe déplace également tous ses éléments et son titre.
        /// </summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                Vector2 offset = value - base.Position;

                foreach (var element in _elements)
                {
                    if (element is IPosition item)
                        item.Position += offset;
                }

                if (_title != null)
                {
                    _title.Position += offset;

                    if (_titleRect.HasValue)
                    {
                        var r = _titleRect.Value;
                        r.Offset((int)offset.X, (int)offset.Y);
                        _titleRect = r;
                    }
                }

                base.Position = value;
                _rect.Location = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }

        /// <summary>
        /// Obtient ou définit les dimensions du groupe.
        /// </summary>
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                _rect.Size = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }

        /// <summary>
        /// Obtient ou définit la visibilité du groupe.
        /// Propager la valeur à tous les éléments implémentant <see cref="IVisible"/>.
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set
            {
                foreach (var element in _elements)
                {
                    if (element is IVisible visible)
                        visible.Visible = value;
                }
                _visible = value;
            }
        }

        /// <summary>
        /// Obtient ou définit la couleur du groupe.
        /// Propage la valeur à tous les éléments implémentant <see cref="IColor"/>.
        /// </summary>
        public Color Color
        {
            get => _color;
            set
            {
                foreach (var element in _elements)
                {
                    if (element is IColor colored)
                        colored.Color = value;
                }
                _color = value;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Propriétés du cadre
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Active ou désactive l'affichage d'un cadre autour du groupe.
        /// </summary>
        public bool HasFrame { get; set; }

        /// <summary>
        /// Couleur du cadre affiché lorsque <see cref="HasFrame"/> est activé.
        /// </summary>
        public Color FrameColor { get; set; } = new Color(50, 50, 50, 200);

        private int _frameThickness = UIScaler.Scale(DEFAULT_FRAMETHICKNESS);
        /// <summary>
        /// Épaisseur du trait du cadre en pixels.
        /// </summary>
        public int FrameThickness
        {
            get => _frameThickness;
            set => _frameThickness = UIScaler.Scale(value);
        }

        private int _framePadding = UIScaler.Scale(DEFAULT_FRAMEPADDING);
        /// <summary>
        /// Espacement en pixels entre les éléments du groupe et le cadre.
        /// </summary>
        public int FramePadding
        {
            get => _framePadding;
            set => _framePadding = UIScaler.Scale(value);
        }
        // ─────────────────────────────────────────────────────────────────────
        // Gestion des éléments
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Ajoute un élément au groupe.
        /// Si l'élément implémente <see cref="IDimensions"/>, les dimensions du groupe sont recalculées.
        /// Les éléments sont ensuite triés par Z-order.
        /// </summary>
        /// <param name="element">Élément à ajouter.</param>
        public void Add(IElement element)
        {
            _elements.Add(element);
            if (element is IDimensions)
                UpdateDimensions();
            SortElements();
        }

        /// <summary>
        /// Trie les éléments du groupe par ordre d'affichage (Z-order) croissant.
        /// </summary>
        public void SortElements()
        {
            _elements.Sort((e1, e2) => e1.ZOrder.CompareTo(e2.ZOrder));
        }
        /// <summary>
        /// Supprime l'élément du groupe.
        /// </summary>
        /// <param name="element">Élément à supprimer.</param>
        public void Remove(IElement element)
        {
            _elements.Remove(element);
            if (_elements.Count > 0)
                UpdateDimensions();
            SortElements();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Interactions
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Vérifie si au moins un élément du groupe a été cliqué (clic gauche ou droit).
        /// </summary>
        /// <returns><c>true</c> si au moins un élément cliquable est cliqué, <c>false</c> sinon.</returns>
        public bool IsClicked()
        {
            foreach (var item in _elements)
            {
                if (item is IClickable clickable && clickable.IsClicked())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Vérifie si au moins un élément du groupe a été cliqué avec le bouton gauche de la souris.
        /// </summary>
        /// <returns><c>true</c> si au moins un élément cliquable est cliqué avec le bouton gauche, <c>false</c> sinon.</returns>
        public bool IsLeftClicked()
        {
            foreach (var item in _elements)
            {
                if (item is IClickable clickable && clickable.IsLeftClicked())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Vérifie si au moins un élément du groupe a été cliqué avec le bouton droit de la souris.
        /// </summary>
        /// <returns><c>true</c> si au moins un élément cliquable est cliqué avec le bouton droit, <c>false</c> sinon.</returns>
        public bool IsRightClicked()
        {
            foreach (var item in _elements)
            {
                if (item is IClickable clickable && clickable.IsRightClicked())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Indique si la souris survole actuellement la zone du groupe.
        /// Mis à jour à chaque appel à <see cref="Update"/>.
        /// </summary>
        /// <returns><c>true</c> si la souris est dans les limites du groupe, <c>false</c> sinon.</returns>
        public bool IsHovered() => _hovered;

        /// <summary>
        /// Simule un clic gauche sur le groupe et le propage à tous les éléments implémentant <see cref="IClickable"/>.
        /// </summary>
        public void LeftClick()
        {
            foreach (var elem in _elements)
            {
                if (elem is IClickable clickable)
                    clickable.LeftClick();
            }
        }

        /// <summary>
        /// Simule un clic droit sur le groupe et le propage à tous les éléments implémentant <see cref="IClickable"/>.
        /// </summary>
        public void RightClick()
        {
            foreach (var elem in _elements)
            {
                if (elem is IClickable clickable)
                    clickable.RightClick();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Collision
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Vérifie si le groupe entre en collision avec un autre élément.
        /// </summary>
        /// <param name="item">Élément à tester pour la collision.</param>
        /// <returns><c>true</c> si les rectangles se chevauchent, <c>false</c> sinon ou si <paramref name="item"/> est <c>null</c>.</returns>
        public bool Collide(ICollide item)
        {
            if (item == null)
                return false;
            return Rectangle.Intersects(item.Rectangle);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Titre et cadre
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Crée et ajoute un titre textuel au groupe, positionné à cheval sur le bord supérieur du cadre.
        /// Active automatiquement l'affichage du cadre (<see cref="HasFrame"/>).
        /// </summary>
        /// <param name="font">Police du titre.</param>
        /// <param name="text">Contenu du titre.</param>
        /// <param name="textcolor">Couleur du texte du titre.</param>
        /// <param name="framecolor">Couleur du cadre. Par défaut, gris semi-transparent.</param>
        /// <param name="framepadding">Espacement entre les éléments et le cadre. Par défaut, 8 px (au minimum la moitié de l'interligne).</param>
        /// <param name="framethickness">Épaisseur du cadre. Par défaut, 2 px.</param>
        /// <param name="shadowcolor">Couleur de l'ombre du titre (facultatif). Nécessite <paramref name="shadowoffset"/>.</param>
        /// <param name="shadowoffset">Décalage de l'ombre du titre (facultatif). Nécessite <paramref name="shadowcolor"/>.</param>
        /// <param name="zorder">Ordre de superposition du titre. Par défaut, 0.</param>
        /// <returns>L'élément titre créé.</returns>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="font"/> est <c>null</c>.</exception>
        public IDrawingElement AddTitle(SpriteFont font, string text, Color textcolor,
                                        Color? framecolor = null, int? framepadding = null, int? framethickness = null,
                                        Color? shadowcolor = null, Vector2? shadowoffset = null, int zorder = 0)
        {
            ArgumentNullException.ThrowIfNull(font, nameof(font));

            DisposeTitle();

            HasFrame = true;
            FrameColor = framecolor ?? new Color(50, 50, 50, 200);
            FramePadding = framepadding ?? DEFAULT_FRAMEPADDING;
            FrameThickness = framethickness ?? DEFAULT_FRAMETHICKNESS;

            // Le padding doit être au moins égal à la moitié de l'interligne pour que le titre ne déborde pas
            if (_framePadding < font.LineSpacing / 2)
                _framePadding = font.LineSpacing / 2 + 1;

            _title = (shadowcolor.HasValue && shadowoffset.HasValue)
                ? new ShadowText(font, text, textcolor, shadowcolor.Value, shadowoffset.Value, zorder: zorder)
                : new Text(font, text, textcolor, Vector2.Zero, zorder: zorder);

            PositionTitle();
            UpdateDimensions();
            return _title;
        }

        /// <summary>
        /// Ajoute un élément titre déjà instancié au groupe, positionné à cheval sur le bord supérieur du cadre.
        /// Active automatiquement l'affichage du cadre (<see cref="HasFrame"/>).
        /// </summary>
        /// <param name="title">Élément titre à utiliser.</param>
        /// <param name="framecolor">Couleur du cadre. Par défaut, gris semi-transparent.</param>
        /// <param name="framepadding">Espacement entre les éléments et le cadre. Par défaut, 8 px.</param>
        /// <param name="framethickness">Épaisseur du cadre. Par défaut, 2 px.</param>
        /// <returns>L'élément titre fourni.</returns>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="title"/> est <c>null</c>.</exception>
        public IDrawingElement AddTitle(IDrawingElement title,
                                        Color? framecolor = null, int? framepadding = null, int? framethickness = null)
        {
            ArgumentNullException.ThrowIfNull(title, nameof(title));

            DisposeTitle();

            _title = title;
            HasFrame = true;
            FrameColor = framecolor ?? new Color(50, 50, 50, 200);
            FramePadding = framepadding ?? DEFAULT_FRAMEPADDING;
            FrameThickness = framethickness ?? DEFAULT_FRAMETHICKNESS;

            PositionTitle();
            UpdateDimensions();
            return _title;
        }

        /// <summary>
        /// Active et configure le cadre du groupe sans ajouter de titre.
        /// </summary>
        /// <param name="framecolor">Couleur du cadre.</param>
        /// <param name="framepadding">Espacement entre les éléments et le cadre en pixels.</param>
        /// <param name="framethickness">Épaisseur du trait du cadre en pixels.</param>
        public void AddFrame(Color framecolor, int framepadding, int framethickness)
        {
            HasFrame = true;
            FrameColor = framecolor;
            FramePadding = framepadding;
            FrameThickness = framethickness;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Rendu et mise à jour
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Dessine le cadre, le titre puis tous les éléments visibles du groupe.
        /// </summary>
        /// <param name="spritebatch">Instance de <see cref="SpriteBatch"/> utilisée pour le rendu.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (!Visible)
                return;

            if (HasFrame)
            {
                Rectangle bounds = CalculateBounds();
                bounds.Inflate(FramePadding, FramePadding);

                if (_titleRect.HasValue)
                {
                    var titleRect = _titleRect.Value;
                    int t = FrameThickness;

                    // Cadre avec interruption en haut pour laisser passer le titre
                    int leftWidth = titleRect.Left - bounds.X - FramePadding;
                    spritebatch.DrawRectangle(_pixel, new Rectangle(bounds.X, bounds.Y, leftWidth, t), FrameColor, t);
                    int rightX = titleRect.Right + FramePadding;
                    int rightWidth = bounds.Right - rightX;
                    spritebatch.DrawRectangle(_pixel, new Rectangle(rightX, bounds.Y, rightWidth, t), FrameColor, t);
                    spritebatch.DrawRectangle(_pixel, new Rectangle(bounds.X, bounds.Bottom - t, bounds.Width, t), FrameColor, t);
                    spritebatch.DrawRectangle(_pixel, new Rectangle(bounds.X, bounds.Y, t, bounds.Height), FrameColor, t);
                    spritebatch.DrawRectangle(_pixel, new Rectangle(bounds.Right - t, bounds.Y, t, bounds.Height), FrameColor, t);
                }
                else
                {
                    spritebatch.DrawRectangle(_pixel, bounds, FrameColor, FrameThickness, isFilled: false);
                }
            }

            if (_title is IDraw drawableTitle)
                drawableTitle.Draw(spritebatch);

            foreach (var element in _elements)
            {
                if (element is IDraw drawable)
                    drawable.Draw(spritebatch);
            }
        }

        /// <summary>
        /// Met à jour l'état du survol et propage la mise à jour à tous les éléments implémentant <see cref="IUpdate"/>.
        /// </summary>
        /// <param name="gametime">Temps de jeu courant.</param>
        public void Update(GameTime gametime)
        {
            // _rect est maintenu à jour par les setters de Position et Dimensions
            _hovered = _rect.Contains(Mouse.GetState().Position);

            foreach (var elem in _elements)
            {
                if (elem is IUpdate updatable)
                    updatable.Update(gametime);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Calculs géométriques
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Calcule le plus petit rectangle aligné sur les axes qui englobe tous les éléments du groupe.
        /// </summary>
        /// <remarks>
        /// Utile pour la mise en page, la détection de collisions ou le rendu du cadre.
        /// </remarks>
        /// <returns>
        /// Un <see cref="Rectangle"/> englobant tous les éléments,
        /// ou <see cref="Rectangle.Empty"/> si le groupe est vide.
        /// </returns>
        public Rectangle CalculateBounds()
        {
            if (_elements.Count == 0)
                return Rectangle.Empty;

            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            foreach (var element in _elements)
            {
                var bounds = new Rectangle(element.Position.ToPoint(), element.Dimensions.ToPoint());
                if (bounds.Left < minX)
                    minX = bounds.Left;
                if (bounds.Top < minY)
                    minY = bounds.Top;
                if (bounds.Right > maxX)
                    maxX = bounds.Right;
                if (bounds.Bottom > maxY)
                    maxY = bounds.Bottom;
            }

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        // ─────────────────────────────────────────────────────────────────────
        // IEnumerable<IElement>
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Retourne un énumérateur pour parcourir les éléments du groupe.
        /// </summary>
        public IEnumerator<IElement> GetEnumerator() => _elements.GetEnumerator();

        /// <summary>
        /// Implémentation non générique de <see cref="IEnumerable"/>, requise par l'interface.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();

        // ─────────────────────────────────────────────────────────────────────
        // IDisposable
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Libère les ressources du groupe et de tous ses éléments.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libère les ressources managées : dispose chaque élément et vide la collection.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                DisposeTitle();

                foreach (var element in _elements)
                {
                    if (element is IDisposable disposable)
                        disposable.Dispose();
                }
                _elements.Clear();
            }

            _disposed = true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes privées
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Recalcule les dimensions du groupe pour englober l'ensemble de ses éléments.
        /// Appelé automatiquement lors de l'ajout d'un élément implémentant <see cref="IDimensions"/>.
        /// </summary>
        private void UpdateDimensions()
        {
            if (_elements.Count == 0)
                return;

            bool hasAny = false;
            float minX = Position.X, minY = Position.Y, maxX = -1, maxY = -1;

            foreach (var element in _elements)
            {
                if (element is not IDimensions elemdim || element is not IPosition elempos)
                    continue;

                hasAny = true;
                var p = elempos.Position;
                var d = elemdim.Dimensions;

                if (p.X < minX)
                    minX = p.X;
                if (p.Y < minY)
                    minY = p.Y;
                if (p.X + d.X > maxX)
                    maxX = p.X + d.X;
                if (p.Y + d.Y > maxY)
                    maxY = p.Y + d.Y;
            }

            if (!hasAny)
                return;

            // Repositionne le titre sur les bounds actuelles des éléments avant de mesurer un éventuel débordement
            PositionTitle();

            if (_title != null)
            {
                if (_title.Position.X < minX)
                    minX = _title.Position.X;
                if (_title.Position.Y < minY)
                    minY = _title.Position.Y;
            }

            if (HasFrame)
            {
                minX -= FramePadding;
                minY -= FramePadding;
                maxX += FramePadding;
                maxY += FramePadding;
            }

            // Si le point le plus haut/à gauche (élément, titre, ou bord du cadre) déborde en dessous
            // de Position, on décale tout le contenu (éléments + titre) pour que Position reste le
            // vrai coin visuel du rendu — comparé à Position actuel, jamais à 0 en dur.
            float shiftX = Math.Max(Position.X - minX, 0f);
            float shiftY = Math.Max(Position.Y - minY, 0f);

            if (shiftX > 0f || shiftY > 0f)
            {
                var shift = new Vector2(shiftX, shiftY);

                foreach (var element in _elements)
                {
                    if (element is IPosition item)
                        item.Position += shift;
                }
                if (_title != null)
                {
                    _title.Position += shift;

                    if (_titleRect.HasValue)
                    {
                        var r = _titleRect.Value;
                        r.Offset((int)shift.X, (int)shift.Y);
                        _titleRect = r;
                    }
                }

                maxX += shift.X;
                maxY += shift.Y;
            }

            base.Dimensions = new Vector2(maxX - Position.X, maxY - Position.Y);
            _rect.Size = new Point(Convert.ToInt32(base.Dimensions.X), Convert.ToInt32(base.Dimensions.Y));
        }

        /// <summary>
        /// Positionne le titre à cheval sur le bord supérieur du cadre et met à jour <see cref="_titleRect"/>.
        /// </summary>
        private void PositionTitle()
        {
            if (_title == null)
                return;

            Rectangle bounds = CalculateBounds();
            bounds.Inflate(FramePadding, FramePadding);

            float titleX = bounds.X + (bounds.Width - _title.Dimensions.X) / 2f;
            float titleY = bounds.Y - _title.Dimensions.Y / 2f;

            _title.Position = new Vector2(titleX, titleY);
            _titleRect = new Rectangle((int)titleX, (int)titleY,
                                             (int)_title.Dimensions.X, (int)_title.Dimensions.Y);
        }

        /// <summary>
        /// Libère le titre courant s'il implémente <see cref="IDisposable"/>, puis le supprime.
        /// </summary>
        private void DisposeTitle()
        {
            if (_title is IDisposable disposable)
                disposable.Dispose();
            _title = null;
            _titleRect = null;
        }
    }
}
