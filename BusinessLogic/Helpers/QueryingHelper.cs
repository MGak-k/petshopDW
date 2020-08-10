using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web;
using BusinessLogic.BusinessObjects;
using System.Linq.Expressions;

namespace BusinessLogic.Helpers
{
    public class QueryingHelper<T>
    {

        public static BaseModelForFiltering<T> Filter(IQueryable<T> query, Dictionary<string, object> options, Type IncomingType)
        {
            var filteredResults = FilterByOptions(query, options, IncomingType);
            filteredResults = HeaderFilterByOptions(filteredResults, options, IncomingType);
            filteredResults = SortByOptions(filteredResults, options, IncomingType);

            int countPrePage = filteredResults.Count();

            filteredResults = PageByOptions(filteredResults, options);

            return new BaseModelForFiltering<T>()
            {
                TotalCount = countPrePage,
                Results = filteredResults
            };
        }
        public static IQueryable<T> PageByOptions(IQueryable<T> query, Dictionary<string, object> options)
        {
            if (options.ContainsKey("skip"))
            {
                var skip = Convert.ToInt32(options["skip"]);
                var take = Convert.ToInt32(options["take"]);
                query = query
                    .Skip(skip)
                    .Take(take);
            }
            return query;
        }


        public static IQueryable<T> SortByOptions(IQueryable<T> query, Dictionary<string, object> options, Type IncomingType)
        {
            if (options.ContainsKey("sort") && options["sort"] != null)
            {
                var sortOptions = JObject.Parse(JArray.Parse(options["sort"] as string)[0].ToString());
                var columnName = (string)sortOptions.SelectToken("selector");
                var descending = (bool)sortOptions.SelectToken("desc");

                var field = IncomingType.GetProperties().FirstOrDefault(x => x.Name == columnName);
                if (field != null)
                {
                    FilterField filterFieldAttribute = (FilterField)Attribute.GetCustomAttribute(field, typeof(FilterField));
                    columnName = filterFieldAttribute != null ? filterFieldAttribute.Name : columnName;
                }

                var columnSplit = columnName.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var count = 0;
                foreach (var cs in columnSplit)
                {
                    columnName = cs;
                    if (descending)
                        columnName += " DESC";

                    if (count == 0)
                        query = query.OrderBy(columnName);
                    //else 
                    // .thenby

                    count++;
                }


            }
            return query;
        }


        public static IQueryable<T> FilterByOptions(IQueryable<T> query, Dictionary<string, object> options, Type IncomingType)
        {
            if (options.ContainsKey("filter") && options["filter"] != null)
            {
                var filterTree = JArray.Parse(options["filter"] as string);
                return ReadExpression(query, filterTree, IncomingType);
            }
            return query;
        }

        public static IQueryable<T> ReadExpression(IQueryable<T> source, JArray array, Type IncomingType)
        {
            if (array[0].Type == JTokenType.String)
                return FilterQuery(source, array[0].ToString(), array[1].ToString(), array[2].ToString(), IncomingType);
            else
            {
                for (int i = 0; i < array.Count; i++)
                {
                    if (array[i].ToString().Equals("and") || array[i].ToString().Equals("or"))
                        continue;

                    source = ReadExpression(source, (JArray)array[i], IncomingType);
                }
                return source;
            }
        }

