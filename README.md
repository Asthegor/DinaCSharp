# DinaCSharp
➡ [English version](#dinaframework-english)

DinaCSharp est un framework développé à partir de [MonoGame](https://github.com/MonoGame/MonoGame), conçu pour faciliter la création de jeux multiplateformes en C#.

## Fonctionnalités

- **Core** : Classes de base, énumérations, arguments d'événements et interfaces du framework.
- **Extensions** : Méthodes d'extension (`Color`, `Dictionary`, `SpriteBatch`, `Texture2D`).
- **Graphics** : Animations et sprites, primitives (lignes, polygones, boîtes de collision), textes et composants d'interface (boutons, cases à cocher, listes, curseurs, barres de progression, etc.).
- **Helpers** : Utilitaires généraux (couleurs, chaînes, mise à l'échelle de l'interface, etc.).
- **Input** : Gestion des entrées utilisateur (clavier et gamepad).
- **Services** : Annuaire global (`ServiceLocator`) et services du framework :
  - **Audio** (`SoundManager`) : Gestion centralisée des musiques et des bruitages (SFX).
  - **Fonts** (`FontManager`) : Gestion centralisée des polices (`SpriteFont`) pour éviter les doublons.
  - **Keys** : Clés fortement typées pour identifier des ressources, des services ou des zones.
  - **Levels** (`LevelManager`) : Chargement et gestion des niveaux issus de [Tiled](https://www.mapeditor.org/).
  - **Localization** (`LocalizationManager`) : Support multilingue pour les jeux.
  - **Logs** (`LogManager`) : Journalisation vers un fichier et/ou la console.
  - **Menus** (`MenuManager`) : Création et gestion des menus dans le jeu.
  - **Save** (`SaveManager`) : Sauvegarde et chargement d'objets dans des fichiers chiffrés (via la bibliothèque native `DLACrypto.dll`, Windows x64).
  - **Scenes** (`SceneManager`) : Gestion des différentes scènes ou états du jeu.
  - **Screen** (`ScreenManager`) : Gestion des changements de résolution.

## Prérequis

- [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0).
- [MonoGame 3.8.5](https://github.com/MonoGame/MonoGame/releases/tag/v3.8.5) (`MonoGame.Framework.DesktopGL`).

## Installation

1. Clonez le dépôt :

   ```bash
   git clone https://github.com/Asthegor/DinaCSharp.git
   ```

2. Ouvrez le fichier `DinaCSharp.sln` avec Visual Studio.
3. Restaurez les packages NuGet nécessaires.

## Utilisation

1. Ajoutez le projet `DinaCSharp` à votre solution.
2. Ajoutez une référence au projet `DinaCSharp` dans votre projet de jeu.
3. Vous n'avez plus qu'à utiliser les fonctionnalités du framework lors du développement de votre jeu.

## Documentation
La documentation détaillée est en cours de développement.
Une première version est tout de même disponible ici : [Documentation DinaCSharp](https://dinacsharp.lacombedominique.com/documentation/)

## Contributions
Les contributions sont les bienvenues. Veuillez ouvrir des issues pour signaler des problèmes ou proposer des améliorations.

## Licence
Ce projet est sous licence MIT.

## Notes
Si vous utilisez ces sources, merci de mentionner cette page dans les crédits de votre jeu ou bien dans le fork que vous créez.

---

# DinaFramework (English)

DinaCSharp is a custom game engine built on top of [MonoGame](https://github.com/MonoGame/MonoGame), designed to simplify the creation of cross-platform games in C#.

## Features

- **Core**: Base classes, enumerations, event arguments and interfaces of the framework.
- **Extensions**: Extension methods (`Color`, `Dictionary`, `SpriteBatch`, `Texture2D`).
- **Graphics**: Animations and sprites, primitives (lines, polygons, collision boxes), texts and UI components (buttons, checkboxes, lists, sliders, progress bars, etc.).
- **Helpers**: General utilities (colors, strings, UI scaling, etc.).
- **Input**: User input handling (keyboard and gamepad).
- **Services**: Global service registry (`ServiceLocator`) and framework services:
  - **Audio** (`SoundManager`): Centralized management of music and sound effects (SFX).
  - **Fonts** (`FontManager`): Centralized management of fonts (`SpriteFont`) to avoid duplicates.
  - **Keys**: Strongly typed keys to identify resources, services or areas.
  - **Levels** (`LevelManager`): Loading and managing game levels from [Tiled](https://www.mapeditor.org/).
  - **Localization** (`LocalizationManager`): Multilingual support for games.
  - **Logs** (`LogManager`): Logging to a file and/or the console.
  - **Menus** (`MenuManager`): Creating and managing in-game menus.
  - **Save** (`SaveManager`): Saving and loading objects to and from encrypted files (through the native `DLACrypto.dll` library, Windows x64).
  - **Scenes** (`SceneManager`): Management of different game scenes or states.
  - **Screen** (`ScreenManager`): Resolution management.

## Requirements

- [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0).
- [MonoGame 3.8.5](https://github.com/MonoGame/MonoGame/releases/tag/v3.8.5) (`MonoGame.Framework.DesktopGL`).

## Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/Asthegor/DinaCSharp.git
   ```

2. Open the `DinaCSharp.sln` file in Visual Studio.
3. Restore the necessary NuGet packages.

## Usage

1. Add the `DinaCSharp` project to your solution.
2. Add a reference to the `DinaCSharp` project in your game project.
3. You can now use all framework features when developing your game.

## Documentation
Detailed documentation is in progress.
A first version is available here: [DinaCSharp Documentation](https://dinacsharp.lacombedominique.com/documentation/)

## Contributions
Contributions are welcome. Please submit pull requests or open issues to report problems or suggest improvements.

## License
This project is licensed under the MIT License.

## Notes
If you use these sources, please mention this page in your game credits or in your fork.
