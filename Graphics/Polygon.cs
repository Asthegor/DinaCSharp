using DinaCSharp.Core;
using DinaCSharp.Events;
using DinaCSharp.Extensions;
using DinaCSharp.Interfaces;
using DinaCSharp.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DinaCSharp.Graphics
{
    /// <summary>
    /// Représente un polygone graphique pouvant être dessiné et interactif.
    /// </summary>
    public class Polygon : Base, IClickable, IColor, IDraw, IUpdate, IVisible, ICopyable<Polygon>, IDisposable
    {
        private Dictionary<string, object> _originalValues = [];
        private List<string> _modifiedHoverValues = [];
        private List<string> _modifiedClickValues = [];
        private bool _hoverOriginalSaved;

        private Vector2[] _vertices;
        private Color _fillColor;
        private Color _borderColor;
        private int _thickness;
        private bool _visible;
        private MouseState _oldMouseState;
        private bool _leftClicked;
        private bool _rightClicked;
        private bool _hover;
        private bool _hoverInvoked;
        private bool _disposed;

        /// <summary>
        /// Initialise une nouvelle instance de la classe Polygon.
        /// </summary>
        /// <param name="vertices">Tableau de sommets définissant le polygone (minimum 3 points).</param>
        /// <param name="zorder">Ordre de superposition (facultatif).</param>
        public Polygon(Vector2[] vertices, int zorder = 0)
            : base(CalculatePosition(vertices), CalculateDimensions(vertices), zorder)
        {
            if (vertices == null || vertices.Length < 3)
                throw new ArgumentException("Un polygone doit avoir au moins 3 sommets.", nameof(vertices));

            _vertices = (Vector2[])vertices.Clone();
            _fillColor = Color.Transparent;
            _borderColor = Color.Transparent;
            _thickness = 0;
            _visible = true;
            _oldMouseState = Mouse.GetState();
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe Polygon.
        /// </summary>
        /// <param name="vertices">Tableau de sommets définissant le polygone (minimum 3 points).</param>
        /// <param name="fillColor">Couleur de remplissage du polygone.</param>
        /// <param name="zorder">Ordre de superposition (facultatif).</param>
        public Polygon(Vector2[] vertices, Color fillColor, int zorder = 0)
            : this(vertices, zorder)
        {
            _fillColor = fillColor;
            _borderColor = fillColor;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Polygon avec une bordure.
        /// </summary>
        /// <param name="vertices">Tableau de sommets définissant le polygone (minimum 3 points).</param>
        /// <param name="fillColor">Couleur de remplissage du polygone.</param>
        /// <param name="borderColor">Couleur de la bordure.</param>
        /// <param name="thickness">Épaisseur de la bordure.</param>
        /// <param name="zorder">Ordre de superposition (facultatif).</param>
        public Polygon(Vector2[] vertices, Color fillColor, Color borderColor, int thickness, int zorder = 0)
            : this(vertices, fillColor, zorder)
        {
            _borderColor = borderColor;
            _thickness = thickness;
        }

        /// <summary>
        /// Obtient ou définit les sommets du polygone.
        /// </summary>
        public Vector2[] Vertices
        {
            get => (Vector2[])_vertices.Clone();
            set
            {
                if (value == null || value.Length < 3)
                    throw new ArgumentException("Un polygone doit avoir au moins 3 sommets.", nameof(value));

                _vertices = (Vector2[])value.Clone();
                Position = CalculatePosition(_vertices);
                Dimensions = CalculateDimensions(_vertices);
            }
        }

        /// <summary>
        /// Obtient ou définit la couleur de remplissage du polygone.
        /// </summary>
        public Color FillColor
        {
            get => _fillColor;
            set => _fillColor = value;
        }

        /// <summary>
        /// Obtient ou définit la couleur de la bordure du polygone.
        /// </summary>
        public Color BorderColor
        {
            get => _borderColor;
            set => _borderColor = value;
        }

        /// <summary>
        /// Obtient ou définit l'épaisseur de la bordure.
        /// </summary>
        public int Thickness
        {
            get => _thickness;
            set => _thickness = value;
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le polygone est visible.
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        /// <summary>
        /// Obtient ou définit la couleur associée au polygone (mappée sur FillColor).
        /// </summary>
        public Color Color
        {
            get => FillColor;
            set => FillColor = value;
        }

        /// <summary>
        /// Redimensionne le polygone en ajustant tous les vertices proportionnellement.
        /// </summary>
        /// <param name="originalRes">Résolution utilisée pour créer les vertices</param>
        /// <param name="newRes">Résolution utilisée pour afficher le polygone</param>
        public void ResizeForResolution(Vector2 originalRes, Vector2 newRes)
        {
            // Étape 1 : Calculer les ratios
            float ratioX = newRes.X / originalRes.X;
            float ratioY = newRes.Y / originalRes.Y;

            // Étape 2 : Parcourir chaque vertex
            for (int i = 0; i < _vertices.Length; i++)
            {
                // Étape 3 : Multiplier par les ratios
                _vertices[i] = new Vector2(
                    _vertices[i].X * ratioX,
                    _vertices[i].Y * ratioY
                );
            }

            // Étape 4 : Mettre à jour Position et Dimensions
            Position = CalculatePosition(_vertices);
            Dimensions = CalculateDimensions(_vertices);
        }

        /// <summary>
        /// Calcule la position (coin supérieur gauche de la bounding box) à partir des sommets.
        /// </summary>
        private static Vector2 CalculatePosition(Vector2[] vertices)
        {
            float minX = vertices.Min(v => v.X);
            float minY = vertices.Min(v => v.Y);
            return new Vector2(minX, minY);
        }

        /// <summary>
        /// Calcule les dimensions (largeur et hauteur de la bounding box) à partir des sommets.
        /// </summary>
        private static Vector2 CalculateDimensions(Vector2[] vertices)
        {
            float minX = vertices.Min(v => v.X);
            float maxX = vertices.Max(v => v.X);
            float minY = vertices.Min(v => v.Y);
            float maxY = vertices.Max(v => v.Y);
            return new Vector2(maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Détermine si un point se trouve à l'intérieur du polygone en utilisant l'algorithme Ray Casting.
        /// </summary>
        /// <param name="point">Le point à tester.</param>
        /// <returns>True si le point est à l'intérieur du polygone, sinon false.</returns>
        private bool IsPointInPolygon(Vector2 point)
        {
            bool inside = false;
            int j = _vertices.Length - 1;

            for (int i = 0; i < _vertices.Length; i++)
            {
                if (((_vertices[i].Y > point.Y) != (_vertices[j].Y > point.Y)) &&
                    (point.X < (_vertices[j].X - _vertices[i].X) * (point.Y - _vertices[i].Y) / (_vertices[j].Y - _vertices[i].Y) + _vertices[i].X))
                {
                    inside = !inside;
                }
                j = i;
            }

            return inside;
        }

        /// <summary>
        /// Dessine le polygone à l'aide d'un SpriteBatch.
        /// </summary>
        /// <param name="spritebatch">Instance de SpriteBatch utilisée pour le rendu.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            if (!Visible || _vertices.Length < 3)
                return;

            Texture2D? texture = ServiceLocator.Get<Texture2D>(ServiceKeys.Texture1px)
                ?? throw new InvalidOperationException("Texture1px non enregistrée dans le ServiceLocator");

            // Dessiner le remplissage du polygone (triangulation simple pour polygones convexes)
            DrawPolygonFill(spritebatch, texture);

            // Dessiner la bordure si nécessaire
            if (_thickness > 0 && _borderColor != _fillColor)
            {
                DrawPolygonBorder(spritebatch, texture);
            }
        }

        /// <summary>
        /// Dessine le remplissage du polygone par triangulation en éventail.
        /// </summary>
        private void DrawPolygonFill(SpriteBatch spritebatch, Texture2D texture)
        {
            // Triangulation en éventail depuis le premier sommet
            Vector2 center = new Vector2(_vertices.Sum(v => v.X) / _vertices.Length,
                                         _vertices.Sum(v => v.Y) / _vertices.Length);

            for (int i = 0; i < _vertices.Length; i++)
            {
                if (i == _vertices.Length - 1)
                    DrawTriangle(spritebatch, texture, center, _vertices[i], _vertices[0], _fillColor);
                else
                    DrawTriangle(spritebatch, texture, center, _vertices[i], _vertices[i + 1], _fillColor);
            }
        }

        /// <summary>
        /// Dessine un triangle rempli.
        /// </summary>
        private static void DrawTriangle(SpriteBatch spritebatch, Texture2D texture, Vector2 v1, Vector2 v2, Vector2 v3, Color color)
        {
            // Tri des sommets par Y
            Vector2[] sorted = new[] { v1, v2, v3 }.OrderBy(v => v.Y).ToArray();
            Vector2 top = sorted[0];
            Vector2 mid = sorted[1];
            Vector2 bottom = sorted[2];

            // Dessiner le triangle ligne par ligne
            for (float y = top.Y; y <= bottom.Y; y++)
            {
                float leftX, rightX;

                if (y < mid.Y)
                {
                    // Partie supérieure du triangle
                    leftX = Lerp(top.X, mid.X, (y - top.Y) / (mid.Y - top.Y + 0.001f));
                    rightX = Lerp(top.X, bottom.X, (y - top.Y) / (bottom.Y - top.Y + 0.001f));
                }
                else
                {
                    // Partie inférieure du triangle
                    leftX = Lerp(mid.X, bottom.X, (y - mid.Y) / (bottom.Y - mid.Y + 0.001f));
                    rightX = Lerp(top.X, bottom.X, (y - top.Y) / (bottom.Y - top.Y + 0.001f));
                }

                if (leftX > rightX)
                {
                    float temp = leftX;
                    leftX = rightX;
                    rightX = temp;
                }

                int width = (int)(rightX - leftX);
                if (width > 0)
                {
                    Rectangle rect = new Rectangle((int)leftX, (int)y, width, 1);
                    spritebatch.Draw(texture, rect, color);
                }
            }
        }

        /// <summary>
        /// Interpolation linéaire.
        /// </summary>
        private static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        /// Dessine la bordure du polygone.
        /// </summary>
        private void DrawPolygonBorder(SpriteBatch spritebatch, Texture2D texture)
        {
            for (int i = 0; i < _vertices.Length; i++)
            {
                Vector2 start = _vertices[i];
                Vector2 end = _vertices[(i + 1) % _vertices.Length];
                DrawLine(spritebatch, texture, start, end, _borderColor, _thickness);
            }
        }

        /// <summary>
        /// Dessine une ligne entre deux points.
        /// </summary>
        private static void DrawLine(SpriteBatch spritebatch, Texture2D texture, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            float length = edge.Length();

            spritebatch.Draw(texture,
                new Rectangle((int)start.X, (int)start.Y, (int)length, thickness),
                null,
                color,
                angle,
                new Vector2(0, thickness / 2f),
                SpriteEffects.None,
                0);
        }

        /// <summary>
        /// Met à jour l'état du polygone en fonction des interactions utilisateur.
        /// </summary>
        /// <param name="gametime">Temps de jeu actuel.</param>
        public void Update(GameTime gametime)
        {
            MouseState currentMouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

            bool hoveredNow = IsPointInPolygon(mousePosition);

            if (hoveredNow)
            {
                _hover = true;
                if (!_hoverOriginalSaved)
                {
                    _originalValues = SaveValues();
                    _hoverOriginalSaved = true;
                }
                if (!_hoverInvoked)
                {
                    OnHovered?.Invoke(this, new PolygonEventArgs(this));
                    _hoverInvoked = true;
                    _modifiedHoverValues = [.. _originalValues.GetModifiedKeys(SaveValues())];
                }

                // Vérifie si le clic a eu lieu
                _leftClicked = _hover && currentMouseState.LeftButton == ButtonState.Released && _oldMouseState.LeftButton == ButtonState.Pressed;
                if (_leftClicked)
                {
                    var beforeClickValues = SaveValues();
                    OnClicked?.Invoke(this, new PolygonEventArgs(this));
                    _modifiedClickValues = [.. beforeClickValues.GetModifiedKeys(SaveValues())];

                    _modifiedHoverValues.RemoveAll(k => _modifiedClickValues.Contains(k));
                }

                _rightClicked = _hover && currentMouseState.RightButton == ButtonState.Released && _oldMouseState.RightButton == ButtonState.Pressed;
                if (_rightClicked)
                    OnRightClicked?.Invoke(this, new PolygonEventArgs(this));
            }
            else
            {
                _leftClicked = false;
                _hover = false;
                if (_hoverOriginalSaved)
                {
                    var permanentState = SaveValues();
                    RestoreOriginalValues(_modifiedHoverValues);
                    ApplyValues(permanentState, _modifiedClickValues);

                    _hoverOriginalSaved = false;
                    _hoverInvoked = false;
                    _modifiedHoverValues.Clear();
                }
            }
            _oldMouseState = currentMouseState;
        }

        private void ApplyValues(Dictionary<string, object> reference, List<string> keys)
        {
            foreach (var key in keys)
            {
                if (reference.TryGetValue(key, out var value))
                {
                    PropertyInfo? prop = GetType().GetProperty(key);
                    if (prop != null)
                        prop.SetValue(this, value);
                }
            }
        }

        /// <summary>
        /// Détermine si le polygone a été cliqué (clic droit ou gauche).
        /// </summary>
        /// <returns>True si cliqué, sinon false.</returns>
        public bool IsClicked() => _leftClicked || _rightClicked;

        /// <summary>
        /// Détermine si le polygone a été cliqué avec le bouton gauche.
        /// </summary>
        /// <returns>True si cliqué, sinon false.</returns>
        public bool IsLeftClicked() => _leftClicked;

        /// <summary>
        /// Détermine si le polygone a été cliqué avec le bouton droit.
        /// </summary>
        /// <returns>True si cliqué, sinon false.</returns>
        public bool IsRightClicked() => _rightClicked;

        /// <summary>
        /// Détermine si le polygone est survolé par la souris.
        /// </summary>
        /// <returns>True si survolé, sinon false.</returns>
        public bool IsHovered() => _hover;

        /// <summary>
        /// Déclenche les événements lorsque le polygone est survolé.
        /// </summary>
        public event EventHandler<PolygonEventArgs>? OnHovered;

        /// <summary>
        /// Déclenche les événements lorsque le polygone est cliqué avec le bouton gauche.
        /// </summary>
        public event EventHandler<PolygonEventArgs>? OnClicked;

        /// <summary>
        /// Déclenche les événements lorsque le polygone est cliqué avec le bouton droit.
        /// </summary>
        public event EventHandler<PolygonEventArgs>? OnRightClicked;

        /// <summary>
        /// Crée une copie du polygone actuel.
        /// </summary>
        /// <returns>Une nouvelle instance de Polygon avec les mêmes propriétés.</returns>
        public Polygon Copy()
        {
            return new Polygon((Vector2[])_vertices.Clone(), _fillColor, ZOrder)
            {
                _borderColor = _borderColor,
                _thickness = _thickness,
                _visible = _visible,
                _leftClicked = _leftClicked,
                _rightClicked = _rightClicked,
                _oldMouseState = _oldMouseState
            };
        }

        private Dictionary<string, object> SaveValues()
        {
            Dictionary<string, object> values = [];
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                values[property.Name] = property.GetValue(this)!;
            }
            return values;
        }

        private void RestoreOriginalValues(List<string>? modifiedKeys = null)
        {
            if (modifiedKeys == null || modifiedKeys.Count == 0)
                return;

            foreach (string key in modifiedKeys)
            {
                if (_originalValues.ContainsKey(key))
                {
                    PropertyInfo? property = GetType().GetProperty(key);
                    if (property != null)
                    {
                        property.SetValue(this, _originalValues[key]);
                    }
                }
            }
        }

        /// <summary>
        /// Simule un clic gauche sur le Polygon.
        /// </summary>
        public void LeftClick()
        {
            OnClicked?.Invoke(this, new PolygonEventArgs(this));
        }

        /// <summary>
        /// Simule un clic droit sur le Polygon.
        /// </summary>
        public void RightClick()
        {
            OnRightClicked?.Invoke(this, new PolygonEventArgs(this));
        }

        /// <summary>
        /// Libère les ressources utilisées par le Polygon.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Désabonne tous les événements.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (OnHovered != null)
                {
                    foreach (var handler in OnHovered.GetInvocationList())
                        OnHovered -= (EventHandler<PolygonEventArgs>)handler;
                }
                if (OnClicked != null)
                {
                    foreach (var handler in OnClicked.GetInvocationList())
                        OnClicked -= (EventHandler<PolygonEventArgs>)handler;
                }
                if (OnRightClicked != null)
                {
                    foreach (var handler in OnRightClicked.GetInvocationList())
                        OnRightClicked -= (EventHandler<PolygonEventArgs>)handler;
                }
                _originalValues.Clear();
                _modifiedClickValues.Clear();
                _modifiedHoverValues.Clear();
            }
            _disposed = true;
        }
    }
}