        public static IQueryable<T> FilterQuery(IQueryable<T> source, string ColumnName, string Clause, string Value, Type IncomingType)
        {
            bool convertDate = false;
            string dataType = null;

            var field = IncomingType.GetProperties().FirstOrDefault(x => x.Name == ColumnName);
            if (field != null)
            {
                dataType = field.PropertyType.Name;
                FilterField filterFieldAttribute = (FilterField)Attribute.GetCustomAttribute(field, typeof(FilterField));
                ColumnName = filterFieldAttribute != null ? filterFieldAttribute.Name : ColumnName;
                FilterToUTCAttributeAttribute filterConvertTimezoneAttribute = (FilterToUTCAttributeAttribute)Attribute.GetCustomAttribute(field, typeof(FilterToUTCAttributeAttribute));
                convertDate = filterConvertTimezoneAttribute != null ? filterConvertTimezoneAttribute.Convert : false;
            }

            ProcessFilter(ref source, ColumnName, Clause, ref Value, convertDate, dataType);
            return source;
        }
        private static Expression<Func<T, bool>> CreateExpression(string ColumnName, string Value, ExpressionType expressionType)
        {
            var arg = Expression.Parameter(typeof(T), "p");
            var property = Expression.Property(arg, ColumnName);
            var guidValue = Guid.Parse(Value);

            var fieldAccess = Expression.PropertyOrField(arg, ColumnName);
            var value = Expression.Constant(guidValue, guidValue.GetType());

            var converted = value.Type != fieldAccess.Type ? (Expression)Expression.Convert(value, fieldAccess.Type) : (Expression)value;

            var body = Expression.MakeBinary(expressionType, fieldAccess, converted);
            var predicate = Expression.Lambda<Func<T, bool>>(body, arg);
            return predicate;
        }

        private static void ProcessFilter(ref IQueryable<T> source, string ColumnName, string Clause, ref string Value, bool convert, string dataType)
        {
            var dateRegex = @"(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}( \d{1,2}[:-]\d{2}([:-]\d{2,3})*)?";
            var dateRegex1 = @"([0-2][0-9]{3})\/(0[1-9]|1[0-2])\/([0-2][0-9]|3[0-1])(\s?([0-1][0-9]|2[0-3]):([0-5][0-9])\:([0-5][0-9])( ([\-\+]([0-1][0-9])\:00))?)?";

            string columnJoin3 = null;
            var columnSplit3 = ColumnName.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> columns = new List<string>();
            List<string> nullColumns = new List<string>();
            //check for null values
            if (columnSplit3.Count > 1)
            {
                for (int i = 0; i < columnSplit3.Count; i++)
                {
                    if (Regex.IsMatch(columnSplit3[i], ".HasValue"))
                        nullColumns.Add(columnSplit3[i].Replace(".HasValue", " != null"));
                    else
                        columns.Add(columnSplit3[i]);
                }

                columnJoin3 = String.Join(" && ", nullColumns);

                if (!string.IsNullOrEmpty(columnJoin3))
                    source = source.Where(columnJoin3);

                ColumnName = string.Join(";", columns);
            }



            switch (Clause)
            {
                case "=":
                    // todo: column split

                    Guid guidV;
                    var isGuid = Guid.TryParse(Value, out guidV);

                    if (!isGuid)
                    {
                        if (string.Compare(dataType, "String", true) == 0)
                            Value = String.Format("\"{0}\"", Value);
                        else
                            Value = System.Text.RegularExpressions.Regex.IsMatch(Value, @"^\d+$") || System.Text.RegularExpressions.Regex.IsMatch(Value, @"^true|false$", RegexOptions.IgnoreCase) ? Value : String.Format("\"{0}\"", Value);
                    }
                    if (Regex.Match(ColumnName, @"\bValue\b").Success)
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(Value, @"^\d+$") && Regex.Match(ColumnName, @"\bRefValue\b").Success)
                            Value = String.Format("\"{0}\"", Value);

                        ColumnName = Regex.Replace(ColumnName, @"\bValue\b", Value);
                        source = source.Where(ColumnName);
                    }
                    else if (!ColumnName.Contains(".Any()"))
                    {
                        // if guid create lambda
                        if (isGuid)
                        {
                            var predicate = CreateExpression(ColumnName, Value, ExpressionType.Equal);
                            source = source.Where(predicate);
                        }
                        else
                            source = source.Where(String.Format("{0} == {1}", ColumnName, Value));
                    }
                    else
                        source = source.Where(String.Format(ColumnName, Value));

