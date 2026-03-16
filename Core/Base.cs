using DinaCSharp.Interfaces;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DinaCSharp.Core
{
    /// <summary>
    /// Classe abstraite de base représentant un élément avec une position, des dimensions et un ordre d'affichage.
    /// </summary>
    public abstract class Base : IElement, INotifyPropertyChanged
    {
        private int _zorder;
        private Vector2 _position;
        private Vector2 _dimensions;

        /// <summary>
        /// Valeur maximale du ZOrder
        /// </summary>
        protected const float MAX_ZORDER = 10000;
        /// <summary>
        /// Valeur minimale du ZOrder
        /// </summary>
        protected const float MIN_ZORDER = -10000;

        /// <summary>
        /// Ordre d'affichage (Z-order) de l'élément.
        /// </summary>
        public int ZOrder
        {
            get => _zorder;
            set
            {
                if (value > MAX_ZORDER || value < MIN_ZORDER)
                    throw new ArgumentOutOfRangeException($"{value} must be between {MIN_ZORDER} and {MAX_ZORDER}");
                SetProperty(ref _zorder, value);
            }
        }
        /// <summary>
        /// Position de l'élément dans l'espace.
        /// </summary>
        public virtual Vector2 Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }
        /// <summary>
        /// Dimensions de l'élément.
        /// </summary>
        public virtual Vector2 Dimensions
        {
            get => _dimensions;
            set => SetProperty(ref _dimensions, value);
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="position"></param>
        /// <param name="dimensions"></param>
        /// <param name="zorder"></param>
        protected Base(Vector2 position = new Vector2(), Vector2 dimensions = new Vector2(), int zorder = 0)
        {
            Position = position;
            Dimensions = dimensions;
            ZOrder = zorder;
        }

        /// <summary>
        /// Constructeur de copie
        /// </summary>
        /// <param name="base"></param>
        protected Base(Base @base)
        {
            ArgumentNullException.ThrowIfNull(@base);

            Position = @base.Position;
            Dimensions = @base.Dimensions;
            ZOrder = @base.ZOrder;
        }

        /// <summary>
        /// Événement déclenché lorsque la valeur d'une propriété change.
        /// Les abonnés peuvent filtrer sur <see cref="PropertyChangedEventArgs.PropertyName"/>
        /// pour réagir uniquement aux propriétés qui les intéressent.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifie les abonnés qu'une propriété a changé de valeur.
        /// </summary>
        /// <param name="propertyName">
        /// Nom de la propriété ayant changé. Ce paramètre est automatiquement
        /// renseigné par le compilateur via <see cref="CallerMemberNameAttribute"/>.
        /// Il n'est généralement pas nécessaire de le fournir manuellement.
        /// </param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Assigne une nouvelle valeur à un champ et notifie les abonnés si la valeur a changé.
        /// </summary>
        /// <typeparam name="T">Type de la propriété.</typeparam>
        /// <param name="field">Référence au champ de la propriété.</param>
        /// <param name="value">Nouvelle valeur à assigner.</param>
        /// <param name="propertyName">
        /// Nom de la propriété ayant changé. Ce paramètre est automatiquement
        /// renseigné par le compilateur via <see cref="CallerMemberNameAttribute"/>.
        /// Il n'est généralement pas nécessaire de le fournir manuellement.
        /// </param>
        /// <remarks>
        /// Si la nouvelle valeur est identique à la valeur actuelle, aucune notification
        /// n'est émise et le champ n'est pas modifié.
        /// À utiliser pour les setters simples sans logique métier.
        /// Pour les setters complexes, utiliser <see cref="NotifyPropertyChanged"/> à la place.
        /// </remarks>
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
