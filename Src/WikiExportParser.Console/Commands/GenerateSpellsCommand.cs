﻿// -----------------------------------------------------------------------
// <copyright file="GenerateSpellsCommand.cs" organization="Pathfinder-Fr">
// Copyright (c) Pathfinder-fr. Tous droits reserves.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using PathfinderDb.Schema;

namespace WikiExportParser.Commands
{
    public class GenerateSpellsCommand : SpellCommandBase, ICommand
    {
        public string Alias
        {
            get { return "spells"; }
        }

        public string Help
        {
            get { return "Exporte les données sur les sorts contenus dans le wiki."; }
        }

        public void Execute(DataSetCollection dataSets)
        {
            var spells = ReadSpells();

            AddSpellLists(spells);

            GenerateIds(spells);

            // Adding to dataset
            foreach (var sourceSpells in spells.GroupBy(s => s.Source.Id).Where(s => !string.IsNullOrEmpty(s.Key)))
            {
                var dataSet = dataSets.ResolveDataSet(sourceSpells.Key);
                dataSet.Sources.GetOrAdd(s => s.Id.Equals(sourceSpells.Key), () => new Source {Id = sourceSpells.Key});
                dataSet.Spells.AddRange(sourceSpells);
            }

            // Spells dataset
            //var ds = dataSets.ResolveDataSet("spells");
            //ds.Spells.AddRange(spells);
            //foreach (var source in spells.Select(s => s.Source.Id).Distinct().Where(s => !string.IsNullOrEmpty(s)))
            //{
            //    ds.Sources.Add(new Source { Id = source });
            //}
        }
    }
}