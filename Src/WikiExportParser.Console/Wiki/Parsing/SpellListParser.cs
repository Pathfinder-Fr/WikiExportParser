﻿// -----------------------------------------------------------------------
// <copyright file="SpellListParser.cs" organization="Pathfinder-Fr">
// Copyright (c) Pathfinder-fr. Tous droits reserves.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PathfinderDb.Schema;
using WikiExportParser.Logging;

namespace WikiExportParser.Wiki.Parsing
{
    internal class SpellListParser
    {
        //private const string SpellPattern = @"^\* '''''\[\[?(?<Name>[^\]]+)\]\]?( \((?<Components>[MFX])(, (?<Components>[MFX]))*\))?( \((?<APG>APG)\))?[\.]?'''''\s*(\((?<Components>[MFX])(, (?<Components>[MFX]))*\))?(?<APG>'''\(APG\)''')?\.?";
        private const string SpellPattern = @"^\*\s*'*\[\[?(?<Name>[^\]]+)\]\]?'*(\s*'*\([AdMFX, ]+\)'*\s*)?\.?'*";

        private readonly ILog log;

        private readonly WikiExport wiki;

        private WikiPage page;

        private string listName;

        private int currentLevel;

        public SpellListParser(WikiExport wiki, ILog log)
        {
            this.wiki = wiki;
            this.log = log ?? NullLog.Instance;
        }

        public void Parse(WikiPage page, string listName, List<Spell> spells)
        {
            this.page = page;
            this.listName = listName;

            this.log.Information("Début de l'analyse de la page \"{0}\"", this.page.Title);

            var wiki = this.page.Raw;

            var lines = wiki.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            this.currentLevel = -1;

            foreach (var line in lines)
            {
                this.ReadSpellLine(spells, line);
            }
        }

        private void ReadSpellLine(List<Spell> spells, string line)
        {
            Match match;
            if (line.StartsWith("===") && !line.StartsWith("===="))
            {
                match = Regex.Match(line, @"(niveau (?<Level>\d+)|(?<Level>\d+)(\<sup\>er\</sup\>|\<sup\>ème\</sup\>|er|e) niveau)", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

                if (!match.Success)
                {
                    this.log.Warning("Niveau indétectable dans l'entête \"{0}\"", line);
                }
                else
                {
                    this.currentLevel = int.Parse(match.Groups["Level"].Value);
                    //log.Information("Détection du début d'une liste de sort de niveau {0}", this.currentLevel);
                }
                return;
            }

            if (line.Length == 0 || line[0] != '*')
            {
                return;
            }

            match = Regex.Match(line, SpellPattern, RegexOptions.ExplicitCapture);

            if (!match.Success)
            {
                this.log.Warning("Sort introuvable sur la ligne \"{0}\"", line);
                return;
            }

            //var isApg = match.Groups["APG"].Success;
            var name = match.Groups["Name"].Value;
            //var components = match.Groups["Component"].Value;
            //var description = line.Substring(match.Length).Trim();
            if (name.IndexOf('|') != -1)
            {
                name = name.Substring(0, name.IndexOf('|'));
            }

            var wikiName = WikiName.FromLink(name);
            if (string.IsNullOrWhiteSpace(wikiName.Namespace))
            {
                wikiName.Namespace = "Pathfinder-RPG";
            }
            var spellPageId = wikiName.Id;


            // On cherche un sort dans la librairie dont l'id correspond à l'id de la page désignée
            var spell = spells.FirstOrDefault(s => s.Id == spellPageId);

            if (spell == null)
            {
                // On a pas trouvé de sort correspond à l'id désigné par le lien wiki
                // On vérifie si la page existe
                var page = this.wiki.FindPage(wikiName);

                if (page == null)
                {
                    // La page n'existe pas dans le wiki. Donc le lien est un lien wiki rouge (page non encore créée)
                    this.log.Error("Le sort \"{0}\" pour l'id {1} ne désigne pas une page existante", name, spellPageId);
                }
                else
                {
                    // La page existe. Il faut vérifier si elle est bien dans la liste spells, et si elle n'y est pas il faut savoir pourquoi
                    this.log.Warning("La page \"{0}\" pour l'id {1} n'est pas connue comme un sort, il faut l'ajouter dans la liste alphabétique des sorts", name, spellPageId);
                }
                return;
            }

            var list = spell.Levels.FirstOrDefault(l => l.List == this.listName);

            if (this.listName == SpellList.Ids.Cleric && spell.Levels.All(l => l.List != SpellList.Ids.Oracle))
            {
                spell.Levels = spell.Levels.Concat(new[] {new SpellListLevel {List = SpellList.Ids.Oracle, Level = this.currentLevel}}).ToArray();
            }

            if (list == null)
            {
                //log.Information("Ajout du sort {0} à la liste {1} {2}", spell.Name, this.listName, this.currentLevel);
                spell.Levels = spell.Levels.Concat(new[] {new SpellListLevel {List = this.listName, Level = this.currentLevel}}).ToArray();
            }
            else
            {
                if (list.Level != this.currentLevel)
                {
                    this.log.Error("Erreur de niveau pour le sort \"{0}\" (liste niv. {1} et description niv. {2})", spell.Name, this.currentLevel, list.Level);
                }
            }
        }
    }
}