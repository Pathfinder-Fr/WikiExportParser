﻿// -----------------------------------------------------------------------
// <copyright file="ExceptionExtension.cs" organization="Pathfinder-Fr">
// Copyright (c) Pathfinder-fr. Tous droits reserves.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text;

namespace WikiExportParser.Wiki.Parsing
{
    internal static class ExceptionExtension
    {
        public static string RecursiveMessage(this Exception @this)
        {
            var msgbuilder = new StringBuilder();

            do
            {
                if (msgbuilder.Length != 0)
                {
                    msgbuilder.Append(" => ");
                }

                msgbuilder.Append(@this.Message);
                @this = @this.InnerException;
            } while (@this != null);

            return msgbuilder.ToString();
        }
    }
}