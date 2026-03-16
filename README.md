# DinaCSharp
➡ [English version](#dinaframework-english)

DinaCSharp est un framework développé à partir de [MonoGame](https://github.com/MonoGame/MonoGame), conçu pour faciliter la création de jeux multiplateformes en C#.

## Fonctionnalités

- **Controls** : Composants pour gérer les entrées utilisateur (clavier et gamepad).
- **Core** : Regroupe les entités principales du framework.
- **Extensions** : Méthodes pour effectuer des conversions (ToPoint et ToVector2).
- **Functions** : Utilitaires généraux pour diverses opérations.
- **Graphics** : Outils pour la gestion et le rendu des éléments graphiques.
- **LevelManager** : Chargement et gestion des niveaux issus de [Tiled](https://www.mapeditor.org/).
- **Menus** : Structures pour créer et gérer des menus dans le jeu.
- **Scenes** : Gestion des différentes scènes ou états du jeu.
- **Screen** : Gestion des changements de résolution.
- **Services** : Annuaire global (ServiceLocator) des services.
- **SoundManager** : Gestion centralisée des musiques et des bruitages (SFX).
- **FontManager** : Gestion centralisée des polices (`SpriteFont`) pour éviter les doublons.
- **Localization** : Support multilingue pour les jeux.

## Prérequis

- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) ou version ultérieure.
- [MonoGame 3.8.2](https://github.com/MonoGame/MonoGame/releases/tag/v3.8.2) ou version ultérieure.

## Installation

1. Clonez le dépôt :

   ```bash
   git clone https://github.com/Asthegor/DinaCSharp.git
   ```

2. Ouvrez le fichier `DinaCSharp.sln` avec Visual Studio.
3. Restaurez les packages NuGet nécessaires.

## Utilisation
Compilez le projet DinaCSharp.

1. Ajoutez le projet `DinaCSharp` à votre solution.
2. Ajoutez une référence au projet `DinaCSharp` dans votre projet de jeu.
3. Vous n'avez plus qu'à utiliser les fonctionnalités du framework lors du développement de votre jeu.

Ajoutez une référence à la dll DinaCSharp.dll dans votre projet de jeu.

Vous n'avez plus qu'à utiliser les fonctionnalités du moteur lors du développement de votre jeu.

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

- **Controls**: Components for handling user input (keyboard and gamepad).
- **Core**: Contains the main entities of the framework.
- **Extensions**: Methods for performing conversions (ToPoint and ToVector2).
- **Functions**: General utilities for various operations.
- **Graphics**: Tools for managing and rendering graphical elements.
- **LevelManager**: Loading and managing game levels from [Tiled](https://www.mapeditor.org/).
- **Menus**: Structures for creating and managing in-game menus.
- **Scenes**: Management of different game scenes or states.
- **Screen**: Resolution management.
- **Services**: Global service registry (ServiceLocator).
- **SoundManager**: Centralized management of music and sound effects (SFX).
- **FontManager**: Centralized management of fonts (`SpriteFont`) to avoid duplicates.
- **Localization**: Multilingual support for games.

## Requirements

- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) or later.
- [MonoGame 3.8.2](https://github.com/MonoGame/MonoGame/releases/tag/v3.8.2) or later.

## Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/Asthegor/DinaCSharp.git
2. Open the DinaCSharp.sln file in Visual Studio.

3. Restore the necessary NuGet packages.

## Usage
Build the DinaCSharp project.

Add the generated DinaCSharp.dll to your game project.

Reference DinaCSharp.dll in your project.

You can now use all framework features when developing your game.

## Documentation
Detailed documentation is in progress.
A first version is available here: [DinaCSharp Documentation](https://dinacsharp.lacombedominique.com/documentation/)

## Contributions
Contributions are welcome. Please submit pull requests or open issues to report problems or suggest improvements.

## License
This project is licensed under the MIT License.

## Notes
If you use these sources, please mention this page in your game credits or in your fork.
