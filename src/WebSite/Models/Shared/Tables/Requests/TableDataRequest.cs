using System.Collections.Generic;

namespace WebSite.Models.Shared.Tables.Requests
{
    public class TableDataRequest
    {
        public TableDataRequest(int draw, IDictionary<string, string> columns, IDictionary<string, string> order, int start, int length, IDictionary<string, string> search)
        {
            Draw = draw;

            var columnsList = new List<Column>();
            for (   var i = 0; i < columns.Count / 4; i++)
            {
                var column = new Column
                {
                    Data = columns[$"{i}data"],
                    Name = columns[$"{i}name"],
                    Orderable = bool.Parse(columns[$"{i}orderable"]),
                    Searchable = bool.Parse(columns[$"{i}searchable"]),
                    Search = new Search
                    {
                        Regex = columns[$"{i}searchregex"],
                        Value = columns[$"{i}searchvalue"]
                    }
                };

                columnsList.Add(column);
            }

            Columns = columnsList;

            Order = new Order
            {
                Column = int.Parse(order["0column"]),
                Dir = order["0dir"]
            };

            Start = start;
            Length = length;

            Search = new Search {Value = search["value"], Regex = search["regex"]};
        }

        public int Draw { get; }

        public IList<Column> Columns { get; }

        public int Start { get; }

        public int Length { get; }

        public Order Order { get; }

        public Search Search { get; }
    }
}
