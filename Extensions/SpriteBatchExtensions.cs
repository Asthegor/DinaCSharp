using DinaCSharp.Internal;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaCSharp.Extensions
{
    public static class SpriteBatchExtensions
    {
        /// <summary>
        /// Dessine un arc de cercle dans une zone donnée.
        /// </summary>
        /// <param name="sb">Le <see cref="SpriteBatch"/> utilisé pour le rendu.</param>
        /// <param name="color">Couleur de l’arc.</param>
        /// <param name="rect">Rectangle définissant la zone où l’arc sera placé.</param>
        /// <param name="radius">Rayon de l’arc.</param>
        /// <param name="startAngle">Angle de départ (en radians).</param>
        /// <param name="endAngle">Angle de fin (en radians).</param>
        public static void DrawArc(this SpriteBatch sb, Color color, Rectangle rect, float radius, float startAngle, float endAngle)
        {
            ArgumentNullException.ThrowIfNull(sb, nameof(sb));
            using Texture2D arcBitmap = sb.CreateArcBitmap(color, new Rectangle((int)rect.X, (int)rect.Y, (int)radius * 2, (int)radius * 2), radius, startAngle, endAngle);
            sb.Draw(arcBitmap, new Rectangle(rect.X, rect.Y, (int)radius * 2, (int)radius * 2), color);
        }
        /// <summary>
        /// Permet de dessiner une ligne entre deux points en utilisant une texture pixel.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="pixel"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        public static void DrawLine(this SpriteBatch sb, Texture2D pixel, Color color, Vector2 start, Vector2 end, int thickness = 1)
        {
            ArgumentNullException.ThrowIfNull(sb);
            ArgumentNullException.ThrowIfNull(pixel);
            Vector2 direction = end - start;
            float length = direction.Length();
            if (length < 1f)
                return;
            direction.Normalize();
            // Calculer l'angle de rotation
            float angle = (float)Math.Atan2(direction.Y, direction.X);
            // Dessiner la ligne
            sb.Draw(pixel, start, null, color.PreMultiply(), angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Dessine les contours d’un rectangle à l’aide d’une texture, d’une couleur et d’une épaisseur.
        /// Le contour est composé de 4 segments : haut, bas, gauche et droite.
        /// Utile pour encadrer des éléments UI ou des blocs de jeu.
        /// </summary>
        /// <param name="sb">SpriteBatch utilisé pour le rendu.</param>
        /// <param name="pixel">Texture utilisée pour dessiner les bords. Doit être une texture pleine, comme un pixel blanc.</param>
        /// <param name="rect">Rectangle cible à entourer.</param>
        /// <param name="color">Couleur du contour.</param>
        /// <param name="thickness">Épaisseur du contour en pixels (par défaut : 1).</param>
        /// <param name="isFilled">Indique si le rectangle doit être plein.</param>
        public static void DrawRectangle(this SpriteBatch sb, Texture2D pixel, Rectangle rect, Color color, int thickness = 1, bool isFilled = false)
        {
            ArgumentNullException.ThrowIfNull(sb);
            ArgumentNullException.ThrowIfNull(pixel);

            if (isFilled)
                sb.Draw(pixel, rect, color.PreMultiply());
            else
            {
                sb.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color.PreMultiply()); // top
                sb.Draw(pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color.PreMultiply()); // left
                sb.Draw(pixel, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color.PreMultiply()); // right
                sb.Draw(pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color.PreMultiply()); // bottom
            }
        }
        /// <summary>
        /// Dessine des masques arrondis sur les quatre coins d’un rectangle
        /// en utilisant une texture de cercle prédéfinie.
        /// </summary>
        /// <param name="sb">Le <see cref="SpriteBatch"/> utilisé pour le rendu.</param>
        /// <param name="pos">Position (coin supérieur gauche) du rectangle.</param>
        /// <param name="dim">Dimensions du rectangle.</param>
        /// <param name="radius">Rayon des coins arrondis.</param>
        /// <param name="color">Couleur appliquée au masque.</param>
        public static void MaskCorners(this SpriteBatch sb, Vector2 pos, Vector2 dim, int radius, Color color)
        {
            ArgumentNullException.ThrowIfNull(sb);
            Texture2D circle = InternalAssets.Circle(sb.GraphicsDevice); // 512x512
            if (circle == null)
                return;
            int srcSize = circle.Width / 2; // 256

            Rectangle src = new Rectangle(0, 0, srcSize, srcSize);
            Rectangle dst = new Rectangle((int)pos.X, (int)pos.Y, radius, radius);

            (int srcX, int srcY, int dstX, int dstY)[] corners =
            [
                (0, 0, (int)pos.X, (int)pos.Y),                                                     // Top-Left
                (srcSize, 0, (int)(pos.X + dim.X - radius), (int)pos.Y),                            // Top-Right
                (0, srcSize, (int)pos.X, (int)(pos.Y + dim.Y - radius)),                            // Bottom-Left
                (srcSize, srcSize, (int)(pos.X + dim.X - radius), (int)(pos.Y + dim.Y - radius)),   // Bottom-Right
            ];

            foreach (var (sx, sy, dx, dy) in corners)
            {
                src.X = sx;
                src.Y = sy;
                dst.X = dx;
                dst.Y = dy;
                sb.Draw(circle, dst, src, color.PreMultiply());
            }
        }
        private static Texture2D CreateArcBitmap(this SpriteBatch sb, Color color, Rectangle arcRect, float radius, float startAngle, float endAngle)
        {
            Texture2D bitmap = new Texture2D(sb.GraphicsDevice, arcRect.Width, arcRect.Height);
            Color[] data = new Color[arcRect.Width * arcRect.Height];

            for (int y = 0; y < arcRect.Height; y++)
            {
                for (int x = 0; x < arcRect.Width; x++)
                {
                    float dx = x - radius;
                    float dy = y - radius;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (dist <= radius)
                    {
                        float angle = (float)Math.Atan2(dy, dx);
                        angle += angle > 0 ? 0 : (float)(Math.PI * 2); // Convertir en radians positifs

                        if (angle >= startAngle && angle <= endAngle)
                        {
                            data[y * arcRect.Width + x] = color;
                        }
                    }
                }
            }

            bitmap.SetData(data);
            return bitmap;
        }
    }
}