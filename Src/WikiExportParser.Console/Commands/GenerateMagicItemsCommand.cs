﻿// -----------------------------------------------------------------------
// <copyright file="GenerateMagicItemsCommand.cs" organization="Pathfinder-Fr">
// Copyright (c) Pathfinder-fr. Tous droits reserves.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using WikiExportParser.Wiki;

namespace WikiExportParser.Commands
{
    public class GenerateMagicItemsCommand : ICommand
    {
        public WikiExport Wiki { get; set; }

        public ILog Log { get; set; }

        public string Help
        {
            get { return "(non implémentée) Exporte les données sur les objets magiques contenus dans le wiki."; }
        }

        public string Alias
        {
            get { return "magicitems"; }
        }

        public void Execute(DataSetCollection dataSets)
        {
            var export = Wiki;

            // Pages des listes d'objets
            var objectLists = new[]
            {
                "pathfinder-rpg.objets merveilleux",
                "pathfinder-rpg.objets merveilleux apg"
            }.Select(pn => export.Pages[new WikiName(pn)]);

            // Liste des pages décrivant des objets magiques
            var objectPages = objectLists
                .SelectMany(p => p.OutLinks)
                .Distinct()
                .ToList();

            // Feats dataset
            //var ds = dataSets.ResolveDataSet("feats");
        }
    }
}