                    break;
                case "contains":
                    if (!ColumnName.Contains(".Any()"))
                    {
                        var columnSplit = ColumnName.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        var separator = " || ";
                        for (int i = 0; i < columnSplit.Count; i++)
                        {
                            if (Regex.Match(columnSplit[i], @"\bValue\b").Success)
                                columnSplit[i] = Regex.Replace(columnSplit[i], @"\b == Value\b", $".Contains(\"{Value}\")");
                            else
                                columnSplit[i] += $".Contains(\"{Value}\")";
                        }
                        var columnJoin = String.Join(separator, columnSplit);
                        source = source.Where(columnJoin);
                    }
                    else
                        source = source.Where(String.Format(ColumnName, Value));

                    break;
                case "notcontains":
                    var columnSplit2 = ColumnName.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    var separator2 = " || ";
                    for (int i = 0; i < columnSplit2.Count; i++)
                    {
                        if (Regex.Match(columnSplit2[i], @"\bValue\b").Success)
                            columnSplit2[i] = Regex.Replace(columnSplit2[i], @"\b == Value\b", $".Contains(\"{Value}\")");
                        else
                            columnSplit2[i] += $".Contains(\"{Value}\")";
                    }
                    var columnJoin2 = String.Join(separator2, columnSplit2);

                    if (Regex.Match(columnJoin2, @"\bCount\b").Success)
                        columnJoin2 = "(" + columnJoin2 + ")";
                    source = source.Where($"!{columnJoin2}");
                    break;
                case "<>":
                    // todo: column split
                    // if guid create lambda
                    Guid guidVn;
                    var isGuidn = Guid.TryParse(Value, out guidVn);
                    if (isGuidn)
                    {
                        var predicaten = CreateExpression(ColumnName, Value, ExpressionType.NotEqual);
                        source = source.Where(predicaten);
                    }
                    else if (Regex.Match(ColumnName, @"\bValue\b").Success)
                    {
                        ColumnName = Regex.Replace(ColumnName, @"\bValue\b", $"\"{Value}\"");

                        if (Regex.Match(ColumnName, @"\bCount\b").Success)
                            ColumnName = Regex.Replace(ColumnName, @"==", "!=");

                        if (Regex.Match(ColumnName, @"\b||\b").Success)
                            ColumnName = Regex.Replace(ColumnName, @"\|\|", "&&");

                        source = source.Where(ColumnName);
                    }
                    else
                    {
                        if (string.Compare(dataType, "String", true) == 0)
                            Value = String.Format("\"{0}\"", Value);
                        else
                            Value = System.Text.RegularExpressions.Regex.IsMatch(Value, @"^\d+$") || System.Text.RegularExpressions.Regex.IsMatch(Value, @"^true|false$", RegexOptions.IgnoreCase) ? Value : String.Format("\"{0}\"", Value);

                        source = source.Where($"{ColumnName} != {Value}");//it was startswith, why?
                    }

