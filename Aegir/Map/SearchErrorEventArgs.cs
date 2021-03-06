﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aegir.MapSearch
{
    public class SearchErrorEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the SearchErrorEventArgs class.</summary>
        /// <param name="error">The error message.</param>
        public SearchErrorEventArgs(string error)
        {
            this.Error = error;
        }

        /// <summary>Gets the error generated by the search.</summary>
        public string Error { get; private set; }
    }
}