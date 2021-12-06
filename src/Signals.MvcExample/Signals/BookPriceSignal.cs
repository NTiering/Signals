using Signals.MvcExample.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.MvcExample.Signals
{
    public class BookPriceSignal : Signal
    {
        public BookPriceSignal(int bookId)
        {
            BookId = bookId;
        }

        public int BookId { get; }
        public Book BookDetails { get; set; }
        public List<string> Notes { get; } = new List<string>();
        public decimal PostageCost { get; set; }
        public decimal Discount { get; set; }
    }
}
