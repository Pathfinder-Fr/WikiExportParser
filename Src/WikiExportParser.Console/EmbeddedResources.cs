﻿// -----------------------------------------------------------------------
// <copyright file="EmbeddedResources.cs" organization="Pathfinder-Fr">
// Copyright (c) Pathfinder-fr. Tous droits reserves.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace WikiExportParser
{
    /// <summary>
    /// Permet d'accéder aux ressources incorporées stockées au sein de l'assembly.
    /// </summary>
    internal static class EmbeddedResources
    {
        /// <summary>
        /// Cache des scripts SQL pour ce repository.
        /// </summary>
        private static readonly Dictionary<string, string> stringCache = new Dictionary<string, string>();

        /// <summary>
        /// Charge le contenu d'une ressource incorporée sous forme d'une chaîne.
        /// </summary>
        /// <param name="fullName">Nom complet de la ressource, en partant du dossier dans le projet (ex: Resources.Machin.txt).</param>
        /// <returns>Chaîne de caractères.</returns>
        public static string LoadString(string fullName)
        {
            string result;

            if (!stringCache.TryGetValue(fullName, out result))
            {
                var type = typeof (EmbeddedResources);

                using (var stream = type.Assembly.GetManifestResourceStream(type.Namespace + "." + fullName))
                {
                    if (stream == null)
                    {
                        throw new ArgumentOutOfRangeException("fullName", string.Format("Impossible de charger la ressource {0}", fullName));
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }

                stringCache[fullName] = result;
            }

            return result;
        }
    }
}