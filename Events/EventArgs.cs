using DinaCSharp.Graphics;
using DinaCSharp.Core.Interfaces;
using DinaCSharp.Services.Menus;
using DinaCSharp.Services.Scenes;
using DinaCSharp.Services.Screen;

using System;

namespace DinaCSharp.Events
{
    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Button"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="ButtonEventArgs"/> pour le bouton spécifié.
    /// </remarks>
    /// <param name="button">Le bouton associé à l'événement.</param>
    public class ButtonEventArgs(Button button) : EventArgs
    {
        /// <summary>
        /// Le bouton qui a déclenché l'événement.
        /// </summary>
        public Button Button { get; } = button;
    }

    /// <summary>
    /// Contient les informations d'un événement lié à une <see cref="CheckBox"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="CheckBoxEventArgs"/> pour la CheckBox spécifiée.
    /// </remarks>
    /// <param name="checkBox">La CheckBox associée à l'événement.</param>
    public class CheckBoxEventArgs(CheckBox checkBox) : EventArgs
    {
        /// <summary>
        /// La case à cocher qui a déclenché l'événement.
        /// </summary>
        public CheckBox CheckBox { get; } = checkBox;
    }
    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Graphics.Text"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="TextEventArgs"/> pour le Text spécifié.
    /// </remarks>
    /// <param name="text">Le Text associé à l'événement.</param>
    public class TextEventArgs(Text text) : EventArgs
    {
        /// <summary>
        /// Le Text qui a déclenché l'événement.
        /// </summary>
        public Text Text { get; } = text;
    }
    /// <summary>
    /// Contient les informations d'un événement lié à une <see cref="ListBox"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="ListBoxEventArgs"/> pour la ListBox spécifiée.
    /// </remarks>
    /// <param name="listBox">La ListBox associée à l'événement.</param>
    public class ListBoxEventArgs(ListBox listBox) : EventArgs
    {
        /// <summary>
        /// La liste qui a déclenché l'événement.
        /// </summary>
        public ListBox ListBox { get; } = listBox;
    }

    /// <summary>
    /// Contient les informations d'un clic sur un élément d'une <see cref="ListBox"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="ListBoxClickEventArgs"/> pour l'index spécifié.
    /// </remarks>
    /// <param name="index">L'index de l'élément cliqué.</param>
    public class ListBoxClickEventArgs(int index) : EventArgs
    {
        /// <summary>
        /// L'index de l'élément cliqué dans la liste.
        /// </summary>
        public int Index { get; } = index;
    }

    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="MenuItem"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="MenuItemEventArgs"/> pour le MenuItem spécifié.
    /// </remarks>
    /// <param name="menuitem">Le MenuItem associé à l'événement.</param>
    public class MenuItemEventArgs(MenuItem menuitem) : EventArgs
    {
        /// <summary>
        /// Le MenuItem qui a déclenché l'événement.
        /// </summary>
        public MenuItem MenuItem { get; } = menuitem;
    }

    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Panel"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="PanelEventArgs"/> pour le panneau spécifié.
    /// </remarks>
    /// <param name="panel">Le panneau associé à l'événement.</param>
    public class PanelEventArgs(Panel panel) : EventArgs
    {
        /// <summary>
        /// Le panneau qui a déclenché l'événement.
        /// </summary>
        public Panel Panel { get; } = panel;
    }

    /// <summary>
    /// Fournit des données pour les événements liés au Polygon.
    /// </summary>
    public class PolygonEventArgs(Polygon polygon) : EventArgs
    {
        /// <summary>
        /// Obtient le Polygon associé à l'événement.
        /// </summary>
        public Polygon Polygon { get; } = polygon;
    }

    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Slider"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="SliderEventArgs"/> pour le slider spécifié.
    /// </remarks>
    /// <param name="slider">Le slider associé à l'événement.</param>
    public class SliderEventArgs(Slider slider) : EventArgs
    {
        /// <summary>
        /// Le slider qui a déclenché l'événement.
        /// </summary>
        public Slider Slider { get; } = slider;
    }

    /// <summary>
    /// Contient les informations d'un événement lié à une <see cref="Scene"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="SceneEventArgs"/> pour la scène spécifiée.
    /// </remarks>
    /// <param name="scene">La scène associée à l'événement.</param>
    public class SceneEventArgs(Scene scene) : EventArgs
    {
        /// <summary>
        /// La scène qui a déclenché l'événement.
        /// </summary>
        public Scene Scene { get; } = scene;
    }

    /// <summary>
    /// Contient les informations d'un événement lié au <see cref="SceneManager"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="SceneManagerEventArgs"/> pour le SceneManager spécifié.
    /// </remarks>
    /// <param name="sceneManager">Le SceneManager associé à l'événement.</param>
    public class SceneManagerEventArgs(SceneManager sceneManager) : EventArgs
    {
        /// <summary>
        /// Le SceneManager qui a déclenché l'événement.
        /// </summary>
        public SceneManager SceneManager { get; } = sceneManager;
    }

    /// <summary>
    /// Contient les informations d'un événement lié au <see cref="ScreenManager"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="ScreenManagerEventArgs"/> pour le ScreenManager spécifié.
    /// </remarks>
    /// <param name="screenManager">Le ScreenManager associé à l'événement.</param>
    public class ScreenManagerEventArgs(ScreenManager screenManager) : EventArgs
    {
        /// <summary>
        /// Le ScreenManager qui a déclenché l'événement.
        /// </summary>
        public ScreenManager ScreenManager { get; } = screenManager;
    }
}
