namespace DinaCSharp.Graphics
{
    /// <summary>
    /// Encapsule la logique de temporisation d'affichage d'un texte.
    /// Gère les phases d'attente, d'affichage et les cycles de répétition.
    /// </summary>
    /// <remarks>
    /// Struct à valeur sémantique : l'assignation (<c>timer = other</c>) produit
    /// une copie complète de l'état, ce qui simplifie la copie dans <see cref="Text.Copy"/>.
    /// </remarks>
    struct TextTimer
    {
        // ── Configuration ─────────────────────────────────────────────────────
        private float _waitTime;
        private float _displayTime;
        private int   _nbLoops;

        // ── Accumulateurs ─────────────────────────────────────────────────────
        private float _timerWait;
        private float _timerDisplay;

        // ── Machine d'état ────────────────────────────────────────────────────
        private bool _waiting;
        private bool _displayed;

        // ─────────────────────────────────────────────────────────────────────
        // Constructeur
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Initialise un timer en mode "affiché en permanence" (aucune temporisation).
        /// </summary>
        public TextTimer()
        {
            _displayed = true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Propriétés
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Indique si le texte doit actuellement être rendu à l'écran.
        /// </summary>
        public readonly bool IsDisplayed => _displayed;

        // ─────────────────────────────────────────────────────────────────────
        // API publique
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Configure les paramètres de temporisation et réinitialise l'état.
        /// </summary>
        /// <param name="waitTime">
        ///   Délai (s) avant affichage. -1 ou ≤ 0 : aucun délai, affichage immédiat.
        ///   0 : texte masqué dès le départ, sans délai ni affichage.
        /// </param>
        /// <param name="displayTime">
        ///   Durée (s) d'affichage. -1 ou ≤ 0 : affichage indéfini.
        ///   0 : texte masqué immédiatement.
        /// </param>
        /// <param name="nbLoops">
        ///   Nombre de cycles. -1 : répétition infinie. 0 : pas de répétition.
        /// </param>
        public void Configure(float waitTime, float displayTime, int nbLoops)
        {
            _waitTime    = waitTime;
            _displayTime = displayTime;
            _nbLoops     = nbLoops;
            _timerWait   = 0f;
            _timerDisplay= 0f;

            if (displayTime == 0f)
            {
                // Masqué immédiatement, sans attente
                _displayed = false;
                _waiting   = false;
            }
            else if (waitTime > 0f)
            {
                // Phase d'attente avant affichage
                _displayed = false;
                _waiting   = true;
            }
            else
            {
                // Affichage immédiat
                _displayed = true;
                _waiting   = false;
            }
        }

        /// <summary>
        /// Remet à zéro les accumulateurs sans toucher à la configuration ni à la machine d'état.
        /// Utilisé lors d'un changement de visibilité externe.
        /// </summary>
        public void Reset()
        {
            _timerWait    = 0f;
            _timerDisplay = 0f;
        }

        /// <summary>
        /// Avance le timer d'une durée <paramref name="dt"/> (en secondes).
        /// </summary>
        public void Update(float dt)
        {
            if (_waiting)
            {
                AdvanceWait(dt);
                return;
            }

            if (_displayed)
                AdvanceDisplay(dt);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes privées
        // ─────────────────────────────────────────────────────────────────────

        private void AdvanceWait(float dt)
        {
            _timerWait += dt;
            if (_timerWait < _waitTime)
                return;

            _timerWait = 0f;
            _waiting   = false;

            // Passe en affichage sauf si displayTime == 0
            _displayed = _displayTime != 0f;
        }

        private void AdvanceDisplay(float dt)
        {
            // Affichage illimité : rien à faire
            if (_displayTime <= 0f)
                return;

            _timerDisplay += dt;
            if (_timerDisplay < _displayTime)
                return;

            // Fin de la fenêtre d'affichage
            _timerDisplay = 0f;
            _displayed    = false;

            // Gestion des cycles
            if (_nbLoops > 0)
            {
                _nbLoops--;
                if (_nbLoops == 0)
                    return; // Cycles épuisés → reste masqué
            }

            // Cycles infinis ou restants (_nbLoops == -1 ou > 0 restant)
            if (_nbLoops != 0)
            {
                if (_waitTime > 0f)
                    _waiting = true;   // Recommence par une phase d'attente
                else
                    _displayed = true; // Recommence directement l'affichage
            }
        }
    }
}
