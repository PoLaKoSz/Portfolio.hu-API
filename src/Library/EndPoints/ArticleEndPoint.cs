﻿using System;
using PoLaKoSz.Portfolio.Deserializers;
using PoLaKoSz.Portfolio.Models;

namespace PoLaKoSz.Portfolio.EndPoints
{
    public class ArticleEndPoint : EndPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleEndPoint"/> class.
        /// </summary>
        /// <param name="articleAddress">Full URL to the Portfolio.hu article.</param>
        /// <exception cref="ArgumentException">Occurs when the passed <see cref="Uri"/> is not valid.</exception>
        public ArticleEndPoint(Uri articleAddress)
            : base(articleAddress)
        {
            if (!IsValidURL(articleAddress))
            {
                throw new ArgumentException("URL " + articleAddress.ToString() + " is not a valid article!");
            }
        }

        /// <summary>
        /// Determinate that the given <see cref="Uri"/> is a valid article URL.
        /// </summary>
        /// <param name="articleAddress">Questionable URL.</param>
        /// <returns>True if the URL is valid, false otherwise.</returns>
        public static bool IsValidURL(Uri articleAddress)
        {
            if (!(articleAddress.Host.Equals("portfolio.hu") ||
                articleAddress.Host.Equals("www.portfolio.hu")))
            {
                return false;
            }

            if (articleAddress.AbsolutePath.Contains("hir.php"))
            {
                return true;
            }

            if (articleAddress.Segments.Length < 3)
            {
                return false;
            }

            string[] menus =
            {
                "befektetes/", "finanszirozas/", "deviza-kotveny/", "ingatlan/",
                "gazdasag/", "vallalatok/", "unios-forrasok/", "short/", "prof/", "impakt/",
            };

            foreach (var menuName in menus)
            {
                if (articleAddress.Segments[1].Equals(menuName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Return the desired Portfolio article.
        /// </summary>
        /// <returns>Non null object.</returns>
        /// <exception cref="Exception">Occurs when the web response can't be parsed to a <see cref="Article"/>.</exception>
        public Article Load()
        {
            string sourceCode = base.GetAsync(string.Empty);

            Article article;

            try
            {
                article = ArticleDeserializer.Deserialize(sourceCode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't parse article with URL {EndpointAddress.ToString()}", ex);
            }

            return article;
        }
    }
}