                    break;
                case ">=":
                    // todo: column split
                    if (Regex.IsMatch(Value, dateRegex) || Regex.IsMatch(Value, dateRegex1))
                    {
                        var dateVal = DateTime.Parse(Value);

                        source = source.Where($"{ColumnName} >= DateTime({dateVal.Year},{dateVal.Month},{dateVal.Day},{dateVal.Hour},{dateVal.Minute},{dateVal.Second})");
                    }
                    else
                        source = source.Where($"{ColumnName} >= {Value}");
                    break;
                case "<=":
                    // todo: column split
                    if (Regex.IsMatch(Value, dateRegex) || Regex.IsMatch(Value, dateRegex1))
                    {
                        var dateVal = DateTime.Parse(Value);


                        source = source.Where($"{ColumnName} <= DateTime({dateVal.Year},{dateVal.Month},{dateVal.Day},{dateVal.Hour},{dateVal.Minute},{dateVal.Second})");
                    }
                    else
                        source = source.Where($"{ColumnName} <= {Value}");
                    break;
                case ">":
                    // todo: column split
                    if (Regex.IsMatch(Value, dateRegex) || Regex.IsMatch(Value, dateRegex1))
                    {
                        var dateVal = DateTime.Parse(Value);

                        source = source.Where($"{ColumnName} > DateTime({dateVal.Year},{dateVal.Month},{dateVal.Day},{dateVal.Hour},{dateVal.Minute},{dateVal.Second})");
                    }
                    else
                        source = source.Where($"{ColumnName} > {Value}");
                    break;
                case "<":
                    // todo: column split
                    if (Regex.IsMatch(Value, dateRegex) || Regex.IsMatch(Value, dateRegex1))
                    {
                        var dateVal = DateTime.Parse(Value);

                        source = source.Where($"{ColumnName} < DateTime({dateVal.Year},{dateVal.Month},{dateVal.Day},{dateVal.Hour},{dateVal.Minute},{dateVal.Second})");
                    }
                    else
                        source = source.Where($"{ColumnName} < {Value}");
                    break;
                case "startswith":
                    var columnSplits = ColumnName.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    for (int i = 0; i < columnSplits.Count; i++)
                    {
                        if (Regex.Match(columnSplits[i], @"\bValue\b").Success)
                            columnSplits[i] = Regex.Replace(columnSplits[i], @"\b == Value\b", $".StartsWith(\"{Value}\")");
                        else
                            columnSplits[i] += $".StartsWith(\"{Value}\")";
                    }

                    var columnJoins = String.Join(" || ", columnSplits);
                    source = source.Where(columnJoins);
                    break;
                case "endswith":
                    var columnSplite = ColumnName.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    for (int i = 0; i < columnSplite.Count; i++)
                    {
                        if (Regex.Match(columnSplite[i], @"\bValue\b").Success)
                            columnSplite[i] = Regex.Replace(columnSplite[i], @"\b == Value\b", $".EndsWith(\"{Value}\")");
                        else
                            columnSplite[i] += $".EndsWith(\"{Value}\")";
                    }

