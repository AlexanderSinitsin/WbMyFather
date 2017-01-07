namespace WebSite.Models.Shared.Tables
{
    public class TableButton
    {
        public TableButton(string html, string id, string onClickFunction)
        {
            Html = html;
            Id = id;
            OnClickFunction = onClickFunction;
        }

        public string Html { get; }

        public string Id { get; }

        public string OnClickFunction { get; }
    }
}
