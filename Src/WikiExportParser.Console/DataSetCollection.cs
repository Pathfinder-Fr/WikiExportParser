﻿// -----------------------------------------------------------------------
// <copyright file="DataSetCollection.cs" organization="Pathfinder-Fr">
// Copyright (c) Pathfinder-fr. Tous droits reserves.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using PathfinderDb.Schema;

namespace WikiExportParser
{
    /// <summary>
    /// Collection de <see cref="DataSet" /> rangés selon leur source.
    /// </summary>
    public class DataSetCollection
    {
        private readonly IDictionary<string, DataSet> items = new Dictionary<string, DataSet>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Obtient la liste des <see cref="DataSet" />, rangés selon leur source.
        /// </summary>
        public IDictionary<string, DataSet> DataSets
        {
            get { return items; }
        }

        /// <summary>
        /// Obtient ou définit la langue à indiquer pour chaque dataset créé.
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// Renvoie en créeant si nécessaire le <see cref="DataSet" /> avecc le nom indiqué.
        /// </summary>
        public DataSet ResolveDataSet(string name)
        {
            DataSet dataSet;

            if (!items.TryGetValue(name, out dataSet))
            {
                dataSet = new DataSet();
                if (!string.IsNullOrEmpty(Lang))
                {
                    dataSet.Lang = Lang;
                }

                dataSet.Sources = new List<Source>();
                //dataSet.Sources.Add(new Source { Id = name });
                items.Add(name, dataSet);
            }

            return dataSet;
        }
    }
}