                    var columnJoine = String.Join(" || ", columnSplite);
                    source = source.Where(columnJoine);
                    break;
                default:
                    break;
            }
        }


        public static IQueryable<T> HeaderFilterByOptions(IQueryable<T> query, Dictionary<string, object> options, Type IncomingType)
        {
            if (options.ContainsKey("headerFilter") && options["headerFilter"] != null)
            {
                var filterTree = JArray.Parse(options["headerFilter"] as string);
                foreach (var f in filterTree)
                {
                    string expression = "";
                    ReadOrExpression(ref expression, (JArray)f, IncomingType);
                    query = query.Where(expression);
                }

            }
            return query;
        }
        public static string ReadOrExpression(ref string expression, JArray array, Type IncomingType)
        {
            if (array[0].Type == JTokenType.String)
                return FilterStringQuery(array[0].ToString(), array[1].ToString(), array[2].ToString(), IncomingType);
            else
            {
                for (int i = 0; i < array.Count; i++)
                {
                    if (array[i].ToString().Equals("and"))
                        continue;

                    if (array[i].ToString().Equals("or"))
                    {
                        expression += " || ";
                        continue;
                    }

                    expression += ReadOrExpression(ref expression, (JArray)array[i], IncomingType);
                }
                return expression;
            }
        }


        public static string FilterStringQuery(string ColumnName, string Clause, string Value, Type IncomingType)
        {
            bool convertDate = false;
            var field = IncomingType.GetProperties().FirstOrDefault(x => x.Name == ColumnName);
            if (field != null)
            {
                FilterField filterFieldAttribute = (FilterField)Attribute.GetCustomAttribute(field, typeof(FilterField));
                ColumnName = filterFieldAttribute != null ? filterFieldAttribute.Name : ColumnName;
                FilterToUTCAttributeAttribute filterConvertTimezoneAttribute = (FilterToUTCAttributeAttribute)Attribute.GetCustomAttribute(field, typeof(FilterToUTCAttributeAttribute));
                convertDate = filterConvertTimezoneAttribute != null ? filterConvertTimezoneAttribute.Convert : false;
            }

            return ProcessFilterString(ColumnName, Clause, ref Value, convertDate);
        }

        private static string ProcessFilterString(string ColumnName, string Clause, ref string Value, bool convert)
        {
            var dateRegex = @"(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}( \d{1,2}[:-]\d{2}([:-]\d{2,3})*)?";
            var dateRegex1 = @"([0-2][0-9]{3})\/(0[1-9]|1[0-2])\/([0-2][0-9]|3[0-1]) ([0-1][0-9]|2[0-3]):([0-5][0-9])\:([0-5][0-9])( ([\-\+]([0-1][0-9])\:00))?";

            string filterString = "";
            switch (Clause)
            {
                case "=":
                    // todo: column split
                    Value = System.Text.RegularExpressions.Regex.IsMatch(Value, @"^\d+$") || System.Text.RegularExpressions.Regex.IsMatch(Value, @"^true|false$", RegexOptions.IgnoreCase) ? Value : String.Format("\"{0}\"", Value);
                    if (ColumnName.Contains("Value"))
                        filterString = ColumnName.Replace("Value", Value);
                    else if (!ColumnName.Contains(".Any()"))
                        filterString = String.Format("{0} == {1}", ColumnName, Value);
                    else
                        filterString = String.Format(ColumnName, Value);

                    break;
                case ">=":
                    // todo: column split
                    if (Regex.IsMatch(Value, dateRegex) || Regex.IsMatch(Value, dateRegex1))
                    {
                        var dateVal = DateTime.Parse(Value);
                        //if (convert)
                        //    dateVal = BusinessLogic.TimeZoneHelper.ConvertToUTC(dateVal).Value;
                        filterString = $"{ColumnName} >= DateTime({dateVal.Year},{dateVal.Month},{dateVal.Day},{dateVal.Hour},{dateVal.Minute},{dateVal.Second})";
                    }
                    else
                        filterString = $"{ColumnName} >= {Value}";
                    break;
                case "<=":
                    // todo: column split
                    if (Regex.IsMatch(Value, dateRegex) || Regex.IsMatch(Value, dateRegex1))
                    {
                        var dateVal = DateTime.Parse(Value);
                        //if (convert)
                        //    dateVal = BusinessLogic.TimeZoneHelper.ConvertToUTC(dateVal).Value;

                        filterString = $"{ColumnName} <= DateTime({dateVal.Year},{dateVal.Month},{dateVal.Day},{dateVal.Hour},{dateVal.Minute},{dateVal.Second})";
                    }
                    else
                        filterString = $"{ColumnName} <= {Value}";
                    break;
                case ">":
                    // todo: column split
                    if (Regex.IsMatch(Value, dateRegex) || Regex.IsMatch(Value, dateRegex1))
                    {
                        var dateVal = DateTime.Parse(Value);
                        //if (convert)
                        //    dateVal = BusinessLogic.TimeZoneHelper.ConvertToUTC(dateVal).Value;
                        filterString = $"{ColumnName} > DateTime({dateVal.Year},{dateVal.Month},{dateVal.Day},{dateVal.Hour},{dateVal.Minute},{dateVal.Second})";
                    }
                    else
                        filterString = $"{ColumnName} > {Value}";
                    break;
                case "<":
                    // todo: column split
                    if (Regex.IsMatch(Value, dateRegex) || Regex.IsMatch(Value, dateRegex1))
                    {
                        var dateVal = DateTime.Parse(Value);
                        //if (convert)
                        //    dateVal = BusinessLogic.TimeZoneHelper.ConvertToUTC(dateVal).Value;
                        filterString = $"{ColumnName} < DateTime({dateVal.Year},{dateVal.Month},{dateVal.Day},{dateVal.Hour},{dateVal.Minute},{dateVal.Second})";
                    }
                    else
                        filterString = $"{ColumnName} < {Value}";
                    break;
                default:
                    break;
            }

            return filterString;
        }
    